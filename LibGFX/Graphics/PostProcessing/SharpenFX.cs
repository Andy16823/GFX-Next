using LibGFX.Graphics.Shader;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.PostProcessing
{
    /// <summary>
    /// Provides a post-processing filter that applies a configurable sharpening effect to a rendered image using a
    /// shader-based approach.
    /// </summary>
    /// <remarks>The SharpenFX class is intended for use within a post-processing stack to enhance image
    /// clarity by increasing edge contrast. It exposes several preset configurations for common sharpening strengths,
    /// as well as an adjustable Intensity property for custom tuning. The effect is applied by rendering to an internal
    /// render target using a dedicated shader. This class manages its own resources and must be initialized and
    /// disposed of appropriately within the rendering pipeline.</remarks>
    public class SharpenFX : IPostProcessFilter
    {
        /// <summary>
        /// Gets the underlying render target used for drawing operations.
        /// </summary>
        public RenderTarget2D RenderTarget { get; private set; }

        /// <summary>
        /// Gets or sets the intensity of the sharpening effect.
        /// </summary>
        public float Intensity { get; set; } = 0.5f;

        private SharpenFXShader _shader;
        private Vector2 _texelSize;

        public void Apply(PostProcessStack stack, IRenderDevice renderer, int sourceTexture)
        {
            renderer.BindRenderTarget(RenderTarget);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);
            renderer.BindShaderProgram(_shader);
            renderer.PrepareShader("sourceTexture", 0, sourceTexture);
            renderer.PrepareShader("texelSize", _texelSize);
            renderer.PrepareShader("intensity", Intensity);
            renderer.DrawFullScreenQuad();
            renderer.UnbindShaderProgram();
        }

        public void Dispose(PostProcessStack stack, IRenderDevice renderer)
        {
            this.RenderTarget.Dispose(renderer);
            renderer.DisposeRenderShader(_shader);
        }

        public void Init(PostProcessStack stack, Viewport viewport, IRenderDevice renderer)
        {
            this.RenderTarget = renderer.CreateRenderTarget2D(viewport.Width, viewport.Height);

            _shader = new SharpenFXShader();
            renderer.BuildRenderShader(_shader);

            _texelSize = new Vector2(1.0f / viewport.Width, 1.0f / viewport.Height);
        }

        public void Resize(Viewport viewport, IRenderDevice renderer)
        {
            renderer.ResizeRenderTarget(this.RenderTarget, viewport.Width, viewport.Height);
            _texelSize = new Vector2(1.0f / viewport.Width, 1.0f / viewport.Height);
        }

        public void Update(float deltaTime)
        {
            // No dynamic updates needed for this effect
        }

        #region Presets

        /// <summary>
        /// Subtle sharpen (always-on, barely noticeable)
        /// </summary>
        public static SharpenFX Subtle => new SharpenFX() { Intensity = 0.2f };

        /// <summary>
        /// Standard sharpen (noticeable improvement)
        /// </summary>
        public static SharpenFX Standard => new SharpenFX() { Intensity = 0.5f };

        /// <summary>
        /// Strong sharpen (very crisp)
        /// </summary>
        public static SharpenFX Strong => new SharpenFX() { Intensity = 0.8f };

        /// <summary>
        /// Extreme sharpen (over-sharpened, may show artifacts)
        /// </summary>
        public static SharpenFX Extreme => new SharpenFX() { Intensity = 1.2f };

        /// <summary>
        /// Tactical/Sniper scope sharpen
        /// </summary>
        public static SharpenFX Tactical => new SharpenFX() { Intensity = 0.7f };

        #endregion
    }
}
