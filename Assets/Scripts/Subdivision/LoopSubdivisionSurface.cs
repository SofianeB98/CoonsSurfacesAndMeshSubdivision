using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subdivision
{
    public class LoopSubdivisionSurface : ISubdiviser
    {
        public Model MeshData { get; set; }
        public int Iteration { get; set; }

        public Mesh GetMesh()
        {
            return MeshData.Build();
        }

        public LoopSubdivisionSurface(Model model, int iteration)
        {
            this.MeshData = model;
            this.Iteration = iteration;
        }

        public LoopSubdivisionSurface(Mesh mesh, int iteration)
        {
            //var welded = CatmullClark.Weld(mesh, float.Epsilon, mesh.bounds.size.x);
            this.MeshData = new Model(mesh);
            this.Iteration = iteration;
        }

        public Model Subdivide(int iterations)
        {
            for (int i = 0; i < this.Iteration; i++)
            {
                this.MeshData = Divide(this.MeshData);
            }
            Debug.Log(MeshData);
            return MeshData;
        }

        public Model Divide(Model model)
        {
            var newModel = new Model();

            for (int i = 0, n = model.triangles.Count; i < n; i++)
            {
                if(i >= model.triangles.Count - 1)
                    Debug.Log("Out of Range");
                var f = model.triangles[i];
                
                var ne0 = SubdivisionUtils.GetEdgePoint(f.e0);
                var ne1 = SubdivisionUtils.GetEdgePoint(f.e1);
                var ne2 = SubdivisionUtils.GetEdgePoint(f.e2);
                                                
                var nv0 = SubdivisionUtils.GetVertexPoint(f.v0);
                var nv1 = SubdivisionUtils.GetVertexPoint(f.v1);
                var nv2 = SubdivisionUtils.GetVertexPoint(f.v2);

                newModel.AddTriangle(nv0, ne0, ne2);
                newModel.AddTriangle(ne0, nv1, ne1);
                newModel.AddTriangle(ne0, ne1, ne2);
                newModel.AddTriangle(ne2, ne1, nv2);
            }

            return newModel;
        }

        

       
    }
}