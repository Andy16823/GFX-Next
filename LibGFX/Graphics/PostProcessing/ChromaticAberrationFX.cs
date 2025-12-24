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
    /// Applies a chromatic aberration post-processing effect to a rendered scene, simulating color channel separation
    /// for visual distortion or stylistic purposes.
    /// </summary>
    /// <remarks>Use this class to add chromatic aberration to a post-processing stack in a rendering
    /// pipeline. The effect can be customized using the Intensity, RadialAmount, and Direction properties, or by
    /// selecting from several built-in presets for common use cases. ChromaticAberrationFX is typically used to enhance
    /// realism, create glitch effects, or emphasize dramatic moments in games and visual applications. This class is
    /// not thread-safe and should be used on the rendering thread.</remarks>
    public class ChromaticAberrationFX : IPostProcessFilter
    {
        /// <summary>
        /// Gets the underlying render target used for drawing operations.
        /// </summary>
        /// <remarks>Use this property to access the current RenderTarget2D instance for advanced
        /// rendering scenarios, such as custom post-processing or direct manipulation of the render target. The
        /// returned object should not be disposed by the caller.</remarks>
        public RenderTarget2D RenderTarget { get; private set; }

        /// <summary>
        /// Gets or sets the intensity value for the effect.
        /// </summary>
        public float Intensity { get; set; } = 0.003f;

        /// <summary>
        /// Gets or sets the proportion of the full radial effect to apply.
        /// </summary>
        /// <remarks>A value of 1.0 applies the effect fully, while lower values reduce the effect
        /// proportionally. Values less than 0 may produce undefined results depending on the effect
        /// implementation.</remarks>
        public float RadialAmount { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the normalized direction vector.
        /// </summary>
        public Vector2 Direction { get; set; } = new Vector2(1.0f, 0.0f);

        private ChromaticAberrationFXShader _shader;

        public void Apply(PostProcessStack stack, IRenderDevice renderer, int sourceTexture)
        {
            renderer.BindRenderTarget(this.RenderTarget);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);
            renderer.BindShaderProgram(_shader);
            renderer.PrepareShader("sourceTexture", 0, sourceTexture);
            renderer.PrepareShader("intensity", this.Intensity);
            renderer.PrepareShader("radialAmount", this.RadialAmount);
            renderer.PrepareShader("direction", this.Direction);
            renderer.DrawFullScreenQuad();
            renderer.UnbindShaderProgram();
        }

        public void Init(PostProcessStack stack, Viewport viewport, IRenderDevice renderer)
        {
            _shader = new ChromaticAberrationFXShader();
            renderer.BuildShaderProgram(_shader);
            this.RenderTarget = renderer.CreateRenderTarget2D(viewport.Width, viewport.Height);
        }

        public void Dispose(PostProcessStack stack, IRenderDevice renderer)
        {
            renderer.DisposeShaderProgram(_shader);
            this.RenderTarget.Dispose(renderer);
        }

        public void Resize(Viewport viewport, IRenderDevice renderer)
        {
            renderer.ResizeRenderTarget(this.RenderTarget, viewport.Width, viewport.Height);
        }

        public void Update(float deltaTime)
        {
            // Not used in this effect
        }

        #region Presets

        /// <summary>
        /// Subtle radial aberration (always-on, realistic)
        /// </summary>
        public static ChromaticAberrationFX Subtle => new ChromaticAberrationFX()
        {
            Intensity = 0.002f,
            RadialAmount = 1.0f
        };

        /// <summary>
        /// Standard radial aberration (noticeable but not distracting)
        /// </summary>
        public static ChromaticAberrationFX Standard => new ChromaticAberrationFX()
        {
            Intensity = 0.005f,
            RadialAmount = 1.0f
        };

        /// <summary>
        /// Strong radial aberration (for dramatic effect)
        /// </summary>
        public static ChromaticAberrationFX Strong => new ChromaticAberrationFX()
        {
            Intensity = 0.01f,
            RadialAmount = 1.0f
        };

        /// <summary>
        /// Extreme aberration (for special effects like explosions)
        /// </summary>
        public static ChromaticAberrationFX Extreme => new ChromaticAberrationFX()
        {
            Intensity = 0.02f,
            RadialAmount = 1.0f
        };

        /// <summary>
        /// Horizontal split (VHS-like)
        /// </summary>
        public static ChromaticAberrationFX Horizontal => new ChromaticAberrationFX()
        {
            Intensity = 0.008f,
            RadialAmount = 0.0f,
            Direction = new Vector2(1.0f, 0.0f)
        };

        /// <summary>
        /// Vertical split (glitch effect)
        /// </summary>
        public static ChromaticAberrationFX Vertical => new ChromaticAberrationFX()
        {
            Intensity = 0.008f,
            RadialAmount = 0.0f,
            Direction = new Vector2(0.0f, 1.0f)
        };

        /// <summary>
        /// Diagonal split (impact effect)
        /// </summary>
        public static ChromaticAberrationFX Diagonal => new ChromaticAberrationFX()
        {
            Intensity = 0.008f,
            RadialAmount = 0.0f,
            Direction = new Vector2(1.0f, 1.0f)
        };

        #endregion
    }
}
