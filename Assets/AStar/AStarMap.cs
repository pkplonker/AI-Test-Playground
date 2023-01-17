using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AStarMap : MonoBehaviour
{
	public Node[,] map;
	public float nodeSize;
	public Vector3 mapBounds;
	public LayerMask hitMask;
	private int mapWidth;
	private int mapHeight;
	public bool debug;

	private void Awake()
	{
		GenerateMapData();
	}

	private void GenerateMapData()
	{
		mapWidth = Mathf.RoundToInt(mapBounds.x / nodeSize);
		mapHeight = Mathf.RoundToInt(mapBounds.z / nodeSize);
		map = new Node[mapWidth, mapHeight];
		for (var z = 0; z < mapHeight; z++)
		{
			for (var x = 0; x < mapWidth; x++)
			{
				var rayDistance = 10f;
				map[x, z] = new Node(x, z,
					!Physics.BoxCast(GetCellLocationFromIndex(x, z) + new Vector3(0, rayDistance, 0),
						new Vector3(nodeSize / 2, 1.0f, nodeSize / 2), Vector3.down, Quaternion.identity, rayDistance,
						hitMask));
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

	public Vector3 GetCellLocationFromNode(Node node) => GetCellLocationFromIndex(node.x, node.z);

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

		var xDistanceFromBL = bottomLeft.x < location.x ? bottomLeft.x - location.x : location.x - bottomLeft.x;
		var yDistanceFromBL = bottomLeft.y < location.z ? bottomLeft.y - location.z : location.z - bottomLeft.y;
		var x = Mathf.Abs(Mathf.RoundToInt(xDistanceFromBL / nodeSize));
		var y = Mathf.Abs(Mathf.RoundToInt(yDistanceFromBL / nodeSize));

		if (x >= mapWidth || y >= mapHeight)
		{
			Debug.LogWarning("Trying to access out of bounds");
			return null;
		}

		return map[x, y]; //update
	}

	private void OnDrawGizmos()
	{
		if (!debug) return;
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, mapBounds);
		if (map == null) return;
		for (var z = 0; z < mapHeight; z++)
		{
			for (var x = 0; x < mapWidth; x++)
			{
				Gizmos.color = map[x, z].walkable ? Color.green : Color.red;
				var loc = GetCellLocationFromIndex(x, z);
				Gizmos.DrawCube(GetCellLocationFromIndex(x, z) - new Vector3(0, 0.35f, 0),
					new Vector3(nodeSize - 0.1f, 0.1f, nodeSize - 0.1f));
			}
		}
	}

	public IEnumerable<Node> CalculateNeighbours(Node target)
	{
		var neighbours = new List<Node>();
		for (var z = -1; z < 2; z++)
		{
			for (var x = -1; x < 2; x++)
			{
				if (x == 0 && z == 0) continue;
				var currentX = target.x + x;
				var currentZ = target.z + z;
				if (currentX >= mapWidth || currentX < 0 || currentZ >= mapWidth || currentZ < 0) continue;
				neighbours.Add(map[currentX, currentZ]);
			}
		}

		return neighbours;
	}

	public int GetNodeCount() => mapWidth * mapHeight;

	public void ClearNodes()
	{
		for (var z = 0; z < mapHeight; z++)
		{
			for (var x = 0; x < mapWidth; x++)
			{
				map[x, z].Clear();
			}
		}
	}
}

public class Node
{
	public int x;
	public int z;
	public bool walkable;
	public float f => g + h;

	public float g; //distance from start
	public float h; //estimated distance from end
	public Node parent = null;

	public Node(int x, int z, bool walkable)
	{
		this.x = x;
		this.z = z;
		this.walkable = walkable;
	}

	public override string ToString()
	{
		return $"Node [{x}:{z}] - f{f}, g{g}, h{h}";
	}

	public void Clear()
	{
		g = 0;
		h = 0;
		parent = null;
	}
}