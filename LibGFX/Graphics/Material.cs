using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public enum MaterialFlags
    {
        None,
        Loaded,
        Disposed,
        Failed
    }

    public class Material
    {
        public String Name { get; set; }
        public Texture BaseColor { get; set; }
        public Texture Normal { get; set; }
        public Texture Metallic { get; set; }
        public Texture Roughness { get; set; }
        public Texture AmbientOcclusion { get; set; }
        public Texture Emissive { get; set; }
        public Texture Height { get; set; }
        public Vector4 VertexColor { get; set; }
        public MaterialFlags Flags { get; set; }

        public Material()
        {
            VertexColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        }

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
                VertexColor = new Vector4(
                    jsonObject["VertexColor"][0].Value<float>(),
                    jsonObject["VertexColor"][1].Value<float>(),
                    jsonObject["VertexColor"][2].Value<float>(),
                    jsonObject["VertexColor"][3].Value<float>()
                )
            };

            return material;
        }

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
