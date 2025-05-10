using Assimp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Lights
{
    public class Light3DManager : ILightManager
    {
        public DirectionalLight DirectionalLight { get; set; }
        public List<PointLight3D> PointLights { get; set; } = new List<PointLight3D>();

        private int _pointLightsSSBO;

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
            var culledLights = new List<PointLight3DData>();
            foreach (var light in PointLights)
            {
                culledLights.Add(light.ToStruct());
            }
            renderer.BindBufferData<PointLight3DData>(_pointLightsSSBO, culledLights.ToArray(), true);
        }

        public void Dispose(IRenderDevice renderDevice)
        {
            renderDevice.DisposeBuffer(_pointLightsSSBO);
            _pointLightsSSBO = 0;
        }

        public int GetLightCount<T>() where T : Light
        {
            return PointLights.Count;
        }

        public void Init(IRenderDevice renderDevice)
        {
            _pointLightsSSBO = renderDevice.CreateEmptyBuffer();
        }
    }
}
