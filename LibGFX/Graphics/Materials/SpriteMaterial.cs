using LibGFX.Core;
using LibGFX.Graphics.Shader;
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
        /// Gets a value indicating whether the object is transparent.
        /// TODO: If this get changed it need to get an setter and also update serialization for this property in all materials.
        /// </summary>
        public bool IsTransparent => this.Texture.HasAlpha;

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
        /// Disables the specified render device, releasing any resources or state associated with it.
        /// </summary>
        /// <param name="renderDevice">The render device to disable. Cannot be null.</param>
        public void Disable(IRenderDevice renderDevice)
        {
            
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
        public void Serialize(JsonWriter writer, SerializationContext context, Action<JsonWriter> callback = null)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(this.GetType().FullName);
            writer.WritePropertyName("Name");
            writer.WriteValue(Name);
            writer.WritePropertyName("ID");
            writer.WriteValue(ID.ToString());
            writer.WritePropertyName("Texture");
            if (Texture != null)
            {
                Texture.Serialize(writer, context);
            }
            else
            {
                writer.WriteNull();
            }
            callback?.Invoke(writer);
            writer.WriteEndObject();
        }

        public void Deserialize(JObject obj, SerializationContext context, Func<JObject, bool> callback = null)
        {
            // Ensure the object is not already initialized
            if (this.IsInitialized)
            {
                throw new InvalidOperationException("Cannot deserialize into an already initialized material.");
            }

            // Basic property deserialization
            this.Name = obj.Value<string>("Name");
            this.ID = Guid.Parse(obj.Value<string>("ID"));

            // Deserialize Texture
            var textureToken = obj.Value<JObject>("Texture");
            if (textureToken != null)
            {
                this.Texture = new Texture();
                this.Texture.Deserialize(textureToken, context);
            }
            else
            {
                this.Texture = null;
            }

            // Invoke the callback if provided
            callback?.Invoke(obj);
        }
    }
}
