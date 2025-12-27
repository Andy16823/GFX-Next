using LibGFX.Core;
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
    /// An directional light
    /// </summary>
    public class DirectionalLight2D : Light
    {
        /// <summary>
        /// Determines if the light has a shadow map.
        /// </summary>
        public override bool HasShadowMap => false;

        /// <summary>
        /// Creates a new instance of the <see cref="DirectionalLight2D"/> class.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="intensity"></param>
        public DirectionalLight2D(Vector4 color, float intensity)
        {
            Color = color;
            Intensity = intensity;
        }

        /// <summary>
        /// Releases all resources used by the object and performs any necessary cleanup using the specified render
        /// device.
        /// </summary>
        /// <param name="renderer">The render device to use for releasing graphics resources. Cannot be null.</param>
        public override void Dispose(IRenderDevice renderer)
        {

        }

        /// <summary>
        /// Initializes the object using the specified render device.
        /// </summary>
        /// <param name="renderer">The render device to use for initialization. Cannot be null.</param>
        public override void Init(IRenderDevice renderer)
        {

        }

        /// <summary>
        /// Serializes the current object to a JSON representation suitable for storage or transmission.
        /// </summary>
        /// <param name="serializationContext">The context that provides information and services required for serialization.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized state of the object, including type, color, position,
        /// intensity, and shadow map size.</returns>
        public override JObject Serialize(SerializationContext serializationContext)
        {
            return new JObject()
            {
                ["Type"] = this.GetType().FullName,
                ["Color"] = Utils.SerializeVec4(this.Color),
                ["Position"] = Utils.SerializeVec3(this.Position),
                ["Intensity"] = this.Intensity,
                ["ShadowMapSize"] = Utils.SerializeVec2i(this.ShadowMapSize),
            };
        }

        /// <summary>
        /// Populates the object's properties from the specified JSON object using the provided serialization context.
        /// </summary>
        /// <param name="jObject">A JSON object containing the data to deserialize. Must include the 'Color', 'Position', 'Intensity', and
        /// 'ShadowMapSize' properties.</param>
        /// <param name="serializationContext">The context to use during deserialization, providing additional information or services required for the
        /// process.</param>
        public override void Deserialize(JObject jObject, SerializationContext serializationContext)
        {
            this.Color = Utils.DeserializeVec4(jObject["Color"] as JObject);
            this.Position = Utils.DeserializeVec3(jObject["Position"] as JObject);
            this.Intensity = jObject["Intensity"]!.Value<float>();
            this.ShadowMapSize = Utils.DeserializeVec2i(jObject["ShadowMapSize"] as JObject);
        }
    }
}
