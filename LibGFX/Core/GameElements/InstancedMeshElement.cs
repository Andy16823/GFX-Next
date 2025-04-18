using LibGFX.Graphics;
using LibGFX.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core.GameElements
{
    public class InstancedMeshElement : GameElement
    {
        public Material Material { get; set; }
        public Mesh Mesh { get; set; }
        public RenderInstanceContainer InstanceContainer { get; set; }

        public InstancedMeshElement(Mesh mesh, Material material)
        {
            this.Mesh = mesh;
            this.Material = material;
            this.InstanceContainer = new RenderInstanceContainer();
        }

        public override void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.Init(scene, viewport, renderer);

            renderer.LoadInstanceContainer(this.InstanceContainer);
            renderer.BindMeshForInstance(this.InstanceContainer, this.Mesh);

            if (this.InstanceContainer.Instances.Count > 0)
            {
                renderer.LoadInstances(this.InstanceContainer);
            }
        }

        public void AddInstance(Transform transform)
        {
            var instance = new RenderInstance();
            instance.Transform = transform;
            instance.Visible = true;

            this.InstanceContainer.Instances.Add(instance);
        }

        public override void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            base.Render(scene, viewport, renderer, camera);

            // Bind the shader program
            renderer.BindShaderProgram(renderer.GetShaderProgram("InstancedShader3D"));

            var light = renderer.GetLightSource<DirectionalLight>();

            // Prepare the shader uniforms
            if (light != null)
            {
                renderer.PrepareShader("lightPos", light.Position);
                renderer.PrepareShader("lightColor", light.Color.Xyz);
                renderer.PrepareShader("lightIntensity", light.Intensity);
                renderer.PrepareShader("viewPos", camera.Transform.Position);
            }

            renderer.DrawInstances(InstanceContainer, this.Material);

            // Unbind the shader program
            renderer.UnbindShaderProgram();
        }

        public override void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            base.Dispose(scene, renderer);
            renderer.DisposeInstanceContainer(this.InstanceContainer);
        }

    }
}
