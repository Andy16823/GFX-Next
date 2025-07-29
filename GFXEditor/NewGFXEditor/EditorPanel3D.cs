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
        /// Resizes the camera based on the current viewport.
        /// </summary>
        /// <param name="camera"></param>
        public void ResizeCamera(Camera camera)
        {
            camera.Transform.Scale = new Vector3(_viewport.Width, _viewport.Height, 0f);
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

        public void Dispose()
        {
            _renderer.MakeCurrent();
            _renderer.Dispose();
        }

        private void GlControl1_KeyUp(object? sender, KeyEventArgs e)
        {
            if(this.OnKeyUp != null)
            {
                this.OnKeyUp(this, e);
                _glControl1.Invalidate();
            }
        }

        private void GlControl1_KeyDown(object? sender, KeyEventArgs e)
        {
            if(this.OnKeyDown != null)
            {
                this.OnKeyDown(this, e);
                _glControl1.Invalidate();
            }
        }

        private void GlControl1_MouseDoubleClick(object? sender, MouseEventArgs e)
        {

        }

        private void GlControl1_MouseUp(object? sender, MouseEventArgs e)
        {
            if(this.OnMouseUp != null)
            {
                this.OnMouseUp(this, e);
                _glControl1.Invalidate();
            }
        }

        private void GlControl1_MouseClick(object? sender, MouseEventArgs e)
        {

        }

        private void GlControl1_MouseDown(object? sender, MouseEventArgs e)
        {
            _glControl1.Focus();
            if(this.OnMouseDown != null)
            {
                this.OnMouseDown(this, e);
                _glControl1.Invalidate();
            }
        }

        private void GlControl1_MouseMove(object? sender, MouseEventArgs e)
        {
            if(this.OnMouseMove != null)
            {
                this.OnMouseMove(this, e);
                _glControl1.Invalidate();
            }
        }

        private void GlControl1_Paint(object? sender, PaintEventArgs e)
        {
            try
            {
                //_renderer.SetContext(_glControl1.Context);
                _renderer.MakeCurrent();

                if (this.BeforeRender != null)
                {
                    this.BeforeRender(this, EventArgs.Empty);
                }

                _renderer.ClearColor(0.392f, 0.584f, 0.929f, 1.0f);
                _renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);

                if (this.OnRender != null)
                {
                    this.OnRender(this, EventArgs.Empty);
                }

                _renderer.Flush();
                _renderer.SwapBuffers();

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

        private void GlControl1_Resize(object? sender, EventArgs e)
        {
            _viewport = new Viewport(_glControl1.Width, _glControl1.Height);
        }

        private void GlControl1_Load(object? sender, EventArgs e)
        {
            _renderer = new GLRenderer();
            _renderer.Init(_glControl1.Context);
            _renderer.MakeCurrent();
            Debug.WriteLine("Creating new GLRenderer");

            if (this.EditorLoaded != null)
            {
                this.EditorLoaded(this, EventArgs.Empty);
            }
        }
    }
}
