using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SurfaceManager : MonoBehaviour
{
    [Header("Construction Helper")] public Camera mainCam;
    public Transform creationCamPos = null;
    public Transform finalCamPos = null;


    [Header("Ligne Polygonale")] public float addPointDistance = 5.0f;
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

    [Header("Coons")] public FacetteDeCoons facette;

    private void Start()
    {
        this.mainCam.transform.position = this.creationCamPos.position;
        this.mainCam.transform.rotation = this.creationCamPos.rotation;
    }

    private void Update()
    {
        if (lignes.Count >= 4 && facette.courbes.Count <= 0)
        {
            this.mainCam.transform.position = this.finalCamPos.position;
            this.mainCam.transform.rotation = this.finalCamPos.rotation;

            //UpdateCourbes();

            facette.ComputeFacette(courbes);

            return;
        }

        if (lignes.Count >= 4)
            return;


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
            var dirFirstToLastPts =
                (currentEditedLine.points[maxPointsLine - 1] - currentEditedLine.points[0]).normalized;
            float dot = Vector3.Dot(dirFirstToLastPts, this.mainCam.transform.right);

            if (dot > 0.0f)
                this.mainCam.transform.Rotate(Vector3.up, 90.0f);
            else
                this.mainCam.transform.Rotate(Vector3.up, -90.0f);

            lignes.Add(currentEditedLine);
            courbes.Add(CourbeDeChaikin.ComputeChaikin(ref currentEditedLine, uv, oneLessUV, iteration));
            currentEditedLine = new LignePolygonale();
            currentEditedLine.AddPoint(lignes[lignes.Count - 1].points[maxPointsLine - 1]);
        }
    }


    private void AddPoint()
    {
        Plane p = new Plane(mainCam.transform.forward * -1, mainCam.transform.position + mainCam.transform.forward * addPointDistance);
        var ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (p.Raycast(ray, out float e))
        {
            var points = ray.GetPoint(e);
            currentEditedLine.AddPoint(points);
        }
        
        // var mousePos = Input.mousePosition;
        // mousePos.z = mainCam.nearClipPlane;
        // Vector3 p = mainCam.ScreenToWorldPoint(mousePos);
        // p += (p - mainCam.transform.position).normalized * addPointDistance;
    }

    private void UpdateCourbes()
    {
        var lineCenter = Vector3.zero;
        var courbeCenter = Vector3.zero;

        var dir = Vector3.zero;


        // Dir entre A et B
        dir = courbes[1].points[0] - courbes[0].points[courbes[0].points.Count - 1];
        for (int i = 0; i < courbes[0].points.Count; i++)
        {
            courbes[0].points[i] += dir;
            courbes[2].points[i] += dir;
            courbes[3].points[i] += dir;
        }

        // Dir entre B et C
        dir = courbes[2].points[0] - courbes[1].points[courbes[1].points.Count - 1];
        for (int i = 0; i < courbes[0].points.Count; i++)
        {
            courbes[0].points[i] += dir;
            courbes[1].points[i] += dir;
            courbes[3].points[i] += dir;
        }

        // dir entre C et D
        dir = courbes[3].points[0] - courbes[2].points[courbes[2].points.Count - 1];
        for (int i = 0; i < courbes[0].points.Count; i++)
        {
            courbes[0].points[i] += dir;
            courbes[1].points[i] += dir;
            courbes[2].points[i] += dir;
        }

        var ptsAD = courbes[0].points[0] + courbes[3].points[courbes[3].points.Count - 1];
        ptsAD *= 0.5f;
        courbes[0].points[0] = ptsAD;
        courbes[3].points[courbes[3].points.Count - 1] = ptsAD;

        for (int i = 0; i < courbes[0].points.Count; i++)
        {
            courbeCenter += courbes[0].points[i];
            courbeCenter += courbes[1].points[i];
            courbeCenter += courbes[2].points[i];
            courbeCenter += courbes[3].points[i];
        }

        courbeCenter *= (1.0f / (courbes[0].points.Count * 4));

        int total = 0;
        for (int i = 0; i < lignes.Count; i++)
        {
            for (int j = 0; j < lignes[i].points.Count; j++)
            {
                lineCenter += lignes[i].points[j];
                total++;
            }
        }

        lineCenter *= (1.0f / (float) total);

        dir = lineCenter - courbeCenter;
        for (int i = 0; i < courbes[0].points.Count; i++)
        {
            courbes[0].points[i] += dir;
            courbes[1].points[i] += dir;
            courbes[2].points[i] += dir;
            courbes[3].points[i] += dir;
        }
    }

    /// <summary>
    /// Quand on divise le 3e surface il faut diviser le segment selon les valeur U et V
    /// afin d'avoir un truc homogène 
    /// </summary>
    
    private void OnDrawGizmos()
    {
        if (currentEditedLine.points.Count > 0)
        {
            Gizmos.color = Color.cyan;
            foreach (var p in currentEditedLine.points)
            {
                Gizmos.DrawSphere(p, 0.05f);
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
                    Gizmos.DrawSphere(p, 0.05f);
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
                    Gizmos.DrawSphere(p, 0.05f);
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
                    Gizmos.DrawWireSphere(facette.facettePoints[i, j], 0.05f);
                }
            }
        }
    }
}