using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[Header("Board Settings")]
	[SerializeField] private float loseInstantiateDelay;
	[SerializeField] private float winInstantiateDelay;
	[HideInInspector] public int width;
	[HideInInspector] public int height;
	[HideInInspector] public int mineAmount;

	[Header("Objects")]
	[SerializeField] private GameObject cellPrefab;
	[SerializeField] private Transform cellFolder;
	
	[Header("Inputs")]
	[SerializeField] private Text widthInputText;
	[SerializeField] private Text heightInputText;
	[SerializeField] private Text mineAmountInputText;
	[SerializeField] private Text widthResultsText;
	[SerializeField] private Text heightResultsText;
	[SerializeField] private Text mineAmountResultsText;

	[Header("Cell Textures")]
	public Sprite cellUnchecked;
	public Sprite cellBlank;
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
	[HideInInspector] public bool[,] mines;

	// Game Data
	[HideInInspector] public bool isAlive = false;
	[HideInInspector] public int cellsMined;
	[HideInInspector] public int cellsNotMined;
	[HideInInspector] public int minesNotFlagged;
	[HideInInspector] public bool mobileMode;
	[HideInInspector] public string gameMode;
	// Cell Data
	[HideInInspector] public List<int[]> minePositions;
	[HideInInspector] public List<int[]> missFlaggedPositions;

	// Scripts
	private InputManager im;
	private UIManager uim;
	private TimerManager tm;

	private void Start()
	{
		im = gameObject.GetComponent<InputManager>();
		uim = gameObject.GetComponent<UIManager>();
		tm = gameObject.GetComponent<TimerManager>();

		CellLogic.gm = this;
		CellLogic.tm = tm;
		CellLogic.uim = uim;
		CellLogic.cellPrefab = cellPrefab;
		CellLogic.cellFolder = cellFolder;

		tm.StopTimer();
		tm.ResetTimer();
		mobileMode = false;
	}

	public void OpenMenu()
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
		uim.openMenuButton.interactable = true;
		
		
		// Update the board settings using defaults or input
		width = GetWidth(16);
		height = GetHeight(13);
		mineAmount = GetMineAmount((int)(width*height*0.193f));
		
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
		
		// Clear Cell data
		minePositions = new List<int[]>();
		missFlaggedPositions = new List<int[]>();

		cells = new GameObject[width, height];
		cellStatuses = new int[width, height];
		cellSpriteRenderers = new SpriteRenderer[width, height];

		mines = CellLogic.GenerateRandomMines(mineAmount, width, height);

		im.xCenters = new float[width];
		im.yCenters = new float[height];
		
		for(int x = 0; x < width; x++)
		{
			for(int y = 0; y < height; y++)
			{
				GameObject cell = CellLogic.PlaceCell(x, y, width, height);
				cells[x, y] = cell;
				cellStatuses[x, y] = 0;
				im.xCenters[x] = cell.transform.position.x;
				im.yCenters[y] = cell.transform.position.y;
				if (mines[x, y]) minePositions.Add(new int[3] { x, y, 0 });
			}
		}
	}

	public void EndGame(bool win)
	{
		isAlive = false;
		uim.resetButton.interactable = false;
		uim.openMenuButton.interactable = false;

		tm.StopTimer(false);

		if (!win)
		{
			// Make mine types visible
			
			foreach(int[] minePosition in minePositions)
			{
				if (minePosition[2] == 1) continue;

				int x = minePosition[0];
				int y = minePosition[1];

				cellSpriteRenderers[x, y].sprite = cellNonTriggeredMine;
			}
			
			foreach(int[] missFlaggedPosition in missFlaggedPositions)
			{
				int x = missFlaggedPosition[0];
				int y = missFlaggedPosition[1];

				cellSpriteRenderers[x, y].sprite = cellCrossedMine;
			}
		}
		
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
	
	private int GetHeight(int Default){
		string text = heightInputText.text;
		int number = string.IsNullOrEmpty(text) ? 0 : int.Parse(text);
		if(number <= 0) number = Default;
		return number;
	}
	
	private int GetWidth(int Default){
		string text = widthInputText.text;
		int number = string.IsNullOrEmpty(text) ? 0 : int.Parse(text);
		if(number <= 0) number = Default;
		return number;
	}
	
	private int GetMineAmount(int Default){
		string text = mineAmountInputText.text;
		int number = string.IsNullOrEmpty(text) ? 0 : int.Parse(text);
		if(number <= 0) number = Default;
		return number;
	}
}
