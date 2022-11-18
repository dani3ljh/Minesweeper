using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[Header("Board Settings")]
	[HideInInspector] public int width;
	[HideInInspector] public int height;
	[HideInInspector] public int mineAmount;
	[SerializeField] private float loseInstantiateDelay;
	[SerializeField] private float winInstantiateDelay;

	[Header("Objects")]
	[SerializeField] private GameObject cellPrefab;
	[SerializeField] private Transform cellFolder;
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
		
		if(width==0) width = 16;
		if(height==0) height = 13;
		if(mineAmount==0) mineAmount = (int)(width*height*0.193f);
		
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
			}
		}
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
