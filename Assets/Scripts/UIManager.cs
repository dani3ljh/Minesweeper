using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	[SerializeField] private Transform uiCanvas;
	
	[Header("Buttons")]
	public Button resetButton;
	public Button openMenuButton;
	
	[Header("Amount Info Texts")]
	[SerializeField] private Text flagAmountText;
	[SerializeField] private Text cellAmountText;
	[SerializeField] private Text widthResultsText;
	[SerializeField] private Text heightResultsText;
	[SerializeField] private Text mineAmountResultsText;

	[Header("End Screens Data")]
	[SerializeField] private GameObject loseEndScreen;
	[SerializeField] private GameObject winEndScreen;
	[SerializeField] private float loseResetDelay;
	[SerializeField] private float winResetDelay;
	
	[Header("Start Screen")]
	[SerializeField] private Animator startScreenAnim;
	
	[Header("Menu Screen")]
	[SerializeField] private GameObject textPopupPrefab;

	private GameManager gm;

	private readonly Dictionary<string, string> textPopups = new Dictionary<string, string>()
	{
		{"normal rules", 
			"In minesweeper each revealed cell has a number. \n" +
			"This number refers to the amount of mines next to it including diagonals. \n" +
			"So an 8 for example would have every cell around it be a mine. \n" +
			"Whilst a 1 only has 1 mine around it. \n" +
			"You can flag cells to indicate that it is a mine."
		},
		{"oneOff rules", 
			"In Lying Mode Cells will be one off of their actual value. \n" +
			"For example if a cell said 2 that either means there are 3 mines around it or 1. \n" +
			"Logically 0s will always be 1, 1s will always be 2 and 8s will always be 7. \n" +
			"This mode is recommended for a lower minecount \n" +
			"Also you can't middle click in this mode."

		},
		{"controls", 
			"There are 3 input types: A left click, a right click, and A middle click. \n" +
			"Press r to reset the game on the current mode. \n" +
			"To left click press the left mouse button and release on the cell you want to mine. \n" +
			"To right click press the right mouse button and release on the cell you want to flag. \n" +
			"To middle click press either the scroll button or left click button on a cleared cell. \n" +
			"Middle clicking on a cell with equal to or more flags around it than its number will clear all un checked cells around it."
		},
		{"credits",
			"Assets \n\n" +
			"Title font: https://fonts.google.com/specimen/Black+Ops+One \n" +
			"Text font: https://www.dafont.com/highvoltage-rough.font \n" +
			"Sound fx from: https://sfxr.me/ \n" +
			"Art Program: Pixel Studio \n\n" +
			"Coding and UI: Daniel Hart \n"
		}
	};

	private void Start()
	{
		gm = gameObject.GetComponent<GameManager>();
	}

	public void SetFlagAmountText(int amount)
	{
		flagAmountText.text = amount.ToString();
	}

	public void SetCellAmountText(int amount)
	{
		cellAmountText.text = amount.ToString();
	}
	
	public void InstantiateWinScreen()
	{
		EndScreen endScreen = Instantiate(winEndScreen, uiCanvas).GetComponent<EndScreen>();

		endScreen.gm = gm;
		endScreen.resetDelay = winResetDelay;
	}
	public void InstantiateLoseScreen()
	{
		EndScreen endScreen = Instantiate(loseEndScreen, uiCanvas).GetComponent<EndScreen>();

		endScreen.gm = gm;
		endScreen.resetDelay = loseResetDelay;
	}

	public void CreateTextPopup(string textKey)
	{
		TextPopup textPopupScript = Instantiate(textPopupPrefab, uiCanvas).GetComponent<TextPopup>();

		// If dictionary doesnt contain key throw exepction, which stops the function call
		if (!textPopups.ContainsKey(textKey)) 
		throw new System.Exception($"Dictionary doesn't contain {textKey}");

		textPopupScript.SetText(textPopups[textKey]);
	}

	public void StartScreenRise()
	{
		startScreenAnim.SetTrigger("Rise");
	}

	public void StartScreenFall()
	{
		startScreenAnim.SetTrigger("Fall");
	}

	public void UpdateResults()
	{
		widthResultsText.text = "Width: " + gm.width.ToString();
		heightResultsText.text = "Height: " + gm.height.ToString();
		mineAmountResultsText.text = "Mines: " + gm.mineAmount.ToString();
	}
}