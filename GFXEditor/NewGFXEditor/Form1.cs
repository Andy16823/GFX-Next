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
        AssetManager _assetManager;
        EditorPanel3D _editorPanel3D;


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

            // Load startup assets
            LoadStartupAssets();

            // Load the scene tree
            UpdateSceneTree();
        }

        private void LoadStartupAssets()
        {
            // Create the 3D Cameara
            var perspectiveCamera = new PerspectiveCamera(new Vector3(10, 5, 0), new Vector3(800, 600, 0));
            perspectiveCamera.LookAt(new Vector3(0, 0, 0));
            _editorPanel3D.Camera = perspectiveCamera;

            // Creates an 3D Scene
            var scene3d = new Scene3D("BASE_LAYER", "OBJECT_LAYER", "PLAYER_LAYER", "AI_LAYER");
            scene3d.Sun = new DirectionalLight(new Vector3(-0.2f, 1.0f, -0.3f), new Vector4(1, 1, 1, 1), 1.5f);
            _editorPanel3D.Scene = scene3d;

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
            _editorPanel3D.Scene.AddGameElement("OBJECT_LAYER", cube);

        }

        private void UpdateSceneTree()
        {
            this.treeView1.Nodes.Clear();
            var rootNode = new TreeNode("Scene");
            rootNode.Tag = _editorPanel3D.Scene;
            foreach (var layer in _editorPanel3D.Scene.Layers)
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
            if(this.treeView1.SelectedNode != null)
            {
                this.propertyGrid1.SelectedObject = this.treeView1.SelectedNode.Tag;
            }
        }
    }
}
