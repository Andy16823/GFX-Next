using Assimp;
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
    /// Represents a skybox environment that renders a surrounding cubemap in a 3D scene.
    /// </summary>
    /// <remarks>A skybox is typically used to simulate distant backgrounds such as skies or space by
    /// rendering a textured cube around the camera. The Skybox class manages the cubemap texture and its
    /// transformation, and provides methods to initialize, render, and dispose of the skybox resources. The skybox
    /// automatically follows the camera's position to maintain the illusion of an infinitely distant
    /// environment.</remarks>
    public class Skybox : IEnviroment
    {
        /// <summary>
        /// Gets or sets the transformation applied to the object, including position, rotation, and scale.
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// Gets or sets the cubemap texture used for rendering or environment mapping.
        /// </summary>
        public Cubemap Cubemap { get; set; }

        /// <summary>
        /// Gets a value indicating whether the object has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Initializes a new instance of the Skybox class using the specified cubemap texture.
        /// </summary>
        /// <param name="cubemap">The cubemap texture to use for rendering the skybox. Cannot be null.</param>
        public Skybox(Cubemap cubemap)
        {
            this.Transform = new Transform();
            this.Transform.Scale = Vector3.One;
            this.Cubemap = cubemap;
        }

        /// <summary>
        /// Initializes the cubemap resources using the specified render device.
        /// </summary>
        /// <param name="renderer">The render device to use for initializing cubemap resources. Cannot be null.</param>
        public void Init(IRenderDevice renderer)
        {
            this.IsInitialized = true;
            Cubemap.Init(renderer);
        }

        /// <summary>
        /// Renders the environment cubemap using the specified renderer, camera, and viewport.
        /// </summary>
        /// <param name="renderer">The rendering device used to draw the cubemap. Must not be null.</param>
        /// <param name="camera">The camera that defines the viewpoint and position for rendering. Must not be null.</param>
        /// <param name="viewport">The viewport that specifies the area of the render target to draw to.</param>
        public void Render(IRenderDevice renderer, Camera camera, Viewport viewport)
        {
            this.Transform.Position = camera.Transform.Position;
            renderer.BindShaderProgram(renderer.GetRenderShader("EnviromentShader"));
            renderer.DrawCubemap(this.Transform, this.Cubemap, Vector4.Zero);
            renderer.UnbindShaderProgram();
        }

        /// <summary>
        /// Releases all resources used by the cubemap using the specified render device.
        /// </summary>
        /// <param name="renderer">The render device to use when disposing of the cubemap resources. Cannot be null.</param>
        public void Dispose(IRenderDevice renderer)
        {
            this.IsInitialized = false;
            Cubemap.Dispose(renderer);
        }

        /// <summary>
        /// Serializes the current object to a JSON representation using the specified serialization context.
        /// </summary>
        /// <param name="serializationContext">The context to use during serialization, which may provide settings or state required for the serialization
        /// process.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized representation of the object, including its type, cubemap,
        /// and transform information.</returns>
        public void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(this.GetType().FullName);
            writer.WritePropertyName("Cubemap");
            this.Cubemap.Serialize(writer, serializationContext);
            writer.WritePropertyName("Transform");
            this.Transform.Serialize(writer, serializationContext);
            callback?.Invoke(writer);
            writer.WriteEndObject();
        }

        public void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            if(this.IsInitialized)
            {
                throw new InvalidOperationException("Cannot deserialize an initialized Skybox. Dispose it first.");
            }

            var qubemapToken = obj.Value<JObject>("Cubemap");
            if(qubemapToken != null) {
                this.Cubemap = new Cubemap();
                this.Cubemap.Deserialize(qubemapToken, serializationContext);
            }

            var transformToken = obj.Value<JObject>("Transform");
            if(transformToken != null) {
                this.Transform = new Transform();
                this.Transform.Deserialize(transformToken, serializationContext);
            }

            callback?.Invoke(obj);
        }
    }
}
