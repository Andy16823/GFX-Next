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
    /// </summary>
    public class SpriteMaterial : IMaterial
    {
        /// <summary>
        /// The name of the material.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unique identifier of the material.
        /// </summary>
        public Guid ID { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// The texture of the material.
        /// </summary>
        public Texture Texture { get; set; }

        /// <summary>
        /// Gets a value indicating whether the object has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Default constructor for the SpriteMaterial class.
        /// </summary>
        public SpriteMaterial()
        {
            
        }

        /// <summary>
        /// Creates a new SpriteMaterial with the specified texture and shader.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="shader"></param>
        public SpriteMaterial(Texture texture)
        {
            this.Texture = texture;
        }

        /// <summary>
        /// Disposes the material and releases any resources used by it.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Dispose(IRenderDevice renderDevice)
        {
            Debug.WriteLine($"Disposing material {Name}");
            renderDevice.DisposeTexture(Texture);
            IsInitialized = false;
            Debug.WriteLine($"Disposed material {Name}");
        }

        /// <summary>
        /// Initializes the material and loads its resources.
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

            if(Texture == null)
            {
                throw new InvalidOperationException("Cannot initialize SpriteMaterial without a texture.");
            }
            
            this.Texture.TextureParameters = TextureParameters.PixelPerfect;
            this.Texture.Init(renderDevice);

            IsInitialized = true;
            Debug.WriteLine($"Loaded material {Name}");
        }

        /// <summary>
        /// Sets the material as the current material for rendering. No-op for SpriteMaterial.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Use(IRenderDevice renderDevice)
        {
            renderDevice.PrepareShader("textureSampler", OpenTK.Graphics.OpenGL4.TextureUnit.Texture0, Texture);
        }


        /// <summary>
        /// Throws an exception to indicate that loading materials from Assimp materials is not supported for
        /// SpriteMaterials.
        /// </summary>
        /// <param name="asmat">The Assimp material to attempt to load. This parameter is not supported and will always cause the method to
        /// throw.</param>
        /// <param name="directory">The directory path associated with the material. This parameter is not used.</param>
        /// <returns>This method does not return a value. It always throws a NotSupportedException.</returns>
        /// <exception cref="NotSupportedException">Thrown in all cases to indicate that loading SpriteMaterials from Assimp materials is not supported.</exception>
        public void LoadMaterial(Assimp.Material asmat, String directory)
        {
            throw new NotSupportedException("SpriteMaterials cannot be loaded from Assimp materials.");
        }

        /// <summary>
        /// Serializes the current object to a <see cref="JObject"/> representation suitable for JSON output.
        /// </summary>
        /// <param name="context">The serialization context that provides settings and state for the serialization process.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized data of the current object, including its name, ID, and
        /// texture information.</returns>
        public JObject Serialize(SerializationContext context)
        {
            JObject jObject = new JObject
            {
                ["Type"] = this.GetType().FullName,
                ["Name"] = Name,
                ["ID"] = ID.ToString(),
                ["Texture"] = Texture != null ? Texture.Serialize(context) : null
            };
            return jObject;
        }

        /// <summary>
        /// Populates the properties of the current instance from the specified JSON object using the provided
        /// serialization context.
        /// </summary>
        /// <param name="jObject">The JSON object containing the data to deserialize. Must not be null.</param>
        /// <param name="context">The serialization context to use during deserialization. Provides additional information or services
        /// required for the operation.</param>
        public void Deserialize(JObject jObject, SerializationContext context)
        {
            if (this.IsInitialized)
            {
                throw new InvalidOperationException("Cannot deserialize into an already initialized material.");
            }

            Name = jObject.Value<string>("Name") ?? "Unnamed SpriteMaterial";
            ID = Guid.Parse(jObject.Value<string>("ID") ?? Guid.NewGuid().ToString());
            JObject? textureObj = jObject.Value<JObject>("Texture");
            if (textureObj != null)
            {
                Texture = new Texture();
                Texture.Deserialize(textureObj, context);
            }
        }
    }
}
