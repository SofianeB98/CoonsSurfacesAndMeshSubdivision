using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subdivision
{
    public class Vertex
    {
        public Vector3 position;
        public List<Edge> edges;
        public List<Triangle> triangles;

        public Vertex updated;

        public int index;

        public Vertex(Vector3 pos) : this(pos, -1)
        {
            
        }

        public Vertex(Vector3 pos, int index)
        {
            this.position = pos;
            this.index = index;
            this.edges = new List<Edge>();
            this.triangles = new List<Triangle>();
        }

        public void AddEdge(Edge edge)
        {
            this.edges.Add(edge);
        }

        public void AddTriangle(Triangle triangle)
        {
            triangles.Add(triangle);
        }
    }
}