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

namespace LibGFX.Graphics.Lights
{
    /// <summary>
    /// An directional light in 3D space.
    /// TODO: Create an uniform for the bias
    /// </summary>
    public class DirectionalLight3D : Light
    {
        /// <summary>
        /// The direction of the light.
        /// </summary>
        public Vector3 Direction { get; set; }

        /// <summary>
        /// The ambient color of the light.
        /// </summary>
        public Vector3 Ambient { get; set; }

        /// <summary>
        /// The specular color of the light.
        /// </summary>
        public Vector3 Specular { get; set; }

        /// <summary>
        /// The Bias of the light shadow map.
        /// </summary>
        public float Bias { get; set; } = 0.005f;

        /// <summary>
        /// Gets a value indicating whether this object supports a shadow map.
        /// </summary>
        public override bool HasShadowMap => true;

        /// <summary>
        /// Initializes a new instance of the DirectionalLight3D class.
        /// </summary>
        public DirectionalLight3D()
        {
            
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DirectionalLight3D"/> class.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="color"></param>
        /// <param name="intensity"></param>
        public DirectionalLight3D(Vector3 direction, Vector4 color, float intensity)
        {
            Position = Vector3.PositiveInfinity;
            Direction = direction;
            Color = color;
            Intensity = intensity;
            Ambient = new Vector3(0.2f, 0.2f, 0.2f);
            Specular = new Vector3(1.0f, 1.0f, 1.0f);
        }

        /// <summary>
        /// Initializes the shadow map resources for the directional light using the specified render device.
        /// </summary>
        /// <param name="renderer">The render device used to create the shadow map render target. Cannot be null.</param>
        public override void Init(IRenderDevice renderer)
        {
            Debug.WriteLine($"Creating Shadow Map for Directional Light: {this.GetType().Name} at {Position} with size {ShadowMapSize}");
            this.ShadowMap = renderer.CreateDepthRenderTarget2D(ShadowMapSize.X, ShadowMapSize.Y);
        }

        /// <summary>
        /// Releases all resources used by the shadow map associated with the directional light.
        /// </summary>
        /// <param name="renderer">The rendering device to use when disposing of resources. Cannot be null.</param>
        public override void Dispose(IRenderDevice renderer)
        {
            Debug.WriteLine($"Disposing Shadow Map for Directional Light: {this.GetType().Name} at {Position}");
            this.ShadowMap.Dispose(renderer);
        }

        /// <summary>
        /// Serializes the current light object to a JSON representation suitable for storage or transmission.
        /// </summary>
        /// <remarks>The returned JSON object includes type information and key light properties such as
        /// color, position, intensity, shadow map size, direction, ambient, specular, and bias. This method is
        /// typically used to persist or transfer light configuration data.</remarks>
        /// <param name="serializationContext">The context that provides information and services required for serialization.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized properties of the light object.</returns>
        public override void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            base.Serialize(writer, serializationContext, (w) =>
            {
                w.WritePropertyName("Direction");
                Utils.SerializeVec3(this.Direction, w);
                w.WritePropertyName("Ambient");
                Utils.SerializeVec3(this.Ambient, w);
                w.WritePropertyName("Specular");
                Utils.SerializeVec3(this.Specular, w);
                w.WritePropertyName("Bias");
                w.WriteValue(this.Bias);
                callback?.Invoke(w);
            });
        }

        /// <summary>
        /// Populates the properties of the current instance from the specified JSON object using the provided
        /// serialization context.
        /// </summary>
        /// <remarks>This method updates the state of the current object based on the values found in
        /// <paramref name="jObject"/>. All expected properties must be present in the JSON object for correct
        /// deserialization.</remarks>
        /// <param name="jObject">A <see cref="JObject"/> containing the serialized data to deserialize. Must not be null and should include
        /// all required properties.</param>
        /// <param name="serializationContext">A <see cref="SerializationContext"/> that provides context or settings for the deserialization process.</param>
        public override void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            base.Deserialize(obj, serializationContext, (refObj) =>
            {
                this.Direction = Utils.DeserializeVec3(refObj["Direction"] as JObject);
                this.Ambient = Utils.DeserializeVec3(refObj["Ambient"] as JObject);
                this.Specular = Utils.DeserializeVec3(refObj["Specular"] as JObject);
                this.Bias = Convert.ToSingle(refObj["Bias"].Value<float>());
                callback?.Invoke(refObj);
                return true;
            });
        }
    }
}
