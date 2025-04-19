using LibGFX.Core;
using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents a render instance with a transform and visibility state.
    /// </summary>
    public class RenderInstance
    {
        /// <summary>
        /// The transform of the instance.
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// The visibility state of the instance.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// The UV transform of the instance.
        /// X, Y reprensents the scale of the UV coordinates.
        /// Z, W represents the offset of the UV coordinates.
        /// </summary>
        public Vector4 UVTransofrom { get; set; } = new Vector4(1, 1, 0, 0);

        /// <summary>
        /// Gets the model matrix of the instance.
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetMatrix()
        {
            return Transform.GetMatrix();
        }

        /// <summary>
        /// Gets the extra data for the instance.
        /// </summary>
        /// <returns></returns>
        public Vector4 GetExtras()
        {
            var result = new Vector4();
            result.X = Visible ? 1.0f : 0.0f;

            return result;
        }
    }
}
