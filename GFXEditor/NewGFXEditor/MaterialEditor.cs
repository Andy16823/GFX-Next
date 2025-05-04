using LibGFX.Assets;
using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Primitives;
using LibGFX.Graphics.Shader;
using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewGFXEditor
{
    public partial class MaterialEditor : Form
    {
        public Bitmap MaterialPreview { get; set; }

        EditorPanel3D _editorPanel3D;
        SGMaterial _material;
        Mesh _mesh;
        PerspectiveCamera _camera;
        Transform _transform;
        DirectionalLight _light;
        bool _dragCamera = false;
        Vector2 _mousePos = Vector2.Zero;
        Form1 _parent;

        RenderTarget _renderTarget;

        public MaterialEditor(Form1 parent, SGMaterial material)
        {
            InitializeComponent();

            _parent = parent;

            var imageList = new ImageList();
            this.listView1.View = View.LargeIcon;
            this.listView1.LargeImageList = imageList;
            this.listView1.LargeImageList.ImageSize = new Size(64, 64);
            this.listView1.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;

            if (material.DiffuseTexture != null)
            {
                var textureBitmap = material.DiffuseTexture.ToBitmap();
                imageList.Images.Add("textureDiffuse", textureBitmap);

                var item = new ListViewItem("Diffuse Texture", "textureDiffuse");
                item.Tag = material.DiffuseTexture;
                this.listView1.Items.Add(item);
            }

            if (material.NormalTexture != null)
            {
                var normalBitmap = material.NormalTexture.ToBitmap();
                imageList.Images.Add("textureNormal", normalBitmap);
                var item = new ListViewItem("Normal Texture", "textureNormal");
                item.Tag = material.NormalTexture;
                this.listView1.Items.Add(item);
            }

            if (material.SpecularTexture != null)
            {
                var specularBitmap = material.SpecularTexture.ToBitmap();
                imageList.Images.Add("textureSpecular", specularBitmap);
                var item = new ListViewItem("Specular Texture", "textureSpecular");
                item.Tag = material.SpecularTexture;
                this.listView1.Items.Add(item);
            }

            _editorPanel3D = new EditorPanel3D(this.splitContainer1.Panel2, parent.Editor.GLControl);
            _editorPanel3D.EditorLoaded += EditorPanel3D_EditorLoaded;
            _editorPanel3D.BeforeRender += _editorPanel3D_BeforeRender;
            _editorPanel3D.OnRender += EditorPanel3D_EditorPaint;
            _editorPanel3D.AfterRender += EditorPanel3D_AfterRender;
            _editorPanel3D.OnMouseDown += EditorPanel3D_OnMouseDown;
            _editorPanel3D.OnMouseMove += EditorPanel3D_OnMouseMove;
            _editorPanel3D.OnMouseUp += EditorPanel3D_OnMouseUp;

            _camera = new PerspectiveCamera(new Vector3(0f, 0f, -2.5f), new Vector3(800, 600, 0));
            _camera.LookAt(new Vector3(0, 0, 0));

            _mesh = new Sphere().GetMesh();
            _transform = new Transform();
            _transform.Position = new Vector3(0, 0, 0);
            _transform.Scale = new Vector3(1.5f, 1.5f, 1.5f);

            _material = material;

            _light = new DirectionalLight(new Vector3(0f, 5f, -5f), new Vector4(1, 1, 1, 1), 1.5f);
        }

        private void EditorPanel3D_OnMouseUp(object sender, MouseEventArgs e)
        {
            _dragCamera = false;
        }

        private void EditorPanel3D_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragCamera)
            {
                var delataX = e.X - _mousePos.X;
                var delataY = e.Y - _mousePos.Y;
                _transform.Rotate(new Vector3(0.0f, -delataX * 0.1f, 0.0f));
                _mousePos = new Vector2(e.X, e.Y);
            }
        }

        private void EditorPanel3D_OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (!_dragCamera)
                {
                    _dragCamera = true;
                    _mousePos = new Vector2(e.X, e.Y);
                }
            }
        }

        private void RenderFramebuffer(IRenderDevice renderer, Viewport viewport)
        {
            var dephTest = renderer.IsDepthTestEnabled();
            var shader = renderer.GetShaderProgram("MeshShader");

            renderer.SetViewport(viewport);
            renderer.SetProjectionMatrix(_camera.GetProjectionMatrix(viewport));
            renderer.SetViewMatrix(_camera.GetViewMatrix());

            // Render the scene to the render target
            renderer.ResizeRenderTarget(_renderTarget, viewport.Width, viewport.Height);
            renderer.BindRenderTarget(_renderTarget);
            renderer.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);

            renderer.EnableDepthTest();
            renderer.BindShaderProgram(shader);
            renderer.PrepareShader("dirLight.direction", _light.Direction);
            renderer.PrepareShader("dirLight.lightColor", _light.Color.Xyz);
            renderer.PrepareShader("dirLight.lightIntensity", _light.Intensity);
            renderer.PrepareShader("dirLight.ambient", _light.Ambient);
            renderer.PrepareShader("dirLight.specular", _light.Specular);
            renderer.PrepareShader("viewPos", _camera.Transform.Position);
            renderer.DrawMesh(_transform, _mesh, _material);
            renderer.UnbindShaderProgram();
            renderer.DisableDepthTest();
            renderer.UnbindRenderTarget();
        }

        private void _editorPanel3D_BeforeRender(object sender, EventArgs e)
        {
            var viewport = new Viewport(_editorPanel3D.GLControl.Width, _editorPanel3D.GLControl.Height);
            _editorPanel3D.Renderer.SetViewport(viewport);
            _editorPanel3D.ResizeCamera(_camera);
            RenderFramebuffer(_editorPanel3D.Renderer, viewport);
        }

        private void EditorPanel3D_EditorPaint(object sender, EventArgs e)
        {
            var renderer = _editorPanel3D.Renderer;

            renderer.BindShaderProgram(renderer.GetShaderProgram("ScreenShader"));
            renderer.DrawRenderTarget(_renderTarget);
            renderer.UnbindShaderProgram();
        }

        private void EditorPanel3D_AfterRender(object sender, EventArgs e)
        {

            if (_editorPanel3D.Renderer.GetError() != 0)
            {
                throw new Exception($"Render Error {_editorPanel3D.Renderer.GetError()}");
            }
        }

        private void EditorPanel3D_EditorLoaded(object sender, EventArgs e)
        {
            var viewport = _editorPanel3D.Viewport;
            var renderTargetDescriptor = new RenderTargetDescriptor()
            {
                Width = viewport.Width,
                Height = viewport.Height,
                Border = 0
            };
            _renderTarget = _editorPanel3D.Renderer.CreateRenderTarget(renderTargetDescriptor);

            _editorPanel3D.Renderer.LoadMesh(_mesh);
        }

        private void MaterialEditor_Load(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MaterialEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            _editorPanel3D.Dispose();
            _editorPanel3D.Renderer.DisposeRenderTarget(_renderTarget);
            _editorPanel3D.Renderer.DisposeMesh(_mesh);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var renderer = _editorPanel3D.Renderer;
            var size = renderer.GetRenderTargetSize(_renderTarget);
            var pixeldata = renderer.GetRenderTargetData(_renderTarget, size.X, size.Y);
            var bitmap = Utils.ByteBGRAToBitmap(pixeldata, size.X, size.Y);
            this.pictureBox1.Image = bitmap;

            _parent.SetMaterialThumbnail(_material.Name, bitmap);
        }
    }
}
