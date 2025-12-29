using LibGFX.Assets;
using LibGFX.Core;
using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Renderer.OpenGL;
using OpenTK.GLControl;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewGFXEditor
{
    /// <summary>
    /// Event handler for editor events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void EditorEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Event handler for editor input events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void EditorInputEventHandler(object sender, KeyEventArgs e);

    /// <summary>
    /// Event handler for editor mouse events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void EditorMouseEventHandler(object sender, MouseEventArgs e);

    /// <summary>
    /// Event handler for editor resized events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="viewport"></param>
    /// <param name="e"></param>
    public delegate void EditorResizedEventHandler(object sender, Viewport viewport, EventArgs e);

    /// <summary>
    /// Enum to define the initial mode of the editor.
    /// </summary>
    public enum EditorInitialMode
    {
        None,
        SharedContext,
        NewContext
    }

    /// <summary>
    /// Class representing a 3D editor panel.
    /// </summary>
    public class EditorPanel3D
    {
        /// <summary>
        /// The RenderDevice used for rendering.
        /// </summary>
        public IRenderDevice Renderer { get => _renderer; }

        /// <summary>
        /// The GLControl used for rendering.
        /// </summary>
        public GLControl GLControl { get => _glControl1;}

        /// <summary>
        /// The current viewport.
        /// </summary>
        public Viewport Viewport { get => _viewport; }

        /// <summary>
        /// The initial mode of the editor.
        /// </summary>
        public EditorInitialMode InitialMode { get => _initialMode; }

        /// <summary>
        /// Event triggered when the editor is loaded.
        /// </summary>
        public event EditorEventHandler EditorLoaded;

        /// <summary>
        /// Event triggered when the editor is unloaded.
        /// </summary>
        public event EditorEventHandler EditorUnloaded;

        /// <summary>
        /// Event triggered before rendering.
        /// </summary>
        public event EditorEventHandler BeforeRender;

        /// <summary>
        /// Event triggered for rendering.
        /// </summary>
        public event EditorEventHandler OnRender;

        /// <summary>
        /// Event triggered after rendering.
        /// </summary>
        public event EditorEventHandler AfterRender;

        /// <summary>
        /// Event triggered when a key is pressed down.
        /// </summary>
        public event EditorInputEventHandler OnKeyDown;

        /// <summary>
        /// Event triggered when a key is released.
        /// </summary>
        public event EditorInputEventHandler OnKeyUp;

        /// <summary>
        /// Event triggered when the mouse button is down.
        /// </summary>
        public event EditorMouseEventHandler OnMouseDown;

        /// <summary>
        /// Event triggered when the mouse button is up.
        /// </summary>
        public event EditorMouseEventHandler OnMouseUp;

        /// <summary>
        /// Event triggered when the mouse wheel is scrolled.
        /// </summary>
        public event EditorMouseEventHandler OnMouseWheel;

        /// <summary>
        /// Event triggered when the mouse is moved.
        /// </summary>
        public event EditorMouseEventHandler OnMouseMove;

        /// <summary>
        /// Occurs when the editor's size changes.
        /// </summary>
        /// <remarks>Subscribe to this event to be notified whenever the editor is resized. The event
        /// provides details about the new size through the associated <see cref="EditorResizedEventArgs"/>. This event
        /// is typically raised after a resize operation completes.</remarks>
        public event EditorResizedEventHandler OnResized;

        GLControl _sharedContext;
        Control _host;
        GLControl _glControl1;
        GLRenderer _renderer;
        Viewport _viewport;
        Vector2 _mousePos;
        bool _dragCamera = false;
        EditorInitialMode _initialMode = EditorInitialMode.NewContext;

        /// <summary>
        /// Constructor for the EditorPanel3D class.
        /// </summary>
        /// <param name="host"></param>
        public EditorPanel3D(Control host)
        {
            _host = host;
            this.CreateGraphicsContext();
        }

        /// <summary>
        /// Constructor for the EditorPanel3D class with shared context.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="contextParent"></param>
        public EditorPanel3D(Control host, GLControl contextParent)
        {
            _host = host;
            _sharedContext = contextParent;
            _initialMode = EditorInitialMode.SharedContext;
            this.CreateGraphicsContext();
        }

        /// <summary>
        /// Creates the graphics context for the editor.
        /// </summary>
        public void CreateGraphicsContext()
        {
            _glControl1 = new GLControl();
            _glControl1.APIVersion = new System.Version(3, 3, 0, 0);
            _glControl1.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            _glControl1.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;

            if(_initialMode == EditorInitialMode.SharedContext)
            {
                _glControl1.SharedContext = _sharedContext;
            }
            else if(_initialMode == EditorInitialMode.NewContext)
            {
                _glControl1.SharedContext = null;
            }

            _glControl1.Dock = DockStyle.Fill;
            _host.Controls.Add(_glControl1);

            _glControl1.Load += GlControl1_Load;
            _glControl1.Resize += GlControl1_Resize;
            _glControl1.Paint += GlControl1_Paint;
            _glControl1.MouseMove += GlControl1_MouseMove;
            _glControl1.MouseDown += GlControl1_MouseDown;
            _glControl1.MouseUp += GlControl1_MouseUp;
            _glControl1.MouseClick += GlControl1_MouseClick;
            _glControl1.MouseDoubleClick += GlControl1_MouseDoubleClick;
            _glControl1.KeyDown += GlControl1_KeyDown;
            _glControl1.KeyUp += GlControl1_KeyUp;
        }

        /// <summary>
        /// Redraws the editor panel.
        /// </summary>
        public void Redraw()
        {
            _glControl1.Invalidate();
        }

        /// <summary>
        /// Releases all resources used by the current instance.
        /// </summary>
        /// <remarks>Call this method when you are finished using the object to free associated resources
        /// immediately. After calling Dispose, the object should not be used.</remarks>
        public void Dispose()
        {
            _renderer.MakeCurrent();
            _renderer.Dispose();
        }

        /// <summary>
        /// Controls the KeyUp event of the GLControl.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlControl1_KeyUp(object? sender, KeyEventArgs e)
        {
            if(this.OnKeyUp != null)
            {
                this.OnKeyUp(this, e);
                _glControl1.Invalidate();
            }
        }

        /// <summary>
        /// Controls the KeyDown event of the GLControl.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlControl1_KeyDown(object? sender, KeyEventArgs e)
        {
            if(this.OnKeyDown != null)
            {
                this.OnKeyDown(this, e);
                _glControl1.Invalidate();
            }
        }

        /// <summary>
        /// Mouse double click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlControl1_MouseDoubleClick(object? sender, MouseEventArgs e)
        {

        }

        /// <summary>
        /// Mouse up event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlControl1_MouseUp(object? sender, MouseEventArgs e)
        {
            if(this.OnMouseUp != null)
            {
                this.OnMouseUp(this, e);
                _glControl1.Invalidate();
            }
        }

        /// <summary>
        /// Mouse click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlControl1_MouseClick(object? sender, MouseEventArgs e)
        {

        }

        /// <summary>
        /// Mouse down event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlControl1_MouseDown(object? sender, MouseEventArgs e)
        {
            _glControl1.Focus();
            if(this.OnMouseDown != null)
            {
                this.OnMouseDown(this, e);
                _glControl1.Invalidate();
            }
        }

        /// <summary>
        /// Mouse move event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlControl1_MouseMove(object? sender, MouseEventArgs e)
        {
            if(this.OnMouseMove != null)
            {
                this.OnMouseMove(this, e);
                _glControl1.Invalidate();
            }
        }

        /// <summary>
        /// Paint event handler for the GLControl.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GlControl1_Paint(object? sender, PaintEventArgs e)
        {
            try
            {
                // Make the renderer current
                _renderer.MakeCurrent();

                // Trigger the BeforeRender event
                if (this.BeforeRender != null)
                {
                    this.BeforeRender(this, EventArgs.Empty);
                }

                // Clear the backbuffer
                _renderer.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
                _renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);

                // Trigger the OnRender event
                if (this.OnRender != null)
                {
                    this.OnRender(this, EventArgs.Empty);
                }

                // Flush and swap buffers
                _renderer.Flush();
                _renderer.SwapBuffers();

                // Trigger the AfterRender event
                if (this.AfterRender != null)
                {
                    this.AfterRender(this, EventArgs.Empty);
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Render Error");
            }
        }

        /// <summary>
        /// Handles the resize event for the OpenGL control and updates the viewport dimensions accordingly.
        /// </summary>
        /// <remarks>This method updates the internal viewport to match the new size of the control and
        /// raises the OnResized event if any handlers are attached.</remarks>
        /// <param name="sender">The source of the event, typically the OpenGL control being resized.</param>
        /// <param name="e">An object that contains the event data.</param>
        private void GlControl1_Resize(object? sender, EventArgs e)
        {
            _viewport = new Viewport(_glControl1.Width, _glControl1.Height);
            if(this.OnResized != null)
            {
                this.OnResized(this, _viewport, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the Load event of the OpenGL control and initializes the rendering context.
        /// </summary>
        /// <remarks>This method sets up the OpenGL rendering context and raises the EditorLoaded event if
        /// any handlers are attached. It should be called when the OpenGL control is first loaded to ensure proper
        /// initialization.</remarks>
        /// <param name="sender">The source of the event, typically the OpenGL control being loaded.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void GlControl1_Load(object? sender, EventArgs e)
        {
            _renderer = new GLRenderer();
            _renderer.Init(_glControl1.Context);
            _renderer.MakeCurrent();
            Debug.WriteLine("Creating new GLRenderer");

            _viewport = new Viewport(_glControl1.Width, _glControl1.Height);

            if (this.EditorLoaded != null)
            {
                this.EditorLoaded(this, EventArgs.Empty);
            }
        }
    }
}
