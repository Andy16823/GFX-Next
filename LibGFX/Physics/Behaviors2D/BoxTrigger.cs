using BulletSharp;
using LibGFX.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Physics.Behaviors2D
{
    /// <summary>
    /// Represents a 2D box trigger collider.
    /// </summary>
    public class BoxTrigger : TriggerBehavior
    {
        /// <summary>
        /// Creates a new box trigger collider.
        /// </summary>
        /// <param name="physicsHandler"></param>
        public BoxTrigger(PhysicsHandler physicsHandler) : base(physicsHandler)
        {
        }

        /// <summary>
        /// Creates a box trigger collider with the given collision groups.
        /// </summary>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateTrigger(int collisionGroup = -1, int collisionMask = -1)
        {
            this.CreateTrigger(new Vector3(0.5f, 0.5f, 0.5f), collisionGroup, collisionMask);
        }

        /// <summary>
        /// Creates a box trigger collider with the given half extends and collision groups.
        /// </summary>
        /// <param name="halfExtends"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateTrigger(Vector3 halfExtends, int collisionGroup = -1, int collisionMask = -1)
        {
            var btStartTransform = Utils.GetBtTransform(Parent, this.Offset);
            var shape = new Box2DShape((System.Numerics.Vector3)halfExtends);

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
