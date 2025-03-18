using LibGFX.Graphics;
using LibGFX.Graphics.Primitives;
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

        public Primitive(String name, Material material, IPrimitive primitive) 
        {
            this.Name = name;
            this.Mesh = primitive.GetMesh(material);

        }

        public override void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.Init(scene, viewport, renderer);
            renderer.LoadMaterial(this.Mesh.Material);
            renderer.LoadMesh(this.Mesh);
        }

        public override void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            base.Render(scene, viewport, renderer, camera);
            var light = renderer.GetLightSource<DirectionalLight>();
            renderer.BindShaderProgram(renderer.GetShaderProgram("MeshShader"));
            if (light != null)
            {
                renderer.PrepareShader("lightPos", light.Position);
                renderer.PrepareShader("lightColor", light.Color.Xyz);
                renderer.PrepareShader("lightIntensity", light.Intensity);
                renderer.PrepareShader("viewPos", camera.Transform.Position);
            }
            renderer.DrawMesh(this.Transform, Mesh);
            renderer.UnbindShaderProgram();
        }

        public override void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            base.Dispose(scene, renderer);
            renderer.DisposeMaterial(this.Mesh.Material);
            renderer.DisposeMesh(this.Mesh);
        }
    }
}
