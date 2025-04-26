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
    /// Represents a capsule trigger collider in 2D physics
    /// </summary>
    public class CapsuleTrigger : TriggerBehavior
    {
        /// <summary>
        /// Creates a new capsule trigger collider
        /// </summary>
        /// <param name="physicsHandler"></param>
        public CapsuleTrigger(PhysicsHandler physicsHandler) : base(physicsHandler)
        {
        }

        /// <summary>
        /// Creates a capsule trigger collider with the given radius, height and collision groups
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="height"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateTrigger(float radius = 0.5f, float height = 0.5f, int collisionGroup = -1, int collisionMask = -1)
        {
            var btStartTransform = Utils.GetBtTransform(Parent, this.Offset);
            var shape = new Convex2DShape(new CapsuleShape(radius, height));

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
