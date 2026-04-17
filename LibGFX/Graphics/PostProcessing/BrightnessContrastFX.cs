using LibGFX.Graphics.Shader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.PostProcessing
{
    /// <summary>
    /// Applies a post-processing effect that adjusts the brightness and contrast of a rendered image.
    /// </summary>
    /// <remarks>Use this class as part of a post-processing stack to modify the visual appearance of a scene
    /// by altering its brightness and contrast. The effect is controlled by the Brightness and Contrast properties,
    /// which can be set before applying the filter. This class manages its own render target and shader resources, and
    /// should be initialized and disposed using the provided methods. Thread safety is not guaranteed; use from a
    /// single rendering thread.</remarks>
    public class BrightnessContrastFX : IPostProcessFilter
    {
        /// <summary>
        /// Gets the render target used for drawing operations.
        /// </summary>
        public RenderTarget2D RenderTarget { get; private set; }

        /// <summary>
        /// Gets or sets the brightness adjustment value.
        /// </summary>
        public float Brightness { get; set; } = 0.0f;

        /// <summary>
        /// Gets or sets the contrast adjustment value.
        /// </summary>
        public float Contrast { get; set; } = 1.0f;

        private BrightnessContrastFXShader _shader;

        public void Apply(PostProcessStack stack, IRenderDevice renderer, int sourceTexture)
        {
            renderer.BindRenderTarget(RenderTarget);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);
            renderer.BindShaderProgram(_shader);
            renderer.PrepareShader("sourceTexture", 0, sourceTexture);
            renderer.PrepareShader("brightness", Brightness);
            renderer.PrepareShader("contrast", Contrast);
            renderer.DrawFullScreenQuad();
            renderer.UnbindShaderProgram();
        }

        public void Dispose(PostProcessStack stack, IRenderDevice renderer)
        {
            this.RenderTarget.Dispose();
            renderer.DisposeRenderShader(_shader);
        }

        public void Init(PostProcessStack stack, Viewport viewport, IRenderDevice renderer)
        {
            this.RenderTarget = new RenderTarget2D(viewport.Width, viewport.Height);
            this.RenderTarget.Create();

            _shader = new BrightnessContrastFXShader();
            renderer.BuildRenderShader(_shader);
        }

        public void Resize(Viewport viewport, IRenderDevice renderer)
        {
            this.RenderTarget.Resize(viewport.Width, viewport.Height);
        }

        public void Update(float deltaTime)
        {
            // No dynamic updates needed for this effect
        }

        #region Presets

        /// <summary>
        /// No adjustment (neutral)
        /// </summary>
        public static BrightnessContrastFX Neutral => new BrightnessContrastFX()
        {
            Brightness = 0.0f,
            Contrast = 1.0f
        };

        /// <summary>
        /// Brighter, lower contrast (washed out look)
        /// </summary>
        public static BrightnessContrastFX Bright => new BrightnessContrastFX()
        {
            Brightness = 0.15f,
            Contrast = 0.9f
        };

        /// <summary>
        /// Darker, higher contrast (dramatic look)
        /// </summary>
        public static BrightnessContrastFX Dark => new BrightnessContrastFX()
        {
            Brightness = -0.1f,
            Contrast = 1.3f
        };

        /// <summary>
        /// High contrast (bold, punchy)
        /// </summary>
        public static BrightnessContrastFX HighContrast => new BrightnessContrastFX()
        {
            Brightness = 0.0f,
            Contrast = 1.5f
        };

        /// <summary>
        /// Low contrast (flat, muted)
        /// </summary>
        public static BrightnessContrastFX LowContrast => new BrightnessContrastFX()
        {
            Brightness = 0.0f,
            Contrast = 0.7f
        };

        /// <summary>
        /// Cinematic (slightly dark, high contrast)
        /// </summary>
        public static BrightnessContrastFX Cinematic => new BrightnessContrastFX()
        {
            Brightness = -0.05f,
            Contrast = 1.2f
        };

        /// <summary>
        /// Washed out (bright, low contrast)
        /// </summary>
        public static BrightnessContrastFX WashedOut => new BrightnessContrastFX()
        {
            Brightness = 0.2f,
            Contrast = 0.8f
        };

        /// <summary>
        /// Horror (dark, high contrast)
        /// </summary>
        public static BrightnessContrastFX Horror => new BrightnessContrastFX()
        {
            Brightness = -0.15f,
            Contrast = 1.4f
        };

        #endregion
    }
}
