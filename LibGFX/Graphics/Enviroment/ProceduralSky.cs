using LibGFX.Core;
using LibGFX.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Enviroment
{
    /// <summary>
    /// Represents a procedural sky
    /// </summary>
    public class ProceduralSky : IEnviroment
    {
        /// <summary>
        /// The transform of the sky
        /// </summary>
        public Transform Transform { get; set ; }

        /// <summary>
        /// The top color of the sky
        /// </summary>
        public Vector3 SkyTopColor { get; set; } = new Vector3(0.2f, 0.5f, 0.9f);

        /// <summary>
        /// The bottom color of the sky
        /// </summary>
        public Vector3 SkyBottomColor { get; set; } = new Vector3(1.0f, 0.6f, 0.3f);

        /// <summary>
        /// The direction of the sun
        /// </summary>
        public Vector3 SunDirection { get; set; } = new Vector3(0.0f, 1.0f, 0.0f);

        /// <summary>
        /// The color of the sun
        /// </summary>
        public Vector3 SunColor { get; set; } = new Vector3(1.0f, 0.9f, 0.7f);

        /// <summary>
        /// The size of the sun
        /// </summary>
        public float SunSize { get; set; } = 800.0f;

        /// <summary>
        /// The intensity of the sun
        /// </summary>
        public float SunIntensity { get; set; } = 1.5f;

        /// <summary>
        /// The offset of the skyline
        /// </summary>
        public float SkylineOffset { get; set; } = 0.0f;

        /// <summary>
        /// The scale of the skyline
        /// </summary>
        public float SkylineScale { get; set; } = 1.0f;

        /// <summary>
        /// The coverage of the clouds
        /// </summary>
        public bool Coverage { get; set; } = false;

        /// <summary>
        /// The texture used for the cloud coverage
        /// </summary>
        public Texture CoverageTexture { get; set; }

        /// <summary>
        /// The color of the clouds
        /// </summary>
        public Vector3 CloudColor { get; set; } = new Vector3(0.8f, 0.8f, 0.8f);

        /// <summary>
        /// The coverage factor for the clouds
        /// </summary>
        public float CoverageFactor { get; set; } = 1.0f;

        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Creates a new instance of the ProceduralSky class
        /// </summary>
        public ProceduralSky()
        {
            this.Transform = new Transform();
            this.Transform.Scale = Vector3.One;
        }

        /// <summary>
        /// Disposes the procedural sky
        /// </summary>
        /// <param name="renderer"></param>
        public void Dispose(IRenderDevice renderer)
        {
            IsInitialized = false;
        }

        /// <summary>
        /// Initializes the procedural sky
        /// </summary>
        /// <param name="renderer"></param>
        /// <exception cref="Exception"></exception>
        public void Init(IRenderDevice renderer)
        {
            IsInitialized = true;
        }

        /// <summary>
        /// Renders the procedural sky
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        /// <param name="viewport"></param>
        public void Render(IRenderDevice renderer, Camera camera, Viewport viewport)
        {
            this.Transform.Position = camera.Transform.Position;
            var shader = renderer.GetShaderProgram("ProceduralSkyShader");
            var projectionMatrix = renderer.GetProjectionMatrix();
            var viewMatrix = renderer.GetViewMatrix();

            renderer.BindShaderProgram(shader);
            renderer.PrepareShader("skyTopColor", SkyTopColor);
            renderer.PrepareShader("skyBottomColor", SkyBottomColor);
            renderer.PrepareShader("sunDirection", SunDirection);
            renderer.PrepareShader("sunColor", SunColor);
            renderer.PrepareShader("sunSize", SunSize);
            renderer.PrepareShader("sunIntensity", SunIntensity);
            renderer.PrepareShader("skylineOffset", SkylineOffset);
            renderer.PrepareShader("skylineScale", SkylineScale);
            renderer.PrepareShader("coverage", Coverage);
            if(CoverageTexture != null)
            {
                renderer.PrepareShader("coverageTexture", OpenTK.Graphics.OpenGL4.TextureUnit.Texture0, CoverageTexture);
            }
            renderer.PrepareShader("coverageFactor", CoverageFactor);
            renderer.PrepareShader("cloudColor", CloudColor);
            renderer.PrepareShader("p_mat", true, projectionMatrix);
            renderer.PrepareShader("v_mat", true, viewMatrix);
            renderer.PrepareShader("m_mat", true, Transform.GetMatrix());
            renderer.SetDepthMask(false);
            renderer.DrawShape(renderer.GetShape("CubeShape"));
            renderer.SetDepthMask(true);
            renderer.UnbindShaderProgram();
        }

        /// <summary>
        /// Serializes the current object to a JSON representation suitable for storage or transmission.
        /// </summary>
        /// <param name="serializationContext">The context that provides serialization settings and state information used during the serialization
        /// process.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized representation of the object, including its type and
        /// relevant properties.</returns>
        public void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(this.GetType().FullName);
            writer.WritePropertyName("Transform");
            Transform.Serialize(writer, serializationContext);
            writer.WritePropertyName("SkyTopColor");
            Utils.SerializeVec3(SkyTopColor, writer);
            writer.WritePropertyName("SkyBottomColor");
            Utils.SerializeVec3(SkyBottomColor, writer);
            writer.WritePropertyName("SunDirection");
            Utils.SerializeVec3(SunDirection, writer);
            writer.WritePropertyName("SunColor");
            Utils.SerializeVec3(SunColor, writer);
            writer.WritePropertyName("SunSize");
            writer.WriteValue(SunSize);
            writer.WritePropertyName("SunIntensity");
            writer.WriteValue(SunIntensity);
            writer.WritePropertyName("SkylineOffset");
            writer.WriteValue(SkylineOffset);
            writer.WritePropertyName("SkylineScale");
            writer.WriteValue(SkylineScale);
            writer.WritePropertyName("Coverage");
            writer.WriteValue(Coverage);
            writer.WritePropertyName("CoverageTexture");
            if(CoverageTexture != null)
            {
                CoverageTexture.Serialize(writer, serializationContext);
            }
            else
            {
                writer.WriteNull();
            }
            writer.WritePropertyName("CoverageFactor");
            writer.WriteValue(CoverageFactor);
            writer.WritePropertyName("CloudColor");
            Utils.SerializeVec3(CloudColor, writer);
            callback?.Invoke(writer);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Populates the object's properties from the specified JSON data using the provided serialization context.
        /// </summary>
        /// <remarks>This method expects the JSON object to contain all necessary fields for successful
        /// deserialization. Missing or invalid fields may result in incomplete or incorrect object state.</remarks>
        /// <param name="jObject">A <see cref="JObject"/> containing the JSON data to deserialize. Must not be null and should include all
        /// required fields for this object.</param>
        /// <param name="serializationContext">The <see cref="SerializationContext"/> to use during deserialization. Provides context or settings that may
        /// influence how the data is interpreted.</param>
        public void Deserialize(JObject jObject, SerializationContext serializationContext)
        {
            Transform.Deserialize(jObject["Transform"] as JObject, serializationContext);
            SkyTopColor = Utils.DeserializeVec3(jObject["SkyTopColor"] as JObject);
            SkyBottomColor = Utils.DeserializeVec3(jObject["SkyBottomColor"] as JObject);
            SunDirection = Utils.DeserializeVec3(jObject["SunDirection"] as JObject);
            SunColor = Utils.DeserializeVec3(jObject["SunColor"] as JObject);
            SunSize = jObject["SunSize"].Value<float>();
            SunIntensity = jObject["SunIntensity"].Value<float>();
            SkylineOffset = jObject["SkylineOffset"].Value<float>();
            SkylineScale = jObject["SkylineScale"].Value<float>();
            Coverage = jObject["Coverage"].Value<bool>();
            if (jObject["CoverageTexture"] != null && jObject["CoverageTexture"].Type != JTokenType.Null)
            {
                CoverageTexture = new Texture();
                CoverageTexture.Deserialize(jObject["CoverageTexture"] as JObject, serializationContext);
            }
            CoverageFactor = jObject["CoverageFactor"].Value<float>();
            CloudColor = Utils.DeserializeVec3(jObject["CloudColor"] as JObject);
        }
    }
}
