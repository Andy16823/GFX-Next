using LibGFX.Core;
using LibGFX.Graphics;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Physics
{
    /// <summary>
    /// Represents a physics handler
    /// </summary>
    public abstract class PhysicsHandler
    {
        /// <summary>
        /// The debug physics flag
        /// </summary>
        public bool DebugPhysics { get; set; } = false;

        /// <summary>
        /// The fixed time step for physics updates
        /// </summary>
        public float FixedTimeStep { get; set; } = 1.0f / 60.0f;

        /// <summary>
        /// Processes the physics handler
        /// </summary>
        /// <param name="scene"></param>
        public abstract void Process(BaseScene scene);

        /// <summary>
        /// Performs a ray test and returns the hit result
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public abstract HitResult RayTest(Vector3 start, Vector3 end);

        /// <summary>
        /// Manages a physics element
        /// </summary>
        /// <param name="physicsBehavior"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public abstract void ManageElement(PhysicsBehavior physicsBehavior, int collisionGroup = -1, int collisionMask = -1);

        /// <summary>
        /// Removes a physics element
        /// </summary>
        /// <param name="physicsBehavior"></param>
        public abstract void RemoveElement(PhysicsBehavior physicsBehavior);

        /// <summary>
        /// Determines if the physics handler has a debug drawer
        /// </summary>
        /// <returns></returns>
        public abstract bool HasDebugDrawer();

        /// <summary>
        /// Debug draws the physics handler
        /// </summary>
        /// <param name="renderer"></param>
        public abstract void DebugDraw(IRenderDevice renderer);

        /// <summary>
        /// Gets the debug drawer
        /// </summary>
        /// <returns></returns>
        public abstract DebugDrawer GetDebugDrawer();

        /// <summary>
        /// Sets the debug drawer
        /// </summary>
        /// <param name="debugDrawer"></param>
        public abstract void SetDebugDrawer(DebugDrawer debugDrawer);
    }
}
