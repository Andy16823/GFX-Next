using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Primitives;
using LibGFX.Graphics.Shader;
using LibGFX.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core.GameElements
{
    public class Primitive : GameElement
    {
        public Mesh Mesh { get; set; }
        public IMaterial Material { get; set; }
        public ShaderProgram Shader { get; set; }

        public Primitive(String name, IMaterial material, IPrimitive primitive) 
        {
            this.Name = name;
            this.Mesh = primitive.GetMesh();
            this.Material = material;
        }

        public override void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.Init(scene, viewport, renderer);
            renderer.LoadMesh(this.Mesh);

            if(this.Shader == null)
            {
                this.Shader = renderer.GetShaderProgram("MeshShader");
            }
        }

        public override void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            base.Render(scene, viewport, renderer, camera);
            var light = renderer.GetLightSource<DirectionalLight>();
            renderer.BindShaderProgram(this.Shader);
            if (light != null)
            {
                renderer.PrepareShader("dirLight.direction", light.Direction);
                renderer.PrepareShader("dirLight.lightColor", light.Color.Xyz);
                renderer.PrepareShader("dirLight.lightIntensity", light.Intensity);
                renderer.PrepareShader("dirLight.ambient", light.Ambient);
                renderer.PrepareShader("dirLight.specular", light.Specular);
                renderer.PrepareShader("viewPos", camera.Transform.Position);
            }
            renderer.DrawMesh(this.Transform, Mesh, Material);
            renderer.UnbindShaderProgram();
        }

        public override void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            base.Dispose(scene, renderer);
            renderer.DisposeMesh(this.Mesh);
        }
    }
}
