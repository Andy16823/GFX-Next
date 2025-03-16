using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents the flags of a material
    /// </summary>
    public enum MaterialFlags
    {
        None,
        Loaded,
        Disposed,
        Failed
    }

    /// <summary>
    /// Represents a material
    /// </summary>
    public class Material
    {
        /// <summary>
        /// The name of the material
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The base color texture of the material
        /// </summary>
        public Texture BaseColor { get; set; }

        /// <summary>
        /// The normal texture of the material
        /// </summary>
        public Texture Normal { get; set; }

        /// <summary>
        /// The metallic texture of the material
        /// </summary>
        public Texture Metallic { get; set; }

        /// <summary>
        /// The roughness texture of the material
        /// </summary>
        public Texture Roughness { get; set; }

        /// <summary>
        /// The ambient occlusion texture of the material
        /// </summary>
        public Texture AmbientOcclusion { get; set; }

        /// <summary>
        /// The emissive texture of the material
        /// </summary>
        public Texture Emissive { get; set; }

        /// <summary>
        /// The height texture of the material
        /// </summary>
        public Texture Height { get; set; }

        /// <summary>
        /// The diffuse color of the material
        /// </summary>
        public Vector4 DiffuseColor { get; set; }

        /// <summary>
        /// The opacity of the material
        /// </summary>
        public float Opacity { get; set; }

        /// <summary>
        /// The flags of the material
        /// </summary>
        public MaterialFlags Flags { get; set; }

        /// <summary>
        /// Creates a new material
        /// </summary>
        public Material()
        {
            DiffuseColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        }

        /// <summary>
        /// Loads a material from a file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Material LoadMaterial(String file)
        {
            if (!File.Exists(file))
            {
                throw new ArgumentException($"Material file '{file}' does not exist.");
            }

            var basePath = Path.GetDirectoryName(file);
            var jsonString = File.ReadAllText(file);
            var jsonObject = JObject.Parse(jsonString);

            Material material = new Material
            {
                Name = jsonObject["Name"].Value<String>(),
                BaseColor = LoadTextureIfExists(jsonObject, "BaseColor", basePath),
                Normal = LoadTextureIfExists(jsonObject, "Normal", basePath),
                Metallic = LoadTextureIfExists(jsonObject, "Metallic", basePath),
                Roughness = LoadTextureIfExists(jsonObject, "Roughness", basePath),
                AmbientOcclusion = LoadTextureIfExists(jsonObject, "AmbientOcclusion", basePath),
                Emissive = LoadTextureIfExists(jsonObject, "Emissive", basePath),
                Height = LoadTextureIfExists(jsonObject, "Height", basePath),
                DiffuseColor = new Vector4(
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
        /// Loads a texture if it exists
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <param name="propertyName"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        private static Texture LoadTextureIfExists(JObject jsonObject, string propertyName, string basePath)
        {
            if (jsonObject[propertyName].Value<String>() != "null")
            {
                var texturePath = Path.Combine(basePath, jsonObject[propertyName].Value<String>());
                return Texture.LoadTexture(texturePath);
            }
            return null;
        }
    }
}
