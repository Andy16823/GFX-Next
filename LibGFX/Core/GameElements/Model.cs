using Assimp;
using Assimp.Configs;
using LibGFX.Graphics;
using OpenTK.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Creates a new model
        /// </summary>
        /// <param name="name"></param>
        /// <param name="file"></param>
        public Model(String name, String file)
        {
            this.Name = name;
            var directory = Path.GetDirectoryName(file);

            // Load the model using Assimp
            var importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            var assimpScene = importer.ImportFile(file, PostProcessPreset.TargetRealTimeQuality | PostProcessSteps.PreTransformVertices);

            var materials = ExtractMaterials(assimpScene, directory);
            ExtractMeshes(assimpScene, materials);
        }

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

        private void ExtractMeshes(Scene assimpScene, List<Graphics.Material> materials)
        {
            this.Meshes = new List<Graphics.Mesh>();

            foreach (var asmesh in assimpScene.Meshes)
            {
                var mesh = new Graphics.Mesh();
                mesh.Name = asmesh.Name;
                mesh.Material = materials[asmesh.MaterialIndex];

                mesh.Indices.AddRange(asmesh.GetIndices());
                mesh.Vertices.AddRange(asmesh.Vertices.SelectMany(v => new float[] { v.X, v.Y, v.Z }));
                mesh.Normals.AddRange(asmesh.Normals.SelectMany(n => new float[] { n.X, n.Y, n.Z }));
                mesh.TexCoords.AddRange(asmesh.TextureCoordinateChannels[0].SelectMany(t => new float[] { t.X, t.Y }));
                mesh.Tangents.AddRange(asmesh.Tangents.SelectMany(t => new float[] { t.X, t.Y, t.Z, 1.0f }));
                this.Meshes.Add(mesh);
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

            renderer.BindShaderProgram(renderer.GetShaderProgram("MeshShader"));
            foreach (var mesh in Meshes)
            {
                if(light != null)
                {
                    renderer.PrepareShader("lightPos", light.Position);
                    renderer.PrepareShader("lightColor", light.Color.Xyz);
                    renderer.PrepareShader("lightIntensity", light.Intensity);
                    renderer.PrepareShader("viewPos", camera.Transform.Position);
                }
                renderer.DrawMesh(Transform, mesh);
            }
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
    }
}
