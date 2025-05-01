using LibGFX.Assets;
using LibGFX.Core;
using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
using OpenTK.GLControl;
using OpenTK.Mathematics;
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

    public class EditorPanel3D
    {
        public BaseScene Scene { get; set; }
        public Camera Camera { get; set; }
        public IRenderDevice Renderer { get => _renderer;}

        public event EditorEventHandler EditorLoaded;
        public event EditorEventHandler EditorUnloaded;
        public event EditorEventHandler EditorPaint;

        Control _host;
        GLControl _glControl1;
        GLRenderer _renderer;
        Viewport _viewport;
        Vector2 _mousePos;
        bool _dragCamera = false;

        public EditorPanel3D(Control host)
        {
            _host = host;

            this.CreateGraphicsContext();
        }

        private void CreateGraphicsContext()
        {
            _glControl1 = new GLControl();
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
        }

        private void GlControl1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                var front = Camera.Transform.Forward * 0.1f;
                Camera.Transform.Position += front;
            }
            else if (e.KeyCode == Keys.S)
            {
                var back = Camera.Transform.Forward * 0.1f;
                Camera.Transform.Position -= back;
            }

            if (e.KeyCode == Keys.A)
            {
                var right = Camera.Transform.GetRightFlat() * 0.1f;
                Camera.Transform.Position -= right;
            }
            else if (e.KeyCode == Keys.D)
            {
                var right = Camera.Transform.GetRightFlat() * 0.1f;
                Camera.Transform.Position += right;
            }
            _glControl1.Invalidate();
        }

        private void GlControl1_MouseDoubleClick(object? sender, MouseEventArgs e)
        {

        }

        private void GlControl1_MouseUp(object? sender, MouseEventArgs e)
        {
            _dragCamera = false;
        }

        private void GlControl1_MouseClick(object? sender, MouseEventArgs e)
        {

        }

        private void GlControl1_MouseDown(object? sender, MouseEventArgs e)
        {
            _glControl1.Focus();

            if (e.Button == MouseButtons.Right)
            {
                if (!_dragCamera)
                {
                    _dragCamera = true;
                    _mousePos = new Vector2(e.X, e.Y);
                    Debug.WriteLine("Mouse Down: " + _mousePos.ToString());
                }
            }
        }

        private void GlControl1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (_dragCamera)
            {
                var delataX = e.X - _mousePos.X;
                var delataY = e.Y - _mousePos.Y;
                Camera.Transform.Rotate(new Vector3(-delataY * 0.1f, -delataX * 0.1f, 0.0f));
                _glControl1.Invalidate();
                _mousePos = new Vector2(e.X, e.Y);
            }
        }

        private void GlControl1_Paint(object? sender, PaintEventArgs e)
        {
            Camera.Transform.Scale = new Vector3(_viewport.Width, _viewport.Height, 0f);

            _renderer.SetViewport(_viewport);
            _renderer.MakeCurrent();
            _renderer.ClearColor(0.392f, 0.584f, 0.929f, 1.0f);
            _renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);

            if (Scene != null)
            {
                Scene.Render(_viewport, _renderer, Camera);
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
            _renderer = new GLRenderer();
            _renderer.Init(_glControl1.Context);

            Scene.Init(_viewport, _renderer);

            if(this.EditorLoaded != null)
            {
                this.EditorLoaded(this, EventArgs.Empty);
            }
        }
    }
}
