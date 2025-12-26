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
        /// The unique identifier of the material.
        /// </summary>
        public Guid ID { get; } = Guid.NewGuid();

        /// <summary>
        /// The opacity of the material.
        /// </summary>
        public float Opacity { get; set; }

        /// <summary>
        /// The color of the material.
        /// </summary>
        public Vector4 Color { get; set; }

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
        /// The UV scale of the material.
        /// </summary>
        public Vector2 UVScale { get; set; } = Texture.DefaultUVScale;

        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SGMaterial"/> class.
        /// </summary>
        public SGMaterial()
        {
            Name = "Unnamed Material";
            Opacity = 1.0f;
            Color = Vector4.One;
            DiffuseTexture = null;
            NormalTexture = null;
            SpecularTexture = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SGMaterial"/> class with the specified name and color.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="color"></param>
        public SGMaterial(string name, Vector4 color)
        {
            Name = name;
            Opacity = 1.0f;
            Color = color;
            DiffuseTexture = new Texture(1, 1, new Vector4i(255, 255, 255, 255));
            NormalTexture = new Texture(1, 1, new Vector4i(128, 128, 255, 255));
            SpecularTexture = new Texture(1, 1, new Vector4i(0, 0, 0, 255));
        }

        /// <summary>
        /// Initializes the material.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Init(IRenderDevice renderDevice)
        {
            Debug.WriteLine($"Loading material {Name}");
            if (this.IsInitialized)
            {
                Debug.WriteLine($"Material {Name} is already loaded.");
                return;
            }

            if(DiffuseTexture != null)
            {
                DiffuseTexture.TextureParameters = TextureParameters.Mipmapped;
                DiffuseTexture.Init(renderDevice);
            }
            if (NormalTexture != null)
            {
                NormalTexture.TextureParameters = TextureParameters.Mipmapped;
                NormalTexture.Init(renderDevice);
            }
            if (SpecularTexture != null)
            {
                SpecularTexture.TextureParameters = TextureParameters.Mipmapped;
                SpecularTexture.Init(renderDevice);
            }

            IsInitialized = true;
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
            renderDevice.PrepareShader("material.uvScale", UVScale);
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
            if (DiffuseTexture != null)
            {
                DiffuseTexture.Dispose(renderDevice);
            }
            if (NormalTexture != null)
            {
                NormalTexture.Dispose(renderDevice);
            }
            if (SpecularTexture != null)
            {
                SpecularTexture.Dispose(renderDevice);
            }
            IsInitialized = false;
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
            };
            return material;
        }

        /// <summary>
        /// Creates a new material instance from the specified Assimp material, loading associated textures from the
        /// given directory.
        /// </summary>
        /// <remarks>If the Assimp material does not specify a diffuse, normal, or specular texture, a
        /// default 1x1 texture is used for the corresponding property. The returned material's properties are
        /// initialized based on the values present in the Assimp material.</remarks>
        /// <param name="asmat">The Assimp material to convert. Must not be null.</param>
        /// <param name="directory">The directory path used to resolve texture file locations. Must not be null or empty.</param>
        /// <returns>An IMaterial instance representing the converted material, with textures loaded from the specified
        /// directory.</returns>
        public static IMaterial LoadMaterial(Assimp.Material asmat, String directory)
        {
            var material = new Graphics.Materials.SGMaterial();
            material.Name = asmat.Name;
            material.Opacity = asmat.Opacity;
            material.Color = new Vector4(asmat.ColorDiffuse.X, asmat.ColorDiffuse.Y, asmat.ColorDiffuse.Z, asmat.ColorDiffuse.W);

            if (asmat.Shininess > 0)
            {
                material.Shininess = asmat.Shininess;
            }

            if (asmat.HasTextureDiffuse)
            {
                material.DiffuseTexture = new Texture(Path.Combine(directory, asmat.TextureDiffuse.FilePath));
            }
            else
            {
                material.DiffuseTexture = new Texture(1, 1, new Vector4i(255, 255, 255, 255));
            }

            if (asmat.HasTextureNormal)
            {
                material.NormalTexture = new Texture(Path.Combine(directory, asmat.TextureNormal.FilePath));
            }
            else
            {
                material.NormalTexture = new Texture(1, 1, new Vector4i(128, 128, 255, 255));
            }

            if (asmat.HasTextureSpecular)
            {
                material.SpecularTexture = new Texture(Path.Combine(directory, asmat.TextureSpecular.FilePath));
            }
            else
            {
                material.SpecularTexture = new Texture(1, 1, new Vector4i(0, 0, 0, 255));
            }

            return material;
        }
    }
}
