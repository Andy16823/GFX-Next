using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
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
            this.Position = postion;
            this.Color = color;
            this.Intensity = 1.0f;
        }

        public PointLightData ToStruct()
        {
            return new PointLightData()
            {
                Position = this.Position,
                Intensity = this.Intensity,
                Color = this.Color
            };
        }
    }
}

