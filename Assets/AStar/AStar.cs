using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    private AStarMap aStarMap;
    private List<Node> open;
    private List<Node> closed;
    public Transform start;
    public Transform end;
    private void Awake()
    {
        aStarMap = GetComponent<AStarMap>();
    }

    private void Start()
    {
        //CalculatePath();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            CalculatePath();
    }

    private void CalculatePath()
    {
        var node = aStarMap.GetNodeFromLocation(start.position);
        if(node==null)return;
        Debug.Log($"Node is {node.x}:{node.z}");
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = aStarMap.GetCellLocationFromIndex(node.x, node.z);
    }
}
