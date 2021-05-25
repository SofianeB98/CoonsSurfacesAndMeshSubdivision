using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LignePolygonale
{
    public List<Vector3> points = new List<Vector3>();

    public LignePolygonale()
    {
        points = new List<Vector3>();
    }
    
    public void AddPoint(Vector3 v)
    {
        points.Add(v);
    }
    
}
