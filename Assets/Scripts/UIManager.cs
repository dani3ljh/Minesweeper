using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	[SerializeField] private Toggle mobileModeButton;
	[SerializeField] private Text flagAmountText;
	[SerializeField] private Text cellAmountText;
	public Button resetButton;

	private Gamemanager gm;

	private void Start()
	{
		gm = gameObject.GetComponent<Gamemanager>();
	}

	public void UpdateMobileMode()
	{
		gm.mobileMode = mobileModeButton.isOn;
	}

	public void SetFlagAmountText(int amount)
	{
		flagAmountText.text = amount.ToString();
	}

	public void SetCellAmountText(int amount)
	{
		cellAmountText.text = amount.ToString();
	}
}