using Assimp;
using Assimp.Configs;
using LibGFX.Graphics;
using LibGFX.Graphics.Animation3D;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Shader;
using LibGFX.Math;
using OpenTK.Compute.OpenCL;
using OpenTK.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Light = LibGFX.Graphics.Lights.Light;

namespace LibGFX.Core.GameElements
{
    public struct MeshMaterialPair
    {
        public String MeshName;
        public int MaterialIndex;
    }


    /// <summary>
    /// Represents a 3D model
    /// </summary>
    public class Model : GameElement
    {

        /// <summary>
        /// The meshes of the model
        /// </summary>
        public MeshCollection Meshes { get; set; }

        /// <summary>
        /// The materials of the model
        /// </summary>
        public MaterialCollection Materials { get; set; }

        /// <summary>
        /// The pair of meshes and materials
        /// </summary>
        public List<MeshMaterialPair> MeshMaterials { get; set; }

        /// <summary>
        /// The name of the model
        /// </summary>
        public ShaderProgram Shader { get; set; }

        /// <summary>
        /// The animations of the model
        /// </summary>
        public List<Graphics.Animation3D.Animation> Animations { get; set; }

        /// <summary>
        /// The animator of the model
        /// </summary>
        public Animator Animator { get; set; }

        /// <summary>
        /// Checks if the model has animations
        /// </summary>
        public bool HasAnimations { get; set; }

        /// <summary>
        /// The speed of the animation
        /// </summary>
        public float AnimationSpeed { get; set; } = 1.0f;

        /// <summary>
        /// The Skeleton of the model
        /// </summary>
        public Skeleton Skeleton { get; set; }


        /// <summary>
        /// Creates a new model
        /// </summary>
        /// <param name="name"></param>
        /// <param name="file"></param>
        public Model(String name, String file)
        {
            this.Name = name;
            this.Skeleton = new Skeleton();
            this.Meshes = new MeshCollection();
            this.Materials = new MaterialCollection();
            this.MeshMaterials = new List<MeshMaterialPair>();

            this.LoadModel(file);
            this.ComputeAABB();
        }

        /// <summary>
        /// Overrides the mesh scale of the model
        /// </summary>
        /// <param name="value"></param>
        public void OverrideMeshScale(Vector3 value)
        {
            this.Meshes.ForEach(m => m.LocalScale = value);
        }

        /// <summary>
        /// Overrides the mesh scale of the model
        /// </summary>
        /// <param name="value"></param>
        public void OverrideMeshScale(float value)
        {
            this.Meshes.ForEach(m => m.LocalScale = new Vector3(value));
        }

        /// <summary>
        /// Overrides the scale of a single mesh
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void OverrideSingleMeshScale(int index, Vector3 value)
        {
            this.Meshes.SingleMeshAction(index, m => m.LocalScale = value);
        }

        /// <summary>
        /// Overrides the scale of a single mesh
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void OverrideSingleMeshScale(String name, Vector3 value)
        {
            this.Meshes.SingleMeshAction(name, m => m.LocalScale = value);
        }

        /// <summary>
        /// Loads the model from the specified file
        /// </summary>
        /// <param name="file"></param>
        private void LoadModel(String file)
        {
            // Get the directory of the file
            var directory = Path.GetDirectoryName(file);

            // Load the model using Assimp
            var importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            var assimpScene = importer.ImportFile(file, Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.CalculateTangentSpace | Assimp.PostProcessSteps.JoinIdenticalVertices);

            // Extract materials and meshes
            ExtractMaterials(assimpScene, directory);
            ExtractMeshes(assimpScene);

            // Extract animations
            this.HasAnimations = assimpScene.HasAnimations;
            if (this.HasAnimations)
            {
                this.ExtractAnimations(assimpScene);
            }
            else
            {
                this.Animator = new Animator();
            }

            // Load the transforms of the model
            LoadTransforms(assimpScene);
        }

        /// <summary>
        /// Loads the transforms of the model
        /// </summary>
        /// <param name="assimpScene"></param>
        private void LoadTransforms(Scene assimpScene)
        {
            this.LoadNodeTransformRecursive(assimpScene.RootNode, Matrix4x4.Identity);
        }

        /// <summary>
        /// Recursively loads the transforms of the nodes in the model
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

            foreach(var child in node.Children)
            {
                LoadNodeTransformRecursive(child, currentTransform);
            }
        }

        /// <summary>
        /// Extracts the animations from the scene
        /// </summary>
        /// <param name="scene"></param>
        private void ExtractAnimations(Scene scene)
        {
            Animations = new List<Graphics.Animation3D.Animation>();
            for (int i = 0; i < scene.AnimationCount; i++)
            {
                var animation = new Graphics.Animation3D.Animation(scene, this, i);
                this.Animations.Add(animation);
            }

            this.Animator = new Graphics.Animation3D.Animator(this.Animations[0]);
        }

        /// <summary>
        /// Extracts the materials from the scene
        /// </summary>
        /// <param name="assimpScene"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        private void ExtractMaterials(Scene assimpScene, String directory)
        {
            var materials = new List<IMaterial>();

            // Load materials
            foreach (var asmat in assimpScene.Materials)
            {
                var material = new Graphics.Materials.SGMaterial();
                material.Name = asmat.Name;
                material.Opacity = asmat.Opacity;
                material.Color = new Vector4(asmat.ColorDiffuse.R, asmat.ColorDiffuse.G, asmat.ColorDiffuse.B, asmat.ColorDiffuse.A);

                if(asmat.Shininess > 0)
                {
                    material.Shininess = asmat.Shininess;
                }

                if (asmat.HasTextureDiffuse)
                {
                    material.DiffuseTexture = Texture.LoadTexture(Path.Combine(directory, asmat.TextureDiffuse.FilePath));
                }

                if (asmat.HasTextureNormal)
                {
                    material.NormalTexture = Texture.LoadTexture(Path.Combine(directory, asmat.TextureNormal.FilePath));
                }

                if (asmat.HasTextureSpecular)
                {
                    material.SpecularTexture = Texture.LoadTexture(Path.Combine(directory, asmat.TextureSpecular.FilePath));
                }

                materials.Add(material);
            }

            this.Materials.AddRange(materials);
        }

        /// <summary>
        /// Extracts the meshes from the scene
        /// </summary>
        /// <param name="assimpScene"></param>
        /// <param name="materials"></param>
        private void ExtractMeshes(Scene assimpScene)
        {
            var meshes = new List<Graphics.Mesh>();

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
                ExtractBoneWeightForVertices(asmesh, assimpScene, mesh);
                meshes.Add(mesh);

                var meshMaterialPair = new MeshMaterialPair();
                meshMaterialPair.MeshName = mesh.ID.ToString();
                meshMaterialPair.MaterialIndex = asmesh.MaterialIndex;
                this.MeshMaterials.Add(meshMaterialPair);
            }

            this.Meshes.AddRange(meshes);
        }

        /// <summary>
        /// Extracts the bone weights for the vertices
        /// </summary>
        /// <param name="asmesh"></param>
        /// <param name="scene"></param>
        /// <param name="mesh"></param>
        private void ExtractBoneWeightForVertices(Assimp.Mesh asmesh, Assimp.Scene scene, Graphics.Mesh mesh)
        {
            for (int boneIndex = 0; boneIndex < asmesh.BoneCount; boneIndex++)
            {
                int boneId = -1;
                var boneName = asmesh.Bones[boneIndex].Name;
                if (!Skeleton.BoneInfoMap.ContainsKey(boneName))
                {
                    var boneInfo = new BoneInfo();
                    boneInfo.id = Skeleton.BoneCounter;
                    boneInfo.offset = Math.Math.ToTKMatrix(asmesh.Bones[boneIndex].OffsetMatrix);
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
        /// Sets the bone data for the vertex
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
        /// Initializes the model
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.Init(scene, viewport, renderer);

            this.Materials.ForEach(material =>
            {
                material.Init(renderer);
            });

            this.Meshes.ForEach(mesh =>
            {
                renderer.LoadMesh(mesh);
                
            });

            if(this.Shader == null)
            {
                if (this.HasAnimations)
                {
                    this.Shader = renderer.GetShaderProgram("AnimatedMeshShader");
                }
                else
                {
                    this.Shader = renderer.GetShaderProgram("MeshShader");
                }
            }

            Debug.WriteLine($"Initialized Model {Name} with error {renderer.GetError()}");
        }

        /// <summary>
        /// Renders the model
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Graphics.Camera camera)
        {
            base.Render(scene, viewport, renderer, camera);
            var light = renderer.GetLightSource<DirectionalLight>();

            if(this.HasAnimations)
            {
                RenderAnimatedModel(scene, viewport, renderer, camera, light);
            }
            else
            {
                RenderStaticModel(scene, viewport, renderer, camera, light);
            }
        }

        private void RenderAnimatedModel(BaseScene scene, Viewport viewport, IRenderDevice renderer, Graphics.Camera camera, DirectionalLight light)
        {
            renderer.BindShaderProgram(this.Shader);
            renderer.PrepareShader("finalBonesMatrices", true, Animator.FinalBoneMatrices.ToArray());
            renderer.PrepareShader("viewPos", camera.Transform.Position);
            if (scene.LightManager != null)
            {
                scene.LightManager.BindLights(viewport, renderer, camera);
            }

            this.MeshMaterials.ForEach(pair =>
            {
                var mesh = this.Meshes.GetMesh(pair.MeshName);
                var material = this.Materials.GetMaterial(pair.MaterialIndex);
                renderer.DrawMesh(Transform, mesh, material);
            });

            renderer.UnbindShaderProgram();
        }

        private void RenderStaticModel(BaseScene scene, Viewport viewport, IRenderDevice renderer, Graphics.Camera camera, DirectionalLight light)
        {
            renderer.BindShaderProgram(this.Shader);
            renderer.PrepareShader("viewPos", camera.Transform.Position);
            if(scene.LightManager != null)
            {
                scene.LightManager.BindLights(viewport, renderer, camera);
            }

            this.MeshMaterials.ForEach(pair =>
            {
                var mesh = this.Meshes.GetMesh(pair.MeshName);
                var material = this.Materials.GetMaterial(pair.MaterialIndex);
                renderer.DrawMesh(Transform, mesh, material);
            });

            renderer.UnbindShaderProgram();
        }


        /// <summary>
        /// Disposes the model
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="renderer"></param>
        public override void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            base.Dispose(scene, renderer);
            Debug.WriteLine($"Disposing Model {Name}");   

            this.Materials.ForEach(material =>
            {
                material.Dispose(renderer);
            });

            this.Meshes.ForEach(mesh =>
            {
                renderer.DisposeMesh(mesh);
            });

            Debug.WriteLine($"Disposed Model {Name}");
        }

        /// <summary>
        /// Plays the specified animation on the model.
        /// </summary>
        public void PlayAnimation(String name)
        {
            var animation = this.FindAnimation(name);
            if (animation != null && this.Animator.CurrentAnimation != animation)
            {
                this.Animator.LoadAnimation(animation);
            }
        }

        /// <summary>
        /// Stops the currently playing animation.
        /// </summary>
        /// <remarks>
        /// This method sets the animator's play state to false, effectively pausing the animation.
        /// </remarks>
        public void StopAnimation()
        {
            if (this.Animator.Play != false)
            {
                this.Animator.Play = false;
            }
        }

        /// <summary>
        /// Finds an animation with the specified name.
        /// </summary>
        public Graphics.Animation3D.Animation FindAnimation(String name)
        {
            var animation = Animations.FirstOrDefault(a => a.Name == name);
            if (animation != null)
            {
                return animation;
            }
            return null;
        }

        public override void Update(BaseScene scene)
        {
            base.Update(scene);

            //float deltaTime = (float)0.1f;
            
            if(this.HasAnimations)
            {
                float deltaTimeInSeconds = scene.RenderStats.DeltaTime / 1000f;
                float animationSpeed = deltaTimeInSeconds * this.AnimationSpeed;
                Animator.UpdateAnimation(animationSpeed);
            }
        }

        public override void ComputeAABB()
        {
            if(this.Meshes.Count == 0)
            {
                this.AABB = new AABB(Vector3.Zero, Vector3.Zero);
                return;
            }

            var min = new Vector3(float.MaxValue);
            var max = new Vector3(float.MinValue);

            foreach (var mesh in Meshes)
            {
                foreach (var vertex in mesh.Vertices)
                {
                    min = Vector3.ComponentMin(min, vertex.Position);
                    max = Vector3.ComponentMax(max, vertex.Position);
                }
            }

            this.AABB = new AABB(min, max);
        }
    }
}
