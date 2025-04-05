using BulletSharp;
using LibGFX.Core;
using LibGFX.Pyhsics.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics.Behaviors3D
{
    public class SphereRigidBody : RigidBodyBehavior
    {
        public SphereRigidBody(PhysicsHandler physicsHandler) : base(physicsHandler)
        {

        }

        public void CreateRigidBody(float mass, float radius = 0.5f, int collisionGroup = -1, int collisionMask = -1)
        {
            var halfExtends = new System.Numerics.Vector3(0.5f, 0.5f, 0.5f);
            var element = this.Parent;

            SphereShape shape = new SphereShape(radius);
            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, shape, shape.CalculateLocalInertia(mass));
            var btStartTransform = Utils.GetBtTransform(element, this.Offset);

            info.MotionState = new DefaultMotionState(btStartTransform);
            RigidBody = new RigidBody(info);
            RigidBody.UserObject = element;
            RigidBody.ApplyGravity();
            RigidBody.CollisionShape.LocalScaling = (System.Numerics.Vector3)element.Transform.Scale;

            PhysicsHandler.ManageElement(this, collisionGroup, collisionMask);
            info.Dispose();
        }

        public override object GetPhysicsObject()
        {
            return RigidBody;
        }
    }
}
