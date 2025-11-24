using Assimp;
using Assimp.Configs;
using LibGFX.Core;
using LibGFX.Graphics.Animation3D;
using LibGFX.Graphics.Materials;
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
        public Dictionary<string, Mesh> Meshes { get; set; }
        public AssimpNodeData NodeStructure { get; set; }
        public Skeleton Skeleton { get; set; }
        public List<Animation3D.Animation> Animations { get; set; }

        public SkinnedMeshModel(String file)
        {
            this.Skeleton = new Skeleton();
            LoadModel(file);
        }


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
                mesh.Material = Utils.LoadMaterial(assimpScene.Materials[asmesh.MaterialIndex], directory);

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

        private AssimpNodeData LoadNodeStructure(Node node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var nodeData = new AssimpNodeData
            {
                name = node.Name,
                transformation = (Matrix4) Math.Math.ToColumnMajorMatrix(node.Transform),
                children = new List<AssimpNodeData>()
            };
            foreach (var child in node.Children)
            {
                var childData = LoadNodeStructure(child);
                nodeData.children.Add(childData);
            }
            return nodeData;
        }

        private void LoadTransforms(Scene assimpScene)
        {
            this.LoadNodeTransformRecursive(assimpScene.RootNode, System.Numerics.Matrix4x4.Identity);
        }

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

        private void ExtractAnimations(Scene scene)
        {
            Animations = new List<Graphics.Animation3D.Animation>();
            for (int i = 0; i < scene.AnimationCount; i++)
            {
                var animation = new Graphics.Animation3D.Animation(scene, i, this.Skeleton);
                this.Animations.Add(animation);
            }
        }

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
                    boneInfo.offset = (Matrix4) Math.Math.ToColumnMajorMatrix(asmesh.Bones[boneIndex].OffsetMatrix);
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

        public void Init(IRenderDevice renderer)
        {
            foreach (var mesh in Meshes.Values)
            {
                mesh.Material.Init(renderer);
                renderer.LoadMesh(mesh);
            }
        }

        public void Dispose(IRenderDevice renderer)
        {
            foreach (var mesh in Meshes.Values)
            {
                renderer.DisposeMesh(mesh);
                mesh.Material.Dispose(renderer);
            }
        }

        public void ImportAnimation(String file)
        {
            var importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            var assimpScene = importer.ImportFile(file, Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.CalculateTangentSpace | Assimp.PostProcessSteps.JoinIdenticalVertices);
            for(int i = 0; i < assimpScene.AnimationCount; i++)
            {
                var animation = new Graphics.Animation3D.Animation(assimpScene, i, Skeleton);
                this.Animations.Add(animation);
            }
        }

        public bool CreateBindPose(string syntheticRootName = "Armature", bool force = false)
        {
            if (Skeleton == null) throw new InvalidOperationException("Skeleton ist null.");
            if (Animations == null) Animations = new List<Graphics.Animation3D.Animation>();

            // Wenn bereits ein BoneInfo-Eintrag mit dem Namen existiert und nicht forced, nichts tun
            if (Skeleton.BoneInfoMap.ContainsKey(syntheticRootName) && !force)
            {
                return false;
            }

            // Füge root BoneInfo hinzu (Identity offset), falls nicht vorhanden oder forced
            if (!Skeleton.BoneInfoMap.ContainsKey(syntheticRootName))
            {
                var rootInfo = new BoneInfo();
                rootInfo.id = Skeleton.BoneCounter;
                rootInfo.offset = Matrix4.Identity;
                Skeleton.BoneInfoMap.Add(syntheticRootName, rootInfo);
                Skeleton.BoneCounter++;
                Debug.WriteLine($"[CreateBindPose] Added synthetic root bone '{syntheticRootName}' with id {rootInfo.id}");
            }

            // Wrap NodeStructure (falls vorhanden) unter einen neuen AssimpNodeData root
            var oldNodeStructure = this.NodeStructure;
            var newRootNode = new Graphics.Animation3D.AssimpNodeData
            {
                name = syntheticRootName,
                transformation = Matrix4.Identity,
                children = new List<Graphics.Animation3D.AssimpNodeData>()
            };

            // Falls es bereits eine NodeStructure gibt, hänge sie an
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

            // Setze die neue NodeStructure
            this.NodeStructure = newRootNode;

            // Wrap Animation RootNodes unter dem neuen Root (sofern Animationen vorhanden)
            foreach (var anim in this.Animations)
            {
                if (anim == null) continue;

                var prevRoot = anim.RootNode;

                // Wenn prevRoot schon der gewünschte Name ist und force==false, skip
                if (!force && prevRoot.name == syntheticRootName) continue;

                var animNewRoot = new Graphics.Animation3D.AssimpNodeData
                {
                    name = syntheticRootName,
                    transformation = Matrix4.Identity,
                    children = new List<Graphics.Animation3D.AssimpNodeData>()
                };

                // Falls prevRoot ist leer (z. B. nicht gesetzt), dann leave children empty
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

                // Stelle sicher, dass Animation auf dieselbe BoneInfoMap verweist wie das Skeleton
                anim.BoneInfoMap = this.Skeleton.BoneInfoMap;
            }

            return true;
        }
    }
}
