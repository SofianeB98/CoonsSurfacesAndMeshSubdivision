using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subdivision
{
    public class Edge
    {
        public Vertex a;
        public Vertex b;

        public List<Triangle> faces;
        public Vertex ept;

        public Edge(Vertex a, Vertex b)
        {
            this.a = a;
            this.b = b;
            this.faces = new List<Triangle>();
        }

        public void AddTriangle(Triangle f)
        {
            faces.Add(f);
        }

        public bool Contains(Vertex v)
        {
            return v == a || v == b;
        }

        public Vertex GetOtherVertex(Vertex v)
        {
            if (a == v)
                return b;
            return a;
        }
    }
}