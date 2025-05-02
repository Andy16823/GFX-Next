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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewGFXEditor
{
    public partial class MaterialEditor : Form
    {
        EditorPanel3D _editorPanel3D;
        SGMaterial _material;
        Mesh _mesh;
        PerspectiveCamera _camera;
        Transform _transform;
        DirectionalLight _light;
        bool _dragCamera = false;
        Vector2 _mousePos = Vector2.Zero;

        public MaterialEditor(AssetManager assetManager, SGMaterial material, EditorPanel3D parentEditor)
        {
            InitializeComponent();

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

            _editorPanel3D = new EditorPanel3D(this.splitContainer1.Panel2, parentEditor.GLControl);
            _editorPanel3D.Renderer = parentEditor.Renderer;
            _editorPanel3D.EditorLoaded += EditorPanel3D_EditorLoaded;
            _editorPanel3D.BeforeRender += _editorPanel3D_BeforeRender;
            _editorPanel3D.OnRender += EditorPanel3D_EditorPaint;
            _editorPanel3D.OnMouseDown += EditorPanel3D_OnMouseDown;
            _editorPanel3D.OnMouseMove += EditorPanel3D_OnMouseMove;
            _editorPanel3D.OnMouseUp += EditorPanel3D_OnMouseUp;

            _camera = new PerspectiveCamera(new Vector3(0f, 0f, -2.5f), new Vector3(800, 600, 0));
            _camera.LookAt(new Vector3(0, 0, 0));

            _mesh = new Cube().GetMesh();
            _transform = new Transform();
            _transform.Position = new Vector3(0, 0, 0);
            _transform.Scale = new Vector3(1, 1, 1);

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

        private void _editorPanel3D_BeforeRender(object sender, EventArgs e)
        {
            _editorPanel3D.Renderer.SetViewport(new Viewport(_editorPanel3D.GLControl.Width, _editorPanel3D.GLControl.Height));

            _editorPanel3D.ResizeCamera(_camera);
            _editorPanel3D.Renderer.SetProjectionMatrix(_camera.GetProjectionMatrix(_editorPanel3D.Viewport));
            _editorPanel3D.Renderer.SetViewMatrix(_camera.GetViewMatrix());
        }

        private void EditorPanel3D_EditorPaint(object sender, EventArgs e)
        {
            var shader = _editorPanel3D.Renderer.GetShaderProgram("MeshShader");

            _editorPanel3D.Renderer.EnableDepthTest();
            _editorPanel3D.Renderer.BindShaderProgram(shader);
            _editorPanel3D.Renderer.PrepareShader("dirLight.direction", _light.Direction);
            _editorPanel3D.Renderer.PrepareShader("dirLight.lightColor", _light.Color.Xyz);
            _editorPanel3D.Renderer.PrepareShader("dirLight.lightIntensity", _light.Intensity);
            _editorPanel3D.Renderer.PrepareShader("dirLight.ambient", _light.Ambient);
            _editorPanel3D.Renderer.PrepareShader("dirLight.specular", _light.Specular);
            _editorPanel3D.Renderer.PrepareShader("viewPos", _camera.Transform.Position);
            _editorPanel3D.Renderer.DrawMesh(_transform, _mesh, _material);
            _editorPanel3D.Renderer.UnbindShaderProgram();
            _editorPanel3D.Renderer.DisableDepthTest();

        }

        private void EditorPanel3D_EditorLoaded(object sender, EventArgs e)
        {
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
            _editorPanel3D.Renderer.DisposeMesh(_mesh);
        }
    }
}
