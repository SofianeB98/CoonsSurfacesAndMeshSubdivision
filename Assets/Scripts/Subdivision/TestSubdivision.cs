using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subdivision
{
    public class TestSubdivision : MonoBehaviour
    {
        private ISubdiviser subdiviser;
        [SerializeField] private MeshFilter meshFilter;
        private Mesh originalMesh;
        [SerializeField] private int iterations = 2;
        public void Awake()
        {
            originalMesh = meshFilter.mesh;
            this.subdiviser = new LoopSubdivisionSurface(SubdivisionUtils.Weld(meshFilter.mesh, float.Epsilon, meshFilter.mesh.bounds.size.x), iterations);
            this.meshFilter.mesh = subdiviser.GetMesh();

        }

        private void OnGUI()
        {
            if (GUILayout.Button("Subdivide"))
            {
                this.subdiviser.Subdivide(iterations);
                this.meshFilter.mesh = this.subdiviser.GetMesh();
            }

            if (GUILayout.Button("Reset Original"))
            {
                this.meshFilter.mesh = originalMesh;
            }
        }
    }
}