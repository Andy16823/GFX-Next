using LibGFX.Core;
using LibGFX.Graphics.Shader;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Materials
{
    /// <summary>
    /// Represents a material used in rendering.
    /// Uses the Specular-Glossiness workflow.
    /// </summary>
    public class SGMaterial : IMaterial
    {
        /// <summary>
        /// The name of the material.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The opacity of the material.
        /// </summary>
        public float Opacity { get; set; }

        /// <summary>
        /// The color of the material.
        /// </summary>
        public Vector4 Color { get; set; }

        /// <summary>
        /// The shader program used by the material.
        /// </summary>
        public ShaderProgram Shader { get; set; }

        /// <summary>
        /// The flags of the material.
        /// </summary>
        public MaterialFlags Flags { get; set; }

        /// <summary>
        /// The diffuse texture of the material.
        /// </summary>
        public Texture DiffuseTexture { get; set; }

        /// <summary>
        /// The normal texture of the material.
        /// </summary>
        public Texture NormalTexture { get; set; }

        /// <summary>
        /// The specular texture of the material.
        /// </summary>
        public Texture SpecularTexture { get; set; }

        /// <summary>
        /// The shininess of the material.
        /// </summary>
        public float Shininess { get; set; } = 64.0f;

        /// <summary>
        /// Indicates whether the normal map should be flipped.
        /// </summary>
        public bool FlipNormal { get; set; } = false;

        /// <summary>
        /// Initializes the material.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Init(IRenderDevice renderDevice)
        {
            Debug.WriteLine($"Loading material {Name}");
            if(this.Flags != MaterialFlags.None)
            {
                Debug.WriteLine($"Material {Name} is already loaded.");
                return;
            }

            renderDevice.LoadTexture(DiffuseTexture);
            renderDevice.LoadTexture(NormalTexture);
            renderDevice.LoadTexture(SpecularTexture);
            Flags = MaterialFlags.Loaded;
        }

        /// <summary>
        /// Prepares the material for rendering.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Use(IRenderDevice renderDevice)
        {
            renderDevice.PrepareShader("material.shininess", Shininess);
            renderDevice.PrepareShader("material.vertexColor", Color);
            renderDevice.PrepareShader("material.flipNormal", FlipNormal);
            if (DiffuseTexture != null)
            {
                renderDevice.PrepareShader("material.textureSampler", OpenTK.Graphics.OpenGL4.TextureUnit.Texture0, DiffuseTexture);
            }
            else
            {
                renderDevice.PrepareShader("material.textureSampler", OpenTK.Graphics.OpenGL4.TextureUnit.Texture0, 0);
            }

            if (NormalTexture != null)
            {
                renderDevice.PrepareShader("material.normalSampler", OpenTK.Graphics.OpenGL4.TextureUnit.Texture1, NormalTexture);
            }
            else
            {
                renderDevice.PrepareShader("material.normalSampler", OpenTK.Graphics.OpenGL4.TextureUnit.Texture1, 0);
            }

            if (SpecularTexture != null)
            {
                renderDevice.PrepareShader("material.specularSampler", OpenTK.Graphics.OpenGL4.TextureUnit.Texture2, SpecularTexture);
            }
            else
            {
                renderDevice.PrepareShader("material.specularSampler", OpenTK.Graphics.OpenGL4.TextureUnit.Texture2, 0);
            }

        }

        /// <summary>
        /// Disposes the material and its resources.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Dispose(IRenderDevice renderDevice)
        {
            Debug.WriteLine($"Disposing material {Name}");
            renderDevice.DisposeTexture(DiffuseTexture);
            renderDevice.DisposeTexture(NormalTexture);
            renderDevice.DisposeTexture(SpecularTexture);
            Flags = MaterialFlags.Disposed;
        }

        /// <summary>
        /// Loads a material from a JSON file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static SGMaterial LoadFromFile(string file)
        {
            if (!File.Exists(file))
            {
                throw new ArgumentException($"Material file '{file}' does not exist.");
            }
            var basePath = Path.GetDirectoryName(file);
            var jsonString = File.ReadAllText(file);
            var jsonObject = JObject.Parse(jsonString);

            var material = new SGMaterial()
            {
                Name = jsonObject["Name"].Value<string>(),
                DiffuseTexture = Utils.LoadTextureIfExists(jsonObject, "BaseColor", basePath),
                NormalTexture = Utils.LoadTextureIfExists(jsonObject, "Normal", basePath),
                SpecularTexture = Utils.LoadTextureIfExists(jsonObject, "Specular", basePath),
                Color = new Vector4(
                    jsonObject["DiffuseColor"][0].Value<float>(),
                    jsonObject["DiffuseColor"][1].Value<float>(),
                    jsonObject["DiffuseColor"][2].Value<float>(),
                    jsonObject["DiffuseColor"][3].Value<float>()
                ),
                Opacity = jsonObject["Opacity"].Value<float>(),
                Flags = MaterialFlags.None
            };
            return material;
        }
    }
}
