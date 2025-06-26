using Assimp;
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
    public class Light3DChunk
    {
        public List<PointLight3D> Lights { get; set; }

        public Light3DChunk()
        {
            Lights = new List<PointLight3D>();
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
            renderer.BindBufferData<PointLight3DData>(_pointLightsSSBO, culledLights.ToArray(), true);
            //Debug.WriteLine($"Culled lights: {culledLights.Count()}");
        }

        /// <summary>
        /// Disposes of the light manager and releases any resources.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Dispose(IRenderDevice renderDevice)
        {
            renderDevice.DisposeBuffer(_pointLightsSSBO);
            _pointLightsSSBO = 0;
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
            _pointLightsSSBO = renderDevice.CreateEmptyBuffer();
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

        public T GetLight<T>() where T : Light
        {
            if (typeof(T) == typeof(DirectionalLight3D))
            {
                return (T)(object)DirectionalLight;
            }
            throw new InvalidOperationException($"Light type {typeof(T).Name} not supported in Light3DManager.");
        }

        public void SetShadowMap(RenderTarget shadowmap)
        {
            _shadowMapId = shadowmap.TextureID;
        }

        public void BindShadowMap(IRenderDevice renderDevice, String location, int textureSlot)
        {
            renderDevice.PrepareShader(location, textureSlot, _shadowMapId);
            renderDevice.PrepareShader("lightSpaceMatrix", true, _lightViewMatrix);
        }

        public void SetLightSpaceMatrix(Matrix4 lightViewMatrix)
        {
            _lightViewMatrix = lightViewMatrix;
        }
    }
}
