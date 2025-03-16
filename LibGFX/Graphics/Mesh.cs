using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public class Mesh
    {
        public String Name { get; set; }
        public List<float> Vertices { get; set; }
        public List<float> Normals { get; set; }
        public List<float> TexCoords { get; set; }
        public List<float> Tangents { get; set; }
        public List<int> Indices { get; set; }
        public List<uint> BoneIDs { get; set; }
        public List<float> BoneWeights { get; set; }
        public Vector3 LocalTranslation { get; set; }
        public Vector4 LocalRotation { get; set; }
        public Vector3 LocalScale { get; set; }
        public Material Material { get; set; }
        public RenderData RenderData { get; set; }

        public Mesh()
        {
            Vertices = new List<float>();
            Normals = new List<float>();
            TexCoords = new List<float>();
            Tangents = new List<float>();
            Indices = new List<int>();
            LocalTranslation = Vector3.Zero;
            LocalRotation = Vector4.Zero;
            LocalScale = Vector3.One;
            RenderData = new RenderData();
        }
    }
}
