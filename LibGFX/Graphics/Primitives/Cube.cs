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
                new Vertex { Position = new Vector3(-0.5f, -0.5f,  0.5f), TexCoord = new Vector2(0, 0), Normal = new Vector3(0, 0, 1), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { Position = new Vector3( 0.5f, -0.5f,  0.5f), TexCoord = new Vector2(1, 0), Normal = new Vector3(0, 0, 1), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { Position = new Vector3( 0.5f,  0.5f,  0.5f), TexCoord = new Vector2(1, 1), Normal = new Vector3(0, 0, 1), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { Position = new Vector3(-0.5f,  0.5f,  0.5f), TexCoord = new Vector2(0, 1), Normal = new Vector3(0, 0, 1), Tangent = new Vector4(1, 0, 0, 1) },

                // Rückseite
                new Vertex { Position = new Vector3(-0.5f, -0.5f, -0.5f), TexCoord = new Vector2(0, 0), Normal = new Vector3(0, 0, -1), Tangent = new Vector4(-1, 0, 0, 1) },
                new Vertex { Position = new Vector3( 0.5f, -0.5f, -0.5f), TexCoord = new Vector2(1, 0), Normal = new Vector3(0, 0, -1), Tangent = new Vector4(-1, 0, 0, 1) },
                new Vertex { Position = new Vector3( 0.5f,  0.5f, -0.5f), TexCoord = new Vector2(1, 1), Normal = new Vector3(0, 0, -1), Tangent = new Vector4(-1, 0, 0, 1) },
                new Vertex { Position = new Vector3(-0.5f,  0.5f, -0.5f), TexCoord = new Vector2(0, 1), Normal = new Vector3(0, 0, -1), Tangent = new Vector4(-1, 0, 0, 1) },

                // Linke Seite
                new Vertex { Position = new Vector3(-0.5f, -0.5f, -0.5f), TexCoord = new Vector2(1, 0), Normal = new Vector3(-1, 0, 0), Tangent = new Vector4(0, 0, 1, 1) },
                new Vertex { Position = new Vector3(-0.5f, -0.5f,  0.5f), TexCoord = new Vector2(0, 0), Normal = new Vector3(-1, 0, 0), Tangent = new Vector4(0, 0, 1, 1) },
                new Vertex { Position = new Vector3(-0.5f,  0.5f,  0.5f), TexCoord = new Vector2(0, 1), Normal = new Vector3(-1, 0, 0), Tangent = new Vector4(0, 0, 1, 1) },
                new Vertex { Position = new Vector3(-0.5f,  0.5f, -0.5f), TexCoord = new Vector2(1, 1), Normal = new Vector3(-1, 0, 0), Tangent = new Vector4(0, 0, 1, 1) },

                // Rechte Seite
                new Vertex { Position = new Vector3(0.5f, -0.5f, -0.5f), TexCoord = new Vector2(0, 0), Normal = new Vector3(1, 0, 0), Tangent = new Vector4(0, 0, -1, 1) },
                new Vertex { Position = new Vector3(0.5f, -0.5f,  0.5f), TexCoord = new Vector2(1, 0), Normal = new Vector3(1, 0, 0), Tangent = new Vector4(0, 0, -1, 1) },
                new Vertex { Position = new Vector3(0.5f,  0.5f,  0.5f), TexCoord = new Vector2(1, 1), Normal = new Vector3(1, 0, 0), Tangent = new Vector4(0, 0, -1, 1) },
                new Vertex { Position = new Vector3(0.5f,  0.5f, -0.5f), TexCoord = new Vector2(0, 1), Normal = new Vector3(1, 0, 0), Tangent = new Vector4(0, 0, -1, 1) },

                // Unterseite
                new Vertex { Position = new Vector3(-0.5f, -0.5f, -0.5f), TexCoord = new Vector2(0, 1), Normal = new Vector3(0, -1, 0), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { Position = new Vector3( 0.5f, -0.5f, -0.5f), TexCoord = new Vector2(1, 1), Normal = new Vector3(0, -1, 0), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { Position = new Vector3( 0.5f, -0.5f,  0.5f), TexCoord = new Vector2(1, 0), Normal = new Vector3(0, -1, 0), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { Position = new Vector3(-0.5f, -0.5f,  0.5f), TexCoord = new Vector2(0, 0), Normal = new Vector3(0, -1, 0), Tangent = new Vector4(1, 0, 0, 1) },

                // Oberseite
                new Vertex { Position = new Vector3(-0.5f,  0.5f, -0.5f), TexCoord = new Vector2(0, 1), Normal = new Vector3(0, 1, 0), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { Position = new Vector3( 0.5f,  0.5f, -0.5f), TexCoord = new Vector2(1, 1), Normal = new Vector3(0, 1, 0), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { Position = new Vector3( 0.5f,  0.5f,  0.5f), TexCoord = new Vector2(1, 0), Normal = new Vector3(0, 1, 0), Tangent = new Vector4(1, 0, 0, 1) },
                new Vertex { Position = new Vector3(-0.5f,  0.5f,  0.5f), TexCoord = new Vector2(0, 0), Normal = new Vector3(0, 1, 0), Tangent = new Vector4(1, 0, 0, 1) }
            };

            mesh.Indices = new List<int>
            {
                0, 1, 2, 2, 3, 0,   // Vorderseite
                5, 4, 7, 7, 6, 5,   // Rückseite
                8, 9, 10, 10, 11, 8, // Linke Seite
                13, 12, 15, 15, 14, 13, // Rechte Seite
                16, 17, 18, 18, 19, 16, // Unterseite
                21, 20, 23, 23, 22, 21  // Oberseite
            };

            return mesh;
        }
    }
}
