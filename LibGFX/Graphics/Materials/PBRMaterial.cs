using LibGFX.Core;
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
        public Guid ID { get; } = Guid.NewGuid();
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

        public static IMaterial LoadMaterial(Assimp.Material asmat, String directory)
        {
            throw new NotImplementedException();
        }
    }
}
