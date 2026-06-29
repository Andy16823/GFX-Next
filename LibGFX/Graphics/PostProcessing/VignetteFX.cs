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
    /// Vignette Post-Processing Effekt
    /// </summary>
    public class VignetteFX : IPostProcessFilter
    {
        /// <summary>
        /// The internal render target used to store the result of this effect. 
        /// </summary>
        public RenderTarget2D RenderTarget { get; private set; }

        /// <summary>
        /// The intensity of the vignette effect (0 = no effect, 1 = full effect).
        /// </summary>
        public float Intensity { get; set; } = 0.5f;

        /// <summary>
        /// Gets or sets the smoothness factor used in rendering or calculations.
        /// </summary>
        public float Smoothness { get; set; } = 0.5f;

        /// <summary>
        /// Gets or sets the color applied to the vignette effect as an RGB vector.
        /// </summary>
        public Vector3 VignetteColor { get; set; } = new Vector3(0f, 0f, 0f);

        private VignetteFXShader _shader;


        public VignetteFX()
        {
            _shader = new VignetteFXShader();
        }

        public VignetteFX(float intensity, float smoothness, Vector3 vignetteColor)
        {
            Intensity = intensity;
            Smoothness = smoothness;
            VignetteColor = vignetteColor;
            _shader = new VignetteFXShader();
        }

        public void Apply(PostProcessStack stack, IRenderDevice renderer, int sourceTexture)
        {
            renderer.BindRenderTarget(this.RenderTarget);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);
            renderer.BindShaderProgram(_shader);
            renderer.PrepareShader("sourceTexture", 0, sourceTexture);
            renderer.PrepareShader("intensity", Intensity);
            renderer.PrepareShader("smoothness", Smoothness);
            renderer.PrepareShader("vignetteColor", VignetteColor);
            renderer.DrawFullScreenQuad();
            renderer.UnbindShaderProgram();
        }

        public void Dispose(PostProcessStack stack, IRenderDevice renderer)
        {
            renderer.DisposeRenderShader(_shader);
            RenderTarget.Dispose();
        }

        public void Init(PostProcessStack stack, Viewport viewport, IRenderDevice renderer)
        {
            this.RenderTarget = new RenderTarget2D(viewport.Width, viewport.Height);
            this.RenderTarget.Create();
            renderer.BuildRenderShader(_shader);
        }

        public void Resize(Viewport viewport, IRenderDevice renderer)
        {
            RenderTarget.Resize(viewport.Width, viewport.Height);
        }

        public void Update(float deltaTime)
        {
            // No dynamic updates needed for this effect
        }

        #region Presets

        /// <summary>
        /// Subtle vignette (barely noticeable, always-on)
        /// </summary>
        public static VignetteFX Subtle => new VignetteFX()
        {
            Intensity = 0.15f,
            Smoothness = 0.9f,
            VignetteColor = new Vector3(0f, 0f, 0f)
        };

        /// <summary>
        /// Standard vignette (balanced, general use)
        /// </summary>
        public static VignetteFX Standard => new VignetteFX()
        {
            Intensity = 0.35f,
            Smoothness = 0.7f,
            VignetteColor = new Vector3(0f, 0f, 0f)
        };

        /// <summary>
        /// Strong vignette (noticeable focus effect)
        /// </summary>
        public static VignetteFX Strong => new VignetteFX()
        {
            Intensity = 0.55f,
            Smoothness = 0.6f,
            VignetteColor = new Vector3(0f, 0f, 0f)
        };

        /// <summary>
        /// Very strong vignette (dramatic, dark edges)
        /// </summary>
        public static VignetteFX VeryStrong => new VignetteFX()
        {
            Intensity = 0.75f,
            Smoothness = 0.5f,
            VignetteColor = new Vector3(0f, 0f, 0f)
        };

        /// <summary>
        /// Soft vignette (very smooth transition)
        /// </summary>
        public static VignetteFX Soft => new VignetteFX()
        {
            Intensity = 0.25f,
            Smoothness = 1.0f,
            VignetteColor = new Vector3(0f, 0f, 0f)
        };

        /// <summary>
        /// Hard vignette (sharp edge, tunnel vision)
        /// </summary>
        public static VignetteFX Hard => new VignetteFX()
        {
            Intensity = 0.65f,
            Smoothness = 0.3f,
            VignetteColor = new Vector3(0f, 0f, 0f)
        };

        /// <summary>
        /// Cinematic vignette (film-like, subtle)
        /// </summary>
        public static VignetteFX Cinematic => new VignetteFX()
        {
            Intensity = 0.3f,
            Smoothness = 0.8f,
            VignetteColor = new Vector3(0f, 0f, 0f)
        };

        /// <summary>
        /// Horror vignette (very dark, oppressive)
        /// </summary>
        public static VignetteFX Horror => new VignetteFX()
        {
            Intensity = 0.85f,
            Smoothness = 0.4f,
            VignetteColor = new Vector3(0f, 0f, 0f)
        };

        /// <summary>
        /// Scope vignette (sniper scope, circular mask)
        /// </summary>
        public static VignetteFX Scope => new VignetteFX()
        {
            Intensity = 0.95f,
            Smoothness = 0.15f,
            VignetteColor = new Vector3(0f, 0f, 0f)
        };

        /// <summary>
        /// Red vignette (damage/low health indicator)
        /// </summary>
        public static VignetteFX RedDamage => new VignetteFX()
        {
            Intensity = 0.45f,
            Smoothness = 0.6f,
            VignetteColor = new Vector3(0.5f, 0f, 0f)
        };

        /// <summary>
        /// Blue vignette (cold/frost effect)
        /// </summary>
        public static VignetteFX Cold => new VignetteFX()
        {
            Intensity = 0.35f,
            Smoothness = 0.7f,
            VignetteColor = new Vector3(0f, 0.2f, 0.4f)
        };

        /// <summary>
        /// Green vignette (poison/toxic effect)
        /// </summary>
        public static VignetteFX Toxic => new VignetteFX()
        {
            Intensity = 0.45f,
            Smoothness = 0.6f,
            VignetteColor = new Vector3(0f, 0.3f, 0f)
        };

        /// <summary>
        /// Purple vignette (magic/mystical effect)
        /// </summary>
        public static VignetteFX Mystical => new VignetteFX()
        {
            Intensity = 0.35f,
            Smoothness = 0.8f,
            VignetteColor = new Vector3(0.3f, 0f, 0.4f)
        };

        /// <summary>
        /// White vignette (fade to white, overexposure)
        /// </summary>
        public static VignetteFX WhiteFade => new VignetteFX()
        {
            Intensity = 0.35f,
            Smoothness = 0.9f,
            VignetteColor = new Vector3(1f, 1f, 1f)
        };

        #endregion
    }
}