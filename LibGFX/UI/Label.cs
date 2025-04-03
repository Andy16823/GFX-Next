using LibGFX.Graphics;
using LibGFX.Math;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.UI
{
    public class Label : Control
    {
        public String Text { get; set; }
        public Font Font { get; set; }
        public float FontScale { get; set; }
        public FontAlignment Alignment { get; set; }

        private OrthographicCamera _camera;
        private Viewport _viewport;


        public Label(Vector2 position, Vector2 scale, String text, Font font, float fontscale = 1.0f, FontAlignment alignment = FontAlignment.Center)
        {
            this.Transform = new Math.Transform();
            this.Transform.Position = new Vector3(position.X, position.Y, 0);
            this.Transform.Scale = new Vector3(scale.X, scale.Y, 0);
            this.Font = font;
            this.Text = text;
            this.FontScale = fontscale;
            this.Alignment = alignment;

            _camera = new OrthographicCamera(new Vector2(0, 0), new Vector2(scale.X, scale.Y));
            _viewport = new Viewport((int) scale.X,(int) scale.Y);
        }

        public override void Dispose(IRenderDevice renderer, Canvas canvas)
        {
            
        }

        public override void Init(IRenderDevice renderer, Canvas canvas)
        {
            var renderTargetDescriptor = new RenderTargetDescriptor()
            {
                Width = (int) this.Transform.Scale.X,
                Height = (int) this.Transform.Scale.Y,
                Border = 0
            };
            this.RenderTarget = renderer.CreateRenderTarget(renderTargetDescriptor);
        }

        public override void Render(IRenderDevice renderer, Canvas canvas)
        {            
            var viewport = renderer.GetViewport();

            _camera.Transform.Scale = new Vector3(this.Transform.Scale.X, this.Transform.Scale.Y, 0);
            _viewport = new Viewport((int) this.Transform.Scale.X, (int)this.Transform.Scale.Y);
            renderer.ResizeRenderTarget(this.RenderTarget, _viewport.Width, _viewport.Height);

            // Set the camera to the size of the label
            renderer.SetViewport(_viewport);
            renderer.SetProjectionMatrix(_camera.GetProjectionMatrix(_viewport));
            renderer.SetViewMatrix(_camera.GetViewMatrix());

            // Render the text to the render target
            renderer.BindRenderTarget(this.RenderTarget);
            renderer.ClearColor(0.0f, 0.0f, 1.0f, 1.0f);
            renderer.Clear((int)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
            renderer.BindShaderProgram(renderer.GetShaderProgram("RectShader"));
            renderer.FillRect(new Rect(0, 0, this.Transform.Scale.X, this.Transform.Scale.Y), new Vector4(0, 1, 0, 1));
            renderer.BindShaderProgram(renderer.GetShaderProgram("FontShader"));
            renderer.DrawString2D(this.Text, new Vector2(0, 0), this.Font, new Vector4(1, 1, 1, 1), this.FontScale, this.Alignment);
            renderer.UnbindShaderProgram();
            renderer.UnbindRenderTarget();

            // Reset the camera and viewport
            renderer.SetProjectionMatrix(canvas.Camera.GetProjectionMatrix(viewport));
            renderer.SetViewMatrix(canvas.Camera.GetViewMatrix());
            renderer.SetViewport(viewport);

            Debug.WriteLine($"Viewport: {viewport.Width}x{viewport.Height}");

            // Draw the render target to the screen
            renderer.BindShaderProgram(renderer.GetShaderProgram("SpriteShader"));
            renderer.DrawTexture(this.Transform, this.RenderTarget.TextureID, new Vector4(1,1,1,1));
            renderer.UnbindShaderProgram();
        }

        public override void Update(Canvas canvas)
        {
            
        }
    }
}
