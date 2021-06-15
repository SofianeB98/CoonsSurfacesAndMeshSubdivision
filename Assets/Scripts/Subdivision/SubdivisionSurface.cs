using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subdivision
{
    public interface ISubdiviser
    {
        Model MeshData { get; set; }
        int Iteration { get; set; }
        Model Subdivide(int iterations);

        Mesh GetMesh();
    }
    public class SubdivisionSurface
    {
    }
}