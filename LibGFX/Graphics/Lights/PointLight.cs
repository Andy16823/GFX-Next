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
    [StructLayout(LayoutKind.Sequential)]
    public struct PointLightData
    {
        public Vector3 Position;
        public float Intensity;
        public Vector4 Color;
    }

    public class PointLight : Light
    {
        public PointLight(Vector3 postion, Vector4 color)
        {
            Position = postion;
            Color = color;
            Intensity = 1.0f;
        }

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

