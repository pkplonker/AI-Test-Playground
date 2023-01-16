using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarMap : MonoBehaviour
{
	public Node[,] map;
	public int nodeSize;
	public Vector3 mapBounds;
	public LayerMask hitMask;
	private int mapWidth;
	private int mapHeight;

	private void Awake()
	{
		GenerateMapData();
	}

	private void GenerateMapData()
	{
		mapWidth = Mathf.RoundToInt(mapBounds.x / nodeSize);
		mapHeight = Mathf.RoundToInt(mapBounds.z / nodeSize);
		map = new Node[mapWidth, mapHeight];
		var bottomLeft = new Vector2(transform.position.x - ((float) mapWidth / 2),
			transform.position.y - ((float) mapHeight / 2));
		for (var z = 0; z < mapHeight; z++)
		{
			for (var x = 0; x < mapWidth; x++)
			{
				var rayDistance = 10f;

				Debug.DrawRay(GetCellLocationFromIndex(x, z) + new Vector3(0, rayDistance, 0),
					Vector3.down * rayDistance * 2, Color.blue);
				RaycastHit hitData;
				var centre = GetCellLocationFromIndex(x, z) + new Vector3(0, rayDistance, 0);
				var boxSize = new Vector3((float) nodeSize / 2, 1, (float) nodeSize / 2);

				var hit = !Physics.BoxCast(centre, boxSize, Vector3.down, Quaternion.identity, rayDistance, hitMask);

				if (hit) Debug.Log($"hit, {x}:{z}");
				else Debug.LogWarning($"Not hit, {x}:{z}");
				map[x, z] = new Node(x, z, hit);
			}
		}
	}

	public Vector3 GetCellLocationFromIndex(int x, int z)
	{
		var bottomLeft = GetBottomLeft();
		var pos = new Vector3(bottomLeft.x + (x * nodeSize) + (float) nodeSize / 2, 0,
			bottomLeft.y + (z * nodeSize) + (float) nodeSize / 2);
		return pos;
	}

	private Vector2 GetBottomLeft() => new(transform.position.x - ((float) mapWidth / 2),
		transform.position.y - ((float) mapHeight / 2));


	public Node GetNodeFromLocation(Vector3 location)
	{
		var bottomLeft = GetBottomLeft();
		if (location.x < bottomLeft.x || location.x > bottomLeft.x + (nodeSize * mapWidth))
		{
			Debug.Log("Outside of bounds X");
			return null;
		}
		if (location.y < bottomLeft.y || location.y > bottomLeft.y + (nodeSize * mapHeight))
		{
			Debug.Log("Outside of bounds Y");
			return null;
		}
		

		return null;//update
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, mapBounds);
		if (map == null) return;
		for (var z = 0; z < mapHeight; z++)
		{
			for (var x = 0; x < mapWidth; x++)
			{
				Gizmos.color = map[x, z].walkable ? Color.green : Color.red;
				var loc = GetCellLocationFromIndex(x, z);
				Gizmos.DrawWireCube(GetCellLocationFromIndex(x, z), new Vector3(nodeSize, 0.5f, nodeSize));
			}
		}
	}
}

public class Node
{
	public int x;
	public int z;
	public bool walkable;

	public Node(int x, int z, bool walkable)
	{
		this.x = x;
		this.z = z;
		this.walkable = walkable;
	}
}