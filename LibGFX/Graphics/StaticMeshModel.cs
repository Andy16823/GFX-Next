using Assimp;
using Assimp.Configs;
using LibGFX.Core;
using LibGFX.Graphics.Animation3D;
using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Static model class that represents a 3D model loaded from a file.
    /// </summary>
    public class StaticMeshModel : IModel {

        /// <summary>
        /// Meshes that make up the static model.
        /// </summary>
        public Dictionary<string, Mesh> Meshes { get; set; }

        /// <summary>
        /// Node structure of the model as imported from Assimp.
        /// </summary>
        public AssimpNodeData NodeStructure { get; set; }

        /// <summary>
        /// Static model constructor that loads model data from a file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="file"></param>
        public StaticMeshModel(string file)
        {
            LoadFromFile(file);
        }

        /// <summary>
        /// Loads model data from a file using Assimp.
        /// </summary>
        /// <param name="file"></param>
        private void LoadFromFile(string file)
        {
            // Get the directory of the file
            var directory = Path.GetDirectoryName(file);

            // Create the Assimp importer and import the file
            var importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            var assimpScene = importer.ImportFile(file, 
                PostProcessSteps.Triangulate | 
                PostProcessSteps.CalculateTangentSpace | 
                PostProcessSteps.JoinIdenticalVertices
                );

            // Load the meshes from the Assimp scene
            Meshes = new Dictionary<string, Mesh>();
            foreach (var asmesh in assimpScene.Meshes)
            {
                var mesh = new Mesh();
                mesh.Name = asmesh.Name;
                mesh.Material = Utils.LoadMaterial(assimpScene.Materials[asmesh.MaterialIndex], directory);

                for (int i = 0; i < asmesh.VertexCount; i++)
                {
                    var vertex = new Vertex();

                    vertex.Position = new Vector3(asmesh.Vertices[i].X, asmesh.Vertices[i].Y, asmesh.Vertices[i].Z);
                    vertex.Normal = new Vector3(asmesh.Normals[i].X, asmesh.Normals[i].Y, asmesh.Normals[i].Z);
                    vertex.TexCoord = new Vector2(asmesh.TextureCoordinateChannels[0][i].X, asmesh.TextureCoordinateChannels[0][i].Y);
                    vertex.Tangent = new Vector4(asmesh.Tangents[i].X, asmesh.Tangents[i].Y, asmesh.Tangents[i].Z, 1.0f);
                    vertex.BoneIDs = new Vector4i(-1);
                    vertex.BoneWeights = new Vector4(0.0f);
                    mesh.Vertices.Add(vertex);
                }

                mesh.Indices.AddRange(asmesh.GetIndices());
                this.Meshes.Add(Guid.NewGuid().ToString(), mesh);
            }

            // Load the transforms of the model
            LoadTransforms(assimpScene);
            NodeStructure = LoadNodeStructure(assimpScene.RootNode);
        }

        /// <summary>
        /// Loads the transforms of the model from the Assimp scene.
        /// </summary>
        /// <param name="assimpScene"></param>
        private void LoadTransforms(Scene assimpScene)
        {
            LoadNodeTransformRecursive(assimpScene.RootNode, System.Numerics.Matrix4x4.Identity);
        }

        /// <summary>
        /// Loads the node transforms recursively.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parentTransform"></param>
        private void LoadNodeTransformRecursive(Node node, System.Numerics.Matrix4x4 parentTransform)
        {
            var currentTransform = parentTransform * node.Transform;

            foreach (var meshIndex in node.MeshIndices)
            {
                var mesh = Meshes.Values.ElementAt(meshIndex);
                System.Numerics.Matrix4x4.Decompose(currentTransform, out System.Numerics.Vector3 scale, out System.Numerics.Quaternion rotation, out System.Numerics.Vector3 translation);
                mesh.LocalTranslation = new Vector3(translation.X, translation.Y, translation.Z);
                mesh.LocalRotation = new OpenTK.Mathematics.Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
                mesh.LocalScale = new Vector3(scale.X, scale.Y, scale.Z);
            }

            foreach (var child in node.Children)
            {
                LoadNodeTransformRecursive(child, currentTransform);
            }
        }

        /// <summary>
        /// Loads the node structure from the Assimp node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private AssimpNodeData LoadNodeStructure(Node node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var nodeData = new AssimpNodeData
            {
                name = node.Name,
                transformation = (Matrix4) Math.MathUtils.ToColumnMajorMatrix(node.Transform),
                children = new List<AssimpNodeData>()
            };
            foreach (var child in node.Children)
            {
                var childData = LoadNodeStructure(child);
                nodeData.children.Add(childData);
            }
            return nodeData;
        }

        public void Init(IRenderDevice renderer)
        {
            Debug.WriteLine("Importing Static Model with " + Meshes.Count + " meshes.");
            foreach (var mesh in Meshes.Values)
            {
                mesh.Material.Init(renderer);
                renderer.LoadMesh(mesh);
            }
            Debug.WriteLine("Static Model import complete.");
        }

        public void Dispose(IRenderDevice renderer)
        {
            Debug.WriteLine("Disposing Static Model with " + Meshes.Count + " meshes.");
            foreach (var mesh in Meshes.Values)
            {
                renderer.DisposeMesh(mesh);
                mesh.Material.Dispose(renderer);
            }
            Debug.WriteLine("Static Model disposal complete.");
        }
    }
}
