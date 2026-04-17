using LibGFX.Graphics.Shader;
using OpenTK.Mathematics;
using System;

namespace LibGFX.Graphics.PostProcessing
{
    /// <summary>
    /// Applies a color tint effect as a post-processing filter to a rendered scene. 
    /// </summary>
    /// <remarks>The ColorTintFX class is intended for use within a post-processing stack to overlay a
    /// configurable color tint on the output image. It manages its own render target and shader resources, and should
    /// be initialized and disposed of using the corresponding methods. This class is not thread-safe.</remarks>
    public class ColorTintFX : IPostProcessFilter
    {
        /// <summary>
        /// Gets the render target used for drawing operations. 
        /// </summary>
        public RenderTarget2D RenderTarget { get; private set; }

        /// <summary>
        /// Gets or sets the color tint applied to the rendered object (RGBA, 0-1 range).
        /// RGB = color, A = intensity
        /// </summary>
        public Vector4 TintColor { get; set; } = new Vector4(1f, 0f, 0f, 0.3f);

        private ColorTintFXShader _shader;

        public ColorTintFX()
        {
            _shader = new ColorTintFXShader();
        }

        public ColorTintFX(Vector4 tintColor)
        {
            _shader = new ColorTintFXShader();
            TintColor = tintColor;
        }

        public ColorTintFX(float r, float g, float b, float intensity)
        {
            _shader = new ColorTintFXShader();
            TintColor = new Vector4(r, g, b, intensity);
        }

        public void Apply(PostProcessStack stack, IRenderDevice renderer, int sourceTexture)
        {
            renderer.BindRenderTarget(this.RenderTarget);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);
            renderer.BindShaderProgram(_shader);
            renderer.PrepareShader("sourceTexture", 0, sourceTexture);
            renderer.PrepareShader("tintColor", TintColor);
            renderer.DrawFullScreenQuad();
            renderer.UnbindShaderProgram();
        }

        public void Init(PostProcessStack stack, Viewport viewport, IRenderDevice renderer)
        {
            this._shader = new ColorTintFXShader();
            this.RenderTarget = new RenderTarget2D(viewport.Width, viewport.Height);
            this.RenderTarget.Create();
            renderer.BuildRenderShader(_shader);
        }

        public void Dispose(PostProcessStack stack, IRenderDevice renderer)
        {
            this.RenderTarget?.Dispose();
            renderer.DisposeRenderShader(_shader);
        }

        public void Resize(Viewport viewport, IRenderDevice renderer)
        {
            this.RenderTarget.Resize(viewport.Width, viewport.Height);
        }

        public void Update(float deltaTime)
        {
            // Not needed for this effect
        }

        #region Presets

        /// <summary>
        /// Night Vision green tint
        /// </summary>
        public static ColorTintFX NightVision => new ColorTintFX(0.0f, 1.0f, 0.0f, 0.3f);

        /// <summary>
        /// Damage/hit red flash
        /// </summary>
        public static ColorTintFX Damage => new ColorTintFX(1.0f, 0.0f, 0.0f, 0.5f);

        /// <summary>
        /// Healing/health regeneration green
        /// </summary>
        public static ColorTintFX Healing => new ColorTintFX(0.2f, 1.0f, 0.3f, 0.25f);

        /// <summary>
        /// Poison/toxic yellow-green
        /// </summary>
        public static ColorTintFX Poison => new ColorTintFX(0.6f, 0.9f, 0.2f, 0.3f);

        /// <summary>
        /// Fire/burning orange-red
        /// </summary>
        public static ColorTintFX Fire => new ColorTintFX(1.0f, 0.4f, 0.0f, 0.35f);

        /// <summary>
        /// Frozen/ice cyan-blue
        /// </summary>
        public static ColorTintFX Frozen => new ColorTintFX(0.4f, 0.8f, 1.0f, 0.4f);

        /// <summary>
        /// Underwater blue tint
        /// </summary>
        public static ColorTintFX Underwater => new ColorTintFX(0.3f, 0.5f, 1.0f, 0.5f);

        /// <summary>
        /// Cave/darkness dark blue-grey
        /// </summary>
        public static ColorTintFX Darkness => new ColorTintFX(0.1f, 0.1f, 0.3f, 0.4f);

        /// <summary>
        /// Sunset/dusk warm orange
        /// </summary>
        public static ColorTintFX Sunset => new ColorTintFX(1.0f, 0.6f, 0.3f, 0.25f);

        /// <summary>
        /// Night/moonlight dark blue
        /// </summary>
        public static ColorTintFX Moonlight => new ColorTintFX(0.2f, 0.3f, 0.6f, 0.3f);

        /// <summary>
        /// Sepia tone (vintage look)
        /// Note: Use SepiaFX for authentic sepia
        /// </summary>
        public static ColorTintFX Sepia => new ColorTintFX(0.9f, 0.7f, 0.4f, 0.4f);

        /// <summary>
        /// Horror/creepy desaturated red
        /// </summary>
        public static ColorTintFX Horror => new ColorTintFX(0.6f, 0.2f, 0.2f, 0.3f);

        /// <summary>
        /// Matrix/digital green
        /// </summary>
        public static ColorTintFX Matrix => new ColorTintFX(0.0f, 1.0f, 0.3f, 0.3f);

        /// <summary>
        /// No tint (transparent)
        /// </summary>
        public static ColorTintFX None => new ColorTintFX(0.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>
        /// White flash (fade to white)
        /// </summary>
        public static ColorTintFX WhiteFlash => new ColorTintFX(1.0f, 1.0f, 1.0f, 0.8f);

        /// <summary>
        /// Black fade (fade to black)
        /// </summary>
        public static ColorTintFX BlackFade => new ColorTintFX(0.0f, 0.0f, 0.0f, 0.5f);

        #endregion
    }
}