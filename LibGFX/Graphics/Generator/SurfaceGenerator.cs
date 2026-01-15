using LibGFX.Graphics.Materials;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Generator
{
    public class SurfaceGenerator
    {
        public static Mesh CreateSurface(int resolution, float size, float uvtiling)
        {
            var mesh = new Mesh();
            mesh.Name = "SurfacePatch";

            float step = size / resolution;
            var halfSize = size / 2.0f;

            for (int z = 0; z <= resolution; z++)
            {
                for (int x = 0; x <= resolution; x++)
                {
                    var posX = -halfSize + x * step;
                    var posZ = -halfSize + z * step;
                    float u = ((float)x / resolution) * uvtiling;
                    float v = ((float)z / resolution) * uvtiling;

                    mesh.Positions.Add(new Vector3(posX, 0.0f, posZ));

                    mesh.Vertices.Add(new Vertex
                    {
                        TexCoord = new Vector2(u, v),
                        Normal = Vector3.UnitY,
                        Tangent = new Vector4(1.0f, 0.0f, 0.0f, 1.0f)
                    });
                }
            }

            for (int z = 0; z < resolution; z++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    int topLeft = x + z * (resolution + 1);
                    int topRight = topLeft + 1;
                    int bottomLeft = topLeft + resolution + 1;
                    int bottomRight = bottomLeft + 1;
                    mesh.Indices.Add(topLeft);
                    mesh.Indices.Add(bottomLeft);
                    mesh.Indices.Add(topRight);
                    mesh.Indices.Add(topRight);
                    mesh.Indices.Add(bottomLeft);
                    mesh.Indices.Add(bottomRight);
                }
            }
            return mesh;
        }
    }
}
