using BulletSharp;
using LibGFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics.Behaviors2D
{
    public class CapsuleTrigger : TriggerBehavior
    {
        public CapsuleTrigger(PhysicsHandler physicsHandler) : base(physicsHandler)
        {
        }

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
