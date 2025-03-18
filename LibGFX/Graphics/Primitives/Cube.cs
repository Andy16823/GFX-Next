using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Primitives
{
    public class Cube : IPrimitive
    {
        public Mesh GetMesh(Material material)
        {
            var mesh = new Mesh();
            mesh.Material = material;
            mesh.Name = "Cube";

            mesh.Vertices = new List<Vertex>
            {
                // Vorderseite
                new Vertex { Position = new Vector3(-1, -1,  1), TexCoord = new Vector2(0, 0), Normal = new Vector3(0, 0, 1), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { Position = new Vector3( 1, -1,  1), TexCoord = new Vector2(1, 0), Normal = new Vector3(0, 0, 1), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { Position = new Vector3( 1,  1,  1), TexCoord = new Vector2(1, 1), Normal = new Vector3(0, 0, 1), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { Position = new Vector3(-1,  1,  1), TexCoord = new Vector2(0, 1), Normal = new Vector3(0, 0, 1), Tangent = new Vector4(1, 0, 0, 1) },

                // Rückseite
                new Vertex { Position = new Vector3(-1, -1, -1), TexCoord = new Vector2(1, 0), Normal = new Vector3(0, 0, -1), Tangent = new Vector4(-1, 0, 0, 1) },
                new Vertex { Position = new Vector3( 1, -1, -1), TexCoord = new Vector2(0, 0), Normal = new Vector3(0, 0, -1), Tangent = new Vector4(-1, 0, 0, 1) },
                new Vertex { Position = new Vector3( 1,  1, -1), TexCoord = new Vector2(0, 1), Normal = new Vector3(0, 0, -1), Tangent = new Vector4(-1, 0, 0, 1) },
                new Vertex { Position = new Vector3(-1,  1, -1), TexCoord = new Vector2(1, 1), Normal = new Vector3(0, 0, -1), Tangent = new Vector4(-1, 0, 0, 1) }
            };

            mesh.Indices = new List<int>
            {
                // Vorderseite
                0, 1, 2, 2, 3, 0,
                // Rückseite
                5, 4, 7, 7, 6, 5,
                // Linke Seite
                4, 0, 3, 3, 7, 4,
                // Rechte Seite
                1, 5, 6, 6, 2, 1,
                // Unterseite
                4, 5, 1, 1, 0, 4,
                // Oberseite
                3, 2, 6, 6, 7, 3
            };

            return mesh;
        }
    }
}
