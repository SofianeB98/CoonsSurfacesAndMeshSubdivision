using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FacetteDeCoons
{
    public List<CourbeDeChaikin> courbes = new List<CourbeDeChaikin>();

    // Plan A-C
    public PlanRegle planAC = new PlanRegle();

    // Plan B-D
    public PlanRegle planBD = new PlanRegle();

    // Plan bilineaire
    public PlanRegle planFromLines = new PlanRegle();

    // Plan final
    public Vector3[,] facettePoints;
    public Mesh mesh;

    public void ComputeFacette(List<CourbeDeChaikin> c)
    {
        this.courbes = c;

        facettePoints = new Vector3[this.courbes[0].points.Count, this.courbes[1].points.Count];

        // Genere le plan A - C
        // Le plan A - C se genere depuis les courbes au index 0 et 2
        var A = courbes[0];
        var C = courbes[2];

        for (int i = 0; i < A.points.Count; i++)
        {
            Edge e = new Edge();
            e.pointA = A.points[i];
            e.pointB = C.points[C.points.Count - 1 - i];
            planAC.AddEdge(e);
        }

        // Genere le plan B - D
        // Le plan B - D se genere depuis les courbes au index 1 et 3
        var B = courbes[1];
        var D = courbes[3];

        for (int i = 0; i < B.points.Count; i++)
        {
            Edge e = new Edge();
            e.pointA = B.points[i];
            e.pointB = D.points[D.points.Count - 1 - i];
            planBD.AddEdge(e);
        }


        // Genere le plan bilineaire

        var p1 = A.points[0];
        var p2 = A.points[A.points.Count - 1];

        float distanceCourbeA = A.GetDistance();
        float ratio = Vector3.Distance(p1, p2) / distanceCourbeA;

        var lineP1P2 = new List<Vector3>();
        var direction = p2 - p1;
        direction.Normalize();

        var startPoints = p1;

        lineP1P2.Add(p1);
        for (int i = 0; i < A.points.Count - 1; i++)
        {
            //var pts = Vector3.Lerp(p1, p2, i / (float)(A.points.Count-1));
            //lineP1P2.Add(pts);

            float d = Vector3.Distance(A.points[i], A.points[i + 1]);
            d *= ratio;
            var pts = startPoints + direction * d;

            startPoints = pts;

            lineP1P2.Add(pts);
        }

        lineP1P2.Add(p2);

        var p3 = C.points[0];
        var p4 = C.points[C.points.Count - 1];

        float distanceCourbeC = C.GetDistance();
        ratio = Vector3.Distance(p3, p4) / distanceCourbeC;

        var lineP3P4 = new List<Vector3>();
        direction = p4 - p3;
        direction.Normalize();
        startPoints = p3;

        lineP3P4.Add(p3);
        for (int i = 0; i < C.points.Count - 1; i++)
        {
            // var pts = Vector3.Lerp(p3, p4, i / (float)(C.points.Count-1));
            // lineP3P4.Add(pts);
            float d = Vector3.Distance(C.points[i], C.points[i + 1]);
            d *= ratio;
            var pts = startPoints + direction * d;

            startPoints = pts;

            lineP3P4.Add(pts);
        }
        lineP3P4.Add(p4);

        for (int i = 0; i < lineP1P2.Count; i++)
        {
            Edge e = new Edge();
            e.pointA = lineP1P2[i];
            e.pointB = lineP3P4[lineP3P4.Count - 1 - i];
            planFromLines.AddEdge(e);
        }

        var p5 = B.points[0];
        var p6 = B.points[C.points.Count - 1];

        float distanceCourbeB = B.GetDistance();
        ratio = Vector3.Distance(p5, p6) / distanceCourbeB;

        var lineP5P6 = new List<Vector3>();
        direction = p6 - p5;
        direction.Normalize();
        
        startPoints = p5;

        lineP5P6.Add(p5);
        for (int i = 0; i < B.points.Count - 1; i++)
        {
            // var pts = Vector3.Lerp(p3, p4, i / (float)(C.points.Count-1));
            // lineP3P4.Add(pts);
            float d = Vector3.Distance(B.points[i], B.points[i + 1]);
            d *= ratio;
            var pts = startPoints + direction * d;

            startPoints = pts;

            lineP5P6.Add(pts);
        }
        lineP5P6.Add(p6);
        
        // Genere le plan final
        for (int i = 0; i < facettePoints.GetLength(0); i++)
        {
            for (int j = 0; j < facettePoints.GetLength(1); j++)
            {
                facettePoints[i, j] = planAC.GetPointsAt(i, j) + planBD.GetPointsAt(j, i) -
                                      planFromLines.GetPointsAt(i, j, ref lineP5P6);
            }
        }

        ComputeMesh();
    }

    private void ComputeMesh()
    {
        mesh = new Mesh();
        Vector3[] vertices = new Vector3[this.facettePoints.GetLength(0) * this.facettePoints.GetLength(1)];
        for (int i = 0; i < this.facettePoints.GetLength(0); i++)
        {
            for (int j = 0; j < this.facettePoints.GetLength(1); j++)
            {
                vertices[i * this.facettePoints.GetLength(0) + j] = facettePoints[i, j];
            }
        }

        mesh.vertices = vertices;

        int m = this.facettePoints.GetLength(0);
        int p = this.facettePoints.GetLength(1);

        var triangles = new List<int>();

        // Triangle
        for (int i = 0; i < m - 1; i++)
        {
            for (int j = 0; j < p - 1; j++)
            {
                int a = i * (p) + j;
                int b = (i + 1) * (p) + j;
                int c = (i + 1) * (p) + j + 1;

                int d = i * (p) + j + 1;

                triangles.Add(a);
                triangles.Add(c);
                triangles.Add(b);

                triangles.Add(a);
                triangles.Add(d);
                triangles.Add(c);
            }
        }

        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        GameObject go = new GameObject("test");
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Standard"));
    }
}