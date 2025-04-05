using BulletSharp;
using LibGFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BulletSharp.Dbvt;

namespace LibGFX.Pyhsics.Behaviors3D
{
    /// <summary>
    /// Represents a 3D capsule trigger
    /// </summary>
    public class CapsuleTrigger : TriggerBehavior
    {
        /// <summary>
        /// Creates a new 3D capsule trigger
        /// </summary>
        /// <param name="physicsHandler"></param>
        public CapsuleTrigger(PhysicsHandler physicsHandler) : base(physicsHandler)
        {

        }

        /// <summary>
        /// Creates a capsule trigger with the given radius, height, collision group and collision mask
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="height"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateTrigger(float radius = 0.5f, float height = 1.0f, int collisionGroup = -1, int collisionMask = -1)
        {
            var element = this.Parent;
            var btStartTransform = Utils.GetBtTransform(element, this.Offset);

            CapsuleShape shape = new CapsuleShape(radius, height);

            Trigger = new GhostObject();
            Trigger.UserObject = element;
            Trigger.CollisionShape = shape;
            Trigger.WorldTransform = btStartTransform;
            Trigger.CollisionShape.LocalScaling = (System.Numerics.Vector3)element.Transform.Scale;
            Trigger.CollisionFlags = CollisionFlags.NoContactResponse;
            PhysicsHandler.ManageElement(this, collisionGroup, collisionMask);
        }

        /// <summary>
        /// Gets the physics object
        /// </summary>
        /// <returns></returns>
        public override object GetPhysicsObject()
        {
            return this.Trigger;
        }
    }
}
