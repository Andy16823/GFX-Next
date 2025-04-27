using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Primitives;
using LibGFX.Graphics.Shader;
using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core.GameElements
{
    public class SpriteInstancer : GameElement
    {
        public IMaterial Material { get; set; }
        public Mesh Mesh { get; internal set; }
        public RenderInstanceContainer InstanceContainer { get; set; }
        public ShaderProgram Shader { get; set; }

        public SpriteInstancer(IMaterial material)
        {
            this.Mesh = new Quad().GetMesh();
            this.Material = material;
            this.InstanceContainer = new RenderInstanceContainer();
        }

        public override void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.Init(scene, viewport, renderer);
            renderer.LoadMesh(Mesh);
            renderer.LoadInstanceContainer(InstanceContainer);
            renderer.BindMeshForInstance(InstanceContainer, Mesh);

            if (this.InstanceContainer.Instances.Count > 0)
            {
                renderer.LoadInstances(this.InstanceContainer);
            }

            if (this.Shader == null)
            {
                this.Shader = renderer.GetShaderProgram("InstancedShader2D");
            }
        }

        public int AddInstance(Transform transform, Vector4 uvTransform, bool visible = true)
        {
            var instanceId = this.InstanceContainer.AddInstance(transform, true);
            this.InstanceContainer.Instances[instanceId].UVTransform = uvTransform;

            return instanceId;
        }

        public override void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            base.Render(scene, viewport, renderer, camera);

            // Bind the shader program
            renderer.BindShaderProgram(this.Shader);
            renderer.DrawInstances(InstanceContainer, this.Material);
            renderer.UnbindShaderProgram();
        }

        public override void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            base.Dispose(scene, renderer);
            renderer.DisposeInstanceContainer(this.InstanceContainer);
        }

        public InstanceHandle CreateInstanceHandle(int instanceId)
        {
            if (instanceId < 0 || instanceId >= this.InstanceContainer.Instances.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(instanceId), "Instance ID is out of range.");
            }

            var handle = new InstanceHandle(this.InstanceContainer, instanceId);
            return handle;
        }

        public void BakeInstances(uint instanceCount = 10)
        {
            if (instanceCount == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(instanceCount), "Instance count must be greater than zero.");
            }

            for (uint i = 0; i < instanceCount; i++)
            {
                var transform = new Transform();
                transform.Position = Vector3.Zero;
                transform.Rotation = Quaternion.Identity;
                transform.Scale = Vector3.One;
                this.AddInstance(transform, Vector4.One, false);
            }
        }
    }
}
