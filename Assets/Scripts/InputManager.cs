using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	// Lists of centers of cells
	[HideInInspector] public float[] xCenters;
	[HideInInspector] public float[] yCenters;

	private GameManager gm;
	private UIManager uim;

	private Sprite cellFlagged;
	private Sprite cellUnchecked;
	private float startTime;

	void Start()
	{
		gm = gameObject.GetComponent<GameManager>();
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
		
		int width = gm.width;
		int height = gm.height;
		Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		int[] indexes = GetIndexesFromPoint(worldPosition.x, worldPosition.y, width, height);
		int x = indexes[0];
		int y = indexes[1];

		if (x == -1 || y == -1) return;

		// Left Click
		if (Input.GetMouseButtonUp(0))
		{
			LeftClick(x, y, width, height);
		}

		// Right Click
		if (Input.GetMouseButtonUp(1))
		{
			RightClick(x, y);
		}

		// Middle Click
		if (Input.GetMouseButtonUp(2))
		{
			MiddleClick(x, y, width, height);
		}
	}

	private void LeftClick(int x, int y, int width, int height)
	{
		switch (gm.cellStatuses[x, y])
		{
			case 0:
				CellLogic.MineCell(x, y, width, height);
				break;
			case int n when (n >= 1 && n <= 8):
				MiddleClick(x, y, width, height);
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

	private void MiddleClick(int x, int y, int width, int height)
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

				if (gm.cellStatuses[newX, newY] == 0) CellLogic.MineCell(newX, newY, gm.width, gm.height);
			}
		}
	}

	private int[] GetIndexesFromPoint(float x, float y, int width, int height)
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
}