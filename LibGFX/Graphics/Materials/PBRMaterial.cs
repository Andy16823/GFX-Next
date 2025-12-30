using LibGFX.Core;
using Newtonsoft.Json;
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
    public class PBRMaterial : IMaterial
    {
        public string Name { get; set; }
        public Guid ID { get; private set; } = Guid.NewGuid();
        public Vector3 Albedo { get; set; } = new Vector3(1, 1, 0);
        public float Metallic { get; set; } = 1.0f;
        public float Roughness { get; set; } = 0.5f;
        public float Occlusion { get; set; } = 0f;
        public Texture AlbedoTexture { get; set; } = null;
        public Texture NormalTexture { get; set; } = null;
        public Texture MetallicTexture { get; set; } = null;
        public Texture RoughnessTexture { get; set; } = null;
        public Texture OcclusionTexture { get; set; } = null;
        public bool IsInitialized { get; private set; } = false;

        public void Dispose(IRenderDevice renderDevice)
        {
            Debug.WriteLine($"Disposing material {Name}");
            if (this.AlbedoTexture != null)
            {
                AlbedoTexture.Dispose(renderDevice);
            }

            if(this.NormalTexture != null)
            {
                NormalTexture.Dispose(renderDevice);
            }

            if(this.MetallicTexture != null)
            {
                MetallicTexture.Dispose(renderDevice);
            }

            if(this.RoughnessTexture != null)
            {
                RoughnessTexture.Dispose(renderDevice);
            }

            if (this.OcclusionTexture != null)
            {
                OcclusionTexture.Dispose(renderDevice);
            }

            this.IsInitialized = false;
        }

        public void Init(IRenderDevice renderDevice)
        {
            if (this.IsInitialized)
            {
                return;
            }

            if(this.AlbedoTexture != null)
            {
                AlbedoTexture.Init(renderDevice);
            }

            if(this.NormalTexture != null)
            {
                NormalTexture.Init(renderDevice);
            }

            if(this.MetallicTexture != null)
            {
                MetallicTexture.Init(renderDevice);
            }

            if(this.RoughnessTexture != null)
            {
                RoughnessTexture.Init(renderDevice);
            }

            if(this.OcclusionTexture != null)
            {
                OcclusionTexture.Init(renderDevice);
            }

            this.IsInitialized = true;
        }

        public void Use(IRenderDevice renderDevice)
        {
            renderDevice.PrepareShader("albedoMap", 0, AlbedoTexture);
            renderDevice.PrepareShader("normalMap", 1, NormalTexture);
            renderDevice.PrepareShader("metallicMap", 2, MetallicTexture);
            renderDevice.PrepareShader("roughnessMap", 3, RoughnessTexture);

            if(OcclusionTexture != null)
            {
                renderDevice.PrepareShader("aoMap", 4, OcclusionTexture);
            }

            //renderDevice.PrepareShader("albedo", Albedo);
            //renderDevice.PrepareShader("metallic", Metallic);
            //renderDevice.PrepareShader("roughness", Roughness);
            //renderDevice.PrepareShader("ao", Occlusion);
        }

        public static PBRMaterial LoadFromFile(string file)
        {
            if (!File.Exists(file))
            {
                throw new ArgumentException($"Material file '{file}' does not exist.");
            }
            var basePath = Path.GetDirectoryName(file);
            var jsonString = File.ReadAllText(file);
            var jsonObject = JObject.Parse(jsonString);

            var material = new PBRMaterial()
            {
                Name = jsonObject["Name"].Value<string>(),
                AlbedoTexture = Utils.LoadTextureIfExists(jsonObject, "AlbedoTexture", basePath),
                NormalTexture = Utils.LoadTextureIfExists(jsonObject, "NormalTexture", basePath),
                MetallicTexture = Utils.LoadTextureIfExists(jsonObject, "MetallicTexture", basePath),
                RoughnessTexture = Utils.LoadTextureIfExists(jsonObject, "RoughnessTexture", basePath),
                OcclusionTexture = Utils.LoadTextureIfExists(jsonObject, "OcclusionTexture", basePath),
            };
            return material;
        }

        /// <summary>
        /// Loads a material from the specified Assimp material definition and associated directory.
        /// </summary>
        /// <param name="asmat">The Assimp material to convert to an internal material representation. Cannot be null.</param>
        /// <param name="directory">The directory path used to resolve any external resources referenced by the material. Cannot be null or
        /// empty.</param>
        /// <returns>An <see cref="IMaterial"/> instance representing the loaded material.</returns>
        /// <exception cref="NotImplementedException">The method is not implemented.</exception>
        public void LoadMaterial(Assimp.Material asmat, String directory)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes the current object to a JSON representation using the specified serialization context.
        /// </summary>
        /// <param name="serializationContext">The context that provides information and settings required for the serialization process.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized representation of the object.</returns>
        /// <exception cref="NotImplementedException">The method is not implemented.</exception>
        public void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(this.GetType().FullName);
            writer.WritePropertyName("Name");
            writer.WriteValue(this.Name);
            writer.WritePropertyName("ID");
            writer.WriteValue(this.ID.ToString());
            writer.WritePropertyName("Albedo");
            Utils.SerializeVec3(this.Albedo, writer);
            writer.WritePropertyName("Metallic");
            writer.WriteValue(this.Metallic);
            writer.WritePropertyName("Roughness");
            writer.WriteValue(this.Roughness);
            writer.WritePropertyName("Occlusion");
            writer.WriteValue(this.Occlusion);
            writer.WritePropertyName("AlbedoTexture");
            if (this.AlbedoTexture != null)
            {
                this.AlbedoTexture.Serialize(writer, serializationContext);
            }
            else
            {
                writer.WriteNull();
            }
            writer.WritePropertyName("NormalTexture");
            if (this.NormalTexture != null)
            {
                this.NormalTexture.Serialize(writer, serializationContext);
            }
            else
            {
                writer.WriteNull();
            }
            writer.WritePropertyName("MetallicTexture");
            if (this.MetallicTexture != null)
            {
                this.MetallicTexture.Serialize(writer, serializationContext);
            }
            else
            {
                writer.WriteNull();
            }
            writer.WritePropertyName("RoughnessTexture");
            if (this.RoughnessTexture != null)
            {
                this.RoughnessTexture.Serialize(writer, serializationContext);
            }
            else
            {
                writer.WriteNull();
            }
            writer.WritePropertyName("OcclusionTexture");
            if (this.OcclusionTexture != null)
            {
                this.OcclusionTexture.Serialize(writer, serializationContext);
            }
            else
            {
                writer.WriteNull();
            }
            callback?.Invoke(writer);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Populates the current object with values from the specified JSON object using the provided serialization
        /// context.
        /// </summary>
        /// <param name="jObject">The JSON object containing the data to deserialize into the current object. Cannot be null.</param>
        /// <param name="serializationContext">The context that provides information and services for the deserialization process. Cannot be null.</param>
        /// <exception cref="NotImplementedException">The method is not implemented.</exception>
        public void Deserialize(JObject jObject, SerializationContext serializationContext)
        {
            if(this.IsInitialized)
            {
                throw new InvalidOperationException("Cannot deserialize into an already initialized material.");
            }

            this.Name = jObject["Name"].Value<string>();
            this.ID = Guid.Parse(jObject["ID"].Value<string>());
            this.Albedo = Utils.DeserializeVec3(jObject["Albedo"] as JObject);
            this.Metallic = jObject["Metallic"].Value<float>();
            this.Roughness = jObject["Roughness"].Value<float>();
            this.Occlusion = jObject["Occlusion"].Value<float>();
            this.AlbedoTexture = new Texture();
            this.AlbedoTexture.Deserialize(jObject["AlbedoTexture"] as JObject, serializationContext);
            this.NormalTexture = new Texture();
            this.NormalTexture.Deserialize(jObject["NormalTexture"] as JObject, serializationContext);
            this.MetallicTexture = new Texture();
            this.MetallicTexture.Deserialize(jObject["MetallicTexture"] as JObject, serializationContext);
            this.RoughnessTexture = new Texture();
            this.RoughnessTexture.Deserialize(jObject["RoughnessTexture"] as JObject, serializationContext);
            this.OcclusionTexture = new Texture();
            this.OcclusionTexture.Deserialize(jObject["OcclusionTexture"] as JObject, serializationContext);
        }
    }
}
