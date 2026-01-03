using LibGFX.Graphics.Materials;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Generator
{
    public class SphereGenerator
    {
        public static Mesh CreateSphere(int latitudeBands = 20, int longitudeBands = 20, float radius = 0.5f, IMaterial material = null)
        {
            var mesh = new Mesh();
            mesh.Name = "Sphere";
            mesh.Material = material;

            List<Vertex> vertices = new List<Vertex>();
            List<int> indices = new List<int>();

            for (int lat = 0; lat <= latitudeBands; lat++)
            {
                float theta = lat * MathF.PI / latitudeBands;
                float sinTheta = MathF.Sin(theta);
                float cosTheta = MathF.Cos(theta);

                for (int lon = 0; lon <= longitudeBands; lon++)
                {
                    float phi = lon * 2 * MathF.PI / longitudeBands;
                    float sinPhi = MathF.Sin(phi);
                    float cosPhi = MathF.Cos(phi);

                    float x = cosPhi * sinTheta;
                    float y = cosTheta;
                    float z = sinPhi * sinTheta;

                    Vector3 position = new Vector3(x, y, z) * radius;
                    Vector3 normal = Vector3.Normalize(new Vector3(x, y, z));
                    Vector2 texCoord = new Vector2((float)lon / longitudeBands, (float)lat / latitudeBands);

                    // Einfache Tangentenberechnung entlang der Breite (nicht 100% exakt)
                    Vector3 tangent = Vector3.Normalize(new Vector3(-sinPhi, 0, cosPhi));
                    Vector4 tangent4 = new Vector4(tangent, 1);

                    mesh.Positions.Add(position);

                    vertices.Add(new Vertex
                    {
                        Normal = normal,
                        TexCoord = texCoord,
                        Tangent = tangent4
                    });
                }
            }

            for (int lat = 0; lat < latitudeBands; lat++)
            {
                for (int lon = 0; lon < longitudeBands; lon++)
                {
                    int first = (lat * (longitudeBands + 1)) + lon;
                    int second = first + longitudeBands + 1;

                    indices.Add(first);
                    indices.Add(second);
                    indices.Add(first + 1);

                    indices.Add(second);
                    indices.Add(second + 1);
                    indices.Add(first + 1);
                }
            }

            mesh.Vertices = vertices;
            mesh.Indices = indices;

            return mesh;
        }
    }
}
