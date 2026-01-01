using LibGFX.Graphics.Shader;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.PostProcessing
{
    public class OverlayFX : IPostProcessFilter
    {
        public RenderTarget2D RenderTarget { get => _renderTarget; }

        public Vector4 Color { get; set; }
        private RenderShader _shader;
        private RenderTarget2D _renderTarget;

        public OverlayFX(Vector4 color)
        {
            _shader = new OverlayFXShader();
            Color = color;
        }

        public void Apply(PostProcessStack stack, IRenderDevice renderer, int sourceTexture)
        {
            // Set the render target to our internal render target
            renderer.BindRenderTarget(_renderTarget);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);

            // Apply new visual effect
            renderer.BindShaderProgram(_shader);
            renderer.PrepareShader("overlayColor", Color);
            renderer.PrepareShader("sourceTexture", 0, sourceTexture);
            renderer.DrawFullScreenQuad();
        }

        public void Init(PostProcessStack stack, Viewport viewport, IRenderDevice renderer)
        {
            _renderTarget = renderer.CreateRenderTarget2D(viewport.Width, viewport.Height);
            renderer.BuildShaderProgram(_shader);
        }

        public void Dispose(PostProcessStack stack, IRenderDevice renderer)
        {
            _renderTarget.Dispose(renderer);
            renderer.DisposeShaderProgram(_shader);
        }

        public void Resize(Viewport viewport, IRenderDevice renderer)
        {
            renderer.ResizeRenderTarget(_renderTarget, (int)viewport.Width, (int)viewport.Height);
        }

        public void Update(float deltaTime)
        {
            // No dynamic properties to update for this effect
        }
    }
}
