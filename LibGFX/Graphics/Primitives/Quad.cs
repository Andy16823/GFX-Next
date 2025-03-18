using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using OpenTK.Mathematics;

namespace LibGFX.Graphics.Primitives
{
    public class Quad : IPrimitive
    {
        public Mesh GetMesh(Material material)
        {
            var mesh = new Mesh();
            mesh.Material = material;
            mesh.Name = "Quad";

            mesh.Vertices = new List<Vertex>
            {
                new Vertex { Position = new Vector3(-1, -1, 0), TexCoord = new Vector2(0, 0), Normal = new Vector3(0, 0, 1), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { Position = new Vector3( 1, -1, 0), TexCoord = new Vector2(1, 0), Normal = new Vector3(0, 0, 1), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { Position = new Vector3( 1,  1, 0), TexCoord = new Vector2(1, 1), Normal = new Vector3(0, 0, 1), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { Position = new Vector3(-1,  1, 0), TexCoord = new Vector2(0, 1), Normal = new Vector3(0, 0, 1), Tangent = new Vector4(1, 0, 0, 1) }
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
