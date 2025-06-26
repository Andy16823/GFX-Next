using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: Implement shadow mapping for directional lights if needed.
namespace LibGFX.Graphics.Lights
{
    /// <summary>
    /// An directional light
    /// </summary>
    public class DirectionalLight2D : Light
    {
        public override bool HasShadowMap => false;

        /// <summary>
        /// Creates a new instance of the <see cref="DirectionalLight2D"/> class.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="intensity"></param>
        public DirectionalLight2D(Vector4 color, float intensity)
        {
            Color = color;
            Intensity = intensity;
        }

        public override void Dispose(IRenderDevice renderer)
        {

        }

        public override void Init(IRenderDevice renderer)
        {

        }
    }
}
