using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Enviroment
{
    public class ProceduralSky : IEnviroment
    {
        public Transform Transform { get; set ; }
        public Vector3 SkyTopColor { get; set; } = new Vector3(0.2f, 0.5f, 0.9f);
        public Vector3 SkyBottomColor { get; set; } = new Vector3(1.0f, 0.6f, 0.3f);
        public Vector3 SunDirection { get; set; } = new Vector3(0.0f, 1.0f, 0.0f);
        public Vector3 SunColor { get; set; } = new Vector3(1.0f, 0.9f, 0.7f); 
        public float SunSize { get; set; } = 800.0f;
        public float SunIntensity { get; set; } = 1.5f;

        public ProceduralSky()
        {
            this.Transform = new Transform();
            this.Transform.Scale = Vector3.One;
        }

        public void Dispose(IRenderDevice renderer)
        {
            
        }

        public void Init(IRenderDevice renderer)
        {
            
        }

        public void Render(IRenderDevice renderer, Camera camera, Viewport viewport)
        {
            this.Transform.Position = camera.Transform.Position;
            var shader = renderer.GetShaderProgram("ProceduralSkyShader");
            var projectionMatrix = renderer.GetProjectionMatrix();
            var viewMatrix = renderer.GetViewMatrix();

            renderer.BindShaderProgram(shader);
            renderer.PrepareShader("skyTopColor", SkyTopColor);
            renderer.PrepareShader("skyBottomColor", SkyBottomColor);
            renderer.PrepareShader("sunDirection", SunDirection);
            renderer.PrepareShader("sunColor", SunColor);
            renderer.PrepareShader("sunSize", SunSize);
            renderer.PrepareShader("sunIntensity", SunIntensity);
            renderer.PrepareShader("p_mat", true, projectionMatrix);
            renderer.PrepareShader("v_mat", true, viewMatrix);
            renderer.PrepareShader("m_mat", true, Transform.GetMatrix());
            renderer.SetDepthMask(false);
            renderer.DrawShape(renderer.GetShape("CubeShape"));
            renderer.SetDepthMask(true);
            renderer.UnbindShaderProgram();
        }
    }
}
