using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	// Lists of centers of cells
	[HideInInspector] public float[] xCenters;
	[HideInInspector] public float[] yCenters;

	private Gamemanager gm;

	private int width;
	private int height;
	private int mineAmount;
	private bool[,] mines;
	private Sprite cellFlagged;
	private Sprite cellUnchecked;

	void Start()
	{
		gm = gameObject.GetComponent<Gamemanager>();
		cellFlagged = gm.cellFlagged;
		cellUnchecked = gm.cellUnchecked;
		gm.cameraBackgroundColor = Camera.main.backgroundColor;
	}

	void Update()
	{
		if (!gm.isAlive) return;

		// Left Click
		if (Input.GetMouseButtonUp(0))
		{
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			int[] indexes = GetIndexesFromPoint(worldPosition.x, worldPosition.y);
			int x = indexes[0];
			int y = indexes[1];

			if (x == -1 || y == -1) return;

			switch (gm.cellStatuses[x, y])
			{
				case 0:
					gm.MineCell(x, y, width, height);
					break;
				case int n when (n >= 1 && n <= 8):
					MiddleClick(x, y, n);
					break;
			}
		}

		// Right Click
		if (Input.GetMouseButtonUp(1))
		{
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			int[] indexes = GetIndexesFromPoint(worldPosition.x, worldPosition.y);
			int x = indexes[0];
			int y = indexes[1];

			if (x == -1 || y == -1) return;

			switch (gm.cellStatuses[x, y])
			{
				case 0:
					gm.cellSpriteRenderers[x, y].sprite = cellFlagged;
					gm.cellStatuses[x, y] = 10;
					break;
				case 10:
					gm.cellSpriteRenderers[x, y].sprite = cellUnchecked;
					gm.cellStatuses[x, y] = 0;
					break;
			}
		}

		// Middle Click
		if (Input.GetMouseButtonUp(2))
		{
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			int[] indexes = GetIndexesFromPoint(worldPosition.x, worldPosition.y);
			int x = indexes[0];
			int y = indexes[1];

			if (x == -1 || y == -1) return;

			int cellStatus = gm.cellStatuses[x, y];

			MiddleClick(x, y, cellStatus);
		}

		// Reset Button
		if (Input.GetButtonDown("Reset")){
			gm.Start();
		}
	}

	private void MiddleClick(int x, int y, int n)
	{
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
		if (x == 0)
		{
			indexOffsets[2, 0] = 0;
			indexOffsets[4, 0] = 0;
			indexOffsets[7, 0] = 0;
			indexOffsets[2, 1] = 0;
			indexOffsets[4, 1] = 0;
			indexOffsets[7, 1] = 0;
		}
		if (x == width - 1)
		{
			indexOffsets[0, 0] = 0;
			indexOffsets[3, 0] = 0;
			indexOffsets[5, 0] = 0;
			indexOffsets[0, 1] = 0;
			indexOffsets[3, 1] = 0;
			indexOffsets[5, 1] = 0;
		}
		if (y == 0)
		{
			indexOffsets[5, 0] = 0;
			indexOffsets[6, 0] = 0;
			indexOffsets[7, 0] = 0;
			indexOffsets[5, 1] = 0;
			indexOffsets[6, 1] = 0;
			indexOffsets[7, 1] = 0;
		}
		if (y == height - 1)
		{
			indexOffsets[0, 0] = 0;
			indexOffsets[1, 0] = 0;
			indexOffsets[2, 0] = 0;
			indexOffsets[0, 1] = 0;
			indexOffsets[1, 1] = 0;
			indexOffsets[2, 1] = 0;
		}

		for (int i = 0; i < 8; i++)
		{
			int newX = x + indexOffsets[i, 0];
			int newY = y + indexOffsets[i, 1];

			if (gm.cellStatuses[newX, newY] == 10) totalSurroundingFlags++;
		}

		if (totalSurroundingFlags == n)
		{
			for (int i = 0; i < 8; i++)
			{
				int newX = x + indexOffsets[i, 0];
				int newY = y + indexOffsets[i, 1];

				if (gm.cellStatuses[newX, newY] == 0) gm.MineCell(newX, newY, width, height);
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
