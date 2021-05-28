using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CourbeDeChaikin
{
    public LignePolygonale refLignePolygonale;
    public List<Vector3> points = new List<Vector3>();

    public static CourbeDeChaikin ComputeChaikin(ref LignePolygonale line, Vector2 uv, float oneLessUV,
        int iteration = 2)
    {
        CourbeDeChaikin cdc = new CourbeDeChaikin {refLignePolygonale = line};
        
        
        LignePolygonale copyLine = line;
        for (int i = 0; i < iteration; i++)
        {
            LignePolygonale tmpLine = new LignePolygonale();
            
            for (int j = 0; j < copyLine.points.Count - 1; j++)
            {
                Vector3 pu = Vector3.Lerp(copyLine.points[j], copyLine.points[j + 1], uv.x);
                Vector3 pv = Vector3.Lerp(copyLine.points[j], copyLine.points[j + 1], uv.x + oneLessUV);

                tmpLine.AddPoint(pu);
                tmpLine.AddPoint(pv);
            }

            copyLine = tmpLine;
        }

        cdc.points = copyLine.points;
        cdc.points.Insert(0,line.points[0]);
        cdc.points.Add(line.points[line.points.Count - 1]);

        return cdc;
    }

    public float GetDistance()
    {
        float dist = 0.0f;

        for (int i = 0; i < points.Count - 1; i++)
        {
            dist += Vector3.Distance(points[i], points[i + 1]);
        }

        return dist;
    }
}