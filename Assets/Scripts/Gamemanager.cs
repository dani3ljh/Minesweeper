using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
	[Header("Board Settings")]
	private int width;
	private int height;
	private int mineAmount;
	[SerializeField] private float loseInstantiateDelay;
	[SerializeField] private float winInstantiateDelay;

	public float mobileModeFlagTime = 1f;

	[Header("Objects")]
	[SerializeField] private GameObject cellPrefab;
	[SerializeField] private Transform cellFolder;
	[SerializeField] private Text heightInputText;
	[SerializeField] private Text widthInputText;
	[SerializeField] private Text mineAmountInputText;
	[SerializeField] private Text heightResultsText;
	[SerializeField] private Text widthResultsText;
	[SerializeField] private Text mineAmountResultsText;

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
	/*
	 * cellStatus numbers goes as follows:
	 * 0 is unchecked cell
	 * 1 through 8 is the checked cell number 1 through 8
	 * 9 is checked cell blank
	 * 10 is flagged
	 */
	[HideInInspector] public int[,] cellStatuses;
	private bool[,] mines;

	// Game Data
	[HideInInspector] public bool isAlive;
	[HideInInspector] public int cellsMined;
	[HideInInspector] public int cellsNotMined;
	[HideInInspector] public int minesNotFlagged;
	[HideInInspector] public bool mobileMode;
	[HideInInspector] public string gameMode;

	// Scripts
	private InputManager im;
	private UIManager uim;
	private TimerManager tm;

	// Color
	[HideInInspector] public Color cameraBackgroundColor;

	private void Start()
	{
		im = gameObject.GetComponent<InputManager>();
		uim = gameObject.GetComponent<UIManager>();
		tm = gameObject.GetComponent<TimerManager>();

		tm.StopTimer();
		tm.ResetTimer();
		mobileMode = false;
	}

	public void ChooseAnotherMode()
	{
		isAlive = false;
		tm.StopTimer();
		uim.StartScreenFall();
		gameMode = null;

		// if there are cells delete them
		if (cells != null) DeleteCells();
	}

	public void StartGame(string mode)
	{
		gameMode = mode;
		uim.StartScreenRise();
		Invoke(nameof(SetupGame), 0.5f);
	}

	public void SetupGame()
	{
		isAlive = true;
		uim.resetButton.interactable = true;
		
		if(width==0) width = 16;
		if(height==0) height = 13;
		if(mineAmount==0) mineAmount = (int)(width*height*0.19f);
		
		heightResultsText.text = "Height: " + height.ToString();
		widthResultsText.text = "Width: " + width.ToString();
		mineAmountResultsText.text = "Mines: " + mineAmount.ToString();
		
		cellsMined = 0;
		cellsNotMined = (width * height) - mineAmount;
		minesNotFlagged = mineAmount;

		uim.SetFlagAmountText(mineAmount);
		uim.SetCellAmountText(cellsNotMined);

		tm.StopTimer();

		// if there are still cells delete them
		if (cells != null) DeleteCells();

		cells = new GameObject[width, height];
		cellStatuses = new int[width, height];
		cellSpriteRenderers = new SpriteRenderer[width, height];

		mines = GenerateRandomMines(mineAmount);

		im.SetVariables(width, height, mineAmount, mines);

		im.xCenters = new float[width];
		im.yCenters = new float[height];
		
		for(int x = 0; x < width; x++)
		{
			for(int y = 0; y < height; y++)
			{
				GameObject cell = PlaceCell(x, y);
				cells[x, y] = cell;
				cellStatuses[x, y] = 0;
				im.xCenters[x] = cell.transform.position.x;
				im.yCenters[y] = cell.transform.position.y;
			}
		}
	}

	public void MineCell(int x, int y)
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
				MineCell(x, y);
				return;
			}
			
			cellSpriteRenderers[x, y].sprite = cellTriggeredMine;
			EndGame(false);
			return;
		}

		if (cellsMined == 0) tm.StartTimer();

		cellsMined++;
		cellsNotMined--;
		uim.SetCellAmountText(cellsNotMined);
		

		if (cellsMined == width * height - mineAmount) EndGame(true);

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
		for (int i = 0; i < 8; i++)
		{
			int checkX = x + indexOffsets[i, 0];
			int checkY = y + indexOffsets[i, 1];

			if (checkX < 0 || checkY < 0 || checkX >= width || checkY >= height)
			{
				continue;
			}

			MineCell(checkX, checkY);
		}
	}

	private GameObject PlaceCell(int x, int y)
	{
		float scale = 10f / height;
		float transformedX = (width * scale * -0.5f) + scale / 2 + (x * scale);
		float transformedY = 5 - (scale / 2) - (y * scale);
		GameObject cell = Instantiate(cellPrefab, new Vector3(transformedX, transformedY, 0), new Quaternion(0,0,0,0));
		cell.transform.SetParent(cellFolder);
		cell.transform.localScale = new Vector3(scale, scale, scale) * 3f;
		cellSpriteRenderers[x, y] = cell.GetComponent<SpriteRenderer>();
		cellSpriteRenderers[x, y].sprite = cellUnchecked;
		cell.name = $"Cell ({x}, {y})";
		return cell;
	}
	
	private bool[,] GenerateRandomMines(int amountOfMines)
	{
		bool[,] twoDMineArr = new bool[width, height];
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
		for (int i = 0; i < 8; i++)
		{
			int checkX = x + indexOffsets[i, 0];
			int checkY = y + indexOffsets[i, 1];

			if(checkX < 0 || checkY < 0 || checkX >= w || checkY >= h)
			{
				continue;
			}

			if (twoDMineArr[
				checkX,
				checkY
			]){
				total++;
			}
		}
		if (gameMode == "oneOff")
		{
			if(total <= 1)
			{
				return total+1;
			}
			if(total == 8)
			{
				return 7;
			}
			return total + (Random.value > 0.5f ? -1 : 1);
		}
		return total;
	}

	public void EndGame(bool win)
	{
		isAlive = false;
		uim.resetButton.interactable = false;

		tm.StopTimer(false);
		
		// make end screen after delay
		uim.Invoke(win 
			? nameof(uim.InstantiateWinScreen)
			: nameof(uim.InstantiateLoseScreen)
		, win
			? winInstantiateDelay 
			: loseInstantiateDelay
		);
	}

	public void DeleteCells()
	{
		for (int x = 0; x < cells.GetLength(0); x++)
		{
			for (int y = 0; y < cells.GetLength(1); y++)
			{
				Destroy(cells[x, y]);
			}
		}
	}
	
	public void UpdateHeight(){
		// int.Parse in theory should be safe because of the input field is set to only allow numbers
		height = int.Parse(heightInputText.text);
	}
	
	public void UpdateWidth(){
		// int.Parse in theory should be safe because of the input field is set to only allow numbers
		width = int.Parse(widthInputText.text);
	}
	
	public void UpdateMineAmount(){
		// int.Parse in theory should be safe because of the input field is set to only allow numbers
		mineAmount = int.Parse(mineAmountInputText.text);
	}
}
