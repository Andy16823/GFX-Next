using Assimp;
using Assimp.Configs;
using LibGFX.Core;
using LibGFX.Graphics.Animation3D;
using LibGFX.Graphics.Materials;
using LibGFX.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class StaticMeshModel : IModel
    {
        /// <summary>
        /// Gets or sets the name associated with the object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the unique identifier for this instance.
        /// </summary>
        public Guid ID { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Meshes that make up the static model.
        /// </summary>
        public List<Mesh> Meshes { get; set; }

        /// <summary>
        /// Node structure of the model as imported from Assimp.
        /// </summary>
        public SceneNodeData NodeStructure { get; set; }

        /// <summary>
        /// Gets a value indicating whether the object has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Gets or sets the full path to the file associated with this instance.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets a value indicating whether the content includes any transparent pixels.
        /// </summary>
        public bool HasTransparency => this.HasTransparencyCheck();

        /// <summary>
        /// Initializes a new instance of the StaticMeshModel class.
        /// Used for deserialization purposes.
        /// </summary>
        public StaticMeshModel()
        {

        }

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
        /// Determines whether any mesh in the collection uses a transparent material.
        /// </summary>
        /// <returns>true if at least one mesh has a transparent material; otherwise, false.</returns>
        private bool HasTransparencyCheck()
        {
            return Meshes.Any(mesh => mesh.Material.IsTransparent);
        }

        /// <summary>
        /// Orders the mesh transparency for rendering.
        /// </summary>
        private void OrderMeshTransparency()
        {
            this.Meshes.Sort((meshA, meshB) =>
            {
                bool isTransparentA = meshA.Material.IsTransparent;
                bool isTransparentB = meshB.Material.IsTransparent;
                if (isTransparentA && !isTransparentB)
                    return 1; // A is transparent, B is opaque -> A after B
                else if (!isTransparentA && isTransparentB)
                    return -1; // A is opaque, B is transparent -> A before B
                else
                    return 0; // Both are the same type -> maintain order
            });
        }

        /// <summary>
        /// Loads model data from a file using Assimp.
        /// </summary>
        /// <param name="file"></param>
        private void LoadFromFile(string file)
        {
            // Set the file path
            FilePath = file;

            // Get the directory of the file
            var directory = Path.GetDirectoryName(file);

            using (var importer = new AssimpContext())
            {
                // Create the Assimp importer and import the file
                importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));

                // Define post-processing steps
                PostProcessSteps steps = PostProcessSteps.Triangulate |
                                        PostProcessSteps.CalculateTangentSpace |
                                        PostProcessSteps.JoinIdenticalVertices;

                // Import the file
                var assimpScene = importer.ImportFile(file, steps);

                // Load the meshes from the Assimp scene
                Meshes = new List<Mesh>();
                foreach (var asmesh in assimpScene.Meshes)
                {
                    var mesh = new Mesh();
                    mesh.Name = asmesh.Name;
                    mesh.Material = new SGMaterial();
                    mesh.Material.LoadMaterial(assimpScene.Materials[asmesh.MaterialIndex], directory);

                    for (int i = 0; i < asmesh.VertexCount; i++)
                    {
                        mesh.Positions.Add(new Vector3(asmesh.Vertices[i].X, asmesh.Vertices[i].Y, asmesh.Vertices[i].Z));

                        var vertex = new Vertex();
                        vertex.Normal = new Vector3(asmesh.Normals[i].X, asmesh.Normals[i].Y, asmesh.Normals[i].Z);
                        vertex.TexCoord = new Vector2(asmesh.TextureCoordinateChannels[0][i].X, asmesh.TextureCoordinateChannels[0][i].Y);
                        vertex.Tangent = new Vector4(asmesh.Tangents[i].X, asmesh.Tangents[i].Y, asmesh.Tangents[i].Z, 1.0f);
                        vertex.BoneIDs = new Vector4i(-1);
                        vertex.BoneWeights = new Vector4(0.0f);
                        mesh.Vertices.Add(vertex);
                    }

                    mesh.Indices.AddRange(asmesh.GetIndices());
                    this.Meshes.Add(mesh);
                }

                // Load the transforms of the model
                LoadTransforms(assimpScene);
                NodeStructure = LoadNodeStructure(assimpScene.RootNode);
            }
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
                var mesh = Meshes[meshIndex];
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
        private SceneNodeData LoadNodeStructure(Node node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var nodeData = new SceneNodeData
            {
                name = node.Name,
                transformation = (Matrix4) Math.MathUtils.ToColumnMajorMatrix(node.Transform),
                children = new List<SceneNodeData>()
            };
            foreach (var child in node.Children)
            {
                var childData = LoadNodeStructure(child);
                nodeData.children.Add(childData);
            }
            return nodeData;
        }

        /// <summary>
        /// Initializes the model and loads all associated meshes and materials into the specified render device.
        /// </summary>
        /// <remarks>Call this method before attempting to render the model. After initialization, the
        /// model's meshes and materials are prepared for use with the provided render device. This method has no effect
        /// if called multiple times on an already initialized model.</remarks>
        /// <param name="renderer">The render device used to initialize materials and load meshes. Cannot be null.</param>
        public void Init(IRenderDevice renderer)
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException("Model is already initialized.");
            }

            // Order meshes by transparency
            this.OrderMeshTransparency();

            Debug.WriteLine("Importing Static Model with " + Meshes.Count + " meshes.");
            foreach (var mesh in Meshes)
            {
                mesh.Material.Init(renderer);
                mesh.Init(renderer);
            }
            IsInitialized = true;
            Debug.WriteLine("Static Model import complete.");
        }

        /// <summary>
        /// Frees the CPU resources used by the static model and its associated meshes.
        /// </summary>
        public void FreeCPUResources()
        {
            this.Meshes.ForEach(m =>
            {
                m.FreeCPUResources();
                m.Material?.FreeCPUResources();
            });
        }

        /// <summary>
        /// Releases all resources used by the static model and its associated meshes using the specified render device.
        /// </summary>
        /// <remarks>After calling this method, the static model and its meshes should not be used. This
        /// method must be called to free graphics resources when the model is no longer needed.</remarks>
        /// <param name="renderer">The render device used to dispose of the model's meshes and materials. Cannot be null.</param>
        public void Dispose(IRenderDevice renderer)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Model is not initialized.");
            }

            Debug.WriteLine("Disposing Static Model with " + Meshes.Count + " meshes.");
            foreach (var mesh in Meshes)
            {
                mesh.Dispose(renderer);
                mesh.Material.Dispose(renderer);
            }
            IsInitialized = false;
            Debug.WriteLine("Static Model disposal complete.");
        }

        /// <summary>
        /// Searches for a node with the specified name in the scene graph.
        /// </summary>
        /// <param name="name">The name of the node to locate. The search is case-sensitive and cannot be null.</param>
        /// <param name="node">When this method returns, contains the data for the found node if a node with the specified name exists;
        /// otherwise, contains the default value.</param>
        /// <returns>true if a node with the specified name is found; otherwise, false.</returns>
        public bool FindNodeByName(string name, out SceneNodeData node)
        {
            return Utils.FindNodeByNameRecursive(NodeStructure, name, out node);
        }

        /// <summary>
        /// Serializes the current static mesh model to a JSON object using the specified serialization context.
        /// </summary>
        /// <param name="serializationContext">The context that provides information and settings required for serialization.</param>
        /// <returns>A <see cref="JObject"/> representing the serialized static mesh model, including its type, name, ID, and
        /// file path.</returns>
        public void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(this.GetType().FullName);
            writer.WritePropertyName("Name");
            writer.WriteValue(!String.IsNullOrEmpty(Name) ? Name : ID.ToString());
            writer.WritePropertyName("ID");
            writer.WriteValue(ID.ToString());
            writer.WritePropertyName("FilePath");
            writer.WriteValue(FilePath);
            callback?.Invoke(writer);
            writer.WriteEndObject();
        }

        public void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            // Ensure the model is not already initialized
            if (this.IsInitialized)
            {
                throw new InvalidOperationException("Cannot deserialize an initialized model.");
            }

            // Load basic properties
            this.Name = obj.Value<string>("Name");
            this.ID = Guid.Parse(obj.Value<string>("ID"));
            this.FilePath = obj.Value<string>("FilePath");

            // Load from file
            this.LoadFromFile(this.FilePath);

            // Invoke the callback if provided
            callback?.Invoke(obj);

            // Register the deserialized model in the serialization context
            serializationContext.SetValue<StaticMeshModel>(this.ID.ToString(), this);
        }
    }
}
