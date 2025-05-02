using LibGFX.Assets;
using LibGFX.Assets.Loaders;
using LibGFX.Audio;
using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Primitives;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Diagnostics;
using System.Security.Authentication.ExtendedProtection;

namespace NewGFXEditor
{
    public partial class Form1 : Form
    {
        public Camera Camera { get; set; }
        public BaseScene Scene { get; set; }

        AssetManager _assetManager;
        EditorPanel3D _editorPanel3D;
        bool _dragCamera = false;
        Vector2 _mousePos;


        public Form1()
        {
            InitializeComponent();

            // Load the asset manager and register loaders
            _assetManager = new AssetManager();
            _assetManager.RegisterLoader<Texture>(new TextureLoader());
            _assetManager.RegisterLoader<AudioClip>(new AudioLoader());
            _assetManager.RegisterLoader<SGMaterial>(new SGMaterialLoader());
            _assetManager.RegisterLoader<Model>(new ModelLoader());
            _assetManager.RegisterLoader<Cubemap>(new CubemapLoader());
            _assetManager.RegisterLoader<MeshCollection>(new MeshLoader());
            _assetManager.RegisterLoader<SpriteMaterial>(new SpriteMaterialLoader());

            // Load the editor panel
            _editorPanel3D = new EditorPanel3D(this.splitContainer1.Panel2);
            _editorPanel3D.EditorLoaded += EditorPanel3D_EditorLoaded;
            _editorPanel3D.OnKeyDown += EditorPanel3D_OnKeyDown;
            _editorPanel3D.BeforeRender += EditorPanel3D_BeforeRender;
            _editorPanel3D.OnRender += EditorPanel3D_OnRender;
            _editorPanel3D.AfterRender += EditorPanel3D_AfterRender;
            _editorPanel3D.OnMouseDown += EditorPanel3D_OnMouseDown;
            _editorPanel3D.OnMouseMove += EditorPanel3D_OnMouseMove;
            _editorPanel3D.OnMouseUp += EditorPanel3D_OnMouseUp;

            // Load startup assets
            LoadStartupAssets();

            // Load the scene tree
            UpdateSceneTree();
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
                Camera.Transform.Rotate(new Vector3(-delataY * 0.1f, -delataX * 0.1f, 0.0f));
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


        private void EditorPanel3D_BeforeRender(object sender, EventArgs e)
        {
            _editorPanel3D.ResizeCamera(Camera);
        }

        private void EditorPanel3D_OnRender(object sender, EventArgs e)
        {
            this.Scene.Render(_editorPanel3D.Viewport, _editorPanel3D.Renderer, Camera);
        }
        private void EditorPanel3D_AfterRender(object sender, EventArgs e)
        {
            
        }

        private void EditorPanel3D_OnKeyDown(object sender, KeyEventArgs e)
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
        }

        private void LoadStartupAssets()
        {
            // Create the 3D Cameara
            var perspectiveCamera = new PerspectiveCamera(new Vector3(10, 5, 0), new Vector3(800, 600, 0));
            perspectiveCamera.LookAt(new Vector3(0, 0, 0));
            Camera = perspectiveCamera;

            // Creates an 3D Scene
            var scene3d = new Scene3D("BASE_LAYER", "OBJECT_LAYER", "PLAYER_LAYER", "AI_LAYER");
            scene3d.Sun = new DirectionalLight(new Vector3(-0.2f, 1.0f, -0.3f), new Vector4(1, 1, 1, 1), 1.5f);
            Scene = scene3d;

            var blankDiffuseBitmap = Utils.CreateEmptyTexture(1, 1);
            var blankNormalBitmap = Utils.CreateEmptyNormalMap(1, 1);

            // Load assets
            var blankMaterial = new SGMaterial();
            blankMaterial.Name = "BlankMaterial";
            blankMaterial.DiffuseTexture = Texture.LoadTexture(blankDiffuseBitmap);
            blankMaterial.NormalTexture = Texture.LoadTexture(blankNormalBitmap);
            blankMaterial.SpecularTexture = Texture.LoadTexture(blankDiffuseBitmap);
            _assetManager.AddAsset<SGMaterial>(blankMaterial.Name, blankMaterial);

            // Create a cube
            var cube = new Primitive("Cube", blankMaterial, new Cube());
            cube.Transform.Position = new Vector3(0, 0, 0);
            cube.Transform.Scale = new Vector3(1, 1, 1);
            Scene.AddGameElement("OBJECT_LAYER", cube);

        }

        private void UpdateSceneTree()
        {
            this.treeView1.Nodes.Clear();
            var rootNode = new TreeNode("Scene");
            rootNode.Tag = Scene;
            foreach (var layer in Scene.Layers)
            {
                var layerNode = new TreeNode(layer.Name);
                layerNode.Tag = layer;
                foreach (var gameElement in layer.Elements)
                {
                    var elementNode = new TreeNode(gameElement.Name);
                    elementNode.Tag = gameElement;
                    layerNode.Nodes.Add(elementNode);
                }
                rootNode.Nodes.Add(layerNode);
            }
            this.treeView1.Nodes.Add(rootNode);
            this.treeView1.ExpandAll();
        }

        private void EditorPanel3D_EditorLoaded(object sender, EventArgs e)
        {
            _assetManager.ForeachAsset<IMaterial>(material =>
            {
                material.Init(_editorPanel3D.Renderer);
            });

            this.Scene.Init(_editorPanel3D.Viewport, _editorPanel3D.Renderer);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Debug.WriteLine(e.KeyCode.ToString());
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.treeView1.SelectedNode != null)
            {
                this.propertyGrid1.SelectedObject = this.treeView1.SelectedNode.Tag;
            }
        }

        private void importMaterialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var material = _assetManager.Load<SGMaterial>(openFileDialog.FileName);
                if (material != null)
                {
                    if(material.DiffuseTexture == null)
                    {
                        var blankDiffuseBitmap = Utils.CreateEmptyTexture(1, 1);
                        material.DiffuseTexture = Texture.LoadTexture(blankDiffuseBitmap);
                    }

                    if(material.NormalTexture == null)
                    {
                        var blankNormalBitmap = Utils.CreateEmptyNormalMap(1, 1);
                        material.NormalTexture = Texture.LoadTexture(blankNormalBitmap);
                    }

                    if (material.SpecularTexture == null)
                    {
                        var blankDiffuseBitmap = Utils.CreateEmptyTexture(1, 1);
                        material.SpecularTexture = Texture.LoadTexture(blankDiffuseBitmap);
                    }

                    material.Init(_editorPanel3D.Renderer);
                    var materialEditor = new MaterialEditor(_assetManager, material, _editorPanel3D);
                    materialEditor.Show();
                }
            }
        }
    }
}
