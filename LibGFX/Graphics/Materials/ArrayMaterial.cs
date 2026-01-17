using Assimp;
using LibGFX.Core;
using LibGFX.Graphics.Shader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Materials
{
    /// <summary>
    /// A material that uses texture arrays for layered materials.
    /// Used for terrain rendering and other layered materials like in instanced rendering.
    /// </summary>
    public class ArrayMaterial : IMaterial
    {
        /// <summary>
        /// The name of the material.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unique ID of the material.
        /// </summary>
        public Guid ID { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Determines if the material has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Determines if the material has transparency.
        /// </summary>
        public bool IsTransparent => HasTransparency();

        /// <summary>
        /// The shader used by the material.
        /// </summary>
        public RenderShader Shader { get; set; }

        /// <summary>
        /// The size of each texture in the array.
        /// Default is 1024x1024.
        /// </summary>
        public Vector2i TextureSize { get; set; } = new Vector2i(1024, 1024);

        /// <summary>
        /// The number of mip levels for the textures.
        /// Default is 5.
        /// </summary>
        public int MipLevels { get; set; } = 5;

        /// <summary>
        /// The number of layers in the texture array.
        /// </summary>
        public int LayerCount => _albedoTextures.Count;

        // Albedo textures
        private int _albedoTextureId = -1;
        private List<Texture> _albedoTextures = new List<Texture>();

        // Normal textures
        private int _normalTextureId = -1;
        private List<Texture> _normalTextures = new List<Texture>();

        // Specular textures
        private int _specularTextureId = -1;
        private List<Texture> _specularTexture = new List<Texture>();

        /// <summary>
        /// Checks if any of the albedo textures have transparency.
        /// </summary>
        /// <returns></returns>
        private bool HasTransparency()
        {
            // Check if any albedo texture has transparency
            foreach(var tex in _albedoTextures)
            {
                if(tex.HasAlpha)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a new layer to the ArrayMaterial.
        /// </summary>
        /// <param name="albedo"></param>
        /// <param name="normal"></param>
        /// <param name="specular"></param>
        /// <exception cref="ArgumentException"></exception>
        public int AddLayer(Texture albedo, Texture normal, Texture specular)
        {
            if(albedo.Width != TextureSize.X || albedo.Height != TextureSize.Y ||
               normal.Width != TextureSize.X || normal.Height != TextureSize.Y ||
               specular.Width != TextureSize.X || specular.Height != TextureSize.Y)
            {
                throw new ArgumentException("All textures must match the defined TextureSize of the ArrayMaterial.");
            }

            _albedoTextures.Add(albedo);
            _normalTextures.Add(normal);
            _specularTexture.Add(specular);

            return _albedoTextures.Count - 1;
        }

        /// <summary>
        /// Uses the material for rendering.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Use(IRenderDevice renderDevice)
        {
            // Ensure material is initialized
            if (!this.IsInitialized)
                throw new InvalidOperationException("Material must be initialized before use.");

            // Bind shader and set textures
            renderDevice.BindShaderProgram(this.Shader);
            renderDevice.PrepareShader("albedoMap", 0, _albedoTextureId);
            renderDevice.PrepareShader("normalMap", 1, _normalTextureId);
            renderDevice.PrepareShader("specularMap", 2, _specularTextureId);
        }

        /// <summary>
        /// Disables the material after rendering.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Disable(IRenderDevice renderDevice)
        {
            renderDevice.UnbindShaderProgram();
        }

        /// <summary>
        /// Disposes of the material resources.
        /// </summary>
        /// <param name="renderer"></param>
        public void Dispose(IRenderDevice renderer)
        {
            if(!this.IsInitialized)
                return;

            if(_albedoTextureId != -1)
            {
                renderer.DisposeTexture(_albedoTextureId);
                _albedoTextureId = -1;
            }

            if(_normalTextureId != -1)
            {
                renderer.DisposeTexture(_normalTextureId);
                _normalTextureId = -1;
            }

            if(_specularTextureId != -1)
            {
                renderer.DisposeTexture(_specularTextureId);
                _specularTextureId = -1;
            }
        }

        /// <summary>
        /// Frees CPU resources used by the material.
        /// Not used in this implementation yet since we need to keep textures for re-initialization.
        /// </summary>
        public void FreeCPUResources()
        {
            // Not used in this implementation yet
        }

        /// <summary>
        /// Initializes the material with the given render device.
        /// </summary>
        /// <param name="renderer"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Init(IRenderDevice renderer)
        {
            // Ensure all texture layers have the same number of textures
            if (_normalTextures.Count != _albedoTextures.Count || _specularTexture.Count != _albedoTextures.Count)
            {
                throw new InvalidOperationException("All texture layers must have the same number of textures.");
            }

            // Create array textures for each texture type
            _albedoTextureId = renderer.CreateArrayTexture(TextureSize.X, TextureSize.Y, _albedoTextures.Count, this.MipLevels);
            _normalTextureId = renderer.CreateArrayTexture(TextureSize.X, TextureSize.Y, _normalTextures.Count, this.MipLevels);
            _specularTextureId = renderer.CreateArrayTexture(TextureSize.X, TextureSize.Y, _specularTexture.Count, this.MipLevels);

            // Upload texture data for each layer
            for (int i = 0; i < _albedoTextures.Count; i++)
            {
                renderer.SetArrayTextureData(_albedoTextureId, i, 0, _albedoTextures[i]);
                renderer.SetArrayTextureData(_normalTextureId, i, 0, _normalTextures[i]);
                renderer.SetArrayTextureData(_specularTextureId, i, 0, _specularTexture[i]);
            }

            // Set texture parameters
            TextureParameters textureParams = TextureParameters.Default;
            renderer.SetArrayTextureParameters(_albedoTextureId, textureParams);
            renderer.SetArrayTextureParameters(_normalTextureId, textureParams);
            renderer.SetArrayTextureParameters(_specularTextureId, textureParams);

            // Set initialized flag
            this.IsInitialized = true;
        }

        /// <summary>
        /// Loads material data from an Assimp material.
        /// </summary>
        /// <param name="asmat"></param>
        /// <param name="directory"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void LoadMaterial(Material asmat, string directory)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes the material to JSON.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="serializationContext"></param>
        /// <param name="callback"></param>
        public void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {

        }

        /// <summary>
        /// Deserializes the material from JSON.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializationContext"></param>
        /// <param name="callback"></param>
        public void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            
        }
    }
}
