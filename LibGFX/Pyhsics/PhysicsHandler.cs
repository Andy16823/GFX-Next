using LibGFX.Core;
using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics
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
        /// Processes the physics handler
        /// </summary>
        /// <param name="scene"></param>
        public abstract void Process(BaseScene scene);

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
