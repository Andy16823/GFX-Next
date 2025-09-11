using Assimp;
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
        public List<PointLight2D> Lights { get; set; }

        public Light2DChunk()
        {
            Lights = new List<PointLight2D>();
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

        public float ChunkSize { get; set; } = 4000;

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
                Debug.WriteLine("No directional light found");
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
            this.Chunks.Clear();
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
    }
}
