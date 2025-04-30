using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Lights
{
    /// <summary>
    /// Base class for all light types.
    /// </summary>
    public abstract class Light
    {
        /// <summary>
        /// The color of the light.
        /// </summary>
        public Vector4 Color { get; set; }

        /// <summary>
        /// The position of the light.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The intensity of the light.
        /// </summary>
        public float Intensity { get; set; }
    }
}
