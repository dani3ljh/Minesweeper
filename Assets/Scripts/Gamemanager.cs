using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
	[Header("Board Settings")]
	[SerializeField] private int width;
	[SerializeField] private int height;
	[SerializeField] private int mineAmount;

	[Header("Cell Data")]
	[SerializeField] private GameObject cellPrefab;
	[SerializeField] private Transform cellFolder;

	[Header("Cell Textures")]
	[SerializeField] private Sprite cellBlank;
	[SerializeField] private Sprite cellUnchecked;
	[SerializeField] private Sprite[] cellNumbers;
	[SerializeField] private Sprite cellTriggeredMine;
	[SerializeField] private Sprite cellNonTriggeredMine;
	[SerializeField] private Sprite cellCrossedMine;
	[SerializeField] private Sprite cellFlagged;

	// Cell Data 2D Array with length [width, height]
	private GameObject[,] cells;
	private SpriteRenderer[,] cellSpriteRenderers;
	private int[,] cellStatuses;
	private bool[,] mines;

	// Lists of centers of cells
	private float[] xCenters;
	private float[] yCenters;

	// Checks when you die
	private bool isAlive;

	private void Start()
	{
		isAlive = true;

		cells = new GameObject[width, height];
		cellStatuses = new int[width, height];
		cellSpriteRenderers = new SpriteRenderer[width, height];

		mines = GenerateRandomMines(mineAmount, width, height);
		
		xCenters = new float[width];
		yCenters = new float[height];
		
		for(int x = 0; x < width; x++)
		{
			for(int y = 0; y < height; y++)
			{
				GameObject cell = PlaceCell(x, y, width, height);
				cells[x, y] = cell;
				cellStatuses[x, y] = 0;
				xCenters[x] = cell.transform.position.x;
				yCenters[y] = cell.transform.position.y;
			}
		}
	}

	private void Update()
	{
		if (!isAlive) return;
		if (Input.GetMouseButtonUp(0))
		{
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			int[] indexes = GetIndexesFromPoint(worldPosition.x, worldPosition.y, width, height);
			if(cellStatuses[indexes[0], indexes[1]] == 0)
            {
				MineCell(indexes[0], indexes[1], width, height, mines);
			}
		}
        if (Input.GetMouseButtonUp(1))
        {
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			int[] indexes = GetIndexesFromPoint(worldPosition.x, worldPosition.y, width, height);
			if(cellStatuses[indexes[0], indexes[1]] == 0)
            {
				cellSpriteRenderers[indexes[0], indexes[1]].sprite = cellFlagged;
				cellStatuses[indexes[0], indexes[1]] = 10;
            }
		}
	}

	private void MineCell(int x, int y, int w, int h, bool[,] twoDMineArr)
	{
		if (cellStatuses[x, y] != 0) return;

		int cellNumber = GetCellType(x, y, twoDMineArr);

		cellStatuses[x, y] = cellNumber;

		if(cellNumber == -1)
		{
			isAlive = false;
			cellSpriteRenderers[x, y].sprite = cellTriggeredMine;
			Invoke(nameof(ResetScene), 5f);
			return;
		}

		if(cellNumber != 0)
		{
			cellSpriteRenderers[x, y].sprite = cellNumbers[cellNumber - 1];
			return;
		}

		cellStatuses[x, y] = 9;

		cellSpriteRenderers[x, y].sprite = cellBlank;

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
			indexOffsets[2, 0] = 0;
			indexOffsets[4, 0] = 0;
			indexOffsets[7, 0] = 0;
			indexOffsets[2, 1] = 0;
			indexOffsets[4, 1] = 0;
			indexOffsets[7, 1] = 0;
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
			MineCell(x + indexOffsets[i, 0], y + indexOffsets[i, 1], w, h, twoDMineArr);
		}
	}

	private GameObject PlaceCell(int x, int y, int w, int h)
	{
		float scale = 10f / h;
		float transformedX = (w * scale * -0.5f) + scale / 2 + (x * scale);
		float transformedY = 5 - (scale / 2) - (y * scale);
		GameObject cell = Instantiate(cellPrefab, new Vector3(transformedX, transformedY, 0), new Quaternion(0,0,0,0));
		cell.transform.SetParent(cellFolder);
		cell.transform.localScale = new Vector3(scale, scale, scale) * 3f;
		cellSpriteRenderers[x, y] = cell.GetComponent<SpriteRenderer>();
		cellSpriteRenderers[x, y].sprite = cellUnchecked;
		cell.name = $"Cell ({x}, {y})";
		return cell;
	}
	
	private int[] GetIndexesFromPoint(float x, float y, int w, int h)
	{
		int[] indexes = new int[2];
		float scale = 10f / h;
		int xIndex = -1;
		int yIndex = -1;
		for (int i = 0; i < w; i++)
		{
			float differance = Mathf.Abs(x - xCenters[i]);
			if (differance <= (scale / 2))
			{
				xIndex = i;
				break;
			}
		}
		for (int i = 0; i < h; i++)
		{
			float differance = Mathf.Abs(y - yCenters[i]);
			if (differance <= (scale / 2))
			{
				yIndex = i;
				break;
			}
		}
		indexes[0] = xIndex;
		indexes[1] = yIndex;
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

	public static void ResetScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}