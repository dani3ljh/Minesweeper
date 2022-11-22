using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class that only uses static methods and variables
// gm gives this class its variables
public class CellLogic : MonoBehaviour
{
	public static GameManager gm;
	public static TimerManager tm;
	public static UIManager uim;

	public static GameObject cellPrefab;
	public static Transform cellFolder;

	public static void MineCell(int x, int y, int width, int height)
	{
		if (gm.cellStatuses[x, y] != 0) return;

		int cellNumber = GetCellType(x, y, gm.mines);

		gm.cellStatuses[x, y] = cellNumber;

		if (cellNumber == -1)
		{
			if (gm.cellsMined == 0)
			{
				gm.mines[x, y] = false;
				gm.cellStatuses[x, y] = 0;
				int[][] freeIndexes = GetFreeIndexes(gm.mines.ToJaggedArray());
				int index = Random.Range(0, freeIndexes.Length);
				gm.mines[freeIndexes[index][0], freeIndexes[index][1]] = true;
				MineCell(x, y, width, height);
				return;
			}

			gm.EndGame(false);
			gm.cellSpriteRenderers[x, y].sprite = gm.cellTriggeredMine;
			return;
		}

		if (gm.cellsMined == 0) tm.StartTimer();

		gm.cellsMined++;
		gm.cellsNotMined--;
		uim.SetCellAmountText(gm.cellsNotMined);


		if (gm.cellsMined == width * height - gm.mineAmount) gm.EndGame(true);

		if (cellNumber != 0)
		{
			gm.cellSpriteRenderers[x, y].sprite = gm.cellNumbers[cellNumber - 1];
			return;
		}

		gm.cellStatuses[x, y] = 9;

		gm.cellSpriteRenderers[x, y].sprite = gm.cellBlank;

		int[,] indexOffsets = new int[8, 2] {
			{ 1, 1 },
			{ 0, 1 },
			{ -1, 1 },
			{ 1, 0 },
			{ -1, 0 },
			{ 1, -1 },
			{ 0, -1 },
			{ -1, -1 }
		};
		for (int i = 0; i < 8; i++)
		{
			int checkX = x + indexOffsets[i, 0];
			int checkY = y + indexOffsets[i, 1];

			if (checkX < 0 || checkY < 0 || checkX >= width || checkY >= height)
			{
				continue;
			}

			MineCell(checkX, checkY, width, height);
		}
	}

	public static GameObject PlaceCell(int x, int y, int width, int height)
	{
		// make sure to call function with proper arguments
		// i didnt and couldnt fix the error for like a week
		float scale = 10f / height;
		float transformedX = (width * scale * -0.5f) + scale / 2 + (x * scale);
		float transformedY = 5 - (scale / 2) - (y * scale);
		GameObject cell = Instantiate(cellPrefab, new Vector3(transformedX, transformedY, 0), new Quaternion(0, 0, 0, 0));

		cell.transform.SetParent(cellFolder);
		cell.transform.localScale = new Vector3(scale, scale, scale) * 3f;
		gm.cellSpriteRenderers[x, y] = cell.GetComponent<SpriteRenderer>();
		gm.cellSpriteRenderers[x, y].sprite = gm.cellUnchecked;
		cell.name = $"Cell ({x}, {y})";
		return cell;
	}

	public static bool[,] GenerateRandomMines(int amountOfMines, int width, int height)
	{
		bool[,] twoDMineArr = new bool[width, height];
		for (int i = 0; i < amountOfMines; i++)
		{
			int[][] freeIndexes = GetFreeIndexes(twoDMineArr.ToJaggedArray());
			int index = Random.Range(0, freeIndexes.Length);
			twoDMineArr[freeIndexes[index][0], freeIndexes[index][1]] = true;
		}
		return twoDMineArr;
	}

	public static int[][] GetFreeIndexes(bool[][] twoDMineArr)
	{
		List<int[]> indexes = new List<int[]>();
		for (int x = 0; x < twoDMineArr.Length; x++)
		{
			for (int y = 0; y < twoDMineArr[x].Length; y++)
			{
				if (!twoDMineArr[x][y])
				{
					indexes.Add(new int[] { x, y });
				}
			}
		}
		return indexes.ToArray();
	}

	/// <summary>
	/// returns 0 for nothing <br></br>
	/// returns 1 through 8 for cell number <br></br>
	/// returns -1 for mine
	/// </summary>
	/// <returns></returns>
	public static int GetCellType(int x, int y, bool[,] twoDMineArr)
	{
		int w = twoDMineArr.GetLength(0);
		int h = twoDMineArr.GetLength(1);

		if (twoDMineArr[x, y])
		{
			return -1;
		}
		int total = 0;
		int[,] indexOffsets = new int[8, 2] {
			{ 1, 1 },
			{ 0, 1 },
			{ -1, 1 },
			{ 1, 0 },
			{ -1, 0 },
			{ 1, -1 },
			{ 0, -1 },
			{ -1, -1 }
		};
		for (int i = 0; i < 8; i++)
		{
			int checkX = x + indexOffsets[i, 0];
			int checkY = y + indexOffsets[i, 1];

			if (checkX < 0 || checkY < 0 || checkX >= w || checkY >= h)
			{
				continue;
			}

			if (twoDMineArr[
				checkX,
				checkY
			])
			{
				total++;
			}
		}
		if (gm.gameMode == "oneOff")
		{
			if (total <= 1)
			{
				return total + 1;
			}
			if (total == 8)
			{
				return 7;
			}
			return total + (Random.value > 0.5f ? -1 : 1);
		}
		return total;
	}
}
