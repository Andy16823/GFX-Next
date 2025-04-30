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
    public struct PointLightData
    {
        public Vector3 Position;
        public float Intensity;
        public Vector4 Color;
    }

    /// <summary>
    /// Represents a point light in the scene.
    /// </summary>
    public class PointLight : Light
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PointLight"/> class.
        /// </summary>
        /// <param name="postion"></param>
        /// <param name="color"></param>
        public PointLight(Vector3 postion, Vector4 color)
        {
            Position = postion;
            Color = color;
            Intensity = 1.0f;
        }

        /// <summary>
        /// Converts the light data to a structure for use in the shader.
        /// </summary>
        /// <returns></returns>
        public PointLightData ToStruct()
        {
            return new PointLightData()
            {
                Position = Position,
                Intensity = Intensity,
                Color = Color
            };
        }
    }
}

