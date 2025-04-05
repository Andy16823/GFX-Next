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
    public class Button : Control
    {
        /// <summary>
        /// The text of the button
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// The font of the button
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// The font scale of the button
        /// </summary>
        public float FontScale { get; set; }

        /// <summary>
        /// The font alignment of the button
        /// </summary>
        public FontAlignment Alignment { get; set; }

        /// <summary>
        /// The Color of the button
        /// </summary>
        public Vector4 BackgroundColor { get; set; } = new Vector4(0.2f, 0.2f, 0.2f, 0.85f);

        /// <summary>
        /// The Color of the button when hovered
        /// </summary>
        public Vector4 HoverColor { get; set; } = new Vector4(0.4f, 0.4f, 0.4f, 0.85f);

        /// <summary>
        /// The Color of the Font
        /// </summary>
        public Vector4 FontColor { get; set; } = new Vector4(1, 1, 1, 1);

        /// <summary>
        /// The Color of the Border
        /// </summary>
        public Vector4 BorderColor { get; set; } = new Vector4(0, 0, 0, 1.0f);

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
        public Button(String name, String text, Vector2 position, Vector2 scale, Font font, float fontscale = 1.0f, FontAlignment alignment = FontAlignment.Center)
        {
            this.Name = name;
            this.Transform = new Math.Transform();
            this.Transform.Position = new Vector3(position.X, position.Y, 0);
            this.Transform.Scale = new Vector3(scale.X, scale.Y, 0);
            this.Font = font;
            this.Text = text;
            this.FontScale = fontscale;
            this.Alignment = alignment;

            _camera = new OrthographicCamera(new Vector2(0, 0), new Vector2(scale.X, scale.Y));
            _viewport = new Viewport((int)scale.X, (int)scale.Y);
            _color = this.BackgroundColor;
        }

        /// <summary>
        /// Disposes the label and its resources.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="canvas"></param>
        public override void Dispose(IRenderDevice renderer, Canvas canvas)
        {
            Debug.WriteLine($"Disposing {this.Name} Button");
            this.DisposeEvents();
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
            renderer.BindShaderProgram(renderer.GetShaderProgram("RectShader"));
            renderer.FillRect(new Rect(0, 0, this.Transform.Scale.X, this.Transform.Scale.Y), _color);
            renderer.DrawRect(new Rect(0, 0, this.Transform.Scale.X, this.Transform.Scale.Y), this.BorderColor, 0.25f);
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
            // Gets the mouse position in the canvas
            var mousePos = canvas.GetMousePosition(window);

            // Check if the mouse is inside the button
            if (this.Contains(mousePos))
            {
                // Check if the mouse is pressed
                if (window.IsMouseDown(OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left))
                {
                    this.RaiseMouseEvent(mousePos, ControlEventType.MouseDown);
                }

                // Check if the mouse is released
                if (window.IsMouseReleased(OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left))
                {
                    this.RaiseMouseEvent(mousePos, ControlEventType.MouseUp);
                }

                // Check if the control is hovered, if not, set the hovered state to true and rise the mouse enter event
                if (!this.Hovered)
                {
                    this.Hovered = true;
                    this.RaiseMouseEvent(mousePos, ControlEventType.MouseEnter);
                    _color = this.HoverColor;
                }
            }
            else
            {
                // Check if the control is hovered, if so, set the hovered state to false and rise the mouse leave event
                if (this.Hovered)
                {
                    this.Hovered = false;
                    this.RaiseMouseEvent(mousePos, ControlEventType.MouseLeave);
                    _color = this.BackgroundColor;
                }
            }
        }
    }
}
