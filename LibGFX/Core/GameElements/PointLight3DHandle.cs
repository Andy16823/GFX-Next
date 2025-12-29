using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
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
    /// Represents a handle for a 3D point light within a game scene, providing management and synchronization of the
    /// light's position with its associated transform.
    /// </summary>
    /// <remarks>Use this class to control a point light's placement and lifecycle in a 3D environment. The
    /// light's position is automatically updated when the transform changes. Dispose of the handle when the light is no
    /// longer needed to release resources and detach event handlers.</remarks>
    public class PointLight3DHandle : GameElement
    {
        /// <summary>
        /// Gets or sets the point light source used for 3D scene illumination.
        /// </summary>
        /// <remarks>Changing this property affects how objects in the scene are lit and how shadows are
        /// rendered. The light source position and intensity determine the appearance of lighting effects.</remarks>
        public PointLight3D LightSource { get; set; }

        /// <summary>
        /// Initializes a new instance of the PointLight3DHandle class with the specified name, position, color, range,
        /// and intensity.
        /// </summary>
        /// <remarks>If no transform is assigned, a new Transform is created and initialized with the
        /// specified position. The transform's Changed event is automatically subscribed to for internal
        /// updates.
        /// This class adds the point light to the scene and manages its lifecycle, including disposal of resources
        /// </remarks>
        /// <param name="name">The unique name used to identify the point light instance.</param>
        /// <param name="position">The position of the point light in 3D space.</param>
        /// <param name="color">The color of the point light, including its alpha component.</param>
        /// <param name="range">The effective range of the point light, in world units. Must be greater than zero.</param>
        /// <param name="intesity">The intensity of the point light. Must be a non-negative value.</param>
        public PointLight3DHandle(String name, Vector3 position, Vector4 color, float range = 10f, float intesity = 1.0f)
        {
            this.Name = name;
            LightSource = new PointLight3D(position, color, range, intesity);

            if(this.Transform == null) 
            {
                this.Transform = new Transform();
            }

            this.Transform.Position = position;
            this.Transform.Changed += Transform_Changed;
        }

        /// <summary>
        /// Updates the light source position to match the position of the specified transform.
        /// </summary>
        /// <param name="obj">The transform whose position is used to update the light source. Cannot be null.</param>
        private void Transform_Changed(Transform obj)
        {
            LightSource.Position = this.Transform.Position;
        }

        public override void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.Init(scene, viewport, renderer);
            scene.AddLight<PointLight3D>(LightSource);
        }

        /// <summary>
        /// Releases resources used by the object and detaches event handlers associated with the specified scene and
        /// render device.
        /// </summary>
        /// <param name="scene">The scene context for which resources should be released.</param>
        /// <param name="renderer">The render device associated with the scene that is used for resource management.</param>
        public override void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            base.Dispose(scene, renderer);
            if(this.Transform != null)
            {
                this.Transform.Changed -= Transform_Changed;
            }
            // TODO: Add light removal from scene
            //scene.RemoveLight<PointLight3D>(LightSource);
        }

        /// <summary>
        /// Calculates and updates the axis-aligned bounding box (AABB) for the current light source based on its
        /// position and range.
        /// </summary>
        /// <remarks>This method sets the AABB property to encompass the area affected by the light
        /// source. Call this method after modifying the light source's position or range to ensure the bounding box
        /// remains accurate.</remarks>
        public override void ComputeAABB()
        {
            AABB = new AABB(
                LightSource.Position - new Vector3(LightSource.Range),
                LightSource.Position + new Vector3(LightSource.Range)
            );
        }
    }
}
