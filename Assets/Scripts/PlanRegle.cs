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
    
    public Vector3 GetPointsAt(int u, int v, ref List<Vector3> c)
    {
        var e = edges[u];
        if (v == 0)
            return e.pointA;
        else if (v == edges.Count)
            return e.pointB;
        
        Vector3 pts = Vector3.zero;

        var direction = e.pointB - e.pointA;
        direction.Normalize();

        var ratio = Vector3.Distance(e.pointA, e.pointB) / Vector3.Distance(c[0], c[c.Count - 1]);
        
        var startPoints = e.pointA;
        for (int i = 0; i != v; i++)
        {
            if (i + 1 >= c.Count)
                return e.pointB;
            
            float d = Vector3.Distance(c[i], c[i + 1]);
            d *= ratio;
            pts = startPoints + direction * d;
            startPoints = pts;
        }
        
        
        return pts;
    }
}