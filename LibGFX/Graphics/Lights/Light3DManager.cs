using Assimp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Lights
{
    public class Light3DChunk
    {
        public List<PointLight3D> Lights { get; set; }

        public Light3DChunk()
        {
            Lights = new List<PointLight3D>();
        }
    }

    public class Light3DManager : ILightManager
    {
        public DirectionalLight DirectionalLight { get; set; }
        public Dictionary<(int, int, int), Light3DChunk> Chunks { get; set; }
        public float ChunkSize { get; set; } = 4000;

        private int _pointLightsSSBO;

        public Light3DManager()
        {
            Chunks = new Dictionary<(int, int, int), Light3DChunk>();
        }

        public void AddPointLight(PointLight3D light)
        {
            var chunk = this.GetChunk(light.Position.X, light.Position.Y, light.Position.Z, this.ChunkSize);
            if (!Chunks.ContainsKey(chunk))
            {
                Chunks[chunk] = new Light3DChunk();
            }
            Chunks[chunk].Lights.Add(light);
        }

        public void BindLights(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            renderer.PrepareShader("dirLight.direction", DirectionalLight.Direction);
            renderer.PrepareShader("dirLight.lightColor", DirectionalLight.Color.Xyz);
            renderer.PrepareShader("dirLight.lightIntensity", DirectionalLight.Intensity);
            renderer.PrepareShader("dirLight.ambient", DirectionalLight.Ambient);
            renderer.PrepareShader("dirLight.specular", DirectionalLight.Specular);
            renderer.BindShaderStorageBuffer(4, _pointLightsSSBO);
        }

        public void CullLights(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            var chunk = this.GetChunk(camera.Transform.Position.X, camera.Transform.Position.Y, camera.Transform.Position.Z, 10f);
            var culledLights = this.CullChunkLights(camera, viewport, this.ChunkSize);
            renderer.BindBufferData<PointLight3DData>(_pointLightsSSBO, culledLights.ToArray(), true);
            Debug.WriteLine($"Culled lights: {culledLights.Count()}");
        }

        public void Dispose(IRenderDevice renderDevice)
        {
            renderDevice.DisposeBuffer(_pointLightsSSBO);
            _pointLightsSSBO = 0;
        }

        public int GetLightCount<T>() where T : Light
        {
            return this.GetTotalLightCount();
        }

        public void Init(IRenderDevice renderDevice)
        {
            _pointLightsSSBO = renderDevice.CreateEmptyBuffer();
        }

        private (int, int, int) GetChunk(float x, float y, float z, float chunkSize)
        {
            int chunkX = (int)MathF.Floor(x / chunkSize);
            int chunkY = (int)MathF.Floor(y / chunkSize);
            int chunkZ = (int)MathF.Floor(z / chunkSize);
            return (chunkX, chunkY, chunkZ);
        }

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

        private int GetTotalLightCount()
        {
            int count = 0;
            foreach (var chunk in Chunks)
            {
                count += chunk.Value.Lights.Count;
            }
            return count;
        }

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
    }
}
