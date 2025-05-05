using Assimp;
using Assimp.Configs;
using LibGFX.Assets;
using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Shader;
using LibGFX.Math;
using Microsoft.VisualBasic.Devices;
using NewGFXEditor.Shader;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace NewGFXEditor.Editor
{
    /// <summary>
    /// Enum to represent the active axis of the gizmo.
    /// </summary>
    public enum GizmoActiveAxis
    {
        None,
        X,
        Y,
        Z
    }

    /// <summary>
    /// Class representing a gizmo for 3D transformations.
    /// </summary>
    public class Gizmo
    {
        /// <summary>
        /// Delegate for handling gizmo movement events.
        /// </summary>
        /// <param name="newPosition"></param>
        public delegate void GizmoMoveDelegate(Vector3 newPosition);

        /// <summary>
        /// Collection of meshes that make up the gizmo.
        /// </summary>
        public MeshCollection Meshes { get; set; }

        /// <summary>
        /// Collection of materials used by the gizmo.
        /// </summary>
        public MaterialCollection Materials { get; set; }

        /// <summary>
        /// List of mesh-material pairs for the gizmo.
        /// </summary>
        public List<MeshMaterialPair> MeshMaterials { get; set; }

        /// <summary>
        /// Transform object representing the gizmo's position, rotation, and scale.
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// Shader program used for rendering the gizmo.
        /// </summary>
        public ShaderProgram Shader { get; set; } = new GizmoShader();

        /// <summary>
        /// Picker object for ID picking.
        /// </summary>
        public ColorIDPicker Picker { get; set; } = new ColorIDPicker();

        /// <summary>
        /// Active axis of the gizmo.
        /// </summary>
        public GizmoActiveAxis ActiveAxis { get; set; } = GizmoActiveAxis.None;

        /// <summary>
        /// Flag indicating whether the gizmo is enabled or not.
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Event triggered when the gizmo is moved.
        /// </summary>
        public event GizmoMoveDelegate GizmoMoved;

        /// <summary>
        /// Flag indicating whether to swap the X and Z axes.
        /// </summary>
        private bool _swapXZAxes = false;

        /// <summary>
        /// Constructor for the Gizmo class.
        /// </summary>
        /// <param name="file"></param>
        public Gizmo(String file)
        {
            Transform = new Transform();
            Transform.Position = new Vector3(0, 0, 0);
            Transform.Scale = new Vector3(0.5f, 0.5f, 0.5f);
            Transform.Rotate(0.0f, 180.0f, 0.0f);

            Meshes = new MeshCollection();
            Materials = new MaterialCollection();
            MeshMaterials = new List<MeshMaterialPair>();

            // Load the model using Assimp
            var importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            var assimpScene = importer.ImportFile(file, Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.CalculateTangentSpace | Assimp.PostProcessSteps.JoinIdenticalVertices);

            // Extract materials and meshes
            ExtractMaterials(assimpScene);
            ExtractMeshes(assimpScene);

            LoadNodeTransformRecursive(assimpScene.RootNode, Matrix4x4.Identity);
        }

        /// <summary>
        /// Initializes the gizmo with the specified render device and viewport.
        /// </summary>
        /// <param name="renderDevice"></param>
        /// <param name="viewport"></param>
        public void Init(IRenderDevice renderDevice, Viewport viewport)
        {
            renderDevice.BuildShaderProgram(this.Shader);

            foreach (var mesh in this.Meshes)
            {
                renderDevice.LoadMesh(mesh);
            }

            foreach (var material in this.Materials)
            {
                material.Init(renderDevice);
            }

            this.Picker.Init(renderDevice, viewport);
        }
        
        /// <summary>
        /// Renders the gizmo using the specified render device, camera, and viewport.
        /// </summary>
        /// <param name="renderDevice"></param>
        /// <param name="camera"></param>
        /// <param name="viewport"></param>
        public void RenderGizmo(IRenderDevice renderDevice, LibGFX.Graphics.Camera camera, Viewport viewport)
        {
            // Scale the gizmo based on the camera and viewport
            this.ScaleGizmo((PerspectiveCamera)camera, viewport, 25.0f);

            // Render the gizmo with ID picking
            Picker.StartIdRenderPass(renderDevice, viewport, camera);
            int id = 1;
            foreach (var pair in MeshMaterials)
            {
                var mesh = this.Meshes.GetMesh(pair.MeshName);
                var material = this.Materials.GetMaterial(pair.MaterialIndex);
                Picker.RenderMesh(renderDevice, Transform, mesh, material, id);
                id++;
            }
            Picker.EndIdRenderPass(renderDevice);

            // Render the gizmo with the actual shader
            if(!this.Enabled)
                return;

            renderDevice.SetProjectionMatrix(camera.GetProjectionMatrix(viewport));
            renderDevice.BindShaderProgram(this.Shader);
            foreach (var pair in MeshMaterials)
            {
                var mesh = this.Meshes.GetMesh(pair.MeshName);
                var material = this.Materials.GetMaterial(pair.MaterialIndex);
                renderDevice.DrawMesh(this.Transform, mesh, material);
            }
            renderDevice.UnbindShaderProgram();
        }

        /// <summary>
        /// Disposes of the gizmo resources.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Dispose(IRenderDevice renderDevice)
        {
            foreach (var mesh in this.Meshes)
            {
                renderDevice.DisposeMesh(mesh);
            }
            foreach (var material in this.Materials)
            {
                material.Dispose(renderDevice);
            }
            renderDevice.DisposeShaderProgram(this.Shader);
            this.Picker.Dispose(renderDevice);
        }

        /// <summary>
        /// Highlights the gizmo based on mouse position.
        /// </summary>
        /// <param name="mouseX"></param>
        /// <param name="mouseY"></param>
        public void HighlightGizmo(int mouseX, int mouseY)
        {
            if(this.Enabled == false)
                return;

            ColorPickResult reuslt;
            this.Picker.PerformPick(mouseX, mouseY, out reuslt);
            this.UnhoverMaterials();

            if (reuslt.Success)
            {
                this.HoverMaterial(reuslt.Id);
            }
        }

        /// <summary>
        /// Scales the gizmo based on the camera and viewport.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="viewport"></param>
        /// <param name="desiredPixelHeight"></param>
        public void ScaleGizmo(PerspectiveCamera camera, Viewport viewport, float desiredPixelHeight = 100f)
        {
            if (this.Enabled == false)
                return;

            Vector3 cameraPos = camera.Transform.Position;
            float distance = (this.Transform.Position - cameraPos).Length;

            float fovRadians = MathHelper.DegreesToRadians(camera.Fov);
            float screenHeightAtDistance = 2.0f * distance * (float)System.Math.Tan(fovRadians / 2.0f);

            float pixelToWorld = screenHeightAtDistance / viewport.Height;
            float desiredWorldHeight = desiredPixelHeight * pixelToWorld;

            this.Transform.Scale = new Vector3(desiredWorldHeight);
        }

        /// <summary>
        /// Checks if the gizmo was picked by the mouse.
        /// </summary>
        /// <param name="mosueX"></param>
        /// <param name="mouseY"></param>
        /// <returns></returns>
        public bool PickGizmo(int mosueX, int mouseY)
        {
            if(!this.Enabled)
                return false;

            ColorPickResult reuslt;
            this.Picker.PerformPick(mosueX, mouseY, out reuslt);

            if (reuslt.Success)
            {
                int id = reuslt.Id;
                // Find the corresponding mesh and material
                var pair = this.MeshMaterials[id];
                var mesh = this.Meshes.GetMesh(pair.MeshName);

                if(mesh.Name.StartsWith("Gizmo_X", StringComparison.OrdinalIgnoreCase)) {
                    this.ActiveAxis = GizmoActiveAxis.X;
                    return true;
                }
                else if(mesh.Name.StartsWith("Gizmo_Y", StringComparison.OrdinalIgnoreCase))
                {
                    this.ActiveAxis = GizmoActiveAxis.Y;
                    return true;
                }
                else if (mesh.Name.StartsWith("Gizmo_Z", StringComparison.OrdinalIgnoreCase))
                {
                    this.ActiveAxis = GizmoActiveAxis.Z;
                    return true;
                }
            }
            this.ActiveAxis = GizmoActiveAxis.None;
            return false;
        }

        /// <summary>
        /// Releases the gizmo, setting the active axis to none.
        /// </summary>
        public void ReleaseGizmo()
        {
            this.ActiveAxis = GizmoActiveAxis.None;
        }

        /// <summary>
        /// Moves the gizmo along the specified axis based on mouse movement.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="viewport"></param>
        /// <param name="prevMouseX"></param>
        /// <param name="prevMouseY"></param>
        /// <param name="currMouseX"></param>
        /// <param name="currMouseY"></param>
        public void MoveAlongAxis2D(PerspectiveCamera camera, Viewport viewport, int prevMouseX, int prevMouseY, int currMouseX, int currMouseY)
        {
            if (this.ActiveAxis == GizmoActiveAxis.None || this.Enabled == false)
                return;

            Vector3 axisWorld = GetAxisDirection(this.ActiveAxis, _swapXZAxes);
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix(viewport);

            Vector4 gizmoWorld = new Vector4(Transform.Position, 1.0f);
            Vector4 gizmoClip = gizmoWorld * view * projection;
            gizmoClip /= gizmoClip.W;

            Vector3 gizmoScreen = new Vector3(
                (gizmoClip.X * 0.5f + 0.5f) * viewport.Width, 
                (1.0f - (gizmoClip.Y * 0.5f + 0.5f)) * viewport.Height, 
                0);

            Vector4 endWorld = new Vector4(this.Transform.Position + axisWorld, 1.0f);
            Vector4 endClip = endWorld * view * projection;
            endClip /= endClip.W;

            Vector3 endScreen = new Vector3(
                (endClip.X * 0.5f + 0.5f) * viewport.Width, 
                (1.0f - (endClip.Y * 0.5f + 0.5f)) * viewport.Height, 
                0);

            Vector2 axisScreenDir = (endScreen - gizmoScreen).Xy.Normalized();
            Vector2 mouseDelta = new Vector2(currMouseX - prevMouseX, currMouseY - prevMouseY);

            float movementOnAxis = Vector2.Dot(mouseDelta, axisScreenDir);
            float scaleFactor = 0.01f; // ggf. dynamisch skalieren

            this.Transform.Position += axisWorld * movementOnAxis * scaleFactor;

            if (this.GizmoMoved != null)
            {
                this.GizmoMoved(this.Transform.Position);
            }
        }

        /// <summary>
        /// Unhovers all materials in the gizmo.
        /// </summary>
        private void UnhoverMaterials()
        {
            foreach (var pair in this.MeshMaterials)
            {
                var mesh = this.Meshes.GetMesh(pair.MeshName);
                var mat = (GizmoMaterial)this.Materials.GetMaterial(pair.MaterialIndex);
                mat.Hovered = false;
            }
        }

        /// <summary>
        /// Sets the hovered state of a specific material based on its ID.
        /// </summary>
        /// <param name="id"></param>
        private void HoverMaterial(int id)
        {
            var pair = this.MeshMaterials[id];
            var material = (GizmoMaterial)this.Materials.GetMaterial(pair.MaterialIndex);
            material.Hovered = true;
        }

        /// <summary>
        /// Gets the direction of the specified axis.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        private Vector3 GetAxisDirection(GizmoActiveAxis axis, bool swapXZ = false)
        {
            switch (axis)
            {
                case GizmoActiveAxis.X:
                    if(swapXZ)
                    {
                        return Vector3.UnitZ;
                    }

                    return Vector3.UnitX;
                case GizmoActiveAxis.Y:
                    return Vector3.UnitY;
                case GizmoActiveAxis.Z:
                    if (swapXZ)
                    {
                        return Vector3.UnitX;
                    }

                    return Vector3.UnitZ;
                default:
                    return Vector3.Zero;
            }
        }

        /// <summary>
        /// Extracts materials from the Assimp scene and adds them to the gizmo's material collection.
        /// </summary>
        /// <param name="assimpScene"></param>
        private void ExtractMaterials(Scene assimpScene)
        {
            var materials = new List<IMaterial>();

            // Load materials
            foreach (var asmat in assimpScene.Materials)
            {
                var material = new GizmoMaterial();
                material.Name = asmat.Name;
                material.VertexColor = new Vector4(asmat.ColorDiffuse.R, asmat.ColorDiffuse.G, asmat.ColorDiffuse.B, asmat.ColorDiffuse.A);

                materials.Add(material);
            }

            this.Materials.AddRange(materials);
        }

        /// <summary>
        /// Extracts meshes from the Assimp scene and adds them to the gizmo's mesh collection.
        /// </summary>
        /// <param name="assimpScene"></param>
        private void ExtractMeshes(Scene assimpScene)
        {
            var meshes = new List<LibGFX.Graphics.Mesh>();

            foreach (var asmesh in assimpScene.Meshes)
            {
                var mesh = new LibGFX.Graphics.Mesh();
                mesh.Name = asmesh.Name;

                for (int i = 0; i < asmesh.VertexCount; i++)
                {
                    var vertex = new LibGFX.Graphics.Vertex();

                    vertex.Position = new Vector3(asmesh.Vertices[i].X, asmesh.Vertices[i].Y, asmesh.Vertices[i].Z);
                    vertex.Normal = new Vector3(asmesh.Normals[i].X, asmesh.Normals[i].Y, asmesh.Normals[i].Z);
                    vertex.TexCoord = new Vector2(asmesh.TextureCoordinateChannels[0][i].X, asmesh.TextureCoordinateChannels[0][i].Y);
                    vertex.Tangent = new Vector4(asmesh.Tangents[i].X, asmesh.Tangents[i].Y, asmesh.Tangents[i].Z, 1.0f);
                    mesh.Vertices.Add(vertex);
                }

                mesh.Indices.AddRange(asmesh.GetIndices());
                meshes.Add(mesh);

                var meshMaterialPair = new MeshMaterialPair();
                meshMaterialPair.MeshName = asmesh.Name;
                meshMaterialPair.MaterialIndex = asmesh.MaterialIndex;
                this.MeshMaterials.Add(meshMaterialPair);
            }

            this.Meshes.AddRange(meshes);
        }

        /// <summary>
        /// Recursively loads the transform of each node in the Assimp scene.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parentTransform"></param>
        private void LoadNodeTransformRecursive(Node node, Matrix4x4 parentTransform)
        {
            var currentTransform = parentTransform * node.Transform;

            foreach (var meshIndex in node.MeshIndices)
            {
                var mesh = this.Meshes.GetMesh(meshIndex);
                currentTransform.Decompose(out Assimp.Vector3D scale, out Assimp.Quaternion rotation, out Assimp.Vector3D translation);
                mesh.LocalTranslation = new Vector3(translation.X, translation.Y, translation.Z);
                mesh.LocalRotation = new OpenTK.Mathematics.Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
                mesh.LocalScale = new Vector3(scale.X, scale.Y, scale.Z);
            }

            foreach (var child in node.Children)
            {
                LoadNodeTransformRecursive(child, currentTransform);
            }
        }
    }
}
