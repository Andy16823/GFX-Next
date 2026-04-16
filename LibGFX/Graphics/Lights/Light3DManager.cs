using LibGFX.Core;
using LibGFX.Graphics.Renderer.OpenGL;
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
        /// Tries to remove the specified light from the chunk.
        /// </summary>
        /// <param name="light"></param>
        public bool TryRemoveLight(PointLight3D light)
        {
            if(Lights.Contains(light))
            {
                this.Lights.Remove(light);
                return true;
            }
            return false;
        }

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

        public void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            var lightsArray = obj["Lights"] as JArray;
            Lights.Clear();
            foreach (var lightToken in lightsArray)
            {
                var lightObj = lightToken as JObject;
                var light = new PointLight3D();
                light.Deserialize(lightObj, serializationContext);
                Lights.Add(light);
            }
            callback?.Invoke(obj);
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

        // the light view matrix for shadow mapping
        private List<Matrix4> _lightViewMatrix = new List<Matrix4>();

        // the buffer for the light space matrices
        private int _lightMatrixBuffer;

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
            // Bind the shadow data TODO: Implement shadow mapping for directional light
            var csm = this.DirectionalLight.ShadowMap as CascadedShadowMap;
            if (csm == null)
            {
                throw new Exception("Directional light shadow map is not a CascadedShadowMap. Shadow mapping for directional lights requires a CascadedShadowMap.");
            }
            renderer.PrepareShaderArrayTexture("shadowMap", 6, csm.TextureId);
            renderer.PrepareShader("cascadeCount", csm.CascadeCount);
            renderer.PrepareShader("lightSpaceMatrices", true, _lightViewMatrix.ToArray());
            float[] cascadeLevels = new float[]
            {
                10.0f,
                30.0f,
                100.0f,
                camera.Far
            };
            renderer.PrepareShader("cascadePlaneDistances", cascadeLevels.Length, cascadeLevels);
            renderer.PrepareShader("cameraFar", camera.Far);



            //renderer.PrepareShader("shadowMap", 6, this.DirectionalLight.ShadowMap.DepthTextureId);
            //renderer.PrepareShader("lightSpaceMatrix", true, _lightViewMatrix);

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
            renderDevice.DisposeBuffer(_lightMatrixBuffer);
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

            _lightMatrixBuffer = renderDevice.CreateBuffer();
            var matrixSize = 16 * sizeof(float);
            renderDevice.SetBufferSize(_lightMatrixBuffer, 16 * matrixSize, RenderFlags.GFXBufferTarget.UniformBuffer, RenderFlags.GFXBufferUsageHint.DynamicDraw);
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
        public void ComputeLightSpaceMatrix(Camera camera, Viewport viewport)
        {
            // Clear the previous light view matrix
            _lightViewMatrix.Clear();

            // Get the ligt direction from the directional light
            if (this.DirectionalLight == null)
            {
                Debug.WriteLine("No directional light found in Light3DManager. Cannot compute light space matrix.");
                return;
            }
            var lightDir = this.DirectionalLight.Direction.Normalized();

            // Compute the light space matrix for each cascade level
            var perspectiveCamera = camera as PerspectiveCamera;
            if (perspectiveCamera == null)
            {
                Debug.WriteLine("Camera is not a PerspectiveCamera. Cannot compute light space matrix.");
                return;
            }
            
            var (near, far) = perspectiveCamera.GetNearFar();
            float[] cascadeLevels = new float[]
            {
                10.0f,
                30.0f,
                100.0f,
                perspectiveCamera.Far
            };

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

        public void BindLightSpaceMatrix(IRenderDevice renderDevice, int binding = 0)
        {
            renderDevice.BindBuffer(RenderFlags.GFXBufferTarget.UniformBuffer, _lightMatrixBuffer);
            renderDevice.UpdateBufferData(_lightMatrixBuffer, _lightViewMatrix.ToArray(), 0, RenderFlags.GFXBufferTarget.UniformBuffer);
            renderDevice.BindBufferBase(RenderFlags.GFXBufferTarget.UniformBuffer, binding, _lightMatrixBuffer);
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

        public void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            // Deserialize directional light
            var directionalLightToken = obj["DirectionalLight"] as JObject;
            if (directionalLightToken != null)
            {
                var directionalLight = new DirectionalLight3D();
                directionalLight.Deserialize(directionalLightToken, serializationContext);
                DirectionalLight = directionalLight;
            }
            else
            {
                DirectionalLight = null;
            }

            // Deserialize chunks
            var chunksArray = obj["Chunks"] as JArray;
            Chunks.Clear();
            foreach (var chunkToken in chunksArray)
            {
                var chunkObj = chunkToken as JObject;
                int chunkX = chunkObj["ChunkX"] != null ? chunkObj["ChunkX"].Value<int>() : 0;
                int chunkY = chunkObj["ChunkY"] != null ? chunkObj["ChunkY"].Value<int>() : 0;
                int chunkZ = chunkObj["ChunkZ"] != null ? chunkObj["ChunkZ"].Value<int>() : 0;
                var lightChunkToken = chunkObj["LightChunk"] as JObject;
                if (lightChunkToken != null)
                {
                    var lightChunk = new Light3DChunk();
                    lightChunk.Deserialize(lightChunkToken, serializationContext);
                    Chunks[(chunkX, chunkY, chunkZ)] = lightChunk;
                }
            }
            callback?.Invoke(obj);
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

        /// <summary>
        /// Removes the specified light from the light manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="light"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void RemoveLight<T>(T light) where T : Light
        {
            if(light is DirectionalLight3D dirLight)
            {
                if(this.DirectionalLight == dirLight)
                {
                    this.DirectionalLight = null;
                    return;
                }
            }
            else if(light is PointLight3D pointLight)
            {
                foreach(var chunk in Chunks.Values)
                {
                    if(chunk.TryRemoveLight(pointLight))
                    {
                        return;
                    }
                }
                throw new InvalidOperationException("The specified point light was not found in any chunk.");
            }
            throw new InvalidOperationException($"Light type {typeof(T).Name} not supported in Light3DManager.");
        }
    }
}
