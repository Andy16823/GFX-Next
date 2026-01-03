using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using LibGFX.Graphics.Materials;
using OpenTK.Mathematics;

namespace LibGFX.Graphics.Primitives
{
    public class Quad : IPrimitive<Quad>
    {
        public static Mesh GetMesh(IMaterial material = null)
        {
            var mesh = new Mesh();
            mesh.Name = "Quad";
            mesh.Material = material;

            mesh.Positions = new List<Vector3>
            {
                new Vector3(-0.5f, -0.5f, 0.0f),
                new Vector3(0.5f, -0.5f, 0.0f),
                new Vector3(0.5f, 0.5f, 0.0f),
                new Vector3(-0.5f, 0.5f, 0.0f)
            };

            mesh.Vertices = new List<Vertex>
            {
                new Vertex { TexCoord = new Vector2(0, 0), Normal = new Vector3(0, 0, 1), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { TexCoord = new Vector2(1, 0), Normal = new Vector3(0, 0, 1), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { TexCoord = new Vector2(1, 1), Normal = new Vector3(0, 0, 1), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { TexCoord = new Vector2(0, 1), Normal = new Vector3(0, 0, 1), Tangent = new Vector4(1, 0, 0, 1) }
            };

            mesh.Indices = new List<int>
            {
                0, 1, 2,
                2, 3, 0
            };

            return mesh;
        }
    }
}
