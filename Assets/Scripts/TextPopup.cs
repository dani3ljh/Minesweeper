using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPopup : MonoBehaviour
{
	[SerializeField] private Text textBox;

	private Animator anim;

	private void Start()
	{
		anim = GetComponent<Animator>();
	}

	public void SetText(string newText)
	{
		textBox.text = newText;
	}

	public void ReturnToMainMenu()
	{
		anim.SetTrigger("Rise");
		Destroy(gameObject, 0.5f);
	}

}
