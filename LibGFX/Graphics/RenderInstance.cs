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
        /// Note: Visible is stored in the extra data X component.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Sets the extra component Y of the instance.
        /// </summary>
        public float ExtraComponentY { get; set; } = 0;

        /// <summary>
        /// Sets the extra component Z of the instance.
        /// </summary>
        public float ExtraComponentZ { get; set; } = 0;

        /// <summary>
        /// Sets the extra component W of the instance.
        /// </summary>
        public float ExtraComponentW { get; set; } = 0;

        /// <summary>
        /// The UV transform of the instance.
        /// X, Y reprensents the scale of the UV coordinates.
        /// Z, W represents the offset of the UV coordinates.
        /// </summary>
        public Vector4 UVTransform { get; set; } = new Vector4(1, 1, 0, 0);

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
            result.Y = ExtraComponentY;
            result.Z = ExtraComponentZ;
            result.W = ExtraComponentW;

            return result;
        }
    }
}
