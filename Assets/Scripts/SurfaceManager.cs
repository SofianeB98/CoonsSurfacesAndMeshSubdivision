using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SurfaceManager : MonoBehaviour
{
    public Camera mainCam;

    [Header("Ligne Polygonale")] public List<LignePolygonale> lignes = new List<LignePolygonale>();
    public LignePolygonale currentEditedLine = new LignePolygonale();
    public float addPointDistance = 5.0f;

    [Header("Chaikin")] public int iteration = 2;
    public Vector2 uv = new Vector2(0.33f, 0.33f);

    public float oneLessUV
    {
        get { return 1.0f - (uv.x + uv.y); }
    }

    public List<CourbeDeChaikin> courbes = new List<CourbeDeChaikin>();

    [Header("Coons")] public FacetteDeCoons facette;


    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
            AddPoint();

        if (Input.GetKeyUp(KeyCode.O))
        {
            lignes.Add(currentEditedLine);
            courbes.Add(CourbeDeChaikin.ComputeChaikin(ref currentEditedLine, uv, oneLessUV, iteration));
            currentEditedLine = new LignePolygonale();
        }
    }


    public void AddPoint()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        Debug.Log(mousePos + " mouse pos");
        Vector3 p = mainCam.ScreenToWorldPoint(mousePos);
        Debug.Log(p + " in world spacce");
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
    }
}