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
    /// Represents a 3D box trigger
    /// </summary>
    public class BoxTrigger : TriggerBehavior
    {
        /// <summary>
        /// Creates a new 3D box trigger
        /// </summary>
        /// <param name="physicsHandler"></param>
        public BoxTrigger(PhysicsHandler physicsHandler) : base(physicsHandler)
        {

        }

        /// <summary>
        /// Creates a box trigger with the given collision group and collision mask
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateTrigger(int collisionGroup = -1, int collisionMask = -1)
        {
            var halfExtends = new System.Numerics.Vector3(0.5f, 0.5f, 0.5f);
            var element = this.Parent;
            var btStartTransform = Utils.GetBtTransform(element, this.Offset);

            BoxShape boxShape = new BoxShape(halfExtends);

            Trigger = new GhostObject();
            Trigger.UserObject = element;
            Trigger.CollisionShape = boxShape;
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
