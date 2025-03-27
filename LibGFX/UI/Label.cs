using LibGFX.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.UI
{
    public class Label : Control
    {
        public String Text { get; set; }
        public Font Font { get; set; }

        public Label(Vector2 position, Vector2 scale, String text, Font font)
        {
            this.Transform = new Math.Transform();
            this.Transform.Position = new Vector3(position.X, position.Y, 0);
            this.Transform.Scale = new Vector3(scale.X, scale.Y, 0);
            this.Font = font;
            this.Text = text;
        }

        public override void Dispose(IRenderDevice renderer, Canvas canvas)
        {
            
        }

        public override void Init(IRenderDevice renderer, Canvas canvas)
        {
            var renderTargetDescriptor = new RenderTargetDescriptor()
            {
                Width = (int)this.Transform.Scale.X,
                Height = (int)this.Transform.Scale.Y,
                Border = 0
            };
            this.RenderTarget = renderer.CreateRenderTarget(renderTargetDescriptor);
        }

        public override void Render(IRenderDevice renderer, Canvas canvas)
        {
            var width = this.Transform.Scale.X;
            var height = this.Transform.Scale.Y;
            var cameraSize = canvas.Camera.Transform.Scale;
            var viewport = renderer.GetViewport();


            canvas.Camera.Transform.Scale = new Vector3(width, height, 0);
            renderer.SetViewport(new Viewport((int)width, (int)height));
            renderer.SetProjectionMatrix(canvas.Camera.GetProjectionMatrix(viewport));
            renderer.SetViewMatrix(canvas.Camera.GetViewMatrix());

            // Render the canvas to the render target
            //renderer.ResizeRenderTarget(this.RenderTarget, (int)width, (int)this.Transform.Scale.Y);
            renderer.BindRenderTarget(this.RenderTarget);
            renderer.ClearColor(1.0f, 1.0f, 0.0f, 1.0f);
            renderer.Clear((int)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

            renderer.BindShaderProgram(renderer.GetShaderProgram("FontShader"));
            renderer.DrawString2D(this.Text, new Vector2(0,0), this.Font, new Vector4(1, 1, 1, 1));
            renderer.UnbindShaderProgram();
            renderer.UnbindRenderTarget();


            canvas.Camera.Transform.Scale = cameraSize;
            renderer.SetProjectionMatrix(canvas.Camera.GetProjectionMatrix(viewport));
            renderer.SetViewMatrix(canvas.Camera.GetViewMatrix());
            renderer.SetViewport(viewport);

            renderer.BindShaderProgram(renderer.GetShaderProgram("SpriteShader"));
            renderer.DrawTexture(this.Transform, this.RenderTarget.TextureID, new Vector4(1,1,1,1));
            renderer.UnbindShaderProgram();


        }

        public override void Update(Canvas canvas)
        {
            
        }
    }
}
