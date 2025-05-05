using Assimp;
using Assimp.Configs;
using LibGFX.Assets;
using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Shader;
using LibGFX.Math;
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
    public enum GizmoActiveAxis
    {
        None,
        X,
        Y,
        Z
    }

    public class Gizmo
    {
        public delegate void GizmoMoveDelegate(Vector3 newPosition);

        public MeshCollection Meshes { get; set; }
        public MaterialCollection Materials { get; set; }
        public List<MeshMaterialPair> MeshMaterials { get; set; }
        public Transform Transform { get; set; }
        public ShaderProgram Shader { get; set; } = new GizmoShader();
        public ColorIDPicker Picker { get; set; } = new ColorIDPicker();
        public GizmoActiveAxis ActiveAxis { get; set; } = GizmoActiveAxis.None;
        public bool Enabled { get; set; } = false;

        public event GizmoMoveDelegate GizmoMoved;

        public Gizmo(String file)
        {
            Transform = new Transform();
            Transform.Position = new Vector3(0, 0, 0);
            Transform.Scale = new Vector3(0.5f, 0.5f, 0.5f);

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
        
        public void RenderGizmo(IRenderDevice renderDevice, LibGFX.Graphics.Camera camera, Viewport viewport)
        {
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

        public void ReleaseGizmo()
        {
            this.ActiveAxis = GizmoActiveAxis.None;
        }

        public void MoveAlongAxis2D(PerspectiveCamera camera, Viewport viewport, int prevMouseX, int prevMouseY, int currMouseX, int currMouseY)
        {
            if (this.ActiveAxis == GizmoActiveAxis.None || this.Enabled == false)
                return;

            Vector3 axisWorld = GetAxisDirection(this.ActiveAxis);
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

        private Vector3 GetAxisDirection(GizmoActiveAxis axis)
        {
            switch (axis)
            {
                case GizmoActiveAxis.X:
                    return new Vector3(1, 0, 0);
                case GizmoActiveAxis.Y:
                    return new Vector3(0, 1, 0);
                case GizmoActiveAxis.Z:
                    return new Vector3(0, 0, 1);
                default:
                    return Vector3.Zero;
            }
        }

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
