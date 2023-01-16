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
        CalculatePath();
    }

    private void CalculatePath()
    {
        throw new NotImplementedException();
    }
}
