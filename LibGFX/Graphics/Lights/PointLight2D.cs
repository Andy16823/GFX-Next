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
    /// <summary>
    /// Represents the data structure for a 2D point light for the shader.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Point2DLightData
    {
        public Vector4 Position;
        public Vector4 Color;
        public Vector4 RadiusIntensity;
    }

    /// <summary>
    /// Represents a 2D point light in the scene.
    /// </summary>
    public class PointLight2D : Light
    {
        /// <summary>
        /// The Radius of the light.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Determines if the light has a shadow map.
        /// </summary>
        public override bool HasShadowMap => false;

        /// <summary>
        /// Creates a new instance of the <see cref="PointLight2D"/> class.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <param name="radius"></param>
        /// <param name="intensity"></param>
        public PointLight2D(Vector2 position, Vector3 color, float radius, float intensity)
        {
            Position = new Vector3(position.X, position.Y, 1);
            Color = new Vector4(color.X, color.Y, color.Z, 1);
            Intensity = intensity;
            Radius = radius;
        }

        /// <summary>
        /// Converts the light data to a structure for use in the shader.
        /// </summary>
        /// <returns></returns>
        public Point2DLightData ToStruct()
        {
            return new Point2DLightData()
            {
                Position = new Vector4(Position),
                Color = Color,
                RadiusIntensity = new Vector4(Radius, Intensity, 0.0f, 0.0f)
            };
        }

        public override void Init(IRenderDevice renderer)
        {
            
        }

        public override void Dispose(IRenderDevice renderer)
        {
            
        }
    }
}
