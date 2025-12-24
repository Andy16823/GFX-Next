using LibGFX.Graphics.Shader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.PostProcessing
{
    /// <summary>
    /// Grayscale Post-Processing Effekt
    /// </summary>
    public class GrayscaleFX : IPostProcessFilter
    {
        /// <summary>
        /// Gets the render target used for drawing operations.
        /// </summary>
        public RenderTarget2D RenderTarget { get; private set; }

        /// <summary>
        /// Gets or sets the intensity value for the effect.
        /// </summary>
        public float Intensity { get; set; } = 1.0f;

        private GrayscaleFXShader _shader;

        public GrayscaleFX()
        {
            _shader = new GrayscaleFXShader();
        }

        public GrayscaleFX(float intensity)
        {
            Intensity = intensity;
            _shader = new GrayscaleFXShader();
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
            // No dynamic updates needed for this effect
        }

        #region Presets

        /// <summary>
        /// Subtle desaturation (mostly color, slight B&W)
        /// Muted, cinematic look
        /// </summary>
        public static GrayscaleFX Subtle => new GrayscaleFX()
        {
            Intensity = 0.2f
        };

        /// <summary>
        /// Light desaturation (visible but not dominant)
        /// Washed out, faded look
        /// </summary>
        public static GrayscaleFX Light => new GrayscaleFX()
        {
            Intensity = 0.4f
        };

        /// <summary>
        /// Medium desaturation (balanced mix)
        /// Bleached, atmospheric
        /// </summary>
        public static GrayscaleFX Medium => new GrayscaleFX()
        {
            Intensity = 0.6f
        };

        /// <summary>
        /// Strong grayscale (mostly B&W)
        /// Noir, dramatic
        /// </summary>
        public static GrayscaleFX Strong => new GrayscaleFX()
        {
            Intensity = 0.8f
        };

        /// <summary>
        /// Full grayscale (complete black and white)
        /// Pure monochrome, no color
        /// </summary>
        public static GrayscaleFX Full => new GrayscaleFX()
        {
            Intensity = 1.0f
        };

        /// <summary>
        /// Film noir style (full B&W with high contrast)
        /// Classic noir aesthetic
        /// </summary>
        public static GrayscaleFX Noir => new GrayscaleFX()
        {
            Intensity = 1.0f
        };

        /// <summary>
        /// Documentary style (strong desaturation)
        /// Journalistic, realistic
        /// </summary>
        public static GrayscaleFX Documentary => new GrayscaleFX()
        {
            Intensity = 0.85f
        };

        /// <summary>
        /// Horror/creepy (strong desaturation)
        /// Unsettling atmosphere
        /// </summary>
        public static GrayscaleFX Horror => new GrayscaleFX()
        {
            Intensity = 0.9f
        };

        /// <summary>
        /// Death/game over (full grayscale)
        /// Player death state
        /// </summary>
        public static GrayscaleFX Death => new GrayscaleFX()
        {
            Intensity = 1.0f
        };

        /// <summary>
        /// Surveillance/CCTV (full grayscale)
        /// Security camera footage
        /// </summary>
        public static GrayscaleFX Surveillance => new GrayscaleFX()
        {
            Intensity = 1.0f
        };

        /// <summary>
        /// Muted cinematic (light desaturation)
        /// Subtle, professional look
        /// </summary>
        public static GrayscaleFX Cinematic => new GrayscaleFX()
        {
            Intensity = 0.3f
        };

        /// <summary>
        /// Flashback (medium desaturation)
        /// Memory, past events
        /// </summary>
        public static GrayscaleFX Flashback => new GrayscaleFX()
        {
            Intensity = 0.7f
        };

        /// <summary>
        /// Dramatic focus (strong desaturation)
        /// Slow motion, important moments
        /// </summary>
        public static GrayscaleFX Dramatic => new GrayscaleFX()
        {
            Intensity = 0.75f
        };

        #endregion
    }
}
