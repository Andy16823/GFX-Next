using Assimp;
using LibGFX.Core;
using LibGFX.Graphics.Renderer.OpenGL;
using LibGFX.Graphics.RenderTarget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

        /// <summary>0 = normal rendering, 1 = cascade index, 2 = projected light coordinates, 3 = fragment and sampled depths.</summary>
        public int ShadowDebugMode { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether the object casts shadows in the scene.
        /// </summary>
        public override bool CastsShadows { get; set; } = true;

        private List<Matrix4> _lightViewMatrix = new List<Matrix4>();
        private int _lightMatrixBuffer = -1;

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
            ShadowMapSize = new Vector2i(8192, 8192);
        }

        /// <summary>
        /// Initializes the shadow map resources for the directional light using the specified render device.
        /// </summary>
        /// <param name="renderer">The render device used to create the shadow map render target. Cannot be null.</param>
        public override void Init(IRenderDevice renderer)
        {
            // Create a shadow map render target for the directional light
            var csm = new CascadedShadowMap(this.ShadowMapSize.X, this.ShadowMapSize.Y);
            this.ShadowMap = csm;
            renderer.CreateRenderTarget(this.ShadowMap as CascadedShadowMap);

            // Create a uniform buffer to hold the light space matrices for each cascade level
            _lightMatrixBuffer = renderer.CreateBuffer();
            var matrixSize = csm.GetCascadeCount() * Marshal.SizeOf<Matrix4>();
            renderer.SetBufferSize(_lightMatrixBuffer, matrixSize, GFXBufferTarget.UniformBuffer, GFXBufferUsageHint.DynamicDraw);
        }

        /// <summary>
        /// Computes the light space matrix for the directional light based on the provided camera and viewport. This matrix is used to transform the scene from world space to light space for shadow mapping.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="viewport"></param>
        public void ComputeLightSpaceMatrix(Camera camera, Viewport viewport)
        {
            // Clear the previous light view matrix
            _lightViewMatrix.Clear();

            var csm = this.ShadowMap as CascadedShadowMap;
            if (csm == null)
            {
                Debug.WriteLine("Directional light shadow map is not a CascadedShadowMap. Cannot compute light space matrix.");
                return;
            }

            var lightDir = this.Direction.Normalized();

            // Compute the light space matrix for each cascade level
            var perspectiveCamera = camera as PerspectiveCamera;
            if (perspectiveCamera == null)
            {
                Debug.WriteLine("Camera is not a PerspectiveCamera. Cannot compute light space matrix.");
                return;
            }

            var (near, far) = perspectiveCamera.GetNearFar();
            var cascadeLevels = csm.GetCascadeLevels(perspectiveCamera);

            float lastSplitDist = perspectiveCamera.Near;
            foreach (var cascadeLevel in cascadeLevels)
            {
                perspectiveCamera.SetNearFar(lastSplitDist, cascadeLevel);
                var mat = Utils.ComputeLightViewProjectionMatrix(perspectiveCamera, viewport, lightDir);
                _lightViewMatrix.Add(mat);
                lastSplitDist = cascadeLevel;
            }
            perspectiveCamera.SetNearFar(near, far);
        }

        public void Bind(IRenderDevice renderer, Viewport viewport, Camera camera)
        {
            var csm = this.ShadowMap as CascadedShadowMap;

            // Bind the shadow map texture and light space matrices for each cascade level
            renderer.PrepareShaderArrayTexture("shadowMap", 6, csm!.TextureId);
            renderer.PrepareShader("cascadeCount", csm.GetCascadeCount());
            renderer.PrepareShader("lightSpaceMatrices", true, _lightViewMatrix.ToArray());

            var cascadeLevels = csm.GetCascadeLevels(camera);
            renderer.PrepareShader("cascadePlaneDistances", cascadeLevels.Length, cascadeLevels);
            renderer.PrepareShader("cameraFar", camera.Far);
            renderer.PrepareShader("castsShadows", this.CastsShadows ? 1 : 0);
            renderer.PrepareShader("shadowDebugMode", this.ShadowDebugMode);

            // Bind the lightning data
            renderer.PrepareShader("dirLight.direction", this.Direction);
            renderer.PrepareShader("dirLight.lightColor", this.Color.Xyz);
            renderer.PrepareShader("dirLight.lightIntensity", this.Intensity);
            renderer.PrepareShader("dirLight.ambient", this.Ambient);
            renderer.PrepareShader("dirLight.specular", this.Specular);
        }

        /// <summary>
        /// Binds the computed light space matrix to the shader program for rendering shadows. This method should be called after ComputeLightSpaceMatrix to ensure that the latest light space matrix is used in the shader.
        /// </summary>
        /// <param name="renderDevice"></param>
        /// <param name="binding"></param>
        public void BindLightSpaceMatrix(IRenderDevice renderDevice, int binding = 0)
        {
            renderDevice.BindBuffer(GFXBufferTarget.UniformBuffer, _lightMatrixBuffer);
            renderDevice.UpdateBufferData(_lightMatrixBuffer, _lightViewMatrix.ToArray(), 0, GFXBufferTarget.UniformBuffer);
            renderDevice.BindBufferBase(GFXBufferTarget.UniformBuffer, binding, _lightMatrixBuffer);
        }

        /// <summary>
        /// Releases all resources used by the shadow map associated with the directional light.
        /// </summary>
        /// <param name="renderer">The rendering device to use when disposing of resources. Cannot be null.</param>
        public override void Dispose(IRenderDevice renderer)
        {
            Debug.WriteLine($"Disposing Shadow Map for Directional Light: {this.GetType().Name} at {Position}");
            renderer.DisposeRenderTarget(this.ShadowMap as CascadedShadowMap);
            renderer.DisposeBuffer(_lightMatrixBuffer);
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
