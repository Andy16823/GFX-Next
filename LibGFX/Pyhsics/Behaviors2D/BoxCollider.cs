using BulletSharp;
using LibGFX.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics.Behaviors2D
{
    public class BoxCollider : CollisionBehavior
    {
        public BoxCollider(PhysicsHandler physicsHandler) : base(physicsHandler)
        {
        }

        public void CreateCollider(float mass, int collisionGroup = -1, int collisionMask = -1)
        {
            this.CreateCollider(mass, new Vector3(0.5f, 0.5f, 0.5f), collisionGroup, collisionMask);
        }

        public void CreateCollider(float mass, Vector3 halfExtends, int collisionGroup = -1, int collisionMask = -1)
        {
            var btStartTransform = Utils.GetBtTransform(Parent, (OpenTK.Mathematics.Vector3)this.Offset);
            var shape = new Box2DShape((System.Numerics.Vector3) halfExtends);
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
