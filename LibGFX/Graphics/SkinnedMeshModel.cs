using Assimp;
using Assimp.Configs;
using LibGFX.Core;
using LibGFX.Graphics.Animation3D;
using LibGFX.Graphics.Materials;
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
    /// Skinned mesh model with support for skeletal animation
    /// </summary>
    public class SkinnedMeshModel : IModel
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
        /// The meshes that make up the skinned mesh model.
        /// </summary>
        public Dictionary<string, Mesh> Meshes { get; set; }

        /// <summary>
        /// The node structure of the model as imported from Assimp.
        /// </summary>
        public SceneNodeData NodeStructure { get; set; }

        /// <summary>
        /// The skeleton associated with the skinned mesh model.
        /// </summary>
        public Skeleton Skeleton { get; set; }

        /// <summary>
        /// Gets or sets the collection of 3D animations associated with this object.
        /// </summary>
        public List<Animation3D.Animation3D> Animations { get; set; }

        /// <summary>
        /// Gets a value indicating whether the object has been successfully initialized.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Initializes a new instance of the SkinnedMeshModel class.
        /// Used for deserialization purposes.
        /// </summary>
        public SkinnedMeshModel()
        {

        }

        /// <summary>
        /// Loads a skinned mesh model from the specified file.
        /// </summary>
        /// <param name="file"></param>
        public SkinnedMeshModel(String file)
        {
            this.Skeleton = new Skeleton();
            LoadModel(file);
        }

        /// <summary>
        /// Loads the model from the specified file.
        /// </summary>
        /// <param name="file"></param>
        /// <exception cref="Exception"></exception>
        private void LoadModel(String file)
        {
            var directory = Path.GetDirectoryName(file);

            var importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            var assimpScene = importer.ImportFile(file,
                Assimp.PostProcessSteps.Triangulate |
                Assimp.PostProcessSteps.CalculateTangentSpace |
                Assimp.PostProcessSteps.JoinIdenticalVertices
                );

            if (!assimpScene.HasAnimations)
            {
                throw new Exception("The model does not contain any animations.");
            }

            // Extract materials and meshes
            this.Meshes = new Dictionary<string, Mesh>();
            foreach (var asmesh in assimpScene.Meshes)
            {
                var mesh = new Graphics.Mesh();
                mesh.Name = asmesh.Name;
                mesh.Material = new SGMaterial();
                mesh.Material.LoadMaterial(assimpScene.Materials[asmesh.MaterialIndex], directory);

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
                ExtractBoneWeightForVertices(asmesh, assimpScene, mesh);
                this.Meshes.Add(Guid.NewGuid().ToString(), mesh);
            }

            this.ExtractAnimations(assimpScene);
            LoadTransforms(assimpScene);
            this.NodeStructure = LoadNodeStructure(assimpScene.RootNode);
        }

        /// <summary>
        /// Loads the node structure recursively from the Assimp node.
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
        /// Load the transforms of the model from the Assimp scene.
        /// </summary>
        /// <param name="assimpScene"></param>
        private void LoadTransforms(Scene assimpScene)
        {
            this.LoadNodeTransformRecursive(assimpScene.RootNode, System.Numerics.Matrix4x4.Identity);
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
                var mesh = this.Meshes.Values.ElementAt(meshIndex);
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
        /// Extracts animations from the Assimp scene.
        /// </summary>
        /// <param name="scene"></param>
        private void ExtractAnimations(Scene scene)
        {
            Animations = new List<Graphics.Animation3D.Animation3D>();
            for (int i = 0; i < scene.AnimationCount; i++)
            {
                var animation = new Graphics.Animation3D.Animation3D(scene, i, this.Skeleton);
                this.Animations.Add(animation);
            }
        }

        /// <summary>
        /// Extracts bone weights for vertices from the Assimp mesh.
        /// </summary>
        /// <param name="asmesh"></param>
        /// <param name="scene"></param>
        /// <param name="mesh"></param>
        private void ExtractBoneWeightForVertices(Assimp.Mesh asmesh, Assimp.Scene scene, Graphics.Mesh mesh)
        {
            Debug.WriteLine("Extracting bone weights wit bone count: " + asmesh.BoneCount);
            for (int boneIndex = 0; boneIndex < asmesh.BoneCount; boneIndex++)
            {
                int boneId = -1;
                var boneName = asmesh.Bones[boneIndex].Name;
                if (!Skeleton.BoneInfoMap.ContainsKey(boneName))
                {
                    var boneInfo = new BoneInfo();
                    boneInfo.id = Skeleton.BoneCounter;
                    boneInfo.offset = (Matrix4) Math.MathUtils.ToColumnMajorMatrix(asmesh.Bones[boneIndex].OffsetMatrix);
                    Skeleton.BoneInfoMap.Add(boneName, boneInfo);
                    boneId = Skeleton.BoneCounter;
                    Skeleton.BoneCounter++;
                }
                else
                {
                    boneId = Skeleton.BoneInfoMap[boneName].id;
                }

                var weights = asmesh.Bones[boneIndex].VertexWeights;
                var numWeights = asmesh.Bones[boneIndex].VertexWeightCount;
                for (int weigthIndex = 0; weigthIndex < numWeights; weigthIndex++)
                {
                    int vertexId = weights[weigthIndex].VertexID;
                    float weight = weights[weigthIndex].Weight;
                    Debug.Assert(vertexId <= mesh.Indices.Count);
                    var vertex = mesh.Vertices[vertexId];
                    SetVertexBoneData(ref vertex, boneId, weight);
                    mesh.Vertices[vertexId] = vertex;
                }
            }
        }

        /// <summary>
        /// Sets the bone data for a vertex.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="boneId"></param>
        /// <param name="weight"></param>
        private void SetVertexBoneData(ref Vertex v, int boneId, float weight)
        {
            for (int i = 0; i < 4; ++i)
            {
                if (v.BoneIDs[i] < 0)
                {
                    v.BoneWeights[i] = weight;
                    v.BoneIDs[i] = boneId;
                    break;
                }
            }
        }

        /// <summary>
        /// Initializes the skinned mesh model with the specified render device.
        /// </summary>
        /// <param name="renderer"></param>
        public void Init(IRenderDevice renderer)
        {
            if(IsInitialized)
            {
                throw new InvalidOperationException("Model is already initialized.");
            }

            foreach (var mesh in Meshes.Values)
            {
                mesh.Material.Init(renderer);
                mesh.Init(renderer);
            }
            IsInitialized = true;
        }

        /// <summary>
        /// Disposes the skinned mesh model with the specified render device.
        /// </summary>
        /// <param name="renderer"></param>
        public void Dispose(IRenderDevice renderer)
        {
            if(!IsInitialized)
            {
                throw new InvalidOperationException("Model is not initialized.");
            }

            foreach (var mesh in Meshes.Values)
            {
                mesh.Dispose(renderer);
                mesh.Material.Dispose(renderer);
            }

            IsInitialized = false;
        }

        /// <summary>
        /// Imports an animation from the specified file and adds it to the model's animations.
        /// </summary>
        /// <param name="file"></param>
        public void ImportAnimation(String file)
        {
            var importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            var assimpScene = importer.ImportFile(file, Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.CalculateTangentSpace | Assimp.PostProcessSteps.JoinIdenticalVertices);
            for(int i = 0; i < assimpScene.AnimationCount; i++)
            {
                var animation = new Graphics.Animation3D.Animation3D(assimpScene, i, Skeleton);
                this.Animations.Add(animation);
            }
        }

        /// <summary>
        /// Creates a synthetic root bone to ensure a proper bind pose for the skeleton.
        /// </summary>
        /// <param name="syntheticRootName"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool CreateBindPose(string syntheticRootName = "Armature", bool force = false)
        {
            if (Skeleton == null) throw new InvalidOperationException("Skeleton is null.");
            if (Animations == null) Animations = new List<Graphics.Animation3D.Animation3D>();

            // Check if synthetic root already exists
            if (Skeleton.BoneInfoMap.ContainsKey(syntheticRootName) && !force)
            {
                return false;
            }

            // Add synthetic root to BoneInfoMap if not already present
            if (!Skeleton.BoneInfoMap.ContainsKey(syntheticRootName))
            {
                var rootInfo = new BoneInfo();
                rootInfo.id = Skeleton.BoneCounter;
                rootInfo.offset = Matrix4.Identity;
                Skeleton.BoneInfoMap.Add(syntheticRootName, rootInfo);
                Skeleton.BoneCounter++;
                Debug.WriteLine($"[CreateBindPose] Added synthetic root bone '{syntheticRootName}' with id {rootInfo.id}");
            }

            // Wrap existing NodeStructure under new root
            var oldNodeStructure = this.NodeStructure;
            var newRootNode = new Graphics.Animation3D.SceneNodeData
            {
                name = syntheticRootName,
                transformation = Matrix4.Identity,
                children = new List<Graphics.Animation3D.SceneNodeData>()
            };

            // Attach old root as child of new root (if not empty)
            if (oldNodeStructure.children != null || !string.IsNullOrEmpty(oldNodeStructure.name))
            {
                // Preserve previous node as child of new root
                newRootNode.children.Add(oldNodeStructure);
                newRootNode.childrenCount = 1;
            }
            else
            {
                newRootNode.childrenCount = 0;
            }

            // Set new NodeStructure
            this.NodeStructure = newRootNode;

            // Recreate animations to reference new root
            foreach (var anim in this.Animations)
            {
                if (anim == null) continue;

                var prevRoot = anim.RootNode;

                // Skip if already has synthetic root and not forcing
                if (!force && prevRoot.name == syntheticRootName) continue;

                var animNewRoot = new Graphics.Animation3D.SceneNodeData
                {
                    name = syntheticRootName,
                    transformation = Matrix4.Identity,
                    children = new List<Graphics.Animation3D.SceneNodeData>()
                };

                // Attach previous root as child of new root (if not empty)
                if (prevRoot.children != null || !string.IsNullOrEmpty(prevRoot.name))
                {
                    animNewRoot.children.Add(prevRoot);
                    animNewRoot.childrenCount = 1;
                }
                else
                {
                    animNewRoot.childrenCount = 0;
                }

                anim.RootNode = animNewRoot;

                // Update BoneInfoMap reference
                anim.BoneInfoMap = this.Skeleton.BoneInfoMap;
            }

            return true;
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
        /// Serializes the current object to a JSON representation using the specified serialization context.
        /// </summary>
        /// <param name="serializationContext">The context that provides configuration and state information for the serialization process.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized JSON representation of the object.</returns>
        /// <exception cref="NotImplementedException">Thrown in all cases, as this method is not yet implemented.</exception>
        public JObject Serialize(SerializationContext serializationContext)
        {
            // Serialize meshes
            var meshesArray = new JArray();
            foreach(var mesh in Meshes)
            {
                var meshObj = new JObject()
                {
                    ["Key"] = mesh.Key,
                    ["Mesh"] = mesh.Value.Serialize(serializationContext),
                    ["Material"] = mesh.Value.Material.Serialize(serializationContext)
                };
                meshesArray.Add(meshObj);
            }

            // Serialize animations
            var animationsArray = new JArray();
            foreach(var animation in Animations)
            {
                animationsArray.Add(animation.Serialize(serializationContext));
            }

            // Create main JObject
            return new JObject()
            {
                ["Type"] = this.GetType().FullName,
                ["ID"] = ID.ToString(),
                ["Name"] = !String.IsNullOrEmpty(Name) ? Name : ID.ToString(),
                ["Meshes"] = meshesArray,
                ["NodeStructure"] = Utils.SerializeSceneNodeData(this.NodeStructure),
                ["Skeleton"] = Skeleton.Serialize(serializationContext),
                ["Animations"] = animationsArray
            };
        }

        /// <summary>
        /// Deserializes the specified JSON object into the corresponding object representation using the provided
        /// serialization context.
        /// </summary>
        /// <param name="jObject">The JSON object to deserialize. Cannot be null.</param>
        /// <param name="serializationContext">The context that provides information and services required for the deserialization process. Cannot be null.</param>
        /// <exception cref="NotImplementedException">Thrown in all cases as this method is not yet implemented.</exception>
        public void Deserialize(JObject jObject, SerializationContext serializationContext)
        {
            if(this.IsInitialized)
            {
                throw new InvalidOperationException("Cannot deserialize an initialized model. Please dispose the model before deserialization.");
            }

            this.ID = Guid.Parse(jObject["ID"].ToString());
            this.Name = jObject["Name"].ToString();

            // Deserialize meshes
            this.Meshes = new Dictionary<string, Mesh>();
            var meshesArray = (JArray)jObject["Meshes"];
            foreach(var meshToken in meshesArray)
            {
                // Deserialize each mesh
                var meshObj = meshToken as JObject;
                var key = meshObj["Key"].ToString();

                // Deserialize Material
                var material = new SGMaterial();
                material.Deserialize((JObject)meshObj["Material"], serializationContext);
                serializationContext.SetValue(material.ID.ToString(), material);

                // Deserialize Mesh
                var mesh = new Mesh();
                mesh.Deserialize((JObject)meshObj["Mesh"], serializationContext);

                // Assign material to mesh and add to dictionary
                mesh.Material = material;
                this.Meshes.Add(key, mesh);
            }

            // Deserialize NodeStructure
            this.NodeStructure = Utils.DeserializeSceneNodeData(jObject["NodeStructure"] as JObject);

            // Deserialize Skeleton
            this.Skeleton = new Skeleton();
            this.Skeleton.Deserialize((JObject)jObject["Skeleton"], serializationContext);

            // Deserialize Animations
            this.Animations = new List<Graphics.Animation3D.Animation3D>();
            var animationsArray = (JArray)jObject["Animations"];
            foreach(var animationToken in animationsArray)
            {
                var animation = new Graphics.Animation3D.Animation3D();
                animation.Deserialize((JObject)animationToken, serializationContext);
                this.Animations.Add(animation);
            }
        }
    }
}
