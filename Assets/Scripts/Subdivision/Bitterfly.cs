using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Subdivision
{
    public class Bitterfly : ISubdiviser
    {
        public Model MeshData { get; set; }
        public int Iteration { get; set; }


        public Bitterfly(Mesh mesh, int iteration)
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
            
            foreach (Vertex v in model.vertices) 
                newModel.AddVertex(v.position);
            
            foreach (Edge e in model.edges) 
                newModel.AddVertex(GetEdgePoint(e));

            int vSize = model.vertices.Count;

            foreach (Triangle f in model.triangles)
            {
                var vertices = f.GetVertices().ToList();
                var edges = f.GetEdges().ToList();

                int e0 = model.edges.IndexOf(edges[0]);
                int e1 = model.edges.IndexOf(edges[1]);
                int e2 = model.edges.IndexOf(edges[2]);

                int v0 = model.vertices.IndexOf(vertices[0]);
                int v1 = model.vertices.IndexOf(vertices[1]);
                int v2 = model.vertices.IndexOf(vertices[2]);
                
                newModel.AddTriangle(
                    newModel.vertices[v0],
                    newModel.vertices[vSize + e0],
                    newModel.vertices[vSize + e2]
                );
                
                newModel.AddTriangle(
                    newModel.vertices[vSize + e0],
                    newModel.vertices[v1],
                    newModel.vertices[vSize + e1]
                );
                
                newModel.AddTriangle(
                    newModel.vertices[vSize + e2],
                    newModel.vertices[vSize + e1],
                    newModel.vertices[v2]
                );
                
                newModel.AddTriangle(
                    newModel.vertices[vSize + e0],
                    newModel.vertices[vSize + e1],
                    newModel.vertices[vSize + e2]
                );
            }
            
            return newModel;
        }

        public Mesh GetMesh()
        {
            return MeshData.Build();
        }


        public Vector3 GetEdgePoint(Edge e)
        {
            Vector3 p = (e.a.position + e.b.position) / 2.0f;

            if (e.faces.Count == 2)
            {
                foreach (Triangle f in e.faces)
                {
                    foreach(Vertex v in f.GetVertices())
                        if (v != e.a && v != e.b)
                            p += v.position / 8.0f;

                    foreach (Edge ef in f.GetEdges())
                    {
                        var oth = ef.faces.Find(fe => fe != f);
                        if (oth.GetEdges().ToList().IndexOf(e) == -1)
                        {
                            foreach (Vertex vf in oth.GetVertices())
                            {
                                if (f.GetVertices().ToList().IndexOf(vf) == -1)
                                    p -= vf.position / 16.0f;
                            }
                        }
                    }
                }
            }

            return p;
        }
    }
}