using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Lights
{
    /// <summary>
    /// Represents the data structure for a point light for the shader.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PointLight3DData
    {
        public Vector4 Position;
        public Vector4 ConstantLinearQuadratic;
        public Vector4 Ambient;
        public Vector4 Diffuse;
        public Vector4 Specular;
    }

    /// <summary>
    /// Represents a point light in the scene.
    /// </summary>
    public class PointLight3D : Light
    {
        public float Range { get; set; }
        public float Constant { get; set; }
        public float Linear { get; set; }
        public float Quadratic { get; set; }
        public Vector4 Ambient { get; set; }
        public Vector4 Specular { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="PointLight3D"/> class.
        /// </summary>
        /// <param name="postion"></param>
        /// <param name="color"></param>
        public PointLight3D(Vector3 position, Vector4 color, float range = 10f)
        {
            Position = position;
            Color = color;
            Intensity = 1.0f;

            Ambient = new Vector4(0.05f);
            Specular = new Vector4(1.0f);

            SetRange(range);
        }

        /// <summary>
        /// Sets the attenuation coefficients for the light based on a given range.
        /// </summary>
        /// <param name="range"></param>
        public void SetRange(float range)
        {
            Constant = 1.0f;
            Linear = 4.5f / range;
            Quadratic = 75.0f / (range * range);
            Range = range;
        }

        /// <summary>
        /// Converts the light data to a structure for use in the shader.
        /// </summary>
        /// <returns></returns>
        public PointLight3DData ToStruct()
        {
            return new PointLight3DData()
            {
                Position = new Vector4(Position, 1.0f),
                ConstantLinearQuadratic = new Vector4(Constant, Linear, Quadratic, 0.0f),
                Ambient = Ambient,
                Diffuse = Color * Intensity,
                Specular = Specular
            };
        }

        /// <summary>
        /// Calculates the axis-aligned bounding box (AABB) of the light based on its range.
        /// </summary>
        /// <param name="minThreshold"></param>
        /// <returns></returns>
        public (Vector3 min, Vector3 max) GetAABB(float minThreshold = 0.01f)
        {
            float range = this.Range;
            Vector3 center = Position;

            Vector3 min = center - new Vector3(range);
            Vector3 max = center + new Vector3(range);

            return (min, max);
        }
    }
}

