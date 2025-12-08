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
using LibGFX.Math;
using LibGFX.Physics;
using LibGFX.UI;
using NewGFXEditor.Editor;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.CodeDom;
using System.Diagnostics;
using System.Security.Authentication.ExtendedProtection;
using System.Windows.Forms;
using static System.Formats.Asn1.AsnWriter;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace NewGFXEditor
{
    /// <summary>
    /// Main form for the 3D graphics editor application.
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// The main camera used for rendering the scene.
        /// </summary>
        public Camera Camera { get; set; }

        /// <summary>
        /// Gets or sets the current scene associated with the application context.
        /// </summary>
        /// <remarks>Assigning a new scene replaces the existing scene and may trigger scene-specific
        /// initialization or cleanup logic, depending on the implementation of the scene management system.</remarks>
        public BaseScene Scene { get; set; }

        /// <summary>
        /// The editor panel for 3D rendering and interaction.
        /// </summary>
        public EditorPanel3D Editor { get => _editorPanel3D; }

        /// <summary>
        /// The transform gizmo for manipulating objects in the scene.
        /// </summary>
        public Gizmo TransformGizmo { get; set; }

        EditorPanel3D _editorPanel3D;
        bool _dragCamera = false;
        Vector2 _mousePos;
        GameElement _selectedElement = null;
        Layer _selectedLayer = null;
        PhysicsHandler3D _phyisicHandler3D;

        public Form1()
        {
            InitializeComponent();
            _editorPanel3D = new EditorPanel3D(this.splitContainer1.Panel2);
            _editorPanel3D.EditorLoaded += EditorPanel3D_EditorLoaded;
            _editorPanel3D.OnKeyDown += EditorPanel3D_OnKeyDown;
            _editorPanel3D.BeforeRender += EditorPanel3D_BeforeRender;
            _editorPanel3D.OnRender += EditorPanel3D_OnRender;
            _editorPanel3D.AfterRender += EditorPanel3D_AfterRender;
            _editorPanel3D.OnMouseDown += EditorPanel3D_OnMouseDown;
            _editorPanel3D.OnMouseMove += EditorPanel3D_OnMouseMove;
            _editorPanel3D.OnMouseUp += EditorPanel3D_OnMouseUp;
            _editorPanel3D.OnResized += _editorPanel3D_OnResized;
            LoadStartupAssets();
            this.UpdateGUI();
        }

        private void _editorPanel3D_OnResized(object sender, Viewport viewport, EventArgs e)
        {
            if(this.Camera is PerspectiveCamera pc)
            {
                pc.Resolution = new Vector2(viewport.Width, viewport.Height);
            }
        }

        public Primitive CreateQube(Vector3 position, Vector3 scale, Vector3 rotation, SGMaterial material)
        {
            var cube = new Primitive("Cube", material, new Cube());
            cube.Transform.Position = position;
            cube.Transform.Scale = scale;
            cube.Transform.Rotate(rotation);
            return cube;
        }

        public Primitive CreateSphere(Vector3 position, Vector3 scale, Vector3 rotation, SGMaterial material)
        {
            var sphere = new Primitive("Sphere", material, new Sphere());
            sphere.Transform.Position = position;
            sphere.Transform.Scale = scale;
            sphere.Transform.Rotate(rotation);
            return sphere;
        }

        public Primitive CreateQuad(Vector3 position, Vector3 scale, Vector3 rotation, SGMaterial material)
        {
            var quad = new Primitive("Quad", material, new Quad());
            quad.Transform.Position = position;
            quad.Transform.Scale = scale;
            quad.Transform.Rotate(rotation);
            return quad;
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
            this.TransformGizmo.ReleaseGizmo();
        }

        // Handle mouse movement within the editor panel
        private void EditorPanel3D_OnMouseMove(object sender, MouseEventArgs e)
        {
            // Highlight the gizmo based on mouse position
            TransformGizmo.HighlightGizmo((PerspectiveCamera)Camera, _editorPanel3D.Viewport, e.X, e.Y);

            // Move the gizmo or rotate the camera
            bool setNewMousePos = false;
            if (TransformGizmo.ActiveAxis != GizmoActiveAxis.None)
            {
                if (TransformGizmo.Type == GizmoType.Translation)
                {
                    TransformGizmo.MoveAlongAxis2D((PerspectiveCamera)Camera, _editorPanel3D.Viewport, (int)_mousePos.X, (int)_mousePos.Y, e.X, e.Y);
                }
                else if (TransformGizmo.Type == GizmoType.Scale)
                {
                    TransformGizmo.ScaleAlongAxis((PerspectiveCamera)Camera, _editorPanel3D.Viewport, (int)_mousePos.X, (int)_mousePos.Y, e.X, e.Y);
                }
                setNewMousePos = true;
            }

            // Rotate the camera
            if (_dragCamera)
            {
                var delataX = e.X - _mousePos.X;
                var delataY = e.Y - _mousePos.Y;
                Camera.Transform.Rotate(new Vector3(-delataY * 0.1f, -delataX * 0.1f, 0.0f));
                setNewMousePos = true;
            }

            // Update the mouse position
            if (setNewMousePos)
            {
                _mousePos = new Vector2(e.X, e.Y);
            }
        }

        private void EditorPanel3D_OnMouseDown(object sender, MouseEventArgs e)
        {
            // Start dragging the camera
            if (e.Button == MouseButtons.Right)
            {
                if (!_dragCamera)
                {
                    _dragCamera = true;
                    _mousePos = new Vector2(e.X, e.Y);
                }
            }

            // Pick gizmo or scene element
            if (e.Button == MouseButtons.Left)
            {
                var gizmoPicked = this.TransformGizmo.PickGizmo((PerspectiveCamera)Camera, _editorPanel3D.Viewport, e.X, e.Y);
                if (gizmoPicked)
                {
                    _mousePos = new Vector2(e.X, e.Y);
                }
                else
                {
                    var pickedElement = PickElement((PerspectiveCamera)Camera, Scene.GetAllElements(), e.X, e.Y, _editorPanel3D.Viewport);
                    if (pickedElement != null)
                    {
                        _selectedElement = pickedElement;
                        this.propertyGrid1.SelectedObject = _selectedElement;
                        this.TransformGizmo.Transform.Position = _selectedElement.Transform.Position;
                        this.TransformGizmo.Enabled = true;
                    }
                    else
                    {
                        _selectedElement = null;
                        this.propertyGrid1.SelectedObject = null;
                        this.TransformGizmo.Enabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// Pick an element from the scene based on the mouse position and camera view.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="elements"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="viewport"></param>
        /// <returns></returns>
        private GameElement? PickElement(PerspectiveCamera camera, IEnumerable<GameElement> elements, int x, int y, Viewport viewport)
        {
            var minDist = float.MaxValue;
            GameElement hitElement = null;
            var ray = MeshRaycast.ScreenPointToWorldRay(camera, viewport, x, y);
            foreach (var element in elements)
            {
                var meshes = element.GetMeshes();
                if (meshes == null)
                {
                    continue;
                }
                    
                foreach (var mesh in meshes)
                {
                    var hit = MeshRaycast.IntersectsMesh(ray, element.Transform, mesh);
                    if (hit.Hit && hit.Distance < minDist)
                    {
                        minDist = hit.Distance;
                        hitElement = element;
                    }
                }
            }
            return hitElement;
        }

        private void EditorPanel3D_BeforeRender(object sender, EventArgs e)
        {
            _editorPanel3D.Renderer.SetViewport(new Viewport(_editorPanel3D.GLControl.Width, _editorPanel3D.GLControl.Height));
        }

        private void EditorPanel3D_OnRender(object sender, EventArgs e)
        {
            _phyisicHandler3D.Process(Scene);
            this.Scene.Render(_editorPanel3D.Viewport, _editorPanel3D.Renderer, Camera);
            this.TransformGizmo.RenderGizmo(_editorPanel3D.Renderer, Camera, _editorPanel3D.Viewport);
            if (showAABBsToolStripMenuItem.Checked)
            {
                this.Scene.ForEachElement(element =>
                {
                    var aabb = element.WorldAABB;
                    _editorPanel3D.Renderer.DrawAABB(aabb, new Vector4(1, 0, 0, 1));
                });
            }
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
            else if(e.KeyCode == Keys.Delete)
            {
                if (_selectedElement != null)
                {
                    this.TransformGizmo.Enabled = false;
                    _selectedElement.Dispose(Scene, _editorPanel3D.Renderer);
                    Scene.RemoveElement(_selectedElement);
                    _selectedElement = null;
                    this.propertyGrid1.SelectedObject = null;
                    this.UpdateGUI();
                    _editorPanel3D.Redraw();
                }
            }
        }

        /// <summary>
        /// Create the initial assets and setup the scene.
        /// </summary>
        private void LoadStartupAssets()
        {
            // Create the 3D Cameara
            var perspectiveCamera = new PerspectiveCamera(new Vector3(0, 5, -10), new Vector2(800, 600));
            perspectiveCamera.LookAt(new Vector3(0, 0, 0));
            Camera = perspectiveCamera;

            // Creates an 3D Scene
            var scene3d = new Scene3D("BASE_LAYER", "OBJECT_LAYER", "PLAYER_LAYER", "AI_LAYER");
            scene3d.DirectionalLight = new DirectionalLight3D(new Vector3(-0.2f, 1.0f, -0.3f), new Vector4(1, 1, 1, 1), 1.5f);
            Scene = scene3d;

            // Create an procedural sky for the environment
            scene3d.Enviroment = new ProceduralSky();

            // Create the physics handler
            _phyisicHandler3D = new PhysicsHandler3D(Vector3.Zero);
            scene3d.PhysicsHandler = _phyisicHandler3D;

            // Create an defaul material and add it to the asset manager
            var blankMaterial = new SGMaterial("e_BlankMaterial", Vector4.One);
            GFX.Instance.AssetManager.Add<SGMaterial>(blankMaterial.Name, blankMaterial);

            // Create an cube mesh
            var cubeMesh = new Cube().GetMesh();
            GFX.Instance.AssetManager.Add<Mesh>("e_CubeMesh", cubeMesh);

            // Create an sphere mesh
            var sphereMesh = new Sphere().GetMesh();
            GFX.Instance.AssetManager.Add<Mesh>("e_SphereMesh", sphereMesh);

            // Create an plane mesh
            var planeMesh = new Quad().GetMesh();
            GFX.Instance.AssetManager.Add<Mesh>("e_PlaneMesh", planeMesh);

            // Create a cube and add it to the scene
            var cube = this.CreateQube(new Vector3(0, 0, 0), new Vector3(1, 1, 1), Vector3.Zero, blankMaterial);
            Scene.AddGameElement("OBJECT_LAYER", cube);

            // Load Gizmos
            TransformGizmo = new Gizmo("Assets/Gizmos/Transform/TransformGizmo.obj");
            TransformGizmo.GizmoMoved += TransformGizmo_GizmoMoved;
            TransformGizmo.GizmoScaled += TransformGizmo_GizmoScaled;
        }

        private void TransformGizmo_GizmoScaled(float scaleFactor)
        {
            Debug.WriteLine($"Gizmo scaled with factor: {scaleFactor}");
            if (_selectedElement != null)
            {
                if (TransformGizmo.ActiveAxis == GizmoActiveAxis.X)
                {
                    _selectedElement.Transform.Scale += new Vector3(scaleFactor, 0, 0);
                }
                else if (TransformGizmo.ActiveAxis == GizmoActiveAxis.Y)
                {
                    _selectedElement.Transform.Scale += new Vector3(0, scaleFactor, 0);
                }
                else if (TransformGizmo.ActiveAxis == GizmoActiveAxis.Z)
                {
                    _selectedElement.Transform.Scale += new Vector3(0, 0, scaleFactor);
                }
                _editorPanel3D.Redraw();
            }
        }

        /// <summary>
        /// Event handler for when the transform gizmo is moved.
        /// </summary>
        /// <param name="newPosition"></param>
        private void TransformGizmo_GizmoMoved(Vector3 newPosition)
        {
            if (_selectedElement != null)
            {
                _selectedElement.Transform.Position = newPosition;
            }
        }

        /// <summary>
        /// Updates the GUI elements such as the scene tree, material list view, and layers combobox.
        /// </summary>
        private void UpdateGUI()
        {
            this.UpdateSceneTree();
            this.UpdateMaterialListView();
            this.UpdateLayersCombobox();
        }

        /// <summary>
        /// Updates the scene tree view to reflect the current state of the scene.
        /// </summary>
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

        /// <summary>
        /// Updates the material list view with the materials from the asset manager.
        /// </summary>
        private void UpdateMaterialListView()
        {
            this.materialListView.Items.Clear();

            GFX.Instance.AssetManager.ForeachAsset<IMaterial>(material =>
            {
                if (this.materialImageList.Images.ContainsKey(material.Name))
                {
                    var item = new ListViewItem(material.Name, material.Name);
                    item.Tag = material;
                    this.materialListView.Items.Add(item);
                }
            });

        }

        /// <summary>
        /// Updates the layers combobox with the names of the layers in the scene.
        /// </summary>
        private void UpdateLayersCombobox()
        {
            this.layerComboBox.Items.Clear();
            foreach (var layer in Scene.Layers)
            {
                this.layerComboBox.Items.Add(layer.Name);
            }

            if (_selectedLayer != null)
            {
                this.layerComboBox.SelectedItem = _selectedLayer.Name;
            }
            else
            {
                this.layerComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Event handler for when the editor panel is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditorPanel3D_EditorLoaded(object sender, EventArgs e)
        {
            // Initialize the materials within the asset manager
            GFX.Instance.AssetManager.ForeachAsset<IMaterial>(material =>
            {
                material.Init(_editorPanel3D.Renderer);
            });

            // Initialize the meshes within the asset manager
            GFX.Instance.AssetManager.ForeachAsset<Mesh>(mesh =>
            {
                _editorPanel3D.Renderer.LoadMesh(mesh);
            });

            // Initilize the scene
            this.Scene.Init(_editorPanel3D.Viewport, _editorPanel3D.Renderer);

            // Create the physics debug drawer
            _phyisicHandler3D.PhysicsWorld.DebugDrawer = new DebugDrawer(_editorPanel3D.Renderer);
            _phyisicHandler3D.PhysicsWorld.DebugDrawer.DebugMode = BulletSharp.DebugDrawModes.DrawAabb;
            _phyisicHandler3D.DebugPhysics = true;

            // Initialize the transform gizmo
            TransformGizmo.Init(_editorPanel3D.Renderer, _editorPanel3D.Viewport);
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
            var material = GFX.Instance.AssetManager.Load<SGMaterial>(path);
            if (material != null)
            {
                if (material.DiffuseTexture == null)
                {
                    material.DiffuseTexture = new Texture(1, 1, new Vector4i(255, 255, 255, 255));
                }

                if (material.NormalTexture == null)
                {
                    material.NormalTexture = new Texture(1, 1, new Vector4i(128, 128, 255, 255));
                }

                if (material.SpecularTexture == null)
                {
                    material.SpecularTexture = new Texture(1, 1, new Vector4i(0, 0, 0, 255));
                }


                material.Init(_editorPanel3D.Renderer);
                if (showEditor)
                {
                    var materialEditor = new MaterialEditor(material);
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

        /// <summary>
        /// Creatres a new material and opens the material editor for it.
        /// </summary>
        public void CreateMaterial()
        {
            var materialCount = GFX.Instance.AssetManager.GetAssetCount<SGMaterial>();
            var blankMaterial = new SGMaterial($"e_NewMaterial_{materialCount + 1}", Vector4.One);

            var materialEditor = new MaterialEditor(blankMaterial);
            if(materialEditor.ShowDialog() == DialogResult.OK)
            {
                blankMaterial.Init(_editorPanel3D.Renderer);
                GFX.Instance.AssetManager.Add<SGMaterial>(blankMaterial.Name, blankMaterial);
                this.SetMaterialThumbnail(blankMaterial.Name, blankMaterial.DiffuseTexture.ToBitmap());
                this.UpdateMaterialListView();
            }
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
                        primitive.Mesh.Material = material;
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var renderer = _editorPanel3D.Renderer;

            GFX.Instance.AssetManager.ForeachAsset<IMaterial>(material =>
            {
                material.Dispose(renderer);
            });

            GFX.Instance.AssetManager.ForeachAsset<Mesh>(mesh =>
            {
                renderer.DisposeMesh(mesh);
            });

            Scene.DisposeScene(renderer);
            TransformGizmo.Dispose(renderer);
            renderer.Dispose();
        }

        private void layerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectLayerFromName(this.layerComboBox.SelectedItem.ToString());
        }

        private void SelectLayerFromName(String name)
        {
            foreach (var layer in Scene.Layers)
            {
                if (layer.Name == name)
                {
                    _selectedLayer = layer;
                    break;
                }
            }
        }

        private void cubeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var primitive = this.CreateQube(new Vector3(0, 0, 0), new Vector3(1, 1, 1), Vector3.Zero, GFX.Instance.AssetManager.Load<SGMaterial>("e_BlankMaterial"));
            primitive.Init(Scene, _editorPanel3D.Viewport, _editorPanel3D.Renderer);
            Scene.AddGameElement(_selectedLayer.Name, primitive);
        }

        private void sphereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var primitive = this.CreateSphere(new Vector3(0, 0, 0), new Vector3(1, 1, 1), Vector3.Zero, GFX.Instance.AssetManager.Load<SGMaterial>("e_BlankMaterial"));
            primitive.Init(Scene, _editorPanel3D.Viewport, _editorPanel3D.Renderer);
            Scene.AddGameElement(_selectedLayer.Name, primitive);
        }

        private void quadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var primitive = this.CreateQuad(new Vector3(0, 0, 0), new Vector3(1, 1, 1), Vector3.Zero, GFX.Instance.AssetManager.Load<SGMaterial>("e_BlankMaterial"));
            primitive.Init(Scene, _editorPanel3D.Viewport, _editorPanel3D.Renderer);
            Scene.AddGameElement(_selectedLayer.Name, primitive);
        }

        private void editPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_selectedElement != null)
            {
                var vec3Editor = new Vec3Editor(_selectedElement.Transform.Position);
                if (vec3Editor.ShowDialog() == DialogResult.OK)
                {
                    _selectedElement.Transform.Position = vec3Editor.Value;
                    _editorPanel3D.Redraw();
                }
            }
        }

        private void editRotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_selectedElement != null)
            {
                var vec3Editor = new Vec3Editor(_selectedElement.Transform.GetEulerAngles());
                if (vec3Editor.ShowDialog() == DialogResult.OK)
                {
                    _selectedElement.Transform.Rotate(vec3Editor.Value);
                    _editorPanel3D.Redraw();
                }
            }
        }

        private void editScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_selectedElement != null)
            {
                var vec3Editor = new Vec3Editor(_selectedElement.Transform.Scale);
                if (vec3Editor.ShowDialog() == DialogResult.OK)
                {
                    _selectedElement.Transform.Scale = vec3Editor.Value;
                    _editorPanel3D.Redraw();
                }
            }
        }

        private void gizmoModeTranslateBtn_Click(object sender, EventArgs e)
        {
            TransformGizmo.Type = GizmoType.Translation;
        }

        private void gizmoModeScaleBtn_Click(object sender, EventArgs e)
        {
            TransformGizmo.Type = GizmoType.Scale;
        }

        private void modelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Model Files|*.obj;*.fbx;*.gltf;*.glb";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var staticMeshModel = GFX.Instance.AssetManager.Load<StaticMeshModel>(ofd.FileName);
                if(staticMeshModel.State != ModelState.Initialized)
                {
                    staticMeshModel.Init(_editorPanel3D.Renderer);
                    Debug.WriteLine($"Initializing new StaticMeshModel from {ofd.FileName}");
                }
                else
                {
                    Debug.WriteLine($"Using cached StaticMeshModel from {ofd.FileName}");
                }

                var model = new StaticModel("Model", staticMeshModel);
                Scene.AddGameElement(_selectedLayer.Name, model);
                _editorPanel3D.Redraw();
                this.UpdateGUI();
            }
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_selectedElement != null)
            {
                _selectedElement.Dispose(Scene, _editorPanel3D.Renderer);
                Scene.RemoveElement(_selectedElement);
            }
            this.UpdateGUI();
        }

        private void showAABBsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _editorPanel3D.Redraw();
        }
    }
}
