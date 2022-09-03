using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
	[SerializeField] private Text timerText;

	private float time;
	private bool isRunning;

	private void Update()
	{
		if (!isRunning) return;

		time += Time.deltaTime;
		timerText.text = SecondsToReadableFormat(time);
	}

	public void ResetTimer()
	{
		time = 0;
		timerText.text = SecondsToReadableFormat(time);
	}

	public void StopTimer(bool reset = true)
	{
		isRunning = false;
		if (reset) ResetTimer();
	}

	public void StartTimer()
	{
		isRunning = true;
	}

	private string SecondsToReadableFormat(float secs)
	{
		TimeSpan t = TimeSpan.FromSeconds(secs);

		return t.Minutes == 0 ?
			string.Format("{0:D2}:{1:D3}", t.Seconds, t.Milliseconds) :
			string.Format("{0}:{1:D2}:{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);
	}
}
