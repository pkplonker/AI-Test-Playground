using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class AStar : MonoBehaviour
{
	public static AStarMap aStarMap { get; set; }
	public static List<Node> open { get; private set; } = new();
	public static List<Node> closed { get; private set; } = new();
	private static Node startNode = null;
	private static Node endNode = null;
	private void Awake() => aStarMap = GetComponent<AStarMap>();


	public static List<Vector3> points { get; private set; } = new();
	


	private void OnDrawGizmos()
	{
		if (open.Count != 0)
		{
			for (int i = 0; i < open.Count; i++)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(aStarMap.GetCellLocationFromNode(open[i]), aStarMap.nodeSize * 0.4f);
			}
		}

		if (closed.Count != 0)
		{
			for (int i = 0; i < closed.Count; i++)
			{
				Gizmos.color = Color.black;
				Gizmos.DrawSphere(aStarMap.GetCellLocationFromNode(closed[i]), aStarMap.nodeSize * 0.4f);
			}
		}

		if (points.Count != 0)
		{
			for (int i = 0; i < points.Count; i++)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(points[i], aStarMap.nodeSize * 0.4f);
			}
		}

		if (startNode != null)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(aStarMap.GetCellLocationFromIndex(startNode.x, startNode.z), aStarMap.nodeSize * 0.8f);
		}

		if (endNode != null)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(aStarMap.GetCellLocationFromIndex(endNode.x, endNode.z), aStarMap.nodeSize * 0.8f);
		}
	}

	public static List<Vector3> CalculatePath(Vector3 start, Vector3 end)
	{
		aStarMap.ClearNodes();
		startNode = aStarMap.GetNodeFromLocation(start);
		endNode = aStarMap.GetNodeFromLocation(end);
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
		if (startNode == endNode)
		{
			Debug.Log("Already at destination");
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
				var g = CalculateDistance(neighbour, currentNode) + currentNode.g; //dist from start
				var h = CalculateDistance(neighbour, endNode);
				//dist from end
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

	private static  float CalculateDistance(Node start, Node end)
	{
		var distanceX = Mathf.Abs(start.x - end.x);
		var distanceY = Mathf.Abs(start.z - end.z);

		if (distanceX > distanceY)
			return 1.41421f * distanceY + 1 * (distanceX - distanceY);
		return 1.41421f * distanceX + 1 * (distanceY - distanceX);
	}

	private static List<Vector3> CalculateWaypoints(Node node)
	{
		if (node.parent == null) return null;
		List<Vector3> waypoints = new();
		while (node.parent != null)
		{
			if (waypoints.Count > aStarMap.GetNodeCount())
			{
				Debug.LogError("in valid waypoint route");
				return null;
			}

			waypoints.Add(aStarMap.GetCellLocationFromIndex(node.x, node.z));
			node = node.parent;
		}

		waypoints.Reverse();
		return waypoints;
	}
}