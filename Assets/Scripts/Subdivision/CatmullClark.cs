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
        
        
        public CatmullClark(Mesh mesh, int iteration)
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

            int faceSize = model.triangles.Count;
            int edgeSize = model.edges.Count;

            foreach (var f in model.triangles)
            {
                newModel.AddVertex(GetFacePoint(f));
            }
            
            foreach (var e in model.edges)
            {
                newModel.AddVertex(GetEdgePoint(newModel, model, e));
            }

            foreach (var v in model.vertices)
            {
                newModel.AddVertex(GetVertexPoint(newModel,model, v));
            }

            for (int i = 0; i < model.triangles.Count; i++)
            {
                Triangle f = model.triangles[i];

                for (int j = 0; j < 3; j++)
                {
                    newModel.AddTriangle(
                        newModel.vertices[faceSize + edgeSize + model.vertices.IndexOf(f.GetVertices().ElementAt(j))], // A
                        newModel.vertices[faceSize + model.edges.IndexOf(f.GetEdges().ElementAt(j))], // B
                        newModel.vertices[model.triangles.IndexOf(f)] // C
                        ); 
                    
                    newModel.AddTriangle(
                        newModel.vertices[faceSize + model.edges.IndexOf(f.GetEdges().ElementAt((j == 0) ? 2 : j - 1))], // D
                        newModel.vertices[faceSize + edgeSize + model.vertices.IndexOf(f.GetVertices().ElementAt(j))], // A
                        newModel.vertices[model.triangles.IndexOf(f)] // C
                        ); 
                }
            }
            
            return newModel;
        }

        public Mesh GetMesh()
        {
            return MeshData.Build();
        }
        
        
        public Vector3 GetFacePoint(Triangle f)
        {
            Vector3 p = Vector3.zero;
            foreach (Vertex v in f.GetVertices()) 
                p += v.position;
            
            p /= 3.0f;

            return p;
        }
        
        public Vector3 GetEdgePoint(Model m, Model org, Edge e)
        {
            Vector3 p = Vector3.zero;
            if (e.faces.Count == 2)
            {
                int idxF1 = org.triangles.IndexOf(e.faces[0]);
                int idxF2 = org.triangles.IndexOf(e.faces[1]);
                
                p = (e.a.position + e.b.position + 
                     m.vertices[idxF1].position +
                     m.vertices[idxF2].position) / 4.0f;

            }
            else
            {
                p = (e.a.position + e.b.position) / 2.0f;
            }
            
            return p;
        }
        
        public Vector3 GetVertexPoint(Model m, Model org, Vertex v)
        {
            Vector3 p = Vector3.zero;

            if (v.edges.Count == v.triangles.Count)
            {
                Vector3 fVal = Vector3.zero;
                Vector3 eVal = Vector3.zero;
                Vector3 vVal = Vector3.zero;

                foreach (Triangle f in v.triangles) 
                    fVal += m.vertices[org.triangles.IndexOf(f)].position;
                
                fVal /= (float)v.triangles.Count;

                foreach (Edge e in v.edges)
                    eVal += (e.a.position + e.b.position) / 2.0f;

                float n = (float)v.edges.Count;
                eVal /= n;

                vVal = v.position * (n - 3.0f);

                p = (fVal + (eVal * 2.0f) + vVal) / n;
            }
            else
            {
                foreach (Edge e in v.edges)
                    if (e.faces.Count == 1)
                        p += (e.a.position + e.b.position) / 2.0f;

                p += v.position;
                p /= 3.0f;
            }

            return p;
        }
    }
}