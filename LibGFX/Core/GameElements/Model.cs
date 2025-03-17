﻿using Assimp;
using Assimp.Configs;
using LibGFX.Graphics;
using LibGFX.Graphics.Animation3D;
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
using Light = LibGFX.Graphics.Light;

namespace LibGFX.Core.GameElements
{
    /// <summary>
    /// Represents a 3D model
    /// </summary>
    public class Model : GameElement
    {
        /// <summary>
        /// The meshes of the model
        /// </summary>
        public List<Graphics.Mesh> Meshes { get; set; }

        /// <summary>
        /// Gets or sets the mapping of bone names to bone information.
        /// </summary>
        public Dictionary<String, BoneInfo> BoneInfoMap { get; set; }

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
        public float AnimationSpeed { get; set; } = 5.0f;

        /// <summary>
        /// The counter for the bones
        /// </summary>
        public int BoneCounter;


        /// <summary>
        /// Creates a new model
        /// </summary>
        /// <param name="name"></param>
        /// <param name="file"></param>
        public Model(String name, String file)
        {
            this.Name = name;
            this.BoneInfoMap = new Dictionary<String, BoneInfo>();
            this.LoadModel(file);
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
            var materials = ExtractMaterials(assimpScene, directory);
            ExtractMeshes(assimpScene, materials);

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

            this.Animator = new Graphics.Animation3D.Animator(this.Animations[1]);
        }

        /// <summary>
        /// Extracts the materials from the scene
        /// </summary>
        /// <param name="assimpScene"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        private List<Graphics.Material> ExtractMaterials(Scene assimpScene, String directory)
        {
            var materials = new List<Graphics.Material>();

            // Load materials
            foreach (var asmat in assimpScene.Materials)
            {
                var material = new Graphics.Material();
                material.Name = asmat.Name;
                material.Opacity = asmat.Opacity;
                material.DiffuseColor = new Vector4(asmat.ColorDiffuse.R, asmat.ColorDiffuse.G, asmat.ColorDiffuse.B, asmat.ColorDiffuse.A);

                if (asmat.HasTextureDiffuse)
                {
                    material.BaseColor = Texture.LoadTexture(Path.Combine(directory, asmat.TextureDiffuse.FilePath));
                }

                if (asmat.HasTextureNormal)
                {
                    material.Normal = Texture.LoadTexture(Path.Combine(directory, asmat.TextureNormal.FilePath));
                }

                materials.Add(material);
            }
            return materials;
        }

        /// <summary>
        /// Extracts the meshes from the scene
        /// </summary>
        /// <param name="assimpScene"></param>
        /// <param name="materials"></param>
        private void ExtractMeshes(Scene assimpScene, List<Graphics.Material> materials)
        {
            this.Meshes = new List<Graphics.Mesh>();

            foreach (var asmesh in assimpScene.Meshes)
            {
                var mesh = new Graphics.Mesh();
                mesh.Name = asmesh.Name;
                mesh.Material = materials[asmesh.MaterialIndex];

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
                this.Meshes.Add(mesh);
            }
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
                if (!BoneInfoMap.ContainsKey(boneName))
                {
                    var boneInfo = new BoneInfo();
                    boneInfo.id = BoneCounter;
                    boneInfo.offset = Math.Math.ToTKMatrix(asmesh.Bones[boneIndex].OffsetMatrix);
                    BoneInfoMap.Add(boneName, boneInfo);
                    boneId = BoneCounter;
                    BoneCounter++;
                }
                else
                {
                    boneId = BoneInfoMap[boneName].id;
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
            foreach (var mesh in Meshes)
            {
                renderer.LoadMesh(mesh);
                renderer.LoadMaterial(mesh.Material);
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

        private void RenderAnimatedModel(BaseScene scene, Viewport viewport, IRenderDevice renderer, Graphics.Camera camera, Light light)
        {
            // Bind the shader program
            renderer.BindShaderProgram(renderer.GetShaderProgram("AnimatedMeshShader"));

            // Prepare the shader uniforms
            renderer.PrepareShader("finalBonesMatrices", false, Animator.FinalBoneMatrices.ToArray());
            if (light != null)
            {
                renderer.PrepareShader("lightPos", light.Position);
                renderer.PrepareShader("lightColor", light.Color.Xyz);
                renderer.PrepareShader("lightIntensity", light.Intensity);
                renderer.PrepareShader("viewPos", camera.Transform.Position);
            }

            // Alter code mit mehreren uniforms bindings
            //for (int i = 0; i < 100; i++)
            //{
            //    var matrix = Animator.FinalBoneMatrices[i];
            //    renderer.PrepareShader($"finalBonesMatrices[{i.ToString()}]", false, matrix);
            //}

            // Draw the meshes
            foreach (var mesh in Meshes)
            {
                renderer.DrawMesh(Transform, mesh);
            }

            // Unbind the shader program
            renderer.UnbindShaderProgram();
        }

        private void RenderStaticModel(BaseScene scene, Viewport viewport, IRenderDevice renderer, Graphics.Camera camera, Light light)
        {
            // Bind the shader program
            renderer.BindShaderProgram(renderer.GetShaderProgram("MeshShader"));

            // Prepare the shader uniforms
            if (light != null)
            {
                renderer.PrepareShader("lightPos", light.Position);
                renderer.PrepareShader("lightColor", light.Color.Xyz);
                renderer.PrepareShader("lightIntensity", light.Intensity);
                renderer.PrepareShader("viewPos", camera.Transform.Position);
            }

            // Draw the meshes
            foreach (var mesh in Meshes)
            {
                renderer.DrawMesh(Transform, mesh);
            }

            // Unbind the shader program
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
            foreach (var mesh in Meshes)
            {
                renderer.DisposeMesh(mesh);
                renderer.DisposeMaterial(mesh.Material);
            }
            Debug.WriteLine($"Disposed Model {Name}");
        }

        /// <summary>
        /// Plays the specified animation on the model.
        /// </summary>
        public void PlayAnimation(String name)
        {
            var animation = this.FindAnimation(name);
            if (animation != null)
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
                float animationSpeed = this.AnimationSpeed / 100f;
                Animator.UpdateAnimation(animationSpeed);
                //if (Animator.CurrentAnimation != null && Animator.Play)
                //{
                //    foreach (var callback in AnimationCallbacks)
                //    {
                //        if (callback.AnimationName.Equals(Animator.CurrentAnimation.Name) && Animator.CurrentAnimation.GetKeyFrameIndex(Animator.CurrentTime) == callback.Frame && !callback.CallbackRised)
                //        {
                //            callback.Callback(game, this);
                //            callback.CallbackRised = true;
                //        }
                //        else if (callback.CallbackRised && Animator.CurrentAnimation.GetKeyFrameIndex(Animator.CurrentTime) != callback.Frame)
                //        {
                //            callback.CallbackRised = false;
                //        }
                //    }
                //}
            }
        }
    }
}
