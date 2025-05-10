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
        public PointLight3D(Vector3 postion, Vector4 color)
        {
            Position = postion;
            Color = color;
            Intensity = 1.0f;
            Ambient = new Vector4(0.05f, 0.05f, 0.05f, 1.0f);
            Specular = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            Constant = 1.0f;
            Linear = 0.09f;
            Quadratic = 0.032f;
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
    }
}

