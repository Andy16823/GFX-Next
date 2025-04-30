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
        /// The list of point lights in the scene.
        /// </summary>
        public List<PointLight2D> Lights { get; set; }

        /// <summary>
        /// The shader storage buffer object (SSBO) for the lights.
        /// </summary>
        public int LightSSBO { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Light2DManager"/> class.
        /// </summary>
        public Light2DManager()
        {
            this.Lights = new List<PointLight2D>();
        }

        /// <summary>
        /// Initializes the light manager with the given render device.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Init(IRenderDevice renderDevice)
        {
            var lightInfos = new List<Point2DLightData>();
            foreach (var light in Lights)
            {
                lightInfos.Add(light.ToStruct());
            }
            this.LightSSBO = renderDevice.CreateBuffer<Point2DLightData>(lightInfos.ToArray(), true);
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
            var newBufferData = CullLights(camera).ToArray();
            renderer.BindBufferData<Point2DLightData>(LightSSBO, newBufferData, true);
            Debug.WriteLine($"LightSSBO: {this.LightSSBO} - {newBufferData.Length} lights");

            renderer.PrepareShader("dirLightColor", DirectionalLight.Color.Xyz);
            renderer.PrepareShader("dirLightIntensity", DirectionalLight.Intensity);
            renderer.BindShaderStorageBuffer(4, this.LightSSBO);
        }

        /// <summary>
        /// Culls the lights based on the camera's position and scale.
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Disposes of the light manager and releases any resources.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Dispose(IRenderDevice renderDevice)
        {
            renderDevice.DisposeBuffer(this.LightSSBO);
            this.Lights.Clear();
        }

        /// <summary>
        /// Gets the light count of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
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
