using LibGFX.Core;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Lights
{
    /// <summary>
    /// Base class for all light types.
    /// </summary>
    public abstract class Light : IIdentifier, ISerialization
    {
        /// <summary>
        /// Gets or sets the name associated with the object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the unique identifier for this instance.
        /// </summary>
        public Guid ID { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// The color of the light.
        /// </summary>
        public virtual Vector4 Color { get; set; }

        /// <summary>
        /// The position of the light.
        /// </summary>
        public virtual Vector3 Position { get; set; }

        /// <summary>
        /// The intensity of the light.
        /// </summary>
        public virtual float Intensity { get; set; }

        /// <summary>
        /// The range of the light.
        /// </summary>
        public DepthOnlyRenderTarget ShadowMap { get; set; }

        /// <summary>
        /// The size of the shadow map in pixels.
        /// </summary>
        public Vector2i ShadowMapSize { get; set; } = new Vector2i(2048);

        /// <summary>
        /// Indicates whether the light has a shadow map.
        /// </summary>
        public abstract bool HasShadowMap { get; }

        /// <summary>
        /// Initializes the light with the given renderer.
        /// </summary>
        /// <param name="renderer"></param>
        public abstract void Init(IRenderDevice renderer);

        /// <summary>
        /// Disposes the light resources associated with the renderer.
        /// </summary>
        /// <param name="renderer"></param>
        public abstract void Dispose(IRenderDevice renderer);

        /// <summary>
        /// Serializes the current object to a <see cref="JObject"/> using the specified serialization context.
        /// </summary>
        /// <param name="serializationContext">The context that provides information and settings required for serialization.</param>
        /// <returns>A <see cref="JObject"/> representing the serialized form of the current object.</returns>
        public virtual void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(this.GetType().FullName);
            writer.WritePropertyName("Name");
            writer.WriteValue(!String.IsNullOrEmpty(Name) ? Name : ID.ToString());
            writer.WritePropertyName("ID");
            writer.WriteValue(ID.ToString());
            writer.WritePropertyName("Color");
            Utils.SerializeVec4(Color, writer);
            writer.WritePropertyName("Position");
            Utils.SerializeVec3(Position, writer);
            writer.WritePropertyName("Intensity");
            writer.WriteValue(Intensity);
            writer.WritePropertyName("ShadowMapSize");
            Utils.SerializeVec2i(ShadowMapSize, writer);
            callback?.Invoke(writer);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Populates the current object with values from the specified JSON object using the provided serialization
        /// context.
        /// </summary>
        /// <param name="jObject">The JSON object containing the data to deserialize into the current instance. Cannot be null.</param>
        /// <param name="serializationContext">The context that provides information and services for the deserialization process.</param>
        public virtual void Deserialize(JObject jObject, SerializationContext serializationContext)
        {
            // Populate basic light properties
            Name = jObject.Value<string>("Name");
            ID = Guid.Parse(jObject.Value<string>("ID"));
            Color = Utils.DeserializeVec4(jObject.Value<JObject>("Color"));
            Position = Utils.DeserializeVec3(jObject.Value<JObject>("Position"));
            Intensity = jObject.Value<float>("Intensity");
            ShadowMapSize = Utils.DeserializeVec2i(jObject.Value<JObject>("ShadowMapSize"));

            // Register in context
            serializationContext.SetValue<Light>(ID.ToString(), this);
        }
    }
}
