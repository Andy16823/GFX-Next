using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core.GameElements
{
    /// <summary>
    /// Represents a mesh instancer that can render multiple instances of a mesh with a material.
    /// </summary>
    public class MeshInstancer : GameElement
    {
        /// <summary>
        /// The material used for rendering the mesh instances.
        /// </summary>
        public IMaterial Material { get; set; }

        /// <summary>
        /// The mesh to be instanced.
        /// </summary>
        public Mesh Mesh { get; set; }

        /// <summary>
        /// The container that holds the instances of the mesh.
        /// </summary>
        public RenderInstanceContainer InstanceContainer { get; set; }


        /// <summary>
        /// Creates a new instance of the MeshInstancer class.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="material"></param>
        public MeshInstancer(Mesh mesh, IMaterial material)
        {
            this.Mesh = mesh;
            this.Material = material;
            this.InstanceContainer = new RenderInstanceContainer();
        }

        /// <summary>
        /// Initializes the mesh instancer with the specified scene, viewport, and renderer.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
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

        /// <summary>
        /// Adds a new instance of the mesh to the instancer.
        /// </summary>
        /// <param name="transform"></param>
        public int AddInstance(Transform transform, bool visible = true)
        {
            var instanceId = this.InstanceContainer.AddInstance(transform, true);
            return instanceId;
        }

        /// <summary>
        /// Update the handles of the mesh instances in the instancer.
        /// </summary>
        /// <param name="scene"></param>
        public override void Update(BaseScene scene)
        {
            base.Update(scene);
        }

        /// <summary>
        /// Renders the mesh instances using the specified scene, viewport, renderer, and camera.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            base.Render(scene, viewport, renderer, camera);

            // Bind the shader program
            renderer.BindShaderProgram(renderer.GetShaderProgram("InstancedShader3D"));
            var light = renderer.GetLightSource<DirectionalLight>();

            // Prepare the shader uniforms
            if (light != null)
            {
                renderer.PrepareShader("dirLight.direction", light.Direction);
                renderer.PrepareShader("dirLight.lightColor", light.Color.Xyz);
                renderer.PrepareShader("dirLight.lightIntensity", light.Intensity);
                renderer.PrepareShader("dirLight.ambient", light.Ambient);
                renderer.PrepareShader("dirLight.specular", light.Specular);
                renderer.PrepareShader("viewPos", camera.Transform.Position);
            }

            renderer.DrawInstances(InstanceContainer, this.Material);

            // Unbind the shader program
            renderer.UnbindShaderProgram();
        }

        /// <summary>
        /// Disposes the mesh instancer and its resources.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="renderer"></param>
        public override void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            base.Dispose(scene, renderer);
            renderer.DisposeInstanceContainer(this.InstanceContainer);
        }

        /// <summary>
        /// Creates a new instance handle for the specified instance ID.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public InstanceHandle CreateInstanceHandle(int instanceId)
        {
            if(instanceId < 0 || instanceId >= this.InstanceContainer.Instances.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(instanceId), "Instance ID is out of range.");
            }

            var handle = new InstanceHandle(this, instanceId);
            return handle;
        }

        /// <summary>
        /// Bakes a specified number of instances into the instancer. 
        /// This instances are hidden untill you set them to visible.
        /// </summary>
        /// <param name="instanceCount">The amount of instances you want bake</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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
                this.AddInstance(transform, false);
            }
        }
    }
}
