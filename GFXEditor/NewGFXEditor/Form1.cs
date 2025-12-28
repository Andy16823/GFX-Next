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
using NewGFXEditor.Exporter;
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

        private EditorPanel3D _editorPanel3D;
        private bool _dragCamera = false;
        private Vector2 _mousePos;
        private GameElement _selectedElement = null;
        private Layer _selectedLayer = null;
        private PhysicsHandler3D _phyisicHandler3D;
        private bool _sceneEnabled = true;

        /// <summary>
        /// Initializes a new instance of the Form1 class and sets up the 3D editor panel and related event handlers.
        /// </summary>
        /// <remarks>This constructor configures the main form by initializing UI components, creating the
        /// 3D editor panel, and subscribing to relevant editor events. It also loads startup assets and updates the GUI
        /// to ensure the form is ready for user interaction after construction.</remarks>
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

        /// <summary>
        /// Displays a dialog that allows the user to save the current scene to a GFX level file.
        /// </summary>
        /// <remarks>If the user selects a file and confirms the dialog, the current scene is exported to
        /// the specified file in the GFX level format. If the user cancels the dialog, no file is saved.</remarks>
        public void SaveSceneAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "GFX Level Files|*.gfxlevel";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                GFXExporter exporter = new GFXExporter();
                exporter.Export(sfd.FileName, this.Scene as Scene3D, GFX.Instance.AssetManager);
            }
        }

        public void OpenScene()
        {
            // Dispose existing scene
            _sceneEnabled = false;
            GFX.Instance.AssetManager.DisposeAssets(_editorPanel3D.Renderer);
            GFX.Instance.AssetManager.ClearAssets();
            Scene.FreeScene(_editorPanel3D.Renderer);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "GFX Level Files|*.gfxlevel";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                GFXExporter exporter = new GFXExporter();
                exporter.Import(openFileDialog.FileName, this.Scene as Scene3D, GFX.Instance.AssetManager);
                GFX.Instance.AssetManager.InitializeAssets(_editorPanel3D.Renderer);
                Scene.InitializeElements(_editorPanel3D.Viewport, _editorPanel3D.Renderer);
                _sceneEnabled = true;
                this.UpdateGUI();
                _editorPanel3D.Redraw();
            }
        }

        /// <summary>
        /// Handles the resize event for the 3D editor panel and updates the camera resolution to match the new viewport
        /// size.
        /// </summary>
        /// <param name="sender">The source of the event, typically the 3D editor panel that was resized.</param>
        /// <param name="viewport">The viewport representing the new dimensions of the 3D editor panel after resizing.</param>
        /// <param name="e">An object that contains the event data.</param>
        private void _editorPanel3D_OnResized(object sender, Viewport viewport, EventArgs e)
        {
            if (this.Camera is PerspectiveCamera pc)
            {
                pc.Resolution = new Vector2(viewport.Width, viewport.Height);
            }
        }

        /// <summary>
        /// Creates a cube primitive with the specified position, scale, rotation, and material.
        /// </summary>
        /// <param name="position">The position of the cube in world coordinates.</param>
        /// <param name="scale">The scale of the cube along each axis.</param>
        /// <param name="rotation">The rotation of the cube, specified in degrees for each axis.</param>
        /// <param name="material">The material to apply to the cube. Cannot be null.</param>
        /// <returns>A <see cref="Primitive"/> instance representing the created cube with the specified transformation and
        /// material.</returns>
        public Primitive CreateCube(Vector3 position, Vector3 scale, Vector3 rotation, SGMaterial material)
        {
            // Create the cube primitive
            var cube = Primitive.CreatePrimitive("Cube", material, GFX.Instance.AssetManager, Primitive.PrimitiveType.Cube);

            // Initialize the cube's mesh
            cube.Mesh.Init(_editorPanel3D.Renderer);

            // Set the cube's transform properties
            cube.Transform.Position = position;
            cube.Transform.Scale = scale;
            cube.Transform.Rotate(rotation);
            return cube;
        }

        /// <summary>
        /// Creates a new sphere primitive with the specified position, scale, rotation, and material.
        /// </summary>
        /// <param name="position">The position of the sphere in world coordinates.</param>
        /// <param name="scale">The scale to apply to the sphere along each axis.</param>
        /// <param name="rotation">The rotation to apply to the sphere, specified as Euler angles in degrees.</param>
        /// <param name="material">The material to assign to the sphere. Cannot be null.</param>
        /// <returns>A new <see cref="Primitive"/> instance representing the created sphere.</returns>
        public Primitive CreateSphere(Vector3 position, Vector3 scale, Vector3 rotation, SGMaterial material)
        {
            // Create the sphere primitive
            var sphere = Primitive.CreatePrimitive("Sphere", material, GFX.Instance.AssetManager, Primitive.PrimitiveType.Sphere);

            // Initialize the sphere's mesh
            sphere.Mesh.Init(_editorPanel3D.Renderer);

            // Set the sphere's transform properties
            sphere.Transform.Position = position;
            sphere.Transform.Scale = scale;
            sphere.Transform.Rotate(rotation);
            return sphere;
        }

        /// <summary>
        /// Creates a quad primitive with the specified position, scale, rotation, and material.
        /// </summary>
        /// <param name="position">The position of the quad in world space coordinates.</param>
        /// <param name="scale">The scale to apply to the quad along each axis.</param>
        /// <param name="rotation">The rotation to apply to the quad, specified in degrees for each axis.</param>
        /// <param name="material">The material to apply to the quad. Cannot be null.</param>
        /// <returns>A new <see cref="Primitive"/> instance representing the configured quad.</returns>
        public Primitive CreateQuad(Vector3 position, Vector3 scale, Vector3 rotation, SGMaterial material)
        {
            // Create the quad primitive
            var quad = Primitive.CreatePrimitive("Quad", material, GFX.Instance.AssetManager, Primitive.PrimitiveType.Quad);

            // Initialize the quad's mesh
            quad.Mesh.Init(_editorPanel3D.Renderer);

            // Set the quad's transform properties
            quad.Transform.Position = position;
            quad.Transform.Scale = scale;
            quad.Transform.Rotate(rotation);
            return quad;
        }

        /// <summary>
        /// Sets the thumbnail image associated with the specified material name, replacing any existing thumbnail.
        /// </summary>
        /// <remarks>If a thumbnail already exists for the specified material name, it is replaced with
        /// the new image. The material list view is updated to reflect the change.</remarks>
        /// <param name="materialName">The name of the material for which to set the thumbnail image. Cannot be null or empty.</param>
        /// <param name="bitmap">The image to use as the thumbnail for the material. Cannot be null.</param>
        public void SetMaterialThumbnail(String materialName, Bitmap bitmap)
        {
            if (this.materialImageList.Images.ContainsKey(materialName))
            {
                this.materialImageList.Images.RemoveByKey(materialName);
            }

            this.materialImageList.Images.Add(materialName, bitmap);
            UpdateMaterialListView();
        }

        /// <summary>
        /// Handles the MouseUp event for the 3D editor panel, ending camera drag operations and releasing the transform
        /// gizmo.
        /// </summary>
        /// <param name="sender">The source of the event, typically the 3D editor panel control.</param>
        /// <param name="e">A MouseEventArgs that contains the event data for the mouse button release.</param>
        private void EditorPanel3D_OnMouseUp(object sender, MouseEventArgs e)
        {
            _dragCamera = false;
            this.TransformGizmo.ReleaseGizmo();
        }

        /// <summary>
        /// Handles the MouseMove event for the 3D editor panel, updating gizmo highlighting, object manipulation, or
        /// camera rotation based on the current interaction state.
        /// </summary>
        /// <remarks>This method enables interactive manipulation of objects or the camera within the 3D
        /// editor view. Depending on the current tool and user action, it may highlight gizmo axes, move or scale
        /// objects, or rotate the camera in response to mouse movement.</remarks>
        /// <param name="sender">The source of the event, typically the 3D editor panel control.</param>
        /// <param name="e">A MouseEventArgs that contains the event data, including the current mouse position.</param>
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

        /// <summary>
        /// Handles mouse down events on the 3D editor panel to initiate camera dragging or select scene elements and
        /// gizmos.
        /// </summary>
        /// <remarks>Right mouse button presses begin camera drag operations. Left mouse button presses
        /// attempt to pick and select a gizmo or scene element at the mouse position. If a scene element is selected,
        /// its properties and transform gizmo are updated accordingly.</remarks>
        /// <param name="sender">The source of the event, typically the 3D editor panel.</param>
        /// <param name="e">A MouseEventArgs that contains the event data, including mouse button and position information.</param>
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

        /// <summary>
        /// Handles the event that occurs before the 3D editor panel is rendered, updating the renderer's viewport to
        /// match the current control size.
        /// </summary>
        /// <param name="sender">The source of the event, typically the 3D editor panel control.</param>
        /// <param name="e">An object that contains the event data.</param>
        private void EditorPanel3D_BeforeRender(object sender, EventArgs e)
        {
            _editorPanel3D.Renderer.SetViewport(new Viewport(_editorPanel3D.GLControl.Width, _editorPanel3D.GLControl.Height));
        }

        /// <summary>
        /// Handles the render event for the 3D editor panel, updating the scene and drawing visual elements such as the
        /// scene contents, transform gizmo, and optional bounding boxes.
        /// </summary>
        /// <remarks>This method is typically invoked as part of the rendering loop for the 3D editor
        /// panel. If the option to display axis-aligned bounding boxes (AABBs) is enabled, the method will render
        /// bounding boxes for all scene elements to assist with visualization and debugging.</remarks>
        /// <param name="sender">The source of the event, typically the 3D editor panel control.</param>
        /// <param name="e">An object that contains the event data.</param>
        private void EditorPanel3D_OnRender(object sender, EventArgs e)
        {
            if (_sceneEnabled)
            {
                _phyisicHandler3D.Process(Scene);
                this.Scene.Render(_editorPanel3D.Viewport, _editorPanel3D.Renderer, Camera);
                _editorPanel3D.Renderer.DrawRenderTarget(Scene.RenderTarget as MSAARenderTarget2D, 0);
            }

            this.TransformGizmo.RenderGizmo(_editorPanel3D.Renderer, Camera, _editorPanel3D.Viewport);
            if (_selectedElement != null)
            {
                var aabb = _selectedElement.WorldAABB;
                _editorPanel3D.Renderer.DrawAABB(aabb, ColorPresets.LightCyan);
            }
            if (showAABBsToolStripMenuItem.Checked)
            {
                this.Scene.ForEachElement(element =>
                {
                    if (element == _selectedElement)
                    {
                        return;
                    }
                    var aabb = element.WorldAABB;
                    _editorPanel3D.Renderer.DrawAABB(aabb, ColorPresets.LimeGreen);
                });
            }
        }

        /// <summary>
        /// Handles the event that occurs after the 3D editor panel has completed rendering.
        /// </summary>
        /// <param name="sender">The source of the event, typically the 3D editor panel.</param>
        /// <param name="e">An object that contains the event data.</param>
        private void EditorPanel3D_AfterRender(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles key down events for the 3D editor panel, enabling camera movement and element deletion based on user
        /// input.
        /// </summary>
        /// <remarks>Pressing the W, A, S, or D keys moves the camera in the corresponding direction.
        /// Pressing the Delete key removes the currently selected element from the scene if one is selected.</remarks>
        /// <param name="sender">The source of the event, typically the 3D editor panel control.</param>
        /// <param name="e">A KeyEventArgs that contains the event data, including information about the pressed key.</param>
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
            else if (e.KeyCode == Keys.Delete)
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

            // Create a cube and add it to the scene
            //var cube = this.CreateCube(new Vector3(0, 0, 0), new Vector3(1, 1, 1), Vector3.Zero, blankMaterial);
            //Scene.AddGameElement("OBJECT_LAYER", cube);

            // Load Gizmos
            TransformGizmo = new Gizmo("Assets/Gizmos/Transform/TransformGizmo.obj");
            TransformGizmo.GizmoMoved += TransformGizmo_GizmoMoved;
            TransformGizmo.GizmoScaled += TransformGizmo_GizmoScaled;
        }

        /// <summary>
        /// Handles scaling of the selected gizmo element along the currently active axis by the specified scale factor.
        /// </summary>
        /// <remarks>This method applies scaling only if an element is currently selected. The scaling is
        /// performed along the axis that is active in the transform gizmo. After scaling, the 3D editor panel is
        /// redrawn to reflect the changes.</remarks>
        /// <param name="scaleFactor">The amount by which to scale the selected element along the active axis. Positive values increase the size;
        /// negative values decrease it.</param>
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
            this.materialImageList.Images.Clear();

            GFX.Instance.AssetManager.ForeachAsset<SGMaterial>(material =>
            {
                var thumbnail = material.DiffuseTexture.ToBitmap();
                this.materialImageList.Images.Add(material.Name, thumbnail);

                var item = new ListViewItem(material.Name, material.Name);
                item.Tag = material;
                this.materialListView.Items.Add(item);
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
            GFX.Instance.AssetManager.InitializeAssets(_editorPanel3D.Renderer);

            // Initilize the scene
            this.Scene.Init(_editorPanel3D.Viewport, _editorPanel3D.Renderer);

            // Create the physics debug drawer
            _phyisicHandler3D.PhysicsWorld.DebugDrawer = new DebugDrawer(_editorPanel3D.Renderer);
            _phyisicHandler3D.PhysicsWorld.DebugDrawer.DebugMode = BulletSharp.DebugDrawModes.DrawAabb;
            _phyisicHandler3D.DebugPhysics = true;

            // Initialize the transform gizmo
            TransformGizmo.Init(_editorPanel3D.Renderer, _editorPanel3D.Viewport);
        }

        /// <summary>
        /// Handles the event that occurs when the form is loaded.
        /// </summary>
        /// <param name="sender">The source of the event, typically the form being loaded.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles the KeyDown event for the form and processes key press information.
        /// </summary>
        /// <param name="sender">The source of the event, typically the form that received the key press.</param>
        /// <param name="e">A KeyEventArgs that contains the event data for the key press.</param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Debug.WriteLine(e.KeyCode.ToString());
        }

        /// <summary>
        /// Handles the AfterSelect event of the TreeView control to display the selected node's associated object in
        /// the property grid.
        /// </summary>
        /// <param name="sender">The source of the event, typically the TreeView control.</param>
        /// <param name="e">A TreeViewEventArgs that contains the event data for the selected node.</param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.treeView1.SelectedNode != null)
            {
                this.propertyGrid1.SelectedObject = this.treeView1.SelectedNode.Tag;
            }
        }

        /// <summary>
        /// Imports a material from the specified file path and optionally displays the material editor for further
        /// editing.
        /// </summary>
        /// <remarks>If the imported material is missing any of its standard textures (diffuse, normal, or
        /// specular), default textures are assigned automatically. When showEditor is false, the method updates the
        /// material thumbnail and list view instead of opening the editor.</remarks>
        /// <param name="path">The file path to the material asset to import. Must refer to a valid material file.</param>
        /// <param name="showEditor">true to display the material editor after importing; otherwise, false to import the material without opening
        /// the editor. The default is true.</param>
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

        /// <summary>
        /// Handles the Click event of the Import Material menu item and initiates the process of importing material
        /// from a selected file.
        /// </summary>
        /// <param name="sender">The source of the event, typically the Import Material menu item.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
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
            if (materialEditor.ShowDialog() == DialogResult.OK)
            {
                blankMaterial.Init(_editorPanel3D.Renderer);
                GFX.Instance.AssetManager.Add<SGMaterial>(blankMaterial.Name, blankMaterial);
                this.SetMaterialThumbnail(blankMaterial.Name, blankMaterial.DiffuseTexture.ToBitmap());
                this.UpdateMaterialListView();
            }
        }

        /// <summary>
        /// Handles the Click event of the Material Editor menu item and initiates the material editor workflow.
        /// </summary>
        /// <param name="sender">The source of the event, typically the Material Editor menu item.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void materialEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CreateMaterial();
        }

        /// <summary>
        /// Assigns the material selected in the material list view to the currently selected primitive element in the
        /// 3D editor.
        /// </summary>
        /// <remarks>This method updates the material of the selected primitive and refreshes the 3D
        /// editor view to reflect the change. The method has no effect if no primitive is selected or if no material is
        /// selected in the list view.</remarks>
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

        /// <summary>
        /// Handles the Click event of the Assign Selected Material menu item and initiates the assignment of the
        /// currently selected material.
        /// </summary>
        /// <param name="sender">The source of the event, typically the Assign Selected Material menu item.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void assignSelectedMaterialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssignSelectedMaterial();
        }

        /// <summary>
        /// Handles the DoubleClick event of the material list view to assign the currently selected material.
        /// </summary>
        /// <param name="sender">The source of the event, typically the material list view control.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void materialListView_DoubleClick(object sender, EventArgs e)
        {
            AssignSelectedMaterial();
        }

        /// <summary>
        /// Handles the DragDrop event for the material list view, importing files that are dropped onto the control.
        /// </summary>
        /// <param name="sender">The source of the event, typically the material list view control.</param>
        /// <param name="e">A DragEventArgs that contains the event data, including information about the dropped files.</param>
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

        /// <summary>
        /// Handles the DragEnter event for the material list view to determine whether the dragged data contains files
        /// and sets the appropriate drag-and-drop effect.
        /// </summary>
        /// <param name="sender">The source of the event, typically the material list view control.</param>
        /// <param name="e">A DragEventArgs that contains the event data, including information about the data being dragged.</param>
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

        /// <summary>
        /// Handles the FormClosing event to release resources and perform cleanup before the form is closed.
        /// </summary>
        /// <remarks>This method ensures that assets, scene resources, and rendering components are
        /// properly disposed of when the form is closing. It should be attached to the FormClosing event of the form to
        /// prevent resource leaks.</remarks>
        /// <param name="sender">The source of the event, typically the form being closed.</param>
        /// <param name="e">A FormClosingEventArgs that contains the event data, including the reason for closing and the ability to
        /// cancel the event.</param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            GFX.Instance.AssetManager.DisposeAssets(_editorPanel3D.Renderer);
            Scene.DisposeScene(_editorPanel3D.Renderer);
            TransformGizmo.Dispose(_editorPanel3D.Renderer);
            _editorPanel3D.Renderer.Dispose();
        }

        /// <summary>
        /// Handles the event that occurs when the selected item in the layerComboBox control changes.
        /// </summary>
        /// <param name="sender">The source of the event, typically the layerComboBox control.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void layerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectLayerFromName(this.layerComboBox.SelectedItem.ToString());
        }

        /// <summary>
        /// Selects the first layer in the scene that matches the specified name.
        /// </summary>
        /// <remarks>If multiple layers have the same name, only the first matching layer is selected. If
        /// no layer with the specified name exists, the current selection remains unchanged.</remarks>
        /// <param name="name">The name of the layer to select. Comparison is case-sensitive.</param>
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

        /// <summary>
        /// Handles the Click event of the Cube menu item to add a new cube primitive to the current scene.
        /// </summary>
        /// <param name="sender">The source of the event, typically the Cube menu item.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void cubeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var primitive = this.CreateCube(new Vector3(0, 0, 0), new Vector3(1, 1, 1), Vector3.Zero, GFX.Instance.AssetManager.Load<SGMaterial>("e_BlankMaterial"));
            primitive.Init(Scene, _editorPanel3D.Viewport, _editorPanel3D.Renderer);
            Scene.AddGameElement(_selectedLayer.Name, primitive);
        }

        /// <summary>
        /// Handles the Click event of the Sphere menu item to add a new sphere primitive to the current scene.
        /// </summary>
        /// <param name="sender">The source of the event, typically the Sphere menu item.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void sphereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var primitive = this.CreateSphere(new Vector3(0, 0, 0), new Vector3(1, 1, 1), Vector3.Zero, GFX.Instance.AssetManager.Load<SGMaterial>("e_BlankMaterial"));
            primitive.Init(Scene, _editorPanel3D.Viewport, _editorPanel3D.Renderer);
            Scene.AddGameElement(_selectedLayer.Name, primitive);
        }

        /// <summary>
        /// Handles the Click event of the Quad menu item to add a new quad primitive to the current scene.
        /// </summary>
        /// <param name="sender">The source of the event, typically the Quad menu item.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void quadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var primitive = this.CreateQuad(new Vector3(0, 0, 0), new Vector3(1, 1, 1), Vector3.Zero, GFX.Instance.AssetManager.Load<SGMaterial>("e_BlankMaterial"));
            primitive.Init(Scene, _editorPanel3D.Viewport, _editorPanel3D.Renderer);
            Scene.AddGameElement(_selectedLayer.Name, primitive);
        }

        /// <summary>
        /// Handles the Click event of the Edit Position menu item, allowing the user to modify the position of the
        /// currently selected element.
        /// </summary>
        /// <remarks>If no element is currently selected, this handler does nothing. After a successful
        /// edit, the 3D editor panel is refreshed to reflect the updated position.</remarks>
        /// <param name="sender">The source of the event, typically the Edit Position menu item.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
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

        /// <summary>
        /// Handles the Click event of the Edit Rotation menu item, allowing the user to modify the rotation of the
        /// currently selected element.
        /// </summary>
        /// <remarks>This method displays a rotation editor dialog for the selected element. If the user
        /// confirms the changes, the element's rotation is updated and the 3D editor panel is redrawn to reflect the
        /// modification. If no element is selected, the method does nothing.</remarks>
        /// <param name="sender">The source of the event, typically the Edit Rotation menu item.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
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

        /// <summary>
        /// Handles the Click event of the Edit Scale menu item, allowing the user to modify the scale of the currently
        /// selected 3D element.
        /// </summary>
        /// <remarks>If no element is selected, this handler does nothing. After a successful scale edit,
        /// the 3D editor panel is refreshed to reflect the changes.</remarks>
        /// <param name="sender">The source of the event, typically the Edit Scale menu item.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
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

        /// <summary>
        /// Handles the Click event of the Translate gizmo mode button and sets the transform gizmo to translation mode.
        /// </summary>
        /// <param name="sender">The source of the event, typically the button that was clicked.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void gizmoModeTranslateBtn_Click(object sender, EventArgs e)
        {
            TransformGizmo.Type = GizmoType.Translation;
        }

        /// <summary>
        /// Handles the Click event of the Scale gizmo mode button, setting the transform gizmo to Scale mode.
        /// </summary>
        /// <param name="sender">The source of the event, typically the button that was clicked.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void gizmoModeScaleBtn_Click(object sender, EventArgs e)
        {
            TransformGizmo.Type = GizmoType.Scale;
        }

        /// <summary>
        /// Handles the Click event for the Model menu item, allowing the user to import a 3D model file into the
        /// current scene.
        /// </summary>
        /// <remarks>Supported model file formats include OBJ, FBX, GLTF, and GLB. The imported model is
        /// added to the currently selected scene layer and displayed in the 3D editor panel.</remarks>
        /// <param name="sender">The source of the event, typically the Model menu item.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void modelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Model Files|*.obj;*.fbx;*.gltf;*.glb";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var staticMeshModel = GFX.Instance.AssetManager.Load<StaticMeshModel>(ofd.FileName);
                if (!staticMeshModel.IsInitialized)
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

        /// <summary>
        /// Handles the Click event of the Create menu item to initiate the creation process.
        /// </summary>
        /// <param name="sender">The source of the event, typically the Create menu item.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles the Click event of the Delete menu item to remove the currently selected element from the scene.
        /// </summary>
        /// <remarks>If no element is selected when the menu item is clicked, no action is taken. After
        /// deletion, the user interface is updated to reflect the change.</remarks>
        /// <param name="sender">The source of the event, typically the Delete menu item.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_selectedElement != null)
            {
                _selectedElement.Dispose(Scene, _editorPanel3D.Renderer);
                Scene.RemoveElement(_selectedElement);
            }
            this.UpdateGUI();
        }

        /// <summary>
        /// Handles the Click event of the Show AABBs menu item to update the 3D editor panel display.
        /// </summary>
        /// <param name="sender">The source of the event, typically the Show AABBs menu item.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void showAABBsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _editorPanel3D.Redraw();
        }

        /// <summary>
        /// Handles the Click event of the Add String menu item, allowing the user to add a new string property to the
        /// currently selected element.
        /// </summary>
        /// <remarks>If no element is selected when the menu item is clicked, an error message is
        /// displayed and no property is added.</remarks>
        /// <param name="sender">The source of the event, typically the Add String menu item.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void addStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_selectedElement != null)
            {
                var stringPropertyDialog = new Dialogs.StringProperty("Key", "Value");
                if (stringPropertyDialog.ShowDialog() == DialogResult.OK)
                {
                    var data = stringPropertyDialog.Data;
                    _selectedElement.Properties.Add(data.Name, data.Value);
                }
            }
            else
            {
                MessageBox.Show("No element selected to add string property.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the Add Collision Type menu item, allowing the user to add a collision type
        /// property to the currently selected element.
        /// </summary>
        /// <remarks>If no element is selected when the menu item is clicked, an error message is
        /// displayed and no action is taken.</remarks>
        /// <param name="sender">The source of the event, typically the Add Collision Type menu item.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void addCollisionTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_selectedElement != null)
            {
                var collisionTypePropertyDialog = new Dialogs.CollisionTypeProperty(String.Empty);
                if (collisionTypePropertyDialog.ShowDialog() == DialogResult.OK)
                {
                    _selectedElement.Properties.Add("CollisionType", collisionTypePropertyDialog.CollisionType);
                }
            }
            else
            {
                MessageBox.Show("No element selected to add collision type property.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event for the GFX Level File menu item, allowing the user to export the current 3D scene
        /// to a GFX Level file.
        /// </summary>
        /// <remarks>Displays a Save File dialog for the user to specify the destination file. If a file
        /// is selected, exports the current scene to the specified GFX Level file format.</remarks>
        /// <param name="sender">The source of the event, typically the menu item that was clicked.</param>
        /// <param name="e">An EventArgs instance containing event data.</param>
        private void gFXLevelFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSceneAs();
        }

        /// <summary>
        /// Handles the Click event for the GFX Level File menu item and initiates the process of opening a scene.
        /// </summary>
        /// <param name="sender">The source of the event, typically the menu item that was clicked.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void gFXLevelFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenScene();
        }

        /// <summary>
        /// Handles the Click event of the neuToolStripButton control and releases resources associated with the current
        /// 3D scene.
        /// </summary>
        /// <param name="sender">The source of the event, typically the neuToolStripButton control.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void neuToolStripButton_Click(object sender, EventArgs e)
        {
            this.Scene.FreeScene(_editorPanel3D.Renderer);
        }

        /// <summary>
        /// Handles the Click event of the Öffnen menu item to initiate the process of opening a scene.
        /// </summary>
        /// <param name="sender">The source of the event, typically the Öffnen menu item.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void öffnenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenScene();
        }

        private void speichernunterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSceneAs();
        }
    }
}
