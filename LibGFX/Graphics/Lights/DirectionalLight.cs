using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Lights
{
    /// <summary>
    /// An directional light in 3D space.
    /// </summary>
    public class DirectionalLight : Light
    {
        /// <summary>
        /// The direction of the light.
        /// </summary>
        public Vector3 Direction { get; set; }

        /// <summary>
        /// The ambient color of the light.
        /// </summary>
        public Vector3 Ambient { get; set; }

        /// <summary>
        /// The specular color of the light.
        /// </summary>
        public Vector3 Specular { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="DirectionalLight"/> class.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="color"></param>
        /// <param name="intensity"></param>
        public DirectionalLight(Vector3 direction, Vector4 color, float intensity)
        {
            Position = Vector3.PositiveInfinity;
            Direction = direction;
            Color = color;
            Intensity = intensity;
            Ambient = new Vector3(0.2f, 0.2f, 0.2f);
            Specular = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }
}
