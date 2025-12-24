using LibGFX.Graphics.Shader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.PostProcessing
{
    /// <summary>
    /// Applies a posterization post-processing effect to a rendered image using configurable color levels and gamma
    /// correction.
    /// </summary>
    /// <remarks>PosterizationFX is typically used as part of a post-processing stack to reduce the number of
    /// color tones in an image, creating a stylized, flat-shaded appearance. The effect can be customized by adjusting
    /// the Levels and Gamma properties. This class manages its own render target and shader resources, and should be
    /// initialized, applied, and disposed of through the corresponding methods. PosterizationFX is not thread-safe and
    /// should be used from the rendering thread.</remarks>
    public class PosterizationFX : IPostProcessFilter
    {
        /// <summary>
        /// Gets the render target used for drawing operations.
        /// </summary>
        public RenderTarget2D RenderTarget { get; private set; }

        /// <summary>
        /// Gets or sets the number of color levels per channel.
        /// </summary>
        public float Levels { get; set; } = 8.0f;

        /// <summary>
        /// Gets or sets the gamma correction value applied to the image.
        /// </summary>
        public float Gamma { get; set; } = 1.0f;

        private PosterizationFXShader _shader;

        public void Apply(PostProcessStack stack, IRenderDevice renderer, int sourceTexture)
        {
            renderer.BindRenderTarget(RenderTarget);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);
            renderer.BindShaderProgram(_shader);
            renderer.PrepareShader("sourceTexture", 0, sourceTexture);
            renderer.PrepareShader("levels", Levels);
            renderer.PrepareShader("gamma", Gamma);
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
            _shader = new PosterizationFXShader();
            renderer.BuildShaderProgram(_shader);
        }

        public void Resize(Viewport viewport, IRenderDevice renderer)
        {
            renderer.ResizeRenderTarget(this.RenderTarget, viewport.Width, viewport.Height);
        }

        public void Update(float deltaTime)
        {
            // No time-dependent parameters to update
        }

        #region Presets

        /// <summary>
        /// Extreme posterization (2 levels = 8 total colors)
        /// Very stylized, almost like old computer graphics
        /// </summary>
        public static PosterizationFX Extreme => new PosterizationFX()
        {
            Levels = 2.0f,
            Gamma = 1.0f
        };

        /// <summary>
        /// Strong posterization (4 levels = 64 total colors)
        /// Bold comic book style
        /// </summary>
        public static PosterizationFX Strong => new PosterizationFX()
        {
            Levels = 4.0f,
            Gamma = 1.2f
        };

        /// <summary>
        /// Standard posterization (8 levels = 512 total colors)
        /// Noticeable but not too extreme
        /// </summary>
        public static PosterizationFX Standard => new PosterizationFX()
        {
            Levels = 8.0f,
            Gamma = 1.0f
        };

        /// <summary>
        /// Moderate posterization (12 levels = 1,728 total colors)
        /// Subtle effect
        /// </summary>
        public static PosterizationFX Moderate => new PosterizationFX()
        {
            Levels = 12.0f,
            Gamma = 1.0f
        };

        /// <summary>
        /// Subtle posterization (16 levels = 4,096 total colors)
        /// Barely noticeable
        /// </summary>
        public static PosterizationFX Subtle => new PosterizationFX()
        {
            Levels = 16.0f,
            Gamma = 1.0f
        };

        /// <summary>
        /// Game Boy style (4 levels with gamma correction)
        /// Best combined with grayscale
        /// </summary>
        public static PosterizationFX GameBoy => new PosterizationFX()
        {
            Levels = 4.0f,
            Gamma = 2.2f
        };

        /// <summary>
        /// Comic book style (6 levels with slight gamma)
        /// </summary>
        public static PosterizationFX ComicBook => new PosterizationFX()
        {
            Levels = 6.0f,
            Gamma = 1.4f
        };

        /// <summary>
        /// Thermal/Heat map style (5 levels)
        /// Best combined with color tint
        /// </summary>
        public static PosterizationFX Thermal => new PosterizationFX()
        {
            Levels = 5.0f,
            Gamma = 1.0f
        };

        /// <summary>
        /// Retro CGA style (4 colors, like old DOS games)
        /// </summary>
        public static PosterizationFX CGA => new PosterizationFX()
        {
            Levels = 2.0f,
            Gamma = 1.0f
        };

        /// <summary>
        /// EGA style (16 colors, like old DOS games)
        /// </summary>
        public static PosterizationFX EGA => new PosterizationFX()
        {
            Levels = 4.0f,
            Gamma = 1.0f
        };

        #endregion
    }
}
