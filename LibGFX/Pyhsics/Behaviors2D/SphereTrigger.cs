using BulletSharp;
using LibGFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics.Behaviors2D
{
    /// <summary>
    /// Represents a sphere trigger collider in 2D physics
    /// </summary>
    public class SphereTrigger : TriggerBehavior
    {
        /// <summary>
        /// Creates a new sphere trigger collider
        /// </summary>
        /// <param name="physicsHandler"></param>
        public SphereTrigger(PhysicsHandler physicsHandler) : base(physicsHandler)
        {
        }

        /// <summary>
        /// Creates a sphere trigger collider with the given radius and collision groups
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateTrigger(float radius = 0.5f, int collisionGroup = -1, int collisionMask = -1)
        {
            var btStartTransform = Utils.GetBtTransform(Parent, this.Offset);
            var shape = new Convex2DShape(new SphereShape(radius));

            Trigger = new GhostObject();
            Trigger.UserObject = Parent;
            Trigger.CollisionShape = shape;
            Trigger.WorldTransform = btStartTransform;
            Trigger.CollisionShape.LocalScaling = (System.Numerics.Vector3)Parent.Transform.Scale;
            Trigger.CollisionFlags = CollisionFlags.NoContactResponse;
            PhysicsHandler.ManageElement(this, collisionGroup, collisionMask);
        }
    }
}
