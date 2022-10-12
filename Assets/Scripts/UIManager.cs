using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public Button resetButton;
	
	[SerializeField] private Text flagAmountText;
	[SerializeField] private Text cellAmountText;
	[SerializeField] private GameObject loseEndScreen;
	[SerializeField] private GameObject winEndScreen;
	[SerializeField] private Animator startScreenAnim;
	[SerializeField] private GameObject textPopupPrefab;
	[SerializeField] private float loseResetDelay;
	[SerializeField] private float winResetDelay;
	[SerializeField] private Transform uiCanvas;

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
			"Also you can't middle click in this mode."
		},
		{"controls", 
			"There are 3 input types: A left click, a right click, and A middle click. \n" +
			"Press r to reset the game on the current mode. \n" +
			"To left click press the left mouse button and release on the cell you want to mine. \n" +
			"To right click press the right mouse button and release on the cell you want to flag. \n" +
			"To middle click press either the scroll button or left click button on a cleared cell. \n" +
			"Middle clicking on a cell with equal to or more flags around it than its number will clear all un checked cells around it."
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
		if (!textPopups.ContainsKey(textKey))
		{
			Debug.Log($"Textpopups doesnt contain key {textKey}");
			return;
		}
		
		TextPopup textPopupScript = Instantiate(textPopupPrefab, uiCanvas).GetComponent<TextPopup>();
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
}