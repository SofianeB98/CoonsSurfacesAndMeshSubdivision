using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subdivision
{

    public enum SubdiviserType
    {
        LOOP,
        CATMULLCLARK,
        BUTTERFLY
    }

    public class TestSubdivision : MonoBehaviour
    {
        public SubdiviserType type;

        private ISubdiviser subdiviser;
        [SerializeField] private MeshFilter meshFilter;
        private Mesh originalMesh;
        [SerializeField] private int iterations = 2;
        public void Awake()
        {
            originalMesh = meshFilter.mesh;
            //this.subdiviser = new LoopSubdivisionSurface(SubdivisionUtils.Weld(meshFilter.mesh, float.Epsilon, meshFilter.mesh.bounds.size.x), iterations);
            //this.subdiviser = new CatmullClark(SubdivisionUtils.Weld(meshFilter.mesh, float.Epsilon, meshFilter.mesh.bounds.size.x), iterations);
            //this.subdiviser = new Bitterfly(SubdivisionUtils.Weld(meshFilter.mesh, float.Epsilon, meshFilter.mesh.bounds.size.x), iterations);
           
            //this.meshFilter.mesh = subdiviser.GetMesh();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Subdivide", GUILayout.Width(150), GUILayout.Height(50)))
            {
                switch (type)
                {
                    case SubdiviserType.LOOP:
                        this.subdiviser = new LoopSubdivisionSurface(SubdivisionUtils.Weld(meshFilter.mesh, float.Epsilon, meshFilter.mesh.bounds.size.x), iterations);
                        break;
                    case SubdiviserType.CATMULLCLARK:
                        this.subdiviser = new CatmullClark(SubdivisionUtils.Weld(meshFilter.mesh, float.Epsilon, meshFilter.mesh.bounds.size.x), iterations);
                        break;
                    case SubdiviserType.BUTTERFLY:
                        this.subdiviser = new Bitterfly(SubdivisionUtils.Weld(meshFilter.mesh, float.Epsilon, meshFilter.mesh.bounds.size.x), iterations);
                        break;
                }
                this.subdiviser.Subdivide(iterations);
                this.meshFilter.mesh = this.subdiviser.GetMesh();
            }

            if (GUILayout.Button("Reset Original", GUILayout.Width(150), GUILayout.Height(50)))
            {
                this.meshFilter.mesh = originalMesh;
            }
        }
    }
}