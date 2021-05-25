using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Edge
{
    public Vector3 pointA;
    public Vector3 pointB;

    public List<Vector3> GetAllPoints(int divisions)
    {
        List<Vector3> pts = new List<Vector3>();

        for (int i = 0; i < divisions; i++)
        {
            pts.Add(Vector3.Lerp(pointA, pointB, i/(float)(divisions-1)));
        }
        
        return pts;
    }
}