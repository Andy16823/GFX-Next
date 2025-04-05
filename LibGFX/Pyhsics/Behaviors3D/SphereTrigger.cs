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
    /// Represents a 3D sphere trigger
    /// </summary>
    public class SphereTrigger : TriggerBehavior
    {
        /// <summary>
        /// Creates a new 3D sphere trigger
        /// </summary>
        /// <param name="physicsHandler"></param>
        public SphereTrigger(PhysicsHandler physicsHandler) : base(physicsHandler)
        {

        }

        /// <summary>
        /// Creates a sphere trigger with the given radius, collision group and collision mask
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateTrigger(float radius = 0.5f, int collisionGroup = -1, int collisionMask = -1)
        {
            var element = this.Parent;
            var btStartTransform = Utils.GetBtTransform(element, this.Offset);

            SphereShape shape = new SphereShape(radius);

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
