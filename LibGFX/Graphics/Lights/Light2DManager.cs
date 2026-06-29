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
    public class Light2DChunk
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
        /// Removes a point light from the chunk.
        /// </summary>
        /// <param name="light"></param>
        public bool TryRemoveLight(PointLight2D light)
        {
            if (Lights.Contains(light))
            {
                Lights.Remove(light);
                return true;
            }
            return false;
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
            if (this.DirectionalLight == null) return;
            
            renderer.PrepareShader("dirLightColor", DirectionalLight.Color.Xyz);
            renderer.PrepareShader("dirLightIntensity", DirectionalLight.Intensity);
            renderer.BindBufferBase(RenderFlags.GFXBufferTarget.ShaderStorageBuffer, 4, this.LightSSBO);
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
        public void ComputeLightSpaceMatrix(Camera camera, Viewport viewport)
        {
            throw new NotImplementedException();
        }

        public void BindLightSpaceMatrix(IRenderDevice renderer, int binding = 0)
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

        /// <summary>
        /// Removes the specified light from the scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="light"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void RemoveLight<T>(T light) where T : Light
        {
            if (light is DirectionalLight2D dirLight)
            {
                if (this.DirectionalLight == dirLight)
                {
                    this.DirectionalLight = null;
                    return;
                }
            }
            else if (light is PointLight2D pointLight)
            {
                foreach (var chunk in Chunks.Values)
                {
                    if (chunk.TryRemoveLight(pointLight))
                    {
                        return;
                    }
                }
                throw new InvalidOperationException("The specified PointLight2D was not found in any chunk.");
            }
            throw new InvalidOperationException($"Light type {typeof(T).Name} not supported in Light2DManager.");
        }

        /// <summary>
        /// Serializes the light manager to a JSON object representation, including its directional light and point lights.
        /// </summary>
        /// <param name="writer">The JSON writer to use for serialization.</param>
        /// <param name="serializationContext">The context for serialization, providing additional information or settings.</param>
        /// <param name="callback">An optional callback to invoke after serialization is complete.</param>
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
            writer.WritePropertyName("Lights");
            writer.WriteStartArray();
            this.ForEachLight((light) =>
            {
                light.Serialize(writer, serializationContext);
            });
            writer.WriteEndArray();
            callback?.Invoke(writer);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Deserializes the light manager from a JSON object representation, restoring its state and contained lights.
        /// </summary>
        /// <param name="obj">The JSON object to deserialize from.</param>
        /// <param name="serializationContext">The context for serialization, providing additional information or settings.</param>
        /// <param name="callback">An optional callback to invoke after deserialization is complete.</param>
        public void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            var directionalLightToken = obj["DirectionalLight"];
            this.DirectionalLight = new DirectionalLight2D();
            this.DirectionalLight.Deserialize(directionalLightToken as JObject, serializationContext);

            var lightsArray = obj["Lights"] as JArray;
            foreach(var lightToken in lightsArray)
            {
                var lightObject = lightToken as JObject;
                if (lightObject != null)
                {
                    var pointLight2D = new PointLight2D();
                    pointLight2D.Deserialize(lightObject, serializationContext);
                    this.AddPointLight(pointLight2D);
                }
            }

            callback?.Invoke(obj);
        }
    }
}
