using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subdivision
{
    public class Triangle
    {
        public Vertex v0, v1, v2;
        public Edge e0, e1, e2;

        private Vertex facePoint;
        
        public Triangle(Vertex v0, Vertex v1, Vertex v2, Edge e0, Edge e1, Edge e2)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            this.e0 = e0;
            this.e1 = e1;
            this.e2 = e2;
        }

        public Vertex GetOtherVertex(Edge e)
        {
            if (!e.Contains(v0))
                return v0;
            if (!e.Contains(v1)) 
                return v1;
            return v2;
        }

        public Vertex ComputeFacePoint()
        {
            var average = (v0.position + v1.position + v2.position) / 3;
            facePoint = new Vertex(average);
            return facePoint;
        }
    }
}