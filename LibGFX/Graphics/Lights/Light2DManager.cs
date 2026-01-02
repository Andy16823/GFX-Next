using Assimp;
using LibGFX.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Lights
{
    /// <summary>
    /// Represents an chunk of lights in the scene.
    /// </summary>
    public class Light2DChunk : ISerialization
    {
        /// <summary>
        /// Gets or sets the collection of 2D point lights used in the scene.
        /// </summary>
        public List<PointLight2D> Lights { get; set; }

        /// <summary>
        /// Initializes a new instance of the Light2DChunk class.
        /// </summary>
        /// <remarks>This constructor creates an empty collection of PointLight2D objects, ready to be
        /// populated after instantiation.</remarks>
        public Light2DChunk()
        {
            Lights = new List<PointLight2D>();
        }

        /// <summary>
        /// Serializes the current object and its child lights into a JSON representation.
        /// </summary>
        /// <param name="serializationContext">The context to use during serialization, which may provide settings or state required for the serialization
        /// process.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized data for this object, including its type information and
        /// an array of serialized child lights.</returns>
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
        /// Populates the collection of point lights from the specified JSON object using the provided serialization
        /// context.
        /// </summary>
        /// <remarks>This method clears the existing collection of point lights before adding the
        /// deserialized lights from the JSON object. Any existing lights will be removed.</remarks>
        /// <param name="jObject">A <see cref="JObject"/> containing the serialized data for the point lights. Must include a "Lights" array
        /// property.</param>
        /// <param name="serializationContext">The <see cref="SerializationContext"/> to use during deserialization. Provides context or settings required
        /// for the operation.</param>
        public void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            var lightsArray = obj["Lights"] as JArray;
            foreach (var lightToken in lightsArray)
            {
                var lightObject = lightToken as JObject;
                var light = new PointLight2D();
                light.Deserialize(lightObject, serializationContext);
                Lights.Add(light);
            }
        }
    }

    /// <summary>
    /// Manages 2D lights in the scene.
    /// </summary>
    public class Light2DManager : ILightManager
    {
        /// <summary>
        /// The directional light in the scene.
        /// </summary>
        public DirectionalLight2D DirectionalLight { get; set; }

        /// <summary>
        /// The dictionary of light chunks, where the key is a tuple of chunk coordinates (x, y).
        /// </summary>
        public Dictionary<(int, int), Light2DChunk> Chunks { get; set; } = new Dictionary<(int, int), Light2DChunk>();

        /// <summary>
        /// The shader storage buffer object (SSBO) for the lights.
        /// </summary>
        public int LightSSBO { get; set; }

        /// <summary>
        /// Gets or sets the size, in characters, of each chunk used during processing.
        /// </summary>
        /// <remarks>Adjust this value to control the maximum number of characters included in a single
        /// chunk. Larger chunk sizes may improve performance but can increase memory usage.</remarks>
        public float ChunkSize { get; set; } = 4000;

        /// <summary>
        /// Gets a value indicating whether the object has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Light2DManager"/> class.
        /// </summary>
        public Light2DManager()
        {

        }

        /// <summary>
        /// Adds a directional light to the scene.
        /// </summary>
        /// <param name="light"></param>
        public void AddPointLight(PointLight2D light)
        {
            var chunk = GetChunk(light.Position.X, light.Position.Y, this.ChunkSize);
            if (!Chunks.ContainsKey(chunk))
            {
                Chunks[chunk] = new Light2DChunk();
            }

            Chunks[chunk].Lights.Add(light);
        }

        /// <summary>
        /// Removes a point light from the scene.
        /// </summary>
        /// <param name="light"></param>
        public void RemovePointLight(PointLight2D light)
        {
            var chunk = GetChunk(light.Position.X, light.Position.Y, this.ChunkSize);
            if (Chunks.ContainsKey(chunk))
            {
                Chunks[chunk].Lights.Remove(light);
                Debug.WriteLine($"Removed light from chunk {chunk} at position {light.Position}");
            }
        }

        /// <summary>
        /// Finds nearby chunks based on the given coordinates and chunk size.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public IEnumerable<(int, int)> FindNearbyChunks(float x, float y, float chunkSize)
        {
            var chunk = GetChunk(x, y, chunkSize);

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    yield return (chunk.Item1 + i, chunk.Item2 + j);
                }
            }
        }

        /// <summary>
        /// Gets the lights in the nearby chunks based on the given coordinates and chunk size.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public IEnumerable<Point2DLightData> CullChunkLights(Camera camera, float chunkSize)
        {
            var cullRadius = camera.Transform.Scale.X / 2.0f;

            var nearbyChunks = FindNearbyChunks(camera.Transform.Position.X, camera.Transform.Position.Y, chunkSize);
            //Debug.WriteLine($"Nearby chunks: {nearbyChunks.Count()}");

            var culledLights = new List<Point2DLightData>();
            Parallel.ForEach(nearbyChunks, chunk =>
            {
                if (Chunks.TryGetValue(chunk, out var chunkLights))
                {
                    foreach (var light in chunkLights.Lights)
                    {
                        if (Vector2.DistanceSquared(camera.Transform.Position.Xy, light.Position.Xy) < cullRadius * cullRadius)
                        {
                            lock (culledLights)
                            {
                                culledLights.Add(light.ToStruct());
                            }
                        }
                    }
                }
            });

            //Debug.WriteLine($"Culled lights: {culledLights.Count}");
            return culledLights;
        }

        /// <summary>
        /// Initializes the light manager with the given render device.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Init(IRenderDevice renderDevice)
        {
            this.LightSSBO = renderDevice.CreateBuffer();
            this.ForEachLight(light =>
            {
                light.Init(renderDevice);
            });
            this.IsInitialized = true;
        }

        /// <summary>
        /// Frees any CPU resources used by the light manager.
        /// </summary>
        public void FreeCPUResources()
        {
            // No CPU resources to free in the Light2DManager
        }

        /// <summary>
        /// Culls the lights based on the camera's view and the viewport.
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public void CullLights(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            var newBufferData = CullChunkLights(camera, this.ChunkSize).ToArray();
            renderer.SetVertexBufferData<Point2DLightData>(LightSSBO, newBufferData, true);
        }

        /// <summary>
        /// Binds the lights to the shader program for rendering.
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public void BindLights(Viewport viewport, IRenderDevice renderer, Camera camera)
        {       
            if (this.DirectionalLight == null)
            {
                return;
            }
            
            renderer.PrepareShader("dirLightColor", DirectionalLight.Color.Xyz);
            renderer.PrepareShader("dirLightIntensity", DirectionalLight.Intensity);
            renderer.BindShaderStorageBuffer(4, this.LightSSBO);
        }

        /// <summary>
        /// Disposes of the light manager and releases any resources.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Dispose(IRenderDevice renderDevice)
        {
            renderDevice.DisposeBuffer(this.LightSSBO);
            this.LightSSBO = 0;
            this.DisposeLights(renderDevice);
            this.IsInitialized = false;
        }

        /// <summary>
        /// Gets the light count of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public int GetLightCount<T>() where T : Light
        {
            return 0;
        }

        /// <summary>
        /// Gets the chunk coordinates based on the given x and y coordinates and chunk size.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public (int, int) GetChunk(float x, float y, float chunkSize)
        {
            int chunkX = (int)MathF.Floor(x / chunkSize);
            int chunkY = (int)MathF.Floor(y / chunkSize);
            return (chunkX, chunkY);
        }

        /// <summary>
        /// Gets the total light count across all types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T GetLight<T>() where T : Light
        {
            if (typeof(T) == typeof(DirectionalLight2D))
            {
                return (T)(object) DirectionalLight;
            }
            throw new InvalidOperationException($"Light type {typeof(T).Name} not supported in Light2DManager.");
        }

        /// <summary>
        /// Sets the light view matrix for the light manager, which is used to transform the light's perspective in the scene.
        /// Note: This method is not implemented for 2D lights as they do not have a light space matrix like 3D lights.
        /// </summary>
        /// <param name="lightViewMatrix"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetLightSpaceMatrix(Matrix4 lightViewMatrix)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs an action on each light of the specified type in the scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void ForEachLight<T>(Action<T> action) where T : Light
        {
            if (typeof(T) == typeof(PointLight2D))
            {
                foreach (var chunk in Chunks.Values)
                {
                    foreach (var light in chunk.Lights.OfType<T>())
                    {
                        action(light);
                    }
                }
            }
            else if (typeof(T) == typeof(DirectionalLight2D))
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
        /// Releases all light resources associated with the current instance using the specified render device.
        /// </summary>
        /// <remarks>Call this method to ensure that all light resources are properly released before
        /// disposing of the parent object or when lights are no longer needed. After calling this method, the
        /// collection of lights will be cleared.</remarks>
        /// <param name="renderDevice">The render device to use when disposing of light resources. Cannot be null.</param>
        public void DisposeLights(IRenderDevice renderDevice)
        {
            if(this.DirectionalLight != null)
            {
                this.DirectionalLight.Dispose(renderDevice);
            }

            foreach (var chunk in Chunks.Values)
            {
                foreach (var light in chunk.Lights)
                {
                    light.Dispose(renderDevice);
                }
            }
            this.ClearLights();
        }

        /// <summary>
        /// Removes all lights from the scene, including the directional light and any additional light sources.
        /// </summary>
        /// <remarks>After calling this method, the scene will contain no active lights. Use this method
        /// to reset the lighting configuration before adding new lights or reconfiguring the scene's
        /// illumination.</remarks>
        public void ClearLights()
        {
            this.DirectionalLight = null;
            this.Chunks.Clear();
        }

        /// <summary>
        /// Determines whether the specified light is contained within this collection.
        /// </summary>
        /// <remarks>This method checks for both directional and point lights. For directional lights, it
        /// compares with the collection's directional light. For point lights, it searches all chunks in the
        /// collection.</remarks>
        /// <param name="light">The light to locate in the collection. Can be a directional or point light.</param>
        /// <returns>true if the specified light is present in the collection; otherwise, false.</returns>
        public bool ContainsLight(Light light)
        {
            // Check for directional light
            if (light is DirectionalLight2D dirLight)
            {
                return this.DirectionalLight == dirLight;
            }
            // Check for point lights
            else if (light is PointLight2D pointLight)
            {
                foreach (var chunk in Chunks.Values)
                {
                    if (chunk.Lights.Contains(pointLight))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

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
                writer.WritePropertyName("LightChunk");
                kvp.Value.Serialize(writer, serializationContext);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            callback?.Invoke(writer);
            writer.WriteEndObject();
        }

        public void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            var directionalLightToken = obj["DirectionalLight"];
            this.DirectionalLight = new DirectionalLight2D();
            this.DirectionalLight.Deserialize(directionalLightToken as JObject, serializationContext);

            var chunksArray = obj["Chunks"] as JArray;
            Chunks.Clear();
            foreach (var chunkToken in chunksArray)
            {
                var chunkObject = chunkToken as JObject;
                int chunkX = chunkObject["ChunkX"] != null ? chunkObject["ChunkX"].Value<int>() : 0;
                int chunkY = chunkObject["ChunkY"] != null ? chunkObject["ChunkY"].Value<int>() : 0;

                var lightChunkToken = chunkObject["LightChunk"];
                var lightChunk = new Light2DChunk();
                lightChunk.Deserialize(lightChunkToken as JObject, serializationContext);
                Chunks[(chunkX, chunkY)] = lightChunk;
            }
            callback?.Invoke(obj);
        }
    }
}
