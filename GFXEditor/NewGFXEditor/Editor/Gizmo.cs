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
    public class Gizmo
    {
        public MeshCollection Meshes { get; set; }
        public MaterialCollection Materials { get; set; }
        public List<MeshMaterialPair> MeshMaterials { get; set; }
        public Transform Transform { get; set; }
        public ShaderProgram Shader { get; set; } = new GizmoShader();


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

        public void Init(IRenderDevice renderDevice)
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
        }
        
        public void RenderGizmo(IRenderDevice renderDevice, LibGFX.Graphics.Camera camera, Viewport viewport)
        {
            renderDevice.SetViewMatrix(camera.GetViewMatrix());
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
