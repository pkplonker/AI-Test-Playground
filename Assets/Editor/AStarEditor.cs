using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AStar))]
public class AStarEditor : Editor
{
	private void OnSceneGUI()
	{
		var aStar = (AStar) target;
		var points = aStar.points;
		var open = aStar.open;
		var closed = aStar.closed;


		if (points != null && points.Count > 0)
		{
			Handles.BeginGUI();
			for (int i = 0; i < points.Count; i++)
			{
				Handles.Label(points[i], i.ToString());


				Handles.EndGUI();
			}
		}
	}
}