using LibGFX.Core;
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
    /// Represents a chunk of lights in the scene.
    /// </summary>
    public class Light3DChunk : ISerialization
    {
        /// <summary>
        /// Gets or sets the collection of point lights used in the 3D scene.
        /// </summary>
        public List<PointLight3D> Lights { get; set; }

        /// <summary>
        /// Initializes a new instance of the Light3DChunk class with an empty collection of point lights.
        /// </summary>
        public Light3DChunk()
        {
            Lights = new List<PointLight3D>();
        }

        /// <summary>
        /// Serializes the current object and its contained lights to a JSON representation.
        /// </summary>
        /// <remarks>The returned JSON object includes a "Type" property with the fully qualified type
        /// name and a "Lights" property containing the serialized representations of all contained lights. Each light
        /// is serialized using its own Serialize method.</remarks>
        /// <param name="serializationContext">The context to use during serialization, which may provide settings or state required for serializing the
        /// object and its lights.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized representation of the object, including its type
        /// information and an array of serialized lights.</returns>
        public void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(this.GetType().FullName);
            writer.WritePropertyName("Lights");
            writer.WriteStartArray();
            foreach (var light in Lights)
            {
                light.Serialize(writer, serializationContext);
            }
            writer.WriteEndArray();
            callback?.Invoke(writer);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Populates the collection of lights by deserializing point light data from the specified JSON object.
        /// </summary>
        /// <remarks>Each element in the 'Lights' array of the JSON object is expected to represent a
        /// point light. Existing lights in the collection are not cleared before new lights are added.</remarks>
        /// <param name="jObject">A JSON object containing a 'Lights' array with point light definitions to deserialize.</param>
        /// <param name="serializationContext">The context to use during deserialization, providing additional information or services required for the
        /// process.</param>
        public void Deserialize(JsonReader reader, SerializationContext serializationContext, Func<JsonReader, string, bool> callback = null)
        {
            if(reader.TokenType != JsonToken.StartObject)
                throw new JsonException("Expected StartObject token.");

            while (reader.Read()) 
            {
                if (reader.TokenType == JsonToken.EndObject)
                    break;

                if (reader.TokenType == JsonToken.PropertyName)
                {
                    var propertyName = (string)reader.Value;
                    reader.Read(); // Move to property value

                    switch (propertyName)
                    {
                        case "Type":
                            reader.Skip();
                            break;
                        case "Lights":
                            if (reader.TokenType != JsonToken.StartArray)
                                throw new JsonException("Expected StartArray token for 'Lights'.");

                            Lights.Clear();
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.EndArray)
                                    break;

                                if(reader.TokenType == JsonToken.StartObject)
                                {
                                    var light = new PointLight3D();
                                    light.Deserialize(reader, serializationContext);
                                    Lights.Add(light);
                                }
                            }
                            break;
                        default:
                            if (callback != null && callback(reader, propertyName))
                            {
                                break;
                            }
                            reader.Skip();
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Manages 3D lights in the scene.
    /// </summary>
    public class Light3DManager : ILightManager
    {
        /// <summary>
        /// The directional light in the scene.
        /// </summary>
        public DirectionalLight3D DirectionalLight { get; set; }

        /// <summary>
        /// The dictionary of light chunks, where the key is a tuple of chunk coordinates (x, y, z).
        /// </summary>
        public Dictionary<(int, int, int), Light3DChunk> Chunks { get; set; }

        /// <summary>
        /// The chunk size for the lights. This is used to determine the size of each chunk in the scene.
        /// </summary>
        public float ChunkSize { get; internal set; } = 100f;

        /// <summary>
        /// Gets a value indicating whether the object has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;

        // the point light SSBO
        private int _pointLightsSSBO;

        // the shadow map ID
        private int _shadowMapId;

        /// <summary>
        /// The light view matrix used for shadow mapping. 
        /// </summary>
        private Matrix4 _lightViewMatrix;

        /// <summary>
        /// Initializes a new instance of the <see cref="Light3DManager"/> class.
        /// </summary>
        public Light3DManager()
        {
            Chunks = new Dictionary<(int, int, int), Light3DChunk>();
        }

        /// <summary>
        /// Adds an point light to the light manager.
        /// </summary>
        /// <param name="light"></param>
        public void AddPointLight(PointLight3D light)
        {
            var chunk = this.GetChunk(light.Position.X, light.Position.Y, light.Position.Z, this.ChunkSize);
            if (!Chunks.ContainsKey(chunk))
            {
                Chunks[chunk] = new Light3DChunk();
            }
            Chunks[chunk].Lights.Add(light);
        }

        /// <summary>
        /// Binds the lights to the shader.
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public void BindLights(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            // Bind the shadow data
            renderer.PrepareShader("shadowMap", 6, this.DirectionalLight.ShadowMap.DepthTextureId);
            renderer.PrepareShader("lightSpaceMatrix", true, _lightViewMatrix);

            // Bind the lightning data
            renderer.PrepareShader("dirLight.direction", DirectionalLight.Direction);
            renderer.PrepareShader("dirLight.lightColor", DirectionalLight.Color.Xyz);
            renderer.PrepareShader("dirLight.lightIntensity", DirectionalLight.Intensity);
            renderer.PrepareShader("dirLight.ambient", DirectionalLight.Ambient);
            renderer.PrepareShader("dirLight.specular", DirectionalLight.Specular);
            renderer.BindShaderStorageBuffer(4, _pointLightsSSBO);
        }

        /// <summary>
        /// Culls the lights
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public void CullLights(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            var chunk = this.GetChunk(camera.Transform.Position.X, camera.Transform.Position.Y, camera.Transform.Position.Z, 10f);
            var culledLights = this.CullChunkLights(camera, viewport, this.ChunkSize);
            renderer.SetVertexBufferData<PointLight3DData>(_pointLightsSSBO, culledLights.ToArray(), true);

            //Debug.WriteLine($"Culled {culledLights.Count()} lights from chunk {chunk} for camera at position {camera.Transform.Position}.");
        }

        /// <summary>
        /// Disposes of the light manager and releases any resources.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Dispose(IRenderDevice renderDevice)
        {
            renderDevice.DisposeBuffer(_pointLightsSSBO);
            _pointLightsSSBO = 0;
            this.DisposeLights(renderDevice);
            this.IsInitialized = false;
        }

        /// <summary>
        /// Gets the light count of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int GetLightCount<T>() where T : Light
        {
            return this.GetTotalLightCount();
        }

        /// <summary>
        /// Initializes the light manager with the given render device.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Init(IRenderDevice renderDevice)
        {
            _pointLightsSSBO = renderDevice.CreateBuffer();
            this.ForEachLight(light =>
            {
                light.Init(renderDevice);
            });
            this.IsInitialized = true;
        }

        /// <summary>
        /// Gets the chunk coordinates based on the given x, y, and z coordinates and chunk size.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        private (int, int, int) GetChunk(float x, float y, float z, float chunkSize)
        {
            int chunkX = (int)MathF.Floor(x / chunkSize);
            int chunkY = (int)MathF.Floor(y / chunkSize);
            int chunkZ = (int)MathF.Floor(z / chunkSize);
            return (chunkX, chunkY, chunkZ);
        }

        /// <summary>
        /// Finds the nearby chunks based on the given x, y, z coordinates and chunk size.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        private IEnumerable<(int, int, int)> FindNearbyChunks(float x, float y, float z, float chunkSize)
        {
            var chunk = this.GetChunk(x, y, z, chunkSize);

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    for (int k = -1; k <= 1; k++)
                    {
                        yield return (chunk.Item1 + i, chunk.Item2 + j, chunk.Item3 + k);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the total light count in the scene.
        /// </summary>
        /// <returns></returns>
        private int GetTotalLightCount()
        {
            int count = 0;
            foreach (var chunk in Chunks)
            {
                count += chunk.Value.Lights.Count;
            }
            return count;
        }

        /// <summary>
        /// Culls the lights in the nearby chunks based on the camera's view and the viewport.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="viewport"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        private IEnumerable<PointLight3DData> CullChunkLights(Camera camera, Viewport viewport, float chunkSize)
        {
            var nearbyChunkks = this.FindNearbyChunks(camera.Transform.Position.X, camera.Transform.Position.Y, camera.Transform.Position.Z, chunkSize);
            var culledLights = new List<PointLight3DData>();
            foreach (var chunk in nearbyChunkks)
            {
                if (Chunks.ContainsKey(chunk))
                {
                    foreach (var light in Chunks[chunk].Lights)
                    {
                        var lightAABB = light.GetAABB();
                        if (camera.IsAABBInFrustum(viewport, lightAABB.min, lightAABB.max))
                        {
                            culledLights.Add(light.ToStruct());
                        }
                    }
                }
            }
            return culledLights;
        }

        /// <summary>
        /// Gets all the lights in the scene.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PointLight3D> GetAllLights()
        {
            foreach (var chunk in Chunks)
            {
                foreach (var light in chunk.Value.Lights)
                {
                    yield return light;
                }
            }
        }

        /// <summary>
        /// Resizes the chunk size and re-adds all the lights to the new chunks.
        /// </summary>
        /// <param name="newSize"></param>
        public void ResizeChunkSize(float newSize)
        {
            var lights = this.GetAllLights().ToList();

            this.ChunkSize = newSize;
            this.Chunks.Clear();
            foreach (var light in lights)
            {
                this.AddPointLight(light);
            }
        }

        /// <summary>
        /// Gets the light of the specified type from the light manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T GetLight<T>() where T : Light
        {
            if (typeof(T) == typeof(DirectionalLight3D))
            {
                return (T)(object)DirectionalLight;
            }
            throw new InvalidOperationException($"Light type {typeof(T).Name} not supported in Light3DManager.");
        }

        /// <summary>
        /// Sets the light space matrix for the light manager, which is used to transform the light's perspective in the scene.
        /// </summary>
        /// <param name="lightViewMatrix"></param>
        public void SetLightSpaceMatrix(Matrix4 lightViewMatrix)
        {
            _lightViewMatrix = lightViewMatrix;
        }

        /// <summary>
        /// Performs an action on each light of the specified type in the scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void ForEachLight<T>(Action<T> action) where T : Light
        {
            if (typeof(T) == typeof(PointLight3D))
            {
                foreach (var chunk in Chunks.Values)
                {
                    foreach (var light in chunk.Lights.OfType<T>())
                    {
                        action(light);
                    }
                }
            }
            else if (typeof(T) == typeof(DirectionalLight3D))
            {
                action((T)(object)DirectionalLight);
            }
            else
            {
                throw new InvalidOperationException($"Light type {typeof(T).Name} not supported in Light2DManager.");
            }
        }

        /// <summary>
        /// Performs an action on each light in the scene, regardless of type.
        /// </summary>
        /// <param name="action"></param>
        public void ForEachLight(Action<Light> action)
        {
            if(this.DirectionalLight != null)
            {
                action(DirectionalLight);
            }

            foreach (var chunk in Chunks.Values)
            {
                foreach (var light in chunk.Lights)
                {
                    action(light);
                }
            }
        }

        /// <summary>
        /// Serializes the current object and its associated data into a JSON representation.
        /// </summary>
        /// <param name="serializationContext">The context to use during serialization, which may provide settings or state required for the serialization
        /// process.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized representation of the object, including its type,
        /// directional light, and chunk data.</returns>
        public void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(this.GetType().FullName);
            writer.WritePropertyName("DirectionalLight");
            if (DirectionalLight != null)
            {
                DirectionalLight.Serialize(writer, serializationContext);
            }
            else
            {
                writer.WriteNull();
            }
            writer.WritePropertyName("Chunks");
            writer.WriteStartArray();
            foreach (var kvp in Chunks)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("ChunkX");
                writer.WriteValue(kvp.Key.Item1);
                writer.WritePropertyName("ChunkY");
                writer.WriteValue(kvp.Key.Item2);
                writer.WritePropertyName("ChunkZ");
                writer.WriteValue(kvp.Key.Item3);
                writer.WritePropertyName("LightChunk");
                kvp.Value.Serialize(writer, serializationContext);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            callback?.Invoke(writer);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Populates the current object with values from the specified JSON object using the provided serialization
        /// context.
        /// </summary>
        /// <param name="jObject">The JSON object containing the data to deserialize into the current instance. Cannot be null.</param>
        /// <param name="serializationContext">The context that provides information and services for the deserialization process. Cannot be null.</param>
        /// <exception cref="NotImplementedException">The method is not implemented.</exception>
        public void Deserialize(JsonReader reader, SerializationContext serializationContext, Func<JsonReader, string, bool> callback = null)
        {
            if(reader.TokenType != JsonToken.StartObject)
                throw new JsonException("Expected StartObject token.");

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                    break;

                if (reader.TokenType == JsonToken.PropertyName)
                {
                    var propertyName = (string)reader.Value;
                    reader.Read(); // Move to property value

                    switch (propertyName)
                    {
                        case "Type":
                            reader.Skip();
                            break;
                        case "DirectionalLight":
                            if (reader.TokenType == JsonToken.StartObject)
                            {
                                var directionalLight = new DirectionalLight3D();
                                directionalLight.Deserialize(reader, serializationContext);
                                DirectionalLight = directionalLight;
                            }
                            else
                            {
                                DirectionalLight = null;
                            }
                            break;
                        case "Chunks":
                            if (reader.TokenType != JsonToken.StartArray)
                                throw new JsonException("Expected StartArray token for 'Chunks'.");

                            Chunks.Clear();
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.EndArray)
                                    break;

                                if (reader.TokenType == JsonToken.StartObject)
                                {
                                    int chunkX = 0, chunkY = 0, chunkZ = 0;
                                    Light3DChunk lightChunk = null;
                                    while (reader.Read())
                                    {
                                        if (reader.TokenType == JsonToken.EndObject)
                                            break;

                                        if (reader.TokenType == JsonToken.PropertyName)
                                        {
                                            var chunkPropertyName = (string)reader.Value;
                                            reader.Read();
                                            switch (chunkPropertyName)
                                            {
                                                case "ChunkX":
                                                    chunkX = reader.Value != null ? Convert.ToInt32(reader.Value) : 0;
                                                    break;
                                                case "ChunkY":
                                                    chunkY = reader.Value != null ? Convert.ToInt32(reader.Value) : 0;
                                                    break;
                                                case "ChunkZ":
                                                    chunkZ = reader.Value != null ? Convert.ToInt32(reader.Value) : 0;
                                                    break;
                                                case "LightChunk":
                                                    if (reader.TokenType == JsonToken.StartObject)
                                                    {
                                                        lightChunk = new Light3DChunk();
                                                        lightChunk.Deserialize(reader, serializationContext);
                                                    }
                                                    break;
                                                default:
                                                    reader.Skip();
                                                    break;
                                            }
                                        }
                                    }
                                    if (lightChunk != null)
                                    {
                                        Chunks[(chunkX, chunkY, chunkZ)] = lightChunk;
                                    }
                                }
                            }
                            break;
                        default:
                            if (callback != null && callback(reader, propertyName))
                            {
                                break;
                            }
                            reader.Skip();
                            break;
                    }
                }
            }
        }

        public void DisposeLights(IRenderDevice renderDevice)
        {
            // Dispose directional light
            if (this.DirectionalLight != null)
            {
                this.DirectionalLight.Dispose(renderDevice);
            }

            // Dispose chunk lights
            foreach (var chunk in Chunks.Values)
            {
                foreach(var light in chunk.Lights)
                {
                    light.Dispose(renderDevice);
                }
            }

            this.ClearLights();
        }

        /// <summary>
        /// Removes all light sources from the current scene, including directional and chunk-based lights.
        /// </summary>
        /// <remarks>After calling this method, the scene will contain no active lights. Use this method
        /// to reset lighting before configuring new light sources.</remarks>
        public void ClearLights()
        {
            this.Chunks.Clear();
            this.DirectionalLight = null;
        }

        /// <summary>
        /// Determines whether the specified light is contained within this collection.
        /// </summary>
        /// <remarks>This method checks for both directional and point lights. For directional lights, it
        /// compares with the collection's directional light. For point lights, it searches all chunks in the
        /// collection.</remarks>
        /// <param name="light">The light to locate in the collection. This can be a directional or point light.</param>
        /// <returns>true if the specified light is present in the collection; otherwise, false.</returns>
        public bool ContainsLight(Light light)
        {
            // Check directional light
            if (light is DirectionalLight3D dirLight)
            {
                return this.DirectionalLight == dirLight;
            }
            // Check point lights in chunks
            else if (light is PointLight3D pointLight)
            {
                foreach(var chunk in Chunks.Values)
                {
                    if(chunk.Lights.Contains(pointLight))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
