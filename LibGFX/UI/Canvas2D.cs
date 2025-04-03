using LibGFX.Graphics;
using LibGFX.Math;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.UI
{
    public class Canvas2D : Canvas
    {
        public Canvas2D(float x, float y, float width, float height)
        {
            var transform = new Transform();
            transform.Position = new Vector3(x, y, 0);
            transform.Scale = new Vector3(width, height, 0);
            this.Transform = transform;

            this.Camera = new OrthographicCamera(Vector2.Zero, new Vector2(width, height));
            this.Controls = new Dictionary<string, Control>();
        }

        public override void Dispose(IRenderDevice renderer)
        {
            renderer.DisposeRenderTarget(this.RenderTarget);
            foreach (var control in this.Controls.Values)
            {
                control.Dispose(renderer, this);
            }
        }

        public override void Init(IRenderDevice renderer)
        {
            //Width = (int)this.Transform.Scale.X,
            //Height = (int)this.Transform.Scale.Y,
            //Depth = false,
            //Stencil = false,
            //Multisample = false,
            //Format = RenderTargetFormat.RGBA8
            var renderTargetDescriptor = new RenderTargetDescriptor()
            {
                Width = (int)this.Transform.Scale.X,
                Height = (int)this.Transform.Scale.Y,
                Border = 0
            };
            this.RenderTarget = renderer.CreateRenderTarget(renderTargetDescriptor);

            foreach (var control in this.Controls.Values)
            {
                control.Init(renderer, this);
            }
        }

        public override void Render(Viewport viewport, IRenderDevice renderer)
        {
            // Get the current depth test state
            bool depthTest = renderer.IsDepthTestEnabled();
            this.Camera.Transform.Scale = new Vector3(viewport.Width, viewport.Height, 0);
            this.Transform.Scale = new Vector3(viewport.Width, viewport.Height, 0);


            // Enable depth test and set the viewport, projection and view matrix
            renderer.DisableDepthTest();
            renderer.SetViewport(viewport);
            renderer.SetProjectionMatrix(this.Camera.GetProjectionMatrix(viewport));
            renderer.SetViewMatrix(this.Camera.GetViewMatrix());

            // Render the canvas to the render target
            renderer.ResizeRenderTarget(this.RenderTarget, (int)this.Transform.Scale.X, (int)this.Transform.Scale.Y);
            renderer.BindRenderTarget(this.RenderTarget);
            renderer.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            renderer.Clear((int)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
            foreach (var control in this.Controls.Values)
            {
                control.Render(renderer, this);
            }

            // Unbind the render target and set the depth test state back to the original state
            renderer.UnbindRenderTarget();
            renderer.SetDepthTest(depthTest);

            // Render the render target to the screen
            renderer.BindShaderProgram(renderer.GetShaderProgram("ScreenShader"));
            renderer.DrawRenderTarget(this.RenderTarget);
            renderer.UnbindShaderProgram();
        }

        public override void Update()
        {
            foreach (var control in this.Controls.Values)
            {
                control.Update(this);
            }
        }
    }
}
