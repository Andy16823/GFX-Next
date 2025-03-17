using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents a vertex for the rendering pipeline
    /// </summary>
    public struct Vertex
    {
        public Vector3 Position;
        public Vector2 TexCoord;
        public Vector3 Normal;
        public Vector4 Tangent;
        public Vector4i BoneIDs;
        public Vector4 BoneWeights;
    }

    public struct BoneInfo
    {
        public int id;
        public Matrix4 offset;
    }

    public class Mesh
    {
        public String Name { get; set; }
        public List<Vertex> Vertices { get; set; }
        public List<int> Indices { get; set; }
        public Vector3 LocalTranslation { get; set; }
        public Vector4 LocalRotation { get; set; }
        public Vector3 LocalScale { get; set; }
        public Material Material { get; set; }
        public RenderData RenderData { get; set; }

        public Mesh()
        {
            Vertices = new List<Vertex>();
            Indices = new List<int>();
            LocalTranslation = Vector3.Zero;
            LocalRotation = Vector4.Zero;
            LocalScale = Vector3.One;
            RenderData = new RenderData();
        }
    }
}
