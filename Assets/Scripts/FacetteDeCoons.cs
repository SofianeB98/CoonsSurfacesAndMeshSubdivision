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
        var lineP1P2 = new List<Vector3>();
        for (int i = 0; i < A.points.Count; i++)
        {
            var pts = Vector3.Lerp(p1, p2, i / (float)(A.points.Count-1));
            lineP1P2.Add(pts);
        }
        
        var p3 = C.points[0];
        var p4 = C.points[C.points.Count - 1];
        var lineP3P4 = new List<Vector3>();
        for (int i = 0; i < C.points.Count; i++)
        {
            var pts = Vector3.Lerp(p3, p4, i / (float)(C.points.Count-1));
            lineP3P4.Add(pts);
        }
        
        for (int i = 0; i < lineP1P2.Count; i++)
        {
            Edge e = new Edge();
            e.pointA = lineP1P2[i];
            e.pointB = lineP3P4[lineP3P4.Count - 1 - i];
            planFromLines.AddEdge(e);
        }
        
        // Genere le plan final
        for (int i = 0; i < facettePoints.GetLength(0); i++)
        {
            for (int j = 0; j < facettePoints.GetLength(1); j++)
            {
                facettePoints[i, j] = planAC.GetPointsAt(i, j) + planBD.GetPointsAt(j, i) - planFromLines.GetPointsAt(i,j);
            }
        }
    }
    
}