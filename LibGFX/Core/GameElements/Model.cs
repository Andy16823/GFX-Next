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
    public class Model : GameElement
    {
        public List<Graphics.Mesh> Meshes { get; set; }

        public Model(String name, String file)
        {
            this.Name = name;
            this.Meshes = new List<Graphics.Mesh>();

            var directory = Path.GetDirectoryName(file);
            var materials = new List<Graphics.Material>();

            Assimp.AssimpContext importer = new Assimp.AssimpContext();
            //importer.SetConfig(new Assimp.Configs.NormalSmoothingAngleConfig(66.0f));
            var assimpScene = importer.ImportFile(file, Assimp.PostProcessPreset.TargetRealTimeQuality | Assimp.PostProcessSteps.PreTransformVertices);

            foreach (var asmat in assimpScene.Materials)
            {
                var material = new Graphics.Material();
                material.Opacity = asmat.Opacity;
                material.DiffuseColor = new Vector4(asmat.ColorDiffuse.R, asmat.ColorDiffuse.G, asmat.ColorDiffuse.B, asmat.ColorDiffuse.A);

                if(asmat.HasTextureDiffuse)
                {
                    material.BaseColor = Texture.LoadTexture(Path.Combine(directory, asmat.TextureDiffuse.FilePath));
                }

                if (asmat.HasTextureNormal)
                {
                    material.Normal = Texture.LoadTexture(Path.Combine(directory, asmat.TextureNormal.FilePath));
                }
                //else if (asmat.HasTextureHeight)
                //{
                //    material.Normal = Texture.LoadTexture(Path.Combine(directory, asmat.TextureNormal.FilePath));
                //}

                materials.Add(material);
            }

            foreach (var asmesh in assimpScene.Meshes)
            {
                var mesh = new Graphics.Mesh();
                mesh.Name = asmesh.Name;
                mesh.Material = materials[asmesh.MaterialIndex];

                mesh.Indices.AddRange(asmesh.GetIndices());
                mesh.Vertices.AddRange(asmesh.Vertices.SelectMany(v => new float[] { v.X, v.Y, v.Z }));
                mesh.Normals.AddRange(asmesh.Normals.SelectMany(n => new float[] { n.X, n.Y, n.Z }));
                mesh.TexCoords.AddRange(asmesh.TextureCoordinateChannels[0].SelectMany(t => new float[] { t.X, t.Y }));
                //mesh.Tangents.AddRange(asmesh.Tangents.SelectMany(t => new float[] { t.X, t.Y, t.Z })); Kann zu fehler kommen da shader einen vec4 erwarte
                this.Meshes.Add(mesh);
            }
        }

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

        public override void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Graphics.Camera camera)
        {
            base.Render(scene, viewport, renderer, camera);
            renderer.BindShaderProgram(renderer.GetShaderProgram("MeshShader"));
            foreach (var mesh in Meshes)
            {
                renderer.DrawMesh(Transform, mesh);
            }
            renderer.UnbindShaderProgram();
        }

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
