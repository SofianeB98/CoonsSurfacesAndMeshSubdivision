using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Subdivision
{
    public class CatmullClark : ISubdiviser
    {
        public Model MeshData { get; set; }
        public int Iteration { get; set; }
        public Model Subdivide(int iterations)
        {
            var newModel = new Model();
            var facePoints = new List<Vertex>();
            foreach (var item in MeshData.triangles)
            {
                facePoints.Add(item.ComputeFacePoint());
            }

            var edgePoints = new List<Vertex>();
            foreach (var item in MeshData.edges)
            {
                edgePoints.Add(SubdivisionUtils.GetEdgePoint(item));
            }

            var vertexPoints = new List<Vertex>();
            
            return new Model();

        }

        public Mesh GetMesh()
        {
            throw new System.NotImplementedException();
        }
    }
}