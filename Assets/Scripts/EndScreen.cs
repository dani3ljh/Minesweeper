using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
	[SerializeField] private Animator anim;

	[HideInInspector] public GameManager gm;
	[HideInInspector] public float resetDelay;

	public void OpenMenu()
	{
		gm.OpenMenu();
		Destroy(gameObject);
	}

	public void PlayAgain()
	{
		anim.SetTrigger("Rise");

		gm.Invoke(nameof(gm.SetupGame), resetDelay);

		Invoke(nameof(DestroySelf), resetDelay);
	}

	private void DestroySelf()
	{
		Destroy(gameObject);
	}
}
