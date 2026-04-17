using LibGFX.Graphics.Shader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.PostProcessing
{
    /// <summary>
    /// Applies a scanline post-processing effect to a rendered image, simulating the appearance of CRT or retro
    /// displays.
    /// </summary>
    /// <remarks>The ScanlinesFX class provides configurable scanline effects with adjustable intensity, line
    /// count, and animation speed. It includes several static presets for common visual styles, such as classic CRT,
    /// VHS, and arcade monitors. This effect is typically used as part of a post-processing stack to enhance the visual
    /// presentation of 2D or 3D scenes. The class is not thread-safe and should be used on the rendering
    /// thread.</remarks>
    public class ScanlinesFX : IPostProcessFilter
    {
        /// <summary>
        /// Gets the render target used for drawing operations.
        /// </summary>
        public RenderTarget2D RenderTarget { get; private set; }

        /// <summary>
        /// The intensity of the scanlines effect.
        /// </summary>
        public float Intensity { get; set; } = 0.1f;

        /// <summary>
        /// The number of scanlines to apply.
        /// </summary>
        public float LineCount { get; set; } = 800.0f;

        /// <summary>
        /// Gets or sets the speed value.
        /// </summary>
        public float Speed { get; set; } = 0.0f;

        private ScanlinesFXShader _shader;
        private float _time = 0.0f;
        
        public void Apply(PostProcessStack stack, IRenderDevice renderer, int sourceTexture)
        {
            renderer.BindRenderTarget(this.RenderTarget);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);
            renderer.BindShaderProgram(_shader);
            renderer.PrepareShader("sourceTexture", 0, sourceTexture);
            renderer.PrepareShader("intensity", this.Intensity);
            renderer.PrepareShader("lineCount", this.LineCount);
            renderer.PrepareShader("speed", this.Speed);
            renderer.PrepareShader("time", _time);
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
            _shader = new ScanlinesFXShader();
            renderer.BuildRenderShader(_shader);
            this.RenderTarget = new RenderTarget2D(viewport.Width, viewport.Height);
            this.RenderTarget.Create();
        }

        public void Resize(Viewport viewport, IRenderDevice renderer)
        {
            RenderTarget.Resize(viewport.Width, viewport.Height);
        }

        public void Update(float deltaTime)
        {
            _time += deltaTime;
            if (_time > 100.0f)
            {
                _time -= 100.0f;
            }
        }

        #region Presets

        /// <summary>
        /// Subtle static scanlines (always-on, barely visible)
        /// </summary>
        public static ScanlinesFX Subtle => new ScanlinesFX()
        {
            Intensity = 0.05f,
            Speed = 0.0f
        };

        /// <summary>
        /// Classic CRT monitor look (static)
        /// </summary>
        public static ScanlinesFX CRT => new ScanlinesFX()
        {
            Intensity = 0.15f,
            Speed = 0.0f
        };

        /// <summary>
        /// Strong CRT with visible lines
        /// </summary>
        public static ScanlinesFX StrongCRT => new ScanlinesFX()
        {
            Intensity = 0.25f,
            Speed = 0.0f
        };

        /// <summary>
        /// Tactical display with slow scrolling lines
        /// </summary>
        public static ScanlinesFX Tactical => new ScanlinesFX()
        {
            Intensity = 0.12f,
            Speed = 50.0f
        };

        /// <summary>
        /// VHS tape with rolling scanlines
        /// </summary>
        public static ScanlinesFX VHS => new ScanlinesFX()
        {
            Intensity = 0.20f,
            Speed = 100.0f
        };

        /// <summary>
        /// Old TV with thick scanlines
        /// </summary>
        public static ScanlinesFX OldTV => new ScanlinesFX()
        {
            Intensity = 0.30f,
            LineCount = 400.0f,  // Fewer, thicker lines
            Speed = 0.0f
        };

        /// <summary>
        /// Retro arcade monitor
        /// </summary>
        public static ScanlinesFX Arcade => new ScanlinesFX()
        {
            Intensity = 0.18f,
            LineCount = 600.0f,
            Speed = 0.0f
        };

        #endregion
    }
}
