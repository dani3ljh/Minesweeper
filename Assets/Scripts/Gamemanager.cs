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
	[SerializeField] private float loseResetDelay;
	[SerializeField] private float winResetDelay;

	[Header("Cell Data")]
	[SerializeField] private GameObject cellPrefab;
	[SerializeField] private Transform cellFolder;

	[Header("Cell Textures")]
	public Sprite cellBlank;
	public Sprite cellUnchecked;
	public Sprite[] cellNumbers;
	public Sprite cellTriggeredMine;
	public Sprite cellNonTriggeredMine;
	public Sprite cellCrossedMine;
	public Sprite cellFlagged;

	// Cell Data 2D Array with length [width, height]
	private GameObject[,] cells;
	[HideInInspector] public SpriteRenderer[,] cellSpriteRenderers;
	[HideInInspector] public int[,] cellStatuses;
	private bool[,] mines;
	
	// Game Data
	[HideInInspector] public bool isAlive;
	[HideInInspector] public int cellsMined;

	// Scripts
	private MouseManager mm;

	// Color
	[HideInInspector] public Color cameraBackgroundColor;
	
	private void Start()
	{
		mm = gameObject.GetComponent<MouseManager>();

		isAlive = true;
		cellsMined = 0;

		cells = new GameObject[width, height];
		cellStatuses = new int[width, height];
		cellSpriteRenderers = new SpriteRenderer[width, height];

		mines = GenerateRandomMines(mineAmount, width, height);

		mm.SetVariables(width, height, mineAmount, mines);

		mm.xCenters = new float[width];
		mm.yCenters = new float[height];
		
		for(int x = 0; x < width; x++)
		{
			for(int y = 0; y < height; y++)
			{
				GameObject cell = PlaceCell(x, y, width, height);
				cells[x, y] = cell;
				cellStatuses[x, y] = 0;
				mm.xCenters[x] = cell.transform.position.x;
				mm.yCenters[y] = cell.transform.position.y;
			}
		}
	}

	public void MineCell(int x, int y, int w, int h)
	{
		if (cellStatuses[x, y] != 0) return;

		int cellNumber = GetCellType(x, y, mines);

		cellStatuses[x, y] = cellNumber;

		if(cellNumber == -1)
		{
			if(cellsMined == 0)
            {
				mines[x, y] = false;
				cellStatuses[x, y] = 0;
				int[][] freeIndexes = GetFreeIndexes(mines.ToJaggedArray());
				int index = Random.Range(0, freeIndexes.Length);
				mines[freeIndexes[index][0], freeIndexes[index][1]] = true;
				MineCell(x, y, w, h);
				return;
			}

			isAlive = false;
			cellSpriteRenderers[x, y].sprite = cellTriggeredMine;
			Invoke(nameof(Start), loseResetDelay);
			return;
		}

		cellsMined++;

		if (cellNumber != 0)
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
			MineCell(x + indexOffsets[i, 0], y + indexOffsets[i, 1], w, h);
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
	
	private bool[,] GenerateRandomMines(int amountOfMines, int w, int h)
	{
		bool[,] twoDMineArr = new bool[w, h];
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

	public void Win()
    {
		isAlive = false;
		Camera.main.backgroundColor = new Color(0, 256, 0);
		Debug.Log("You won");
		Invoke(nameof(Start), winResetDelay);
	}
}