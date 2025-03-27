using LibGFX.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    public class Scene2D : BaseScene
    {
        private RenderTarget _renderTarget;

        public override void Init(Viewport viewport, IRenderDevice renderer)
        {
            var renderTargetDescriptor = new RenderTargetDescriptor()
            {
                Width = viewport.Width,
                Height = viewport.Height,
                Border = 0
            };
            _renderTarget = renderer.CreateRenderTarget(renderTargetDescriptor);

            this.Layers.ForEach(l =>
            {
                l.Init(this, viewport, renderer);
            });
        }

        public override void Render(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            var depthTest = renderer.IsDepthTestEnabled();
            var rectShader = renderer.GetShaderProgram("RectShader");

            // Disable depth test and set the viewport, projection and view matrix
            renderer.DisableDepthTest();
            renderer.SetViewport(viewport);
            renderer.SetProjectionMatrix(camera.GetProjectionMatrix(viewport));
            renderer.SetViewMatrix(camera.GetViewMatrix());

            // Render the scene to the render target
            renderer.ResizeRenderTarget(_renderTarget, viewport.Width, viewport.Height);
            renderer.BindRenderTarget(_renderTarget);
            renderer.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            renderer.Clear((int)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

            this.Layers.ForEach(layer => { 
                layer.RenderLayer(this, viewport, renderer, camera); 
            });

            // Unbind the render target and set the depth test state back to the original state
            renderer.UnbindRenderTarget();
            renderer.SetDepthTest(depthTest);

            renderer.BindShaderProgram(renderer.GetShaderProgram("ScreenShader"));
            renderer.DrawRenderTarget(_renderTarget);  
            renderer.UnbindShaderProgram();
        }

        public override void Update()
        {
            this.Layers.ForEach(l => { 
                l.Update(this); 
            });
        }

        public override void DisposeScene(IRenderDevice renderer)
        {
            this.Layers.ForEach(l =>
            {
                l.Dispose(this, renderer);
            });
        }

        public override void UpdatePhysics()
        {
            this.PhysicsHandler.Process(this);
        }

    }
}
