using Assimp.Configs;
using Assimp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGFX.Core.GameElements;
using OpenTK.Mathematics;
using LibGFX.Graphics;

namespace LibGFX.Assets.Loaders
{
    public class MeshLoader : IAssetLoader
    {
        /// <summary>
        /// Indicates whether the asset should be cached.
        /// </summary>
        public bool ShouldCache => true;

        /// <summary>
        /// Indicates whether the asset loader can create new assets.
        /// </summary>
        public bool CanCreate => true;

        /// <summary>
        /// Loads a mesh from the specified path using Assimp.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            var directory = Path.GetDirectoryName(path);

            // Load the model using Assimp
            var importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            var assimpScene = importer.ImportFile(path, Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.CalculateTangentSpace | Assimp.PostProcessSteps.JoinIdenticalVertices);

            var meshes = ExtractMeshes(assimpScene);
            LoadTransforms(assimpScene, meshes);

            return meshes as T;
        }

        /// <summary>
        /// Extracts meshes from the Assimp scene and converts them to the LibGFX MeshCollection format.
        /// </summary>
        /// <param name="assimpScene"></param>
        /// <returns></returns>
        private MeshCollection ExtractMeshes(Scene assimpScene)
        {
            var meshes = new MeshCollection();

            foreach (var asmesh in assimpScene.Meshes)
            {
                var mesh = new Graphics.Mesh();
                mesh.Name = asmesh.Name;

                for (int i = 0; i < asmesh.VertexCount; i++)
                {
                    var vertex = new Graphics.Vertex();

                    vertex.Position = new Vector3(asmesh.Vertices[i].X, asmesh.Vertices[i].Y, asmesh.Vertices[i].Z);
                    vertex.Normal = new Vector3(asmesh.Normals[i].X, asmesh.Normals[i].Y, asmesh.Normals[i].Z);
                    vertex.TexCoord = new Vector2(asmesh.TextureCoordinateChannels[0][i].X, asmesh.TextureCoordinateChannels[0][i].Y);
                    vertex.Tangent = new Vector4(asmesh.Tangents[i].X, asmesh.Tangents[i].Y, asmesh.Tangents[i].Z, 1.0f);
                    vertex.BoneIDs = new Vector4i(-1);
                    vertex.BoneWeights = new Vector4(0.0f);
                    mesh.Vertices.Add(vertex);
                }

                mesh.Indices.AddRange(asmesh.GetIndices());
                meshes.Add(mesh);
            }

            return meshes;
        }

        /// <summary>
        /// Loads the transforms for each mesh in the Assimp scene.
        /// </summary>
        /// <param name="assimpScene"></param>
        private void LoadTransforms(Scene assimpScene, MeshCollection meshes)
        {
            this.LoadNodeTransformRecursive(assimpScene.RootNode, Matrix4x4.Identity, meshes);
        }

        /// <summary>
        /// Loads the transform for each node recursively.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parentTransform"></param>
        private void LoadNodeTransformRecursive(Node node, Matrix4x4 parentTransform, MeshCollection meshes)
        {
            var currentTransform = parentTransform * node.Transform;

            foreach (var meshIndex in node.MeshIndices)
            {
                var mesh = meshes.GetMesh(meshIndex);
                currentTransform.Decompose(out Assimp.Vector3D scale, out Assimp.Quaternion rotation, out Assimp.Vector3D translation);
                mesh.LocalTranslation = new Vector3(translation.X, translation.Y, translation.Z);
                mesh.LocalRotation = new OpenTK.Mathematics.Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
                mesh.LocalScale = new Vector3(scale.X, scale.Y, scale.Z);
            }

            foreach (var child in node.Children)
            {
                LoadNodeTransformRecursive(child, currentTransform, meshes);
            }
        }

        /// <summary>
        /// Creates a new mesh or mesh collection with the specified ID and optional initializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="initializer"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public T Create<T>(string id, Action<T>? initializer = null, object? creationArgs = null) where T : class
        {
            if(typeof(T) == typeof(MeshCollection))
            {
                var meshCollection = new MeshCollection();
                initializer?.Invoke(meshCollection as T);
                return meshCollection as T;
            }
            throw new NotSupportedException($"Asset type '{typeof(T)}' is not supported.");
        }
    }
}
