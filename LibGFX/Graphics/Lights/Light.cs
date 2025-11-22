using NAudio.Wave;
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
        public virtual Vector4 Color { get; set; }

        /// <summary>
        /// The position of the light.
        /// </summary>
        public virtual Vector3 Position { get; set; }

        /// <summary>
        /// The intensity of the light.
        /// </summary>
        public virtual float Intensity { get; set; }

        /// <summary>
        /// The range of the light.
        /// </summary>
        public DepthOnlyRenderTarget ShadowMap { get; set; }

        /// <summary>
        /// The size of the shadow map in pixels.
        /// </summary>
        public Vector2i ShadowMapSize { get; set; } = new Vector2i(2048);

        /// <summary>
        /// Indicates whether the light has a shadow map.
        /// </summary>
        public abstract bool HasShadowMap { get; }

        /// <summary>
        /// Initializes the light with the given renderer.
        /// </summary>
        /// <param name="renderer"></param>
        public abstract void Init(IRenderDevice renderer);

        /// <summary>
        /// Disposes the light resources associated with the renderer.
        /// </summary>
        /// <param name="renderer"></param>
        public abstract void Dispose(IRenderDevice renderer);
    }
}
