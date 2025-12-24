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
    /// Applies a pixelation post-processing effect to a rendered scene using a configurable pixel size.
    /// </summary>
    /// <remarks>PixelationFX is intended for use within a post-processing stack to create a retro or stylized
    /// visual effect by rendering the scene with blocky, pixelated regions. The effect can be configured by adjusting
    /// the PixelSize property. This class manages its own render target and shader resources, and should be
    /// initialized, applied, resized, and disposed in coordination with the rendering pipeline. PixelationFX is not
    /// thread-safe and should be used from the main rendering thread.</remarks>
    public class PixelationFX : IPostProcessFilter
    {
        /// <summary>
        /// Gets the underlying render target used for drawing operations.
        /// </summary>
        public RenderTarget2D RenderTarget { get; private set; }

        /// <summary>
        /// Gets or sets the size of a single pixel in device-independent units.
        /// </summary>
        public float PixelSize { get; set; } = 4;

        private Vector2 _resolution;
        private PixelationFXShader _shader;

        public void Apply(PostProcessStack stack, IRenderDevice renderer, int sourceTexture)
        {
            renderer.BindRenderTarget(RenderTarget);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);
            renderer.BindShaderProgram(_shader);
            renderer.PrepareShader("sourceTexture", 0, sourceTexture);
            renderer.PrepareShader("resolution", _resolution);
            renderer.PrepareShader("pixelSize", PixelSize);
            renderer.DrawFullScreenQuad();
            renderer.UnbindShaderProgram();
        }

        public void Dispose(PostProcessStack stack, IRenderDevice renderer)
        {
            this.RenderTarget.Dispose(renderer);
            renderer.DisposeShaderProgram(_shader);
        }

        public void Init(PostProcessStack stack, Viewport viewport, IRenderDevice renderer)
        {
            this.RenderTarget = renderer.CreateRenderTarget2D(viewport.Width, viewport.Height);
            _resolution = new Vector2(viewport.Width, viewport.Height);
            _shader = new PixelationFXShader();
            renderer.BuildShaderProgram(_shader);
        }

        public void Resize(Viewport viewport, IRenderDevice renderer)
        {
            renderer.ResizeRenderTarget(RenderTarget, viewport.Width, viewport.Height);
            _resolution = new Vector2(viewport.Width, viewport.Height);
        }

        public void Update(float deltaTime)
        {
            // No dynamic parameters to update for pixelation effect
        }

        #region Presets

        /// <summary>
        /// Subtle pixelation (2x2 blocks)
        /// </summary>
        public static PixelationFX Subtle => new PixelationFX() { PixelSize = 2.0f };

        /// <summary>
        /// Standard pixelation (4x4 blocks)
        /// </summary>
        public static PixelationFX Standard => new PixelationFX() { PixelSize = 4.0f };

        /// <summary>
        /// Strong pixelation (6x6 blocks)
        /// </summary>
        public static PixelationFX Strong => new PixelationFX() { PixelSize = 6.0f };

        /// <summary>
        /// Very pixelated (8x8 blocks)
        /// </summary>
        public static PixelationFX VeryStrong => new PixelationFX() { PixelSize = 8.0f };

        /// <summary>
        /// Extreme pixelation (12x12 blocks) - like old mobile games
        /// </summary>
        public static PixelationFX Extreme => new PixelationFX() { PixelSize = 12.0f };

        /// <summary>
        /// Retro Game Boy style (6x6 blocks)
        /// </summary>
        public static PixelationFX GameBoy => new PixelationFX() { PixelSize = 6.0f };

        /// <summary>
        /// NES/SNES style (5x5 blocks)
        /// </summary>
        public static PixelationFX Retro => new PixelationFX() { PixelSize = 5.0f };

        #endregion
    }
}
