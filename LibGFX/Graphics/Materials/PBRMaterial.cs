using LibGFX.Core;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Materials
{
    public class PBRMaterial : IMaterial
    {
        public string Name { get; set; }
        public MaterialFlags Flags { get; set; }
        public Vector3 Albedo { get; set; } = new Vector3(1, 1, 0);
        public float Metallic { get; set; } = 1.0f;
        public float Roughness { get; set; } = 0.5f;
        public float Occlusion { get; set; } = 0f;
        public Texture AlbedoTexture { get; set; } = null;
        public Texture NormalTexture { get; set; } = null;
        public Texture MetallicTexture { get; set; } = null;
        public Texture RoughnessTexture { get; set; } = null;
        public Texture OcclusionTexture { get; set; } = null;

        public void Dispose(IRenderDevice renderDevice)
        {
            if(this.Flags == MaterialFlags.Disposed)
            {
                return;
            }

            if(this.AlbedoTexture != null)
            {
                renderDevice.DisposeTexture(this.AlbedoTexture);
            }

            if(this.NormalTexture != null)
            {
                renderDevice.DisposeTexture(this.NormalTexture);
            }

            if(this.MetallicTexture != null)
            {
                renderDevice.DisposeTexture(this.MetallicTexture);
            }

            if(this.RoughnessTexture != null)
            {
                renderDevice.DisposeTexture(this.RoughnessTexture);
            }

            if (this.OcclusionTexture != null)
            {
                renderDevice.DisposeTexture(this.OcclusionTexture);
            }

            this.Flags = MaterialFlags.Disposed;
        }

        public void Init(IRenderDevice renderDevice)
        {
            if (this.Flags == MaterialFlags.Loaded)
            {
                return;
            }

            if(this.AlbedoTexture != null)
            {
                renderDevice.LoadTexture(this.AlbedoTexture);
            }

            if(this.NormalTexture != null)
            {
                renderDevice.LoadTexture(this.NormalTexture);
            }

            if(this.MetallicTexture != null)
            {
                renderDevice.LoadTexture(this.MetallicTexture);
            }

            if(this.RoughnessTexture != null)
            {
                renderDevice.LoadTexture(this.RoughnessTexture);
            }

            if(this.OcclusionTexture != null)
            {
                renderDevice.LoadTexture(this.OcclusionTexture);
            }

            this.Flags = MaterialFlags.Loaded;
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
                Flags = MaterialFlags.None
            };
            return material;
        }
    }
}
