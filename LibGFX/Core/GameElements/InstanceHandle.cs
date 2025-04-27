using LibGFX.Graphics;
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
    /// Represents a handle for an instance of a mesh in a mesh instancer.
    /// </summary>
    public class InstanceHandle : GameElement
    {
        /// <summary>
        /// The mesh instancer that this handle belongs to.
        /// </summary>
        public RenderInstanceContainer InstanceContainer { get; set; }

        /// <summary>
        /// The ID of the instance in the instancer.
        /// </summary>
        public int InstanceID { get; set; }

        /// <summary>
        /// The transform of the instance.
        /// </summary>
        public override Transform Transform
        {
            get
            {
                return InstanceContainer.Instances[InstanceID].Transform;
            }
            set
            {
                InstanceContainer.Instances[InstanceID].Transform = value;
            }
        }

        /// <summary>
        /// The visibility state of the instance.
        /// </summary>
        public override bool Visible
        {
            get
            {
                return InstanceContainer.Instances[InstanceID].Visible;
            }
            set
            {
                InstanceContainer.Instances[InstanceID].Visible = value;
            }
        }

        /// <summary>
        /// The UV transform of the instance.
        /// </summary>
        public Vector4 UVTransform
        {
            get
            {
                return InstanceContainer.Instances[InstanceID].UVTransform;
            }
            set
            {
                InstanceContainer.Instances[InstanceID].UVTransform = value;
            }
        }

        /// <summary>
        /// Creates a new instance of the InstanceHandle class.
        /// </summary>
        /// <param name="instancer"></param>
        /// <param name="instanceId"></param>
        public InstanceHandle(RenderInstanceContainer instanceContainer, int instanceId)
        {
            this.InstanceContainer = instanceContainer;
            this.InstanceID = instanceId;
        }

        /// <summary>
        /// Initializes the instance handle with the specified scene, viewport, and renderer.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.Init(scene, viewport, renderer);
        }

        /// <summary>
        /// Renders the instance handle in the specified scene, viewport, renderer, and camera.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            base.Render(scene, viewport, renderer, camera);
            renderer.UpdateInstance(InstanceContainer, InstanceID);
        }
    }
}
