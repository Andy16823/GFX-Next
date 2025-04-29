using Assimp;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Lights
{

    public class Light2DManager : ILightManager
    {
        public DirectionalLight2D DirectionalLight { get; set; }
        public List<PointLight2D> Lights { get; set; }
        public int LightSSBO { get; set; }

        public Light2DManager()
        {
            this.Lights = new List<PointLight2D>();
        }

        public void Init(IRenderDevice renderDevice)
        {
            var lightInfos = new List<Point2DLightData>();
            foreach (var light in Lights)
            {
                lightInfos.Add(light.ToStruct());
            }
            this.LightSSBO = renderDevice.CreateBuffer<Point2DLightData>(lightInfos.ToArray(), true);
        }

        public void BindLights(Viewport viewport, IRenderDevice renderer, Camera camera)
        {       
            if (this.DirectionalLight == null)
            {
                Debug.WriteLine("No directional light found");
                return;
            }
            var newBufferData = CullLights(camera).ToArray();
            renderer.BindBufferData<Point2DLightData>(LightSSBO, newBufferData, true);
            Debug.WriteLine($"LightSSBO: {this.LightSSBO} - {newBufferData.Length} lights");

            renderer.PrepareShader("dirLightColor", DirectionalLight.Color.Xyz);
            renderer.PrepareShader("dirLightIntensity", DirectionalLight.Intensity);
            renderer.BindShaderStorageBuffer(4, this.LightSSBO);
        }

        private IEnumerable<Point2DLightData> CullLights(Camera camera)
        {
            var cullRadius = camera.Transform.Scale.X / 2.0f;

            foreach (var light in this.Lights)
            {
                if (Vector2.DistanceSquared(camera.Transform.Position.Xy, light.Position.Xy) < cullRadius * cullRadius)
                {
                    yield return light.ToStruct();
                }
            }
        }

        public void Dispose(IRenderDevice renderDevice)
        {
            renderDevice.DisposeBuffer(this.LightSSBO);
            this.Lights.Clear();
        }

        public int GetLightCount<T>() where T : Light
        {
            if (typeof(T) == typeof(PointLight2D))
            {
                return this.Lights.Count;
            }
            else if (typeof(T) == typeof(DirectionalLight2D))
            {
                return this.DirectionalLight != null ? 1 : 0;
            }
            else
            {
                throw new ArgumentException($"Unsupported light type: {typeof(T)}");
            }
        }
    }
}
