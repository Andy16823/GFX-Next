using LibGFX;
using LibGFX.Assets;
using LibGFX.Assets.Loaders;
using LibGFX.Audio;
using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Graphics;
using LibGFX.Graphics.Enviroment;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Primitives;
using LibGFX.Pyhsics;
using LibGFX.Pyhsics.Behaviors3D;
using LibGFX.UI;
using NewGFXEditor.Editor;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Diagnostics;
using System.Security.Authentication.ExtendedProtection;
using System.Windows.Forms;
using static System.Formats.Asn1.AsnWriter;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace NewGFXEditor
{
    public partial class Form1 : Form
    {
        public Camera Camera { get; set; }
        public BaseScene Scene { get; set; }
        public EditorPanel3D Editor { get => _editorPanel3D; }
        public AssetManager AssetManager { get => _assetManager; }
        public Gizmo TransformGizmo { get; set; }

        PhysicsHandler3D _phyisicHandler3D;
        AssetManager _assetManager;
        EditorPanel3D _editorPanel3D;
        bool _dragCamera = false;
        Vector2 _mousePos;
        GameElement _selectedElement = null;
        ColorIDPicker _colorIDPicker = new ColorIDPicker();

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

        public void CreateQube(Vector3 position, Vector3 scale, Vector3 rotation, SGMaterial material)
        {
            var cube = new Primitive("Cube", material, new Cube());
            cube.Transform.Position = position;
            cube.Transform.Scale = scale;
            cube.Transform.Rotate(rotation);
            Scene.AddGameElement("OBJECT_LAYER", cube);
        }

        public void SetMaterialThumbnail(String materialName, Bitmap bitmap)
        {
            if (this.materialImageList.Images.ContainsKey(materialName))
            {
                this.materialImageList.Images.RemoveByKey(materialName);
            }

            this.materialImageList.Images.Add(materialName, bitmap);
            UpdateMaterialListView();
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

            if (e.Button == MouseButtons.Left)
            {
                ColorPickResult result;
                GameElement element;
                _colorIDPicker.PerformScenePick(Scene, e.X, e.Y, out result, out element);

                if(result.Success)
                {
                    _selectedElement = element;
                    this.propertyGrid1.SelectedObject = _selectedElement;
                }
            }
        }


        private void EditorPanel3D_BeforeRender(object sender, EventArgs e)
        {
            _editorPanel3D.Renderer.SetViewport(new Viewport(_editorPanel3D.GLControl.Width, _editorPanel3D.GLControl.Height));
            _editorPanel3D.ResizeCamera(Camera);
        }

        private void EditorPanel3D_OnRender(object sender, EventArgs e)
        {
            _phyisicHandler3D.Process(Scene);
            this.Scene.Render(_editorPanel3D.Viewport, _editorPanel3D.Renderer, Camera);

            this.TransformGizmo.RenderGizmo(_editorPanel3D.Renderer, Camera, _editorPanel3D.Viewport);

            _colorIDPicker.PrepareSceneForPicking(_editorPanel3D.Renderer, _editorPanel3D.Viewport, Camera, Scene);

            this.pictureBox1.Image = _colorIDPicker.FramebufferToBitmap();
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

            scene3d.Enviroment = new ProceduralSky();

            // Create the physics handler
            _phyisicHandler3D = new PhysicsHandler3D(Vector3.Zero);
            scene3d.PhysicsHandler = _phyisicHandler3D;

            var blankDiffuseBitmap = Utils.CreateEmptyTexture(1, 1);
            var blankNormalBitmap = Utils.CreateEmptyNormalMap(1, 1);

            // Load assets
            var blankMaterial = new SGMaterial();
            blankMaterial.Name = "e_BlankMaterial";
            blankMaterial.DiffuseTexture = Texture.LoadTexture(blankDiffuseBitmap);
            blankMaterial.NormalTexture = Texture.LoadTexture(blankNormalBitmap);
            blankMaterial.SpecularTexture = Texture.LoadTexture(blankDiffuseBitmap);
            _assetManager.AddAsset<SGMaterial>(blankMaterial.Name, blankMaterial);

            var cubeMesh = new Cube().GetMesh();
            _assetManager.AddAsset<Mesh>("e_CubeMesh", cubeMesh);

            var sphereMesh = new Sphere().GetMesh();
            _assetManager.AddAsset<Mesh>("e_SphereMesh", sphereMesh);

            var planeMesh = new Quad().GetMesh();
            _assetManager.AddAsset<Mesh>("e_PlaneMesh", planeMesh);

            // Create a cube and add it to the scene
            this.CreateQube(new Vector3(0, 0, 0), new Vector3(1, 1, 1), Vector3.Zero, blankMaterial);

            // Load Gizmos
            TransformGizmo = new Gizmo("Assets/Gizmos/Transform/TransformGizmo.obj");
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

        private void UpdateMaterialListView()
        {
            this.materialListView.Items.Clear();

            _assetManager.ForeachAsset<IMaterial>(material =>
            {
                if (this.materialImageList.Images.ContainsKey(material.Name))
                {
                    var item = new ListViewItem(material.Name, material.Name);
                    item.Tag = material;
                    this.materialListView.Items.Add(item);
                }
            });

        }

        private void EditorPanel3D_EditorLoaded(object sender, EventArgs e)
        {
            _assetManager.ForeachAsset<IMaterial>(material =>
            {
                material.Init(_editorPanel3D.Renderer);
            });

            _assetManager.ForeachAsset<Mesh>(mesh =>
            {
                _editorPanel3D.Renderer.LoadMesh(mesh);
            });

            this.Scene.Init(_editorPanel3D.Viewport, _editorPanel3D.Renderer);

            _phyisicHandler3D.PhysicsWorld.DebugDrawer = new DebugDrawer(_editorPanel3D.Renderer);
            _phyisicHandler3D.PhysicsWorld.DebugDrawer.DebugMode = BulletSharp.DebugDrawModes.DrawAabb;
            _phyisicHandler3D.DebugPhysics = true;

            _colorIDPicker.Init(_editorPanel3D.Renderer, _editorPanel3D.Viewport);

            TransformGizmo.Init(_editorPanel3D.Renderer);
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

        public void ImportMaterial(String path, bool showEditor = true)
        {
            var material = _assetManager.Load<SGMaterial>(path);
            if (material != null)
            {
                if (material.DiffuseTexture == null)
                {
                    var blankDiffuseBitmap = Utils.CreateEmptyTexture(1, 1);
                    material.DiffuseTexture = Texture.LoadTexture(blankDiffuseBitmap);
                }

                if (material.NormalTexture == null)
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
                if(showEditor)
                {
                    var materialEditor = new MaterialEditor(this, material);
                    materialEditor.Show();
                }
                else
                {
                    this.SetMaterialThumbnail(material.Name, material.DiffuseTexture.ToBitmap());
                    this.UpdateMaterialListView();
                }
            }
        }

        private void importMaterialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.ImportMaterial(openFileDialog.FileName);
            }
        }

        private static SGMaterial CreateMaterial(String name)
        {
            var material = new SGMaterial();
            material.Name = name;

            var blankDiffuseBitmap = Utils.CreateEmptyTexture(512, 512);
            material.DiffuseTexture = Texture.LoadTexture(blankDiffuseBitmap);

            var blankNormalBitmap = Utils.CreateEmptyNormalMap(512, 512);
            material.NormalTexture = Texture.LoadTexture(blankNormalBitmap);

            var blankSpecularBitmap = Utils.CreateEmptyTexture(512, 512);
            material.SpecularTexture = Texture.LoadTexture(blankSpecularBitmap);

            material.Color = new Vector4(1, 1, 1, 1);

            return material;
        }

        public void CreateMaterial()
        {
            var materialCount = _assetManager.GetAssetCount<SGMaterial>();
            var blankMaterial = CreateMaterial($"material_{materialCount}");
            blankMaterial.Init(_editorPanel3D.Renderer);
            _assetManager.AddAsset<SGMaterial>(blankMaterial.Name, blankMaterial);

            var materialEditor = new MaterialEditor(this, blankMaterial);
            materialEditor.Show();
        }

        private void materialEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CreateMaterial();
        }

        public void AssignSelectedMaterial()
        {
            if (_selectedElement != null)
            {
                if (_selectedElement.GetType() == typeof(Primitive))
                {
                    var primitive = (Primitive)_selectedElement;
                    if (this.materialListView.SelectedItems.Count > 0)
                    {
                        var selectedItem = this.materialListView.SelectedItems[0];
                        var material = (SGMaterial)selectedItem.Tag;
                        primitive.Material = material;
                        _editorPanel3D.Redraw();
                    }
                }
            }
        }

        private void assignSelectedMaterialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssignSelectedMaterial();
        }

        private void materialListView_DoubleClick(object sender, EventArgs e)
        {
            AssignSelectedMaterial();
        }

        private void materialListView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    ImportMaterial(file, false);
                }
            }
        }

        private void materialListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
    }
}
