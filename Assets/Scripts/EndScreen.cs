using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
	[SerializeField] private Animator anim;

	[HideInInspector] public Gamemanager gm;
	[HideInInspector] public float resetDelay;

	public void PlayAgain()
	{
		anim.SetTrigger("Rise");

		gm.Invoke(nameof(gm.Start), resetDelay);

		Invoke(nameof(DestroySelf), resetDelay);
	}

	private void DestroySelf()
	{
		Destroy(gameObject);
	}
}
