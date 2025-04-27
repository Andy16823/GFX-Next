using Assimp;
using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Enviroment
{
    public class Skybox : IEnviroment
    {
        public Transform Transform { get; set; }
        public Cubemap Cubemap { get; set; }

        public Skybox(Cubemap cubemap)
        {
            this.Transform = new Transform();
            this.Transform.Scale = Vector3.One;
            this.Cubemap = cubemap;
        }

        public void Init(IRenderDevice renderer)
        {
            renderer.LoadCubemap(Cubemap);
        }

        public void Render(IRenderDevice renderer, Camera camera, Viewport viewport)
        {
            this.Transform.Position = camera.Transform.Position;
            renderer.BindShaderProgram(renderer.GetShaderProgram("EnviromentShader"));
            renderer.DrawCubemap(this.Transform, this.Cubemap, Vector4.Zero);
            renderer.UnbindShaderProgram();
        }

        public void Dispose(IRenderDevice renderer)
        {
            renderer.DisposeCubemap(this.Cubemap);
        }
    }
}
