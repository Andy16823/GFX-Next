using BulletSharp;
using LibGFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics.Behaviors2D
{
    public class SphereCollider : CollisionBehavior
    {
        public SphereCollider(PhysicsHandler physicsHandler) : base(physicsHandler)
        {
        }

        public void CreateCollider(float mass, float radius, int collisionGroup = -1, int collisionMask = -1)
        {
            var btStartTransform = Utils.GetBtTransform(Parent, this.Offset);

            var shape = new Convex2DShape(new SphereShape(radius));
            shape.CalculateLocalInertia(mass);

            Collider = new CollisionObject();
            Collider.UserObject = Parent;
            Collider.CollisionShape = shape;
            Collider.WorldTransform = btStartTransform;
            Collider.CollisionShape.LocalScaling = (System.Numerics.Vector3)Parent.Transform.Scale;
            PhysicsHandler.ManageElement(this, collisionGroup, collisionMask);
        }
    }
}
