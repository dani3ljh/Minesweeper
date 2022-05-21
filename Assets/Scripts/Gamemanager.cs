using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
	[Header("Board Settings")]
	[SerializeField] private int width;
	[SerializeField] private int height;
	[SerializeField] private int mineAmount;

	[Header("Game Objects")]
	[SerializeField] private GameObject cellPrefab;
	[SerializeField] private Transform cellFolder;

	[Header("Cell Textures")]
	[SerializeField] private Sprite cellBlank;
	[SerializeField] private Sprite cellUnchecked;
	[SerializeField] private Sprite cell1;
	[SerializeField] private Sprite cell2;
	[SerializeField] private Sprite cell3;
	[SerializeField] private Sprite cell4;
	[SerializeField] private Sprite cell5;
	[SerializeField] private Sprite cell6;
	[SerializeField] private Sprite cell7;
	[SerializeField] private Sprite cell8;
	[SerializeField] private Sprite cellTriggeredMine;
	[SerializeField] private Sprite cellNonTriggeredMine;
	[SerializeField] private Sprite cellCrossedMine;
	[SerializeField] private Sprite cellFlagged;

	private GameObject[,] cells;
	private bool[,] mines;

	private void Start()
	{
		cells = new GameObject[width, height];
		mines = GenerateRandomMines(mineAmount, width, height);
		for(int x = 0; x < width; x++)
		{
			for(int y = 0; y < height; y++)
			{
				GameObject cell = PlaceCell(x, y, width, height);
				cells[x, y] = cell;
			}
		}
	}

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			int[] indexes = GetIndexesFromPoint(worldPosition.x, worldPosition.y, width, height);
			Debug.Log($"Mouse logged at ({indexes[0]}, {indexes[1]})");
		}
    }

    private GameObject PlaceCell(int x, int y, int w, int h)
	{
		float scale = 10f / h;
		float transformedX = (w * scale * -0.5f) + 0.5f + (x * scale);
		float transformedY = 5 - (scale / 2) - (y * scale);
		GameObject cell = Instantiate(cellPrefab, new Vector3(transformedX, transformedY, 0), new Quaternion(0,0,0,0));
		cell.transform.SetParent(cellFolder);
		cell.transform.localScale = new Vector3(scale, scale, scale) * 3f;
		cell.GetComponent<SpriteRenderer>().sprite = cellUnchecked;
		cell.name = $"Cell ({x}, {y})";
		return cell;
	}
	
	private int[] GetIndexesFromPoint(float x, float y, int w, int h)
    {
		int[] indexes = new int[2];
		float scale = 10f / h;
		indexes[0] = (int)Mathf.Round((x + (w * scale) / 2 - 1 / 2) / scale)+1;
		indexes[1] = (int)Mathf.Round((5-(scale/2)-y)/scale);
		return indexes;
    }

	private bool[,] GenerateRandomMines(int amountOfMines, int w, int h)
	{
		bool[,] twoDMineArr = new bool[w,h];
		for(int i = 0; i < amountOfMines; i++)
		{
			int[][] freeIndexes = GetFreeIndexes(twoDMineArr.ToJaggedArray());
			int index = Random.Range(0, freeIndexes.Length);
			twoDMineArr[freeIndexes[index][0], freeIndexes[index][1]] = true;
		}
		return twoDMineArr;
	}

	private int[][] GetFreeIndexes(bool[][] twoDMineArr)
	{
		List<int[]> indexes = new List<int[]>();
		for(int x = 0; x < twoDMineArr.Length; x++)
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
	private int GetCellType(int x, int y, bool[,] twoDMineArr)
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
		if (x == 0)
		{
			indexOffsets[2,0] = 0;
			indexOffsets[4,0] = 0;
			indexOffsets[7,0] = 0;
			indexOffsets[2,1] = 0;
			indexOffsets[4,1] = 0;
			indexOffsets[7,1] = 0;
		}
		if (x == w - 1)
		{
			indexOffsets[0, 0] = 0;
			indexOffsets[3, 0] = 0;
			indexOffsets[5, 0] = 0;
			indexOffsets[0, 1] = 0;
			indexOffsets[3, 1] = 0;
			indexOffsets[5, 1] = 0;
		}
		if (y == 0)
		{
			indexOffsets[5, 0] = 0;
			indexOffsets[6, 0] = 0;
			indexOffsets[7, 0] = 0;
			indexOffsets[5, 1] = 0;
			indexOffsets[6, 1] = 0;
			indexOffsets[7, 1] = 0;
		}
		if (y == h - 1)
		{
			indexOffsets[0, 0] = 0;
			indexOffsets[1, 0] = 0;
			indexOffsets[2, 0] = 0;
			indexOffsets[0, 1] = 0;
			indexOffsets[1, 1] = 0;
			indexOffsets[2, 1] = 0;
		}
		for (int i = 0; i < 8; i++)
		{
			if (twoDMineArr[
				x + indexOffsets[i, 0],
				y + indexOffsets[i, 1]
			]){
				total++;
			}
		}
		return total;
	}
}