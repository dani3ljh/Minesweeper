using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	// Lists of centers of cells
	[HideInInspector] public float[] xCenters;
	[HideInInspector] public float[] yCenters;

	private Gamemanager gm;
	private UIManager uim;

	private int width;
	private int height; 
	private int mineAmount;
	private bool[,] mines;
	private Sprite cellFlagged;
	private Sprite cellUnchecked;
	private float startTime;

	void Start()
	{
		gm = gameObject.GetComponent<Gamemanager>();
		uim = gameObject.GetComponent<UIManager>();

		cellFlagged = gm.cellFlagged;
		cellUnchecked = gm.cellUnchecked;
		gm.cameraBackgroundColor = Camera.main.backgroundColor;
	}

	void Update()
	{
		if (!gm.isAlive) return;

		if (Input.GetMouseButtonDown(0))
		{
			startTime = Time.time;
		}

		// Reset Button
		if (Input.GetButtonDown("Reset")){
			gm.SetupGame();
		}

		if (!(Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))) return;

		Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		int[] indexes = GetIndexesFromPoint(worldPosition.x, worldPosition.y);
		int x = indexes[0];
		int y = indexes[1];

		if (x == -1 || y == -1) return;

		// Left Click
		if (Input.GetMouseButtonUp(0))
		{
			LeftClick(x, y);
		}

		// Right Click
		if (Input.GetMouseButtonUp(1))
		{
			RightClick(x, y);
		}

		// Middle Click
		if (Input.GetMouseButtonUp(2))
		{
			MiddleClick(x, y);
		}
	}

	private void LeftClick(int x, int y)
	{
		switch (gm.cellStatuses[x, y])
		{
			case 0:
				gm.MineCell(x, y);
				break;
			case int n when (n >= 1 && n <= 8):
				MiddleClick(x, y);
				break;
		}
	}

	private void RightClick(int x, int y)
	{
		switch (gm.cellStatuses[x, y])
		{
			case 0:
				gm.cellSpriteRenderers[x, y].sprite = cellFlagged;
				gm.cellStatuses[x, y] = 10;
				gm.minesNotFlagged--;
				break;
			case 10:
				gm.cellSpriteRenderers[x, y].sprite = cellUnchecked;
				gm.cellStatuses[x, y] = 0;
				gm.minesNotFlagged++;
				break;
		}
		uim.SetFlagAmountText(gm.minesNotFlagged);
	}

	private void MiddleClick(int x, int y)
	{
		if (gm.gameMode == "oneOff") return;
		int totalSurroundingFlags = 0;

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
			int newX = x + indexOffsets[i, 0];
			int newY = y + indexOffsets[i, 1];

			if (newX < 0 || newY < 0 || newX >= width || newY >= height)
			{
				continue;
			}

			if (gm.cellStatuses[newX, newY] == 10) totalSurroundingFlags++;
		}

		if (totalSurroundingFlags == gm.cellStatuses[x, y])
		{
			for (int i = 0; i < 8; i++)
			{
				int newX = x + indexOffsets[i, 0];
				int newY = y + indexOffsets[i, 1];

				if (newX < 0 || newY < 0 || newX >= width || newY >= height)
				{
					continue;
				}

				if (gm.cellStatuses[newX, newY] == 0) gm.MineCell(newX, newY);
			}
		}
	}

	private int[] GetIndexesFromPoint(float x, float y)
	{
		int[] indexes = new int[2];
		float scale = 10f / height;
		int xIndex = -1;
		int yIndex = -1;
		for (int i = 0; i < width; i++)
		{
			float differance = Mathf.Abs(x - xCenters[i]);
			if (differance <= (scale / 2))
			{
				xIndex = i;
				break;
			}
		}
		for (int i = 0; i < height; i++)
		{
			float differance = Mathf.Abs(y - yCenters[i]);
			if (differance <= (scale / 2))
			{
				yIndex = i;
				break;
			}
		}
		indexes[0] = xIndex;
		indexes[1] = yIndex;
		return indexes;
	}

	public void SetVariables(int w, int h, int mA, bool[,] m)
	{
		width = w;
		height = h;
		mineAmount = mA;
		mines = m;
	}
}