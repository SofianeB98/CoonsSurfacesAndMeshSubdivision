using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subdivision
{
    public interface ISubdiviser
    {
        public Model MeshData { get; set; }
        public int Iteration { get; set; }
        public Model Subdivide(int iterations);

        public Mesh GetMesh();
    }
    public class SubdivisionSurface
    {
    }
}