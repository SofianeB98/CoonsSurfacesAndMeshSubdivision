using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SurfaceManager : MonoBehaviour
{
    public Camera mainCam;

    [Header("Ligne Polygonale")] 
    public float addPointDistance = 5.0f;
    public int maxPointsLine = 4;
    public List<LignePolygonale> lignes = new List<LignePolygonale>();
    public LignePolygonale currentEditedLine = new LignePolygonale();

    [Header("Chaikin")] public int iteration = 2;
    public Vector2 uv = new Vector2(0.33f, 0.33f);

    public float oneLessUV
    {
        get { return 1.0f - (uv.x + uv.y); }
    }

    public List<CourbeDeChaikin> courbes = new List<CourbeDeChaikin>();

    [Header("Coons")] 
    public FacetteDeCoons facette;


    private void Update()
    {
        if (lignes.Count >= 4 && facette.courbes.Count <= 0)
        {
            facette.ComputeFacette(courbes);
            return;
        }
        
        if (Input.GetMouseButtonUp(0))
            AddPoint();

        if (currentEditedLine.points.Count + 1 == maxPointsLine &&
            lignes.Count == 3)
        {
            currentEditedLine.AddPoint(lignes[0].points[0]);
            lignes.Add(currentEditedLine);
            courbes.Add(CourbeDeChaikin.ComputeChaikin(ref currentEditedLine, uv, oneLessUV, iteration));
            currentEditedLine = new LignePolygonale();
        }
        
        if (currentEditedLine.points.Count == maxPointsLine)
        {
            lignes.Add(currentEditedLine);
            courbes.Add(CourbeDeChaikin.ComputeChaikin(ref currentEditedLine, uv, oneLessUV, iteration));
            currentEditedLine = new LignePolygonale();
            currentEditedLine.AddPoint(lignes[lignes.Count - 1].points[maxPointsLine - 1]);
        }
    }


    public void AddPoint()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 p = mainCam.ScreenToWorldPoint(mousePos);
        p += (p - mainCam.transform.position).normalized * addPointDistance;
        currentEditedLine.AddPoint(p);
    }

    private void OnDrawGizmos()
    {
        if (currentEditedLine.points.Count > 0)
        {
            Gizmos.color = Color.cyan;
            foreach (var p in currentEditedLine.points)
            {
                Gizmos.DrawSphere(p, 0.1f);
            }

            for (int i = 0; i < currentEditedLine.points.Count - 1; i++)
            {
                Gizmos.DrawLine(currentEditedLine.points[i], currentEditedLine.points[i + 1]);
            }
        }

        if (lignes.Count > 0)
        {
            Gizmos.color = Color.cyan;
            foreach (var c in lignes)
            {
                foreach (var p in c.points)
                {
                    Gizmos.DrawSphere(p, 0.1f);
                }

                for (int i = 0; i < c.points.Count - 1; i++)
                {
                    Gizmos.DrawLine(c.points[i], c.points[i + 1]);
                }
            }
        }

        if (courbes.Count > 0)
        {
            Gizmos.color = Color.red;
            foreach (var c in courbes)
            {
                foreach (var p in c.points)
                {
                    Gizmos.DrawSphere(p, 0.1f);
                }

                for (int i = 0; i < c.points.Count - 1; i++)
                {
                    Gizmos.DrawLine(c.points[i], c.points[i + 1]);
                }
            }
        }

        if (facette.courbes.Count > 0)
        {
            for (int i = 0; i < facette.facettePoints.GetLength(0); i++)
            {
                for (int j = 0; j < facette.facettePoints.GetLength(1); j++)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(facette.facettePoints[i,j], 0.2f);
                }
            }
        }
    }
}