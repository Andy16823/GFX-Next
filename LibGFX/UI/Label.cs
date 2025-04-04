using LibGFX.Core;
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
    /// <summary>
    /// Represents a label control that can be rendered on the screen.
    /// </summary>
    public class Label : Control
    {
        /// <summary>
        /// The text of the label
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// The font of the label
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// The font scale of the label
        /// </summary>
        public float FontScale { get; set; }

        /// <summary>
        /// The font alignment of the label
        /// </summary>
        public FontAlignment Alignment { get; set; }

        private OrthographicCamera _camera;
        private Viewport _viewport;
        private Vector4 _color;

        /// <summary>
        /// Creates a new label control.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="fontscale"></param>
        /// <param name="alignment"></param>
        public Label(String name, String text, Vector2 position, Vector2 scale, Font font, float fontscale = 1.0f, FontAlignment alignment = FontAlignment.Center)
        {
            this.Transform = new Math.Transform();
            this.Transform.Position = new Vector3(position.X, position.Y, 0);
            this.Transform.Scale = new Vector3(scale.X, scale.Y, 0);
            this.Font = font;
            this.Text = text;
            this.FontScale = fontscale;
            this.Alignment = alignment;

            _camera = new OrthographicCamera(new Vector2(0, 0), new Vector2(scale.X, scale.Y));
            _viewport = new Viewport((int)scale.X, (int)scale.Y);
        }

        /// <summary>
        /// Disposes the label and its resources.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="canvas"></param>
        public override void Dispose(IRenderDevice renderer, Canvas canvas)
        {
            renderer.DisposeRenderTarget(this.RenderTarget);
        }

        /// <summary>
        /// Initializes the label and its resources.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="canvas"></param>
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

        /// <summary>
        /// Renders the label on the screen.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="canvas"></param>
        public override void Render(IRenderDevice renderer, Canvas canvas)
        {
            var viewport = renderer.GetViewport();

            // Set the camera to the size of the label
            _camera.Transform.Scale = new Vector3(this.Transform.Scale.X, this.Transform.Scale.Y, 0);
            _viewport = new Viewport((int)this.Transform.Scale.X, (int)this.Transform.Scale.Y);
            renderer.ResizeRenderTarget(this.RenderTarget, _viewport.Width, _viewport.Height);

            // Set the camera to the size of the label
            renderer.SetViewport(_viewport);
            renderer.SetProjectionMatrix(_camera.GetProjectionMatrix(_viewport));
            renderer.SetViewMatrix(_camera.GetViewMatrix());

            // Render the text to the render target
            renderer.BindRenderTarget(this.RenderTarget);
            renderer.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            renderer.Clear((int)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
            //renderer.BindShaderProgram(renderer.GetShaderProgram("RectShader"));
            //renderer.FillRect(new Rect(0, 0, this.Transform.Scale.X, this.Transform.Scale.Y),_color);
            renderer.BindShaderProgram(renderer.GetShaderProgram("FontShader"));
            renderer.DrawString2D(this.Text, new Vector2(0, 0), this.Font, new Vector4(1, 1, 1, 1), this.FontScale, this.Alignment);
            renderer.UnbindShaderProgram();
            renderer.UnbindRenderTarget();

            // Reset the camera and viewport
            renderer.SetProjectionMatrix(canvas.Camera.GetProjectionMatrix(viewport));
            renderer.SetViewMatrix(canvas.Camera.GetViewMatrix());
            renderer.SetViewport(viewport);

            // Draw the render target to the screen
            renderer.BindShaderProgram(renderer.GetShaderProgram("SpriteShader"));
            renderer.DrawTexture(this.Transform, this.RenderTarget.TextureID, new Vector4(1, 1, 1, 1));
            renderer.UnbindShaderProgram();
        }

        /// <summary>
        /// Updates the label and its resources.
        /// </summary>
        /// <param name="canvas"></param>
        public override void Update(Canvas canvas, Window window)
        {
            var mousePos = canvas.GetMousePosition(window);
            if(this.Contains(mousePos))
            {
                _color = new Vector4(0, 1, 0, 1);
            }
            else
            {
                _color = new Vector4(1, 0, 0, 1);
            }
        }
    }
}
