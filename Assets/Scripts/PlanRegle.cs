using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlanRegle
{
    public List<Edge> edges = new List<Edge>();

    public void AddEdge(Edge e)
    {
        edges.Add(e);
    }
    
    public Vector3 GetPointsAt(int u, int v)
    {
        return edges[u].GetAllPoints(edges.Count)[v];
    }
}