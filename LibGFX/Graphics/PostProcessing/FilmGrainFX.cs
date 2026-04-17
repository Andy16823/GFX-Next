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
    /// Applies a film grain post-processing effect to a rendered image as part of a post-processing stack.
    /// </summary>
    /// <remarks>
    /// Thanks to https://www.shadertoy.com/view/dlGGW1 for the shader
    /// FilmGrainFX simulates the appearance of analog film grain by overlaying dynamic noise onto
    /// the rendered output. The effect can be customized using properties such as Intensity, GrainSize, and Colored to
    /// achieve various visual styles. This class is typically used in graphics pipelines to add a cinematic or vintage
    /// look to scenes. FilmGrainFX is not thread-safe and should be used on the rendering thread.</remarks>
    public class FilmGrainFX : IPostProcessFilter
    {
        /// <summary>
        /// Gets the render target used for drawing operations.
        /// </summary>
        public RenderTarget2D RenderTarget { get; private set; }

        /// <summary>
        /// Gets or sets the intensity value.
        /// </summary>
        public float Intensity { get; set; } = 0.3f;

        /// <summary>
        /// Gets the current time value, in seconds.
        /// </summary>
        public float Time { get; private set; } = 0.0f;

        /// <summary>
        /// Gets or sets the resolution of the film, in pixels.
        /// </summary>
        public Vector2 FilmResolution { get; set; } = new Vector2(1280, 720);

        private FilmGrainFXShader _shader;
        private Vector2 _resolution;

        public void Apply(PostProcessStack stack, IRenderDevice renderer, int sourceTexture)
        {
            renderer.BindRenderTarget(this.RenderTarget);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);
            renderer.BindShaderProgram(_shader);
            renderer.PrepareShader("sourceTexture", 0, sourceTexture);
            renderer.PrepareShader("intensity", this.Intensity);
            renderer.PrepareShader("time", this.Time);
            renderer.PrepareShader("resolution", _resolution);
            renderer.PrepareShader("filmResolution", this.FilmResolution);
            renderer.DrawFullScreenQuad();
            renderer.UnbindShaderProgram();
        }

        public void Init(PostProcessStack stack, Viewport viewport, IRenderDevice renderer)
        {
            _shader = new FilmGrainFXShader();
            this.RenderTarget = new RenderTarget2D(viewport.Width, viewport.Height);
            this.RenderTarget.Create();
            renderer.BuildRenderShader(_shader);
        }

        public void Dispose(PostProcessStack stack, IRenderDevice renderer)
        {
            this.RenderTarget.Dispose();
            renderer.DisposeRenderShader(_shader);
        }

        public void Resize(Viewport viewport, IRenderDevice renderer)
        {
            this.RenderTarget.Resize(viewport.Width, viewport.Height);
            _resolution = new Vector2(viewport.Width, viewport.Height);
        }

        public void Update(float deltaTime)
        {
            Time += deltaTime;
            Time = Time % 1000.0f;
        }

        #region Presets

        /// <summary>
        /// Subtle grain (barely noticeable, always-on)
        /// Modern digital camera look
        /// </summary>
        public static FilmGrainFX Subtle => new FilmGrainFX()
        {
            Intensity = 0.05f,
            FilmResolution = new Vector2(1920, 1080)  // Fine grain
        };

        /// <summary>
        /// Standard cinematic grain (noticeable but not distracting)
        /// Modern film look
        /// </summary>
        public static FilmGrainFX Cinematic => new FilmGrainFX()
        {
            Intensity = 0.12f,
            FilmResolution = new Vector2(1280, 720)  // Medium grain
        };

        /// <summary>
        /// Strong grain (classic film look)
        /// 35mm film aesthetic
        /// </summary>
        public static FilmGrainFX Classic => new FilmGrainFX()
        {
            Intensity = 0.18f,
            FilmResolution = new Vector2(960, 540)  // Coarser grain
        };

        /// <summary>
        /// Heavy grain (old film, vintage)
        /// 16mm film or old footage
        /// </summary>
        public static FilmGrainFX Vintage => new FilmGrainFX()
        {
            Intensity = 0.25f,
            FilmResolution = new Vector2(720, 480)  // Heavy grain
        };

        /// <summary>
        /// Very heavy grain (extreme vintage or damaged film)
        /// Super 8 or very old footage
        /// </summary>
        public static FilmGrainFX Super8 => new FilmGrainFX()
        {
            Intensity = 0.35f,
            FilmResolution = new Vector2(640, 360)  // Very coarse
        };

        /// <summary>
        /// VHS tape grain (home video look)
        /// Analog video noise
        /// </summary>
        public static FilmGrainFX VHS => new FilmGrainFX()
        {
            Intensity = 0.28f,
            FilmResolution = new Vector2(720, 480)  // VHS resolution
        };

        /// <summary>
        /// Security camera grain (low quality CCTV)
        /// Surveillance footage look
        /// </summary>
        public static FilmGrainFX SecurityCamera => new FilmGrainFX()
        {
            Intensity = 0.32f,
            FilmResolution = new Vector2(640, 480)  // Low res
        };

        /// <summary>
        /// Night vision grain (electronic noise)
        /// Night vision goggles look
        /// </summary>
        public static FilmGrainFX NightVision => new FilmGrainFX()
        {
            Intensity = 0.22f,
            FilmResolution = new Vector2(800, 600)  // Medium-low res
        };

        /// <summary>
        /// Found footage grain (horror/documentary style)
        /// Blair Witch / Paranormal Activity look
        /// </summary>
        public static FilmGrainFX FoundFootage => new FilmGrainFX()
        {
            Intensity = 0.30f,
            FilmResolution = new Vector2(720, 480)  // Consumer camera
        };

        /// <summary>
        /// Digital camera high ISO noise
        /// Modern camera in low light
        /// </summary>
        public static FilmGrainFX HighISO => new FilmGrainFX()
        {
            Intensity = 0.15f,
            FilmResolution = new Vector2(1920, 1080)  // Fine digital noise
        };

        /// <summary>
        /// Silent film grain (very old, 1920s)
        /// Ancient footage look
        /// </summary>
        public static FilmGrainFX SilentFilm => new FilmGrainFX()
        {
            Intensity = 0.40f,
            FilmResolution = new Vector2(512, 384)  // Very low res
        };

        /// <summary>
        /// IMAX / Premium film (minimal grain, high quality)
        /// High-budget production
        /// </summary>
        public static FilmGrainFX IMAX => new FilmGrainFX()
        {
            Intensity = 0.03f,
            FilmResolution = new Vector2(2560, 1440)  // Very fine grain
        };

        #endregion
    }
}
