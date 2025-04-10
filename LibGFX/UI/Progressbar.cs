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
    /// Delegate for progress bar events.
    /// </summary>
    /// <param name="progressbar"></param>
    public delegate void ProgressbarEventHandler(Progressbar progressbar);

    /// <summary>
    /// Represents a progress bar control that can be rendered on the screen.
    /// </summary>
    public class Progressbar : Control
    {
        /// <summary>
        /// The progress of the progress bar
        /// </summary>
        public float Progress { get; set; } = 50.0f;

        /// <summary>
        /// The maximum progress of the progress bar
        /// </summary>
        public float MaxProgress { get; set; } = 100.0f;

        /// <summary>
        /// The minimum progress of the progress bar
        /// </summary>
        public float MinProgress { get; set; } = 0.0f;

        /// <summary>
        /// The background color of the progress bar
        /// </summary>
        public Vector4 BackgroundColor { get; set; } = new Vector4(0.2f, 0.2f, 0.2f, 0.85f);

        /// <summary>
        /// The color of the progress bar
        /// </summary>
        public Vector4 ProgressColor { get; set; } = new Vector4(0.4f, 0.4f, 0.4f, 0.85f);

        /// <summary>
        /// The color of the border of the progress bar
        /// </summary>
        public Vector4 BorderColor { get; set; } = new Vector4(0, 0, 0, 1.0f);

        /// <summary>
        /// Event that is raised when the progress of the progress bar changes.
        /// </summary>
        public event ProgressbarEventHandler OnProgressChanged;

        /// <summary>
        /// Event that is raised when the progress of the progress bar is completed.
        /// </summary>
        public event ProgressbarEventHandler OnProgressCompleted;


        private OrthographicCamera _camera;
        private Viewport _viewport;

        /// <summary>
        /// Creates a new progress bar control.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        public Progressbar(String name, Vector2 position, Vector2 scale)
        {
            this.Name = name;
            this.Transform = new Math.Transform();
            this.Transform.Position = new Vector3(position.X, position.Y, 0);
            this.Transform.Scale = new Vector3(scale.X, scale.Y, 0);

            _camera = new OrthographicCamera(new Vector2(0, 0), new Vector2(scale.X, scale.Y));
            _viewport = new Viewport((int)scale.X, (int)scale.Y);
        }

        /// <summary>
        /// Gets the progress of the progress bar, clamped between the minimum and maximum progress values.
        /// </summary>
        /// <returns></returns>
        private float GetProgress()
        {
            return MathHelper.Clamp(this.Progress, this.MinProgress, this.MaxProgress);
        }

        /// <summary>
        /// Gets the width of the progress bar based on the current progress.
        /// </summary>
        /// <returns></returns>
        private float GetProgressWidth()
        {
            var progress = this.GetProgress();
            var progressWidth = (progress / this.MaxProgress) * this.Transform.Scale.X;
            return progressWidth;
        }

        /// <summary>
        /// Gets the size of the progress bar based on the current progress.
        /// </summary>
        /// <returns></returns>
        private Vector2 GetProgressSize()
        {
            var progressWidth = this.GetProgressWidth();
            return new Vector2(progressWidth, this.Transform.Scale.Y);
        }

        /// <summary>
        /// Gets the position of the progress bar based on the current progress.
        /// </summary>
        /// <returns></returns>
        private Vector2 GetProgressPosition()
        {
            var progressWidth = this.GetProgressWidth();
            var difference = this.Transform.Scale.X - progressWidth;
            return new Vector2(difference / 2, 0);
        }

        /// <summary>
        /// Processes the progress of the progress bar by a given step value.
        /// </summary>
        /// <param name="step"></param>
        public void Process(float step)
        {
            this.Progress += step;
            if (this.Progress > this.MaxProgress)
            {
                this.Progress = this.MaxProgress;
            }
            else if (this.Progress < this.MinProgress)
            {
                this.Progress = this.MinProgress;
            }
        }

        /// <summary>
        /// Disposes the progress bar and its resources.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="canvas"></param>
        public override void Dispose(IRenderDevice renderer, Canvas canvas)
        {
            Debug.WriteLine($"Disposing {this.Name} Progressbar");
            OnProgressChanged = null;
            OnProgressCompleted = null;
            this.DisposeEvents();
            renderer.DisposeRenderTarget(this.RenderTarget);
        }

        /// <summary>
        /// Initializes the progress bar and its resources.
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
        /// Renders the progress bar to the screen.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="canvas"></param>
        public override void Render(IRenderDevice renderer, Canvas canvas)
        {
            var viewport = renderer.GetViewport();
            var progressSize = this.GetProgressSize();
            var progressOffset = this.GetProgressPosition();    

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
            //renderer.Clear((int)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));#
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);

            renderer.BindShaderProgram(renderer.GetShaderProgram("RectShader"));
            renderer.FillRect(new Rect(0, 0, this.Transform.Scale.X, this.Transform.Scale.Y), this.BackgroundColor);
            renderer.FillRect(new Rect(-progressOffset.X, progressOffset.Y, progressSize.X, progressSize.Y), this.ProgressColor);
            renderer.DrawRect(new Rect(0, 0, this.Transform.Scale.X, this.Transform.Scale.Y), this.BorderColor, 0.25f);
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
                }
            }
            else
            {
                // Check if the control is hovered, if so, set the hovered state to false and rise the mouse leave event
                if (this.Hovered)
                {
                    this.Hovered = false;
                    this.RaiseMouseEvent(mousePos, ControlEventType.MouseLeave);
                }
            }
        }

    }
}
