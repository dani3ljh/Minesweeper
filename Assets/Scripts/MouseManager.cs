using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
	private Gamemanager gm;

	// Lists of centers of cells
	[HideInInspector] public float[] xCenters;
	[HideInInspector] public float[] yCenters;

	private int width;
	private int height;
	private bool[,] mines;
	private Sprite cellFlagged;
	private Sprite cellUnchecked;

	void Start()
    {
		gm = gameObject.GetComponent<Gamemanager>();

		width = gm.width;
		height = gm.height;
		mines = gm.mines;
		cellFlagged = gm.cellFlagged;
		cellUnchecked = gm.cellUnchecked;
    }

    void Update()
    {
		if (!gm.isAlive) return;

		// Left Click
		if (Input.GetMouseButtonUp(0))
		{
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			int[] indexes = GetIndexesFromPoint(worldPosition.x, worldPosition.y);
			if (indexes[0] == -1 || indexes[1] == -1) return;
			if (gm.cellStatuses[indexes[0], indexes[1]] == 0)
			{
				gm.MineCell(indexes[0], indexes[1], width, height, mines);
			}
		}

		// Right Click
		if (Input.GetMouseButtonUp(1))
		{
			Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			int[] indexes = GetIndexesFromPoint(worldPosition.x, worldPosition.y);
			if (indexes[0] == -1 || indexes[1] == -1) return;
			if (gm.cellStatuses[indexes[0], indexes[1]] == 0)
			{
				gm.cellSpriteRenderers[indexes[0], indexes[1]].sprite = cellFlagged;
				gm.cellStatuses[indexes[0], indexes[1]] = 10;
				return;
			}
			if (gm.cellStatuses[indexes[0], indexes[1]] == 10)
			{
				gm.cellSpriteRenderers[indexes[0], indexes[1]].sprite = cellUnchecked;
				gm.cellStatuses[indexes[0], indexes[1]] = 0;
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
}
