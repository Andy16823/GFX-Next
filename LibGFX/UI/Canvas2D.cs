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
            Transform.SetRotation(Vector3.Zero);
            this.Transform = transform;

            this.Camera = new OrthographicCamera(Vector2.Zero, new Vector2(width, height));
        }

        public override void Dispose(IRenderDevice renderer)
        {
            throw new NotImplementedException();
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
            renderer.DisableDepthTest();
            
            renderer.SetViewport(viewport);
            renderer.SetProjectionMatrix(this.Camera.GetProjectionMatrix(viewport));
            renderer.SetViewMatrix(this.Camera.GetViewMatrix());

            renderer.ResizeRenderTarget(this.RenderTarget, (int)this.Transform.Scale.X, (int)this.Transform.Scale.Y);
            renderer.BindRenderTarget(this.RenderTarget);
            renderer.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            renderer.Clear((int)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

            foreach (var control in this.Controls.Values)
            {
                control.Render(renderer, this);
            }

            renderer.UnbindRenderTarget();


            renderer.BindShaderProgram(renderer.GetShaderProgram("ScreenShader"));
            renderer.DrawRenderTarget(this.RenderTarget);
            renderer.UnbindShaderProgram();
        }

        public override void Update()
        {
            
        }
    }
}
