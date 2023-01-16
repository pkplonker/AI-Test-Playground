using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class AStar : MonoBehaviour
{
	private AStarMap aStarMap;
	private List<Node> open;
	private List<Node> closed;
	public Transform startTransform;
	public Transform EndTransform;

	private void Awake()
	{
		aStarMap = GetComponent<AStarMap>();
	}

	private void Start()
	{
		
		
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			var x= CalculatePath(startTransform.position, EndTransform.position);
			Debug.Log(x.Count);
		}
	}

	private List<Vector3> CalculatePath(Vector3 start, Vector3 end)
	{
		var startNode = aStarMap.GetNodeFromLocation(start);
		var endNode = aStarMap.GetNodeFromLocation(end);
		if (startNode == endNode)
		{
			Debug.Log("Already at destination");
			return null;
		}

		if (startNode == null)
		{
			Debug.LogError("Failed to get start node");
			return null;
		}

		if (endNode == null)
		{
			Debug.LogError("Failed to get end node");
			return null;
		}

		open = new List<Node>();
		closed = new List<Node>();
		open.Add(startNode);

		startNode.g = CalculateDistance(startNode, startNode); //dist from start
		startNode.h = CalculateDistance(startNode, endNode);
		while (open.Count > 0 && !closed.Contains(endNode))
		{
			if (open.Count == 0)
			{
				Debug.Log("No more open nodes");
				break;
			}

			open = open.OrderBy(n => n.f).ToList();
			var currentNode = open[0];
			if (open.Contains(currentNode))
				open.Remove(currentNode);
			closed.Add(currentNode);
			if (currentNode == endNode)
				return CalculateWaypoints(currentNode);
			foreach (var neighbour in aStarMap.CalculateNeighbours(currentNode))
			{
				if (!neighbour.walkable) continue;
				if (closed.Contains(neighbour)) continue;
				var g = CalculateDistance(neighbour, startNode);//dist from start
				var h = CalculateDistance(neighbour, endNode);//dist from end
				if (!open.Contains(neighbour))
				{
					neighbour.parent = currentNode;
					neighbour.g = g;
					neighbour.h = h;
					open.Add(neighbour);
				}
				else if (neighbour.f > g + h)
				{
					neighbour.parent = currentNode;
					neighbour.g = g;
					neighbour.h = h;
				}
				
			}
		}

		return CalculateWaypoints(endNode);
	}

	private float CalculateDistance(Node start, Node end) =>
		MathF.Pow(GetDelta(start.x, end.x), 2) + MathF.Pow(GetDelta(start.z, end.z), 2);
	
	private static int GetDelta(int start, int end) => start > end ? start - end : end - start;
	
	private List<Vector3> CalculateWaypoints(Node node)
	{
		if (node.parent == null) return null;
		return null;
	}
}