using LibGFX.Graphics.Shader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.PostProcessing
{
    /// <summary>
    /// Applies a sepia tone post-processing effect to a rendered image using a configurable intensity.
    /// </summary>
    /// <remarks>The SepiaFX filter can be added to a post-processing stack to simulate a warm, vintage
    /// appearance on rendered scenes. The intensity of the sepia effect can be adjusted to achieve the desired visual
    /// result. This class manages its own render target and shader resources, which should be properly initialized and
    /// disposed of using the provided methods. SepiaFX is not thread-safe and should be used from the rendering
    /// thread.</remarks>
    public class SepiaFX : IPostProcessFilter
    {
        /// <summary>
        /// Gets the render target used for drawing operations.
        /// </summary>
        public RenderTarget2D RenderTarget { get; private set; }

        /// <summary>
        /// Gets or sets the intensity value.
        /// </summary>
        public float Intensity { get; set; } = 1.0f;

        private SepiaFXShader _shader;

        public SepiaFX()
        {
            _shader = new SepiaFXShader();
        }

        public SepiaFX(float intensity)
        {
            Intensity = intensity;
            _shader = new SepiaFXShader();
        }

        public void Apply(PostProcessStack stack, IRenderDevice renderer, int sourceTexture)
        {
            renderer.BindRenderTarget(this.RenderTarget);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);
            renderer.BindShaderProgram(_shader);
            renderer.PrepareShader("sourceTexture", 0, sourceTexture);
            renderer.PrepareShader("intensity", Intensity);
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
            renderer.BuildShaderProgram(_shader);
        }

        public void Resize(Viewport viewport, IRenderDevice renderer)
        {
            renderer.ResizeRenderTarget(this.RenderTarget, viewport.Width, viewport.Height);
        }

        public void Update(float deltaTime)
        {
            // No dynamic properties to update for this effect
        }

        #region Presets

        /// <summary>
        /// Subtle sepia tint (slight warmth, mostly color)
        /// Warm filter look
        /// </summary>
        public static SepiaFX Subtle => new SepiaFX()
        {
            Intensity = 0.2f
        };

        /// <summary>
        /// Light sepia (nostalgic warmth)
        /// Instagram-style vintage filter
        /// </summary>
        public static SepiaFX Light => new SepiaFX()
        {
            Intensity = 0.4f
        };

        /// <summary>
        /// Medium sepia (balanced vintage look)
        /// Old photograph aesthetic
        /// </summary>
        public static SepiaFX Medium => new SepiaFX()
        {
            Intensity = 0.6f
        };

        /// <summary>
        /// Strong sepia (classic vintage)
        /// Traditional sepia photograph
        /// </summary>
        public static SepiaFX Strong => new SepiaFX()
        {
            Intensity = 0.8f
        };

        /// <summary>
        /// Full sepia (complete vintage transformation)
        /// Pure sepia tone, no original color
        /// </summary>
        public static SepiaFX Full => new SepiaFX()
        {
            Intensity = 1.0f
        };

        /// <summary>
        /// Faded photograph (very light sepia)
        /// Sun-bleached old photo
        /// </summary>
        public static SepiaFX Faded => new SepiaFX()
        {
            Intensity = 0.3f
        };

        /// <summary>
        /// Aged photograph (strong, authentic vintage)
        /// 1920s-1940s photograph look
        /// </summary>
        public static SepiaFX Aged => new SepiaFX()
        {
            Intensity = 0.9f
        };

        /// <summary>
        /// Western film sepia (desert, dusty)
        /// Old western movie aesthetic
        /// </summary>
        public static SepiaFX Western => new SepiaFX()
        {
            Intensity = 0.85f
        };

        /// <summary>
        /// Victorian photograph (very old, authentic)
        /// 1800s photography look
        /// </summary>
        public static SepiaFX Victorian => new SepiaFX()
        {
            Intensity = 1.0f
        };

        /// <summary>
        /// Warm memory (flashback effect)
        /// Nostalgic, dream-like
        /// </summary>
        public static SepiaFX Memory => new SepiaFX()
        {
            Intensity = 0.5f
        };

        /// <summary>
        /// Antique (very aged appearance)
        /// Ancient photograph
        /// </summary>
        public static SepiaFX Antique => new SepiaFX()
        {
            Intensity = 0.95f
        };

        #endregion
    }
}
