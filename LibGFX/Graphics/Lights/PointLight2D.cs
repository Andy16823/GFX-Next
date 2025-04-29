using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Lights
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Point2DLightData
    {
        public Vector4 Position;
        public Vector4 Color;
        public Vector4 RadiusIntensity;
    }

    public class PointLight2D : Light
    {
        public float Radius { get; set; }

        public PointLight2D(Vector2 position, Vector3 color, float radius, float intensity)
        {
            Position = new Vector3(position.X, position.Y, 1);
            Color = new Vector4(color.X, color.Y, color.Z, 1);
            Intensity = intensity;
            Radius = radius;
        }

        public Point2DLightData ToStruct()
        {
            return new Point2DLightData()
            {
                Position = new Vector4(Position),
                Color = Color,
                RadiusIntensity = new Vector4(Radius, Intensity, 0.0f, 0.0f)
            };
        }

    }
}
