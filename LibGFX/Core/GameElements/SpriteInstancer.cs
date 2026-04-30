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
    /// <summary>
    /// Represents a sprite instancer that can render multiple instances of a sprite using instancing.
    /// </summary>
    public class SpriteInstancer : GameElement
    {
        /// <summary>
        /// The material used for rendering the sprite instances.
        /// </summary>
        public IMaterial Material { get; set; }

        /// <summary>
        /// The mesh used for rendering the sprite instances.
        /// </summary>
        public Mesh Mesh { get; internal set; }

        /// <summary>
        /// The instance container that holds the instances of the sprite.
        /// </summary>
        public RenderInstanceContainer InstanceContainer { get; set; }

        /// <summary>
        /// Gets a value indicating whether the image contains any transparent pixels.
        /// </summary>
        public override bool HasTransparency => false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteInstancer"/> class with the specified material.
        /// </summary>
        /// <param name="material"></param>
        public SpriteInstancer(IMaterial material)
        {
            this.Mesh = Quad.GetMesh();
            this.Material = material;
            this.InstanceContainer = new RenderInstanceContainer();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteInstancer"/> class with the specified material and number of instances.
        /// </summary>
        /// <param name="material"></param>
        /// <param name="instances"></param>
        public SpriteInstancer(IMaterial material, uint instances)
        {
            this.Mesh = Quad.GetMesh();
            this.Material = material;
            this.InstanceContainer = new RenderInstanceContainer();
            this.BakeInstances((uint)instances);
        }

        /// <summary>
        /// Initializes the sprite instancer with the specified scene, viewport, and renderer.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.Init(scene, viewport, renderer);
            Mesh.Init(renderer);
            this.InstanceContainer.Init(renderer);
            renderer.BindMeshForInstance(InstanceContainer, Mesh);

            if (this.InstanceContainer.Instances.Count > 0)
            {
                renderer.LoadInstances(this.InstanceContainer);
            }
        }

        /// <summary>
        /// Adds a new instance to the sprite instancer with the specified transform and UV transform.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="uvTransform"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        public int AddInstance(Transform transform, Vector4 uvTransform, bool visible = true)
        {
            var instanceId = this.InstanceContainer.AddInstance(transform, true);
            this.InstanceContainer.Instances[instanceId].UVTransform = uvTransform;

            return instanceId;
        }

        /// <summary>
        /// Renders the sprite instancer using the specified scene, viewport, renderer, and camera.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            base.Render(scene, viewport, renderer, camera);

            // Bind the shader program
            this.Material.Use(renderer);
            renderer.DrawInstances(InstanceContainer);
            scene.RenderStats.IncrementDrawCalls();
            this.Material.Disable(renderer);
        }

        /// <summary>
        /// Disposes the sprite instancer and its resources.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="renderer"></param>
        public override void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            base.Dispose(scene, renderer);
            this.InstanceContainer.Dispose(renderer);
        }

        /// <summary>
        /// Creates a new instance handle for the specified instance ID.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public InstanceHandle CreateInstanceHandle(int instanceId)
        {
            if (instanceId < 0 || instanceId >= this.InstanceContainer.Instances.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(instanceId), "Instance ID is out of range.");
            }

            var handle = new InstanceHandle(this.InstanceContainer, instanceId);
            return handle;
        }

        /// <summary>
        /// Bakes the specified number of instances into the sprite instancer.
        /// </summary>
        /// <param name="instanceCount"></param>
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
                this.AddInstance(transform, Vector4.One, false);
            }
        }

        /// <summary>
        /// Computes the axis-aligned bounding box (AABB) for the sprite instancer. Since this is an instancer, the AABB is set to zero.
        /// </summary>
        public override void ComputeAABB()
        {
            this.AABB = AABB.Zero;
        }

        /// <summary>
        /// Clones the sprite instancer. This method is not implemented and will throw a NotImplementedException.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override GameElement Clone()
        {
            // TODO: Implement cloning logic for SpriteInstancer
            throw new NotImplementedException();
        }
    }
}
