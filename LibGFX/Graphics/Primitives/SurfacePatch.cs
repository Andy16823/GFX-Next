using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Primitives
{
    /// <summary>
    /// Represents a surface patch primitive for rendering
    /// </summary>
    public class SurfacePatch : IPrimitive
    {
        /// <summary>
        /// The resolution of the surface patch, defining the number of segments along each axis.
        /// </summary>
        public int Resolution { get; set; }

        /// <summary>
        /// The size of the surface patch, defining the extent of the patch in world units.
        /// </summary>
        public float Size { get; set; }

        /// <summary>
        /// The UV tiling factor for the surface patch, defining how many times the texture is repeated across the patch.
        /// </summary>
        public float UVTiling { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SurfacePatch"/> class with specified resolution, size, and UV tiling.
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="size"></param>
        /// <param name="uvtiling"></param>
        public SurfacePatch(int resolution, float size, float uvtiling)
        {
            this.Resolution = resolution;
            this.Size = size;
            this.UVTiling = uvtiling;
        }

        /// <summary>
        /// Generates the mesh for the surface patch based on its resolution, size, and UV tiling.
        /// </summary>
        /// <returns></returns>
        public Mesh GetMesh()
        {
            var mesh = new Mesh();

            float step = this.Size / this.Resolution;
            var halfSize = this.Size / 2.0f;

            for (int z = 0; z <= this.Resolution; z++)
            {
                for (int x = 0; x <= this.Resolution; x++)
                {
                    var posX = -halfSize + x * step;
                    var posZ = -halfSize + z * step;
                    float u = ((float)x / this.Resolution) * this.UVTiling;
                    float v = ((float)z / this.Resolution) * this.UVTiling;

                    mesh.Positions.Add(new Vector3(posX, 0.0f, posZ));

                    mesh.Vertices.Add(new Vertex
                    {
                        TexCoord = new Vector2(u, v),
                        Normal = Vector3.UnitY,
                        Tangent = new Vector4(1.0f, 0.0f, 0.0f, 1.0f)
                    });
                }
            }

            for (int z = 0; z < this.Resolution; z++)
            {
                for (int x = 0; x < this.Resolution; x++)
                {
                    int topLeft = x + z * (this.Resolution + 1);
                    int topRight = topLeft + 1;
                    int bottomLeft = topLeft + this.Resolution + 1;
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
