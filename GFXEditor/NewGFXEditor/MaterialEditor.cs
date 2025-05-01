using LibGFX.Assets;
using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Primitives;
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
        private EditorPanel3D _editorPanel3D;
        private SGMaterial _materialCopy;

        public MaterialEditor(AssetManager assetManager, SGMaterial material)
        {
            InitializeComponent();

            // Copy the material for the new editor
            _materialCopy = new SGMaterial();
            _materialCopy.DiffuseTexture = material.DiffuseTexture.Copy();
            _materialCopy.NormalTexture = material.NormalTexture.Copy();
            _materialCopy.SpecularTexture = material.SpecularTexture.Copy();
            _materialCopy.Name = material.Name;
            _materialCopy.Shininess = material.Shininess;
            _materialCopy.Opacity = material.Opacity;
            _materialCopy.Color = material.Color;


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

            _editorPanel3D = new EditorPanel3D(this.splitContainer1.Panel2);
            _editorPanel3D.EditorLoaded += EditorPanel3D_EditorLoaded;

            var perspectiveCamera = new PerspectiveCamera(new Vector3(0f, 0f, -5f), new Vector3(800, 600, 0));
            perspectiveCamera.LookAt(new Vector3(0, 0, 0));
            _editorPanel3D.Camera = perspectiveCamera;

            var scene3d = new Scene3D("LAYER");
            scene3d.Sun = new DirectionalLight(new Vector3(0f, 0f, -6f), new Vector4(1, 1, 1, 1), 1.5f);
            _editorPanel3D.Scene = scene3d;

            var materialCube = new Primitive("MaterialCube", _materialCopy, new Cube());
            materialCube.Transform.Position = new Vector3(0, 0, 0);
            materialCube.Transform.Scale = new Vector3(3, 3, 3);
            scene3d.AddGameElement("LAYER", materialCube);
        }

        private void EditorPanel3D_EditorLoaded(object sender, EventArgs e)
        {
            _materialCopy.Init(_editorPanel3D.Renderer);
        }

        private void MaterialEditor_Load(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MaterialEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            _materialCopy.Dispose(_editorPanel3D.Renderer);
            _editorPanel3D.Scene.DisposeScene(_editorPanel3D.Renderer);
            _editorPanel3D.Renderer.Dispose();
        }
    }
}
