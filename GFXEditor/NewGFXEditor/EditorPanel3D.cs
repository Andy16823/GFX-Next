using LibGFX.Assets;
using LibGFX.Core;
using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
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
    public delegate void EditorEventHandler(object sender, EventArgs e);
    public delegate void EditorInputEventHandler(object sender, KeyEventArgs e);
    public delegate void EditorMouseEventHandler(object sender, MouseEventArgs e);

    public enum EditorInitialMode
    {
        None,
        SharedContext,
        NewContext
    }

    public class EditorPanel3D
    {
        public IRenderDevice Renderer { get => _renderer; set => _renderer = (GLRenderer) value; }
        public GLControl GLControl { get => _glControl1;}
        public Viewport Viewport { get => _viewport; }
        public EditorInitialMode InitialMode { get => _initialMode; }

        public event EditorEventHandler EditorLoaded;
        public event EditorEventHandler EditorUnloaded;
        public event EditorEventHandler OnRender;
        public event EditorEventHandler BeforeRender;
        public event EditorEventHandler AfterRender;
        public event EditorInputEventHandler OnKeyDown;
        public event EditorInputEventHandler OnKeyUp;
        public event EditorMouseEventHandler OnMouseDown;
        public event EditorMouseEventHandler OnMouseUp;
        public event EditorMouseEventHandler OnMouseWheel;
        public event EditorMouseEventHandler OnMouseMove;

        GLControl _sharedContext;
        Control _host;
        GLControl _glControl1;
        GLRenderer _renderer;
        Viewport _viewport;
        Vector2 _mousePos;
        bool _dragCamera = false;
        EditorInitialMode _initialMode = EditorInitialMode.NewContext;

        public EditorPanel3D(Control host)
        {
            _host = host;
            this.CreateGraphicsContext();
        }

        public EditorPanel3D(Control host, GLControl contextParent)
        {
            _host = host;
            _sharedContext = contextParent;
            _initialMode = EditorInitialMode.SharedContext;
            this.CreateGraphicsContext();
        }

        public void ResizeCamera(Camera camera)
        {
            camera.Transform.Scale = new Vector3(_viewport.Width, _viewport.Height, 0f);
        }

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
            _renderer.SetContext(_glControl1.Context);
            _renderer.MakeCurrent();

            if (this.BeforeRender != null)
            {
                this.BeforeRender(this, EventArgs.Empty);
            }

            //_renderer.SetViewport(_viewport);
            _renderer.ClearColor(0.392f, 0.584f, 0.929f, 1.0f);
            _renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);

            if(this.OnRender != null)
            {
                this.OnRender(this, EventArgs.Empty);
            }

            //canvas.Render(_viewport, _renderer);
            _renderer.Flush();
            _renderer.SwapBuffers();
        }

        private void GlControl1_Resize(object? sender, EventArgs e)
        {
            _viewport = new Viewport(_glControl1.Width, _glControl1.Height);
        }

        private void GlControl1_Load(object? sender, EventArgs e)
        {
            if(_renderer == null)
            {
                _renderer = new GLRenderer();
                _renderer.Init(_glControl1.Context);
                Debug.WriteLine("Creating new GLRenderer"); 
            }
            else
            {
                Debug.WriteLine("Using shared GLRenderer");
            }

            if (this.EditorLoaded != null)
            {
                this.EditorLoaded(this, EventArgs.Empty);
            }
        }
    }
}
