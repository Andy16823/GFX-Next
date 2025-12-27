using LibGFX.Core;
using Newtonsoft.Json.Linq;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Lights
{
    /// <summary>
    /// Represents the data structure for a 2D point light for the shader.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Point2DLightData
    {
        public Vector4 Position;
        public Vector4 Color;
        public Vector4 RadiusIntensity;
    }

    /// <summary>
    /// Represents a 2D point light in the scene.
    /// </summary>
    public class PointLight2D : Light
    {
        /// <summary>
        /// The Radius of the light.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Determines if the light has a shadow map.
        /// </summary>
        public override bool HasShadowMap => false;

        /// <summary>
        /// Creates a new instance of the <see cref="PointLight2D"/> class.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <param name="radius"></param>
        /// <param name="intensity"></param>
        public PointLight2D(Vector2 position, Vector3 color, float radius, float intensity)
        {
            Position = new Vector3(position.X, position.Y, 1);
            Color = new Vector4(color.X, color.Y, color.Z, 1);
            Intensity = intensity;
            Radius = radius;
        }

        /// <summary>
        /// Converts the light data to a structure for use in the shader.
        /// </summary>
        /// <returns></returns>
        public Point2DLightData ToStruct()
        {
            return new Point2DLightData()
            {
                Position = new Vector4(Position),
                Color = Color,
                RadiusIntensity = new Vector4(Radius, Intensity, 0.0f, 0.0f)
            };
        }

        /// <summary>
        /// Initializes the object using the specified render device.
        /// </summary>
        /// <param name="renderer">The render device to use for initialization. Cannot be null.</param>
        public override void Init(IRenderDevice renderer)
        {
            
        }

        /// <summary>
        /// Releases all resources used by the object and performs any necessary cleanup using the specified render
        /// device.
        /// </summary>
        /// <remarks>Call this method when the object is no longer needed to free associated resources.
        /// After calling this method, the object should not be used.</remarks>
        /// <param name="renderer">The render device to use for releasing graphics resources. Cannot be null.</param>
        public override void Dispose(IRenderDevice renderer)
        {
            
        }

        /// <summary>
        /// Serializes the current light object to a JSON representation suitable for storage or transmission.
        /// </summary>
        /// <param name="serializationContext">The context that provides information and services required for serialization.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized properties of the light object.</returns>
        public override JObject Serialize(SerializationContext serializationContext)
        {
            return new JObject()
            {
                ["Type"] = this.GetType().FullName,
                ["Color"] = Utils.SerializeVec4(this.Color),
                ["Position"] = Utils.SerializeVec3(this.Position),
                ["Intensity"] = this.Intensity,
                ["ShadowMapSize"] = Utils.SerializeVec2i(this.ShadowMapSize),
                ["Radius"] = this.Radius
            };
        }

        /// <summary>
        /// Populates the object's properties from the specified JSON object using the provided serialization context.
        /// </summary>
        /// <param name="jObject">A <see cref="JObject"/> containing the JSON data to deserialize. Must not be null and should include the
        /// required properties for this object.</param>
        /// <param name="serializationContext">A <see cref="SerializationContext"/> that provides context or settings for the deserialization process.</param>
        public override void Deserialize(JObject jObject, SerializationContext serializationContext)
        {
            this.Color = Utils.DeserializeVec4(jObject["Color"] as JObject);
            this.Position = Utils.DeserializeVec3(jObject["Position"] as JObject);
            this.Intensity = jObject["Intensity"]!.Value<float>();
            this.ShadowMapSize = Utils.DeserializeVec2i(jObject["ShadowMapSize"] as JObject);
            this.Radius = jObject["Radius"]!.Value<float>();
        }
    }
}
