using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subdivision
{
    public static class SubdivisionUtils
    {
        public static Vertex GetEdgePoint(Edge e)
        {
            if (e.ept != null)
                return e.ept;
            // Handle Borders edges
            if (e.faces.Count != 2)
            {
                var m = (e.a.position + e.b.position) * 0.5f;
                e.ept = new Vertex(m, e.a.index);
            }
            else
            {
                float alpha = 3.0f / 8f;
                float beta = 1f / 8f;

                var left = e.faces[0].GetOtherVertex(e);
                var right = e.faces[1].GetOtherVertex(e);
                e.ept = new Vertex((e.a.position + e.b.position) * alpha + (left.position + right.position) * beta,
                    e.a.index);
            }

            return e.ept;
        }
        
        public static Mesh Weld(Mesh mesh, float threshold, float bucketStep)
        {
            Vector3[] oldVertices = mesh.vertices;
            Vector3[] newVertices = new Vector3[oldVertices.Length];
            int[] old2new = new int[oldVertices.Length];
            int newSize = 0;

            // Find AABB
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for (int i = 0; i < oldVertices.Length; i++)
            {
                if (oldVertices[i].x < min.x) min.x = oldVertices[i].x;
                if (oldVertices[i].y < min.y) min.y = oldVertices[i].y;
                if (oldVertices[i].z < min.z) min.z = oldVertices[i].z;
                if (oldVertices[i].x > max.x) max.x = oldVertices[i].x;
                if (oldVertices[i].y > max.y) max.y = oldVertices[i].y;
                if (oldVertices[i].z > max.z) max.z = oldVertices[i].z;
            }

            // Make cubic buckets, each with dimensions "bucketStep"
            int bucketSizeX = Mathf.FloorToInt((max.x - min.x) / bucketStep) + 1;
            int bucketSizeY = Mathf.FloorToInt((max.y - min.y) / bucketStep) + 1;
            int bucketSizeZ = Mathf.FloorToInt((max.z - min.z) / bucketStep) + 1;
            List<int>[,,] buckets = new List<int>[bucketSizeX, bucketSizeY, bucketSizeZ];

            // Make new vertices
            for (int i = 0; i < oldVertices.Length; i++)
            {
                // Determine which bucket it belongs to
                int x = Mathf.FloorToInt((oldVertices[i].x - min.x) / bucketStep);
                int y = Mathf.FloorToInt((oldVertices[i].y - min.y) / bucketStep);
                int z = Mathf.FloorToInt((oldVertices[i].z - min.z) / bucketStep);

                // Check to see if it's already been added
                if (buckets[x, y, z] == null) buckets[x, y, z] = new List<int>(); // Make buckets lazily

                for (int j = 0; j < buckets[x, y, z].Count; j++)
                {
                    Vector3 to = newVertices[buckets[x, y, z][j]] - oldVertices[i];
                    if (Vector3.SqrMagnitude(to) < threshold)
                    {
                        old2new[i] = buckets[x, y, z][j];
                        goto skip; // Skip to next old vertex if this one is already there
                    }
                }

                // Add new vertex
                newVertices[newSize] = oldVertices[i];
                buckets[x, y, z].Add(newSize);
                old2new[i] = newSize;
                newSize++;

                skip:;
            }

            // Make new triangles
            int[] oldTris = mesh.triangles;
            int[] newTris = new int[oldTris.Length];
            for (int i = 0; i < oldTris.Length; i++)
            {
                newTris[i] = old2new[oldTris[i]];
            }

            Vector3[] finalVertices = new Vector3[newSize];
            for (int i = 0; i < newSize; i++)
            {
                finalVertices[i] = newVertices[i];
            }

            mesh.Clear();
            mesh.vertices = finalVertices;
            mesh.triangles = newTris;
            mesh.RecalculateNormals();

            return mesh;
        }
        
        public static Vertex[] GetAdjancies(Vertex v)
        {
            var adjancies = new Vertex[v.edges.Count];
            for (int i = 0, n = v.edges.Count; i < n; i++)
            {
                adjancies[i] = v.edges[i].GetOtherVertex(v);
            }

            return adjancies;
        }
        
        public static Vertex GetVertexPoint(Vertex v)
        {
            if (v.updated != null)
                return v.updated;

            var adjancies = GetAdjancies(v);
            var n = adjancies.Length;

            if (n < 3)
            {
                var e0 = v.edges[0].GetOtherVertex(v);
                var e1 = v.edges[1].GetOtherVertex(v);
                float k0 = 3f / 4f;
                float k1 = 1f / 8f;
                v.updated = new Vertex(k0 * v.position + k1 * (e0.position + e1.position), v.index);
            }
            else
            {
                float pi2 = Mathf.PI * 2.0f;
                float k0 = 5f / 8f;
                float k1 = 3f / 8f;
                float k2 = 1f / 4f;

                var alpha = (n == 3) ? (3f / 16f) : ((1f / n) * (k0 - Mathf.Pow(k1 + k2 * Mathf.Cos(pi2 / n), 2f)));

                var np = (1f - n * alpha) * v.position;
                for (int i = 0; i < n; i++)
                {
                    var adj = adjancies[i];
                    np += alpha * adj.position;
                }

                v.updated = new Vertex(np, v.index);
            }
            
            return v.updated;
        }
        
    }
}