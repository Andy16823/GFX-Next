using BulletSharp;
using BulletSharp.SoftBody;
using LibGFX.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics.Behaviors2D
{
    public class BoxRigidBody : RigidBodyBehavior
    {
        public BoxRigidBody(PhysicsHandler physicsHandler) : base(physicsHandler)
        {
        }

        public void CreateRigidBody(float mass, Vector3 halfExtends, int collisionGroup = -1, int collisionMask = -1)
        {
            var boxShape = new Box2DShape((System.Numerics.Vector3)halfExtends);

            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, boxShape, boxShape.CalculateLocalInertia(mass));
            var btStartTransform = Utils.GetBtTransform(Parent, (OpenTK.Mathematics.Vector3)this.Offset);

            info.MotionState = new DefaultMotionState(btStartTransform);
            RigidBody = new RigidBody(info);
            RigidBody.UserObject = Parent;
            RigidBody.ApplyGravity();
            RigidBody.AngularFactor = (System.Numerics.Vector3) new Vector3(0, 0, 1);
            RigidBody.LinearFactor = (System.Numerics.Vector3) new Vector3(1, 1, 0);
            RigidBody.CollisionShape.LocalScaling = (System.Numerics.Vector3)Parent.Transform.Scale;

            PhysicsHandler.ManageElement(this, collisionGroup, collisionMask);
            info.Dispose();
        }

        public void CreateRigidBody(float mass, float halfExtends, int collisionGroup = -1, int collisionMask = -1)
        {
            CreateRigidBody(mass, new Vector3(halfExtends), collisionGroup, collisionMask);
        }

        public void CreateRigidBody(float mass, int collisionGroup = -1, int collisionMask = -1)
        {
            CreateRigidBody(mass, new Vector3(0.5f, 0.5f, 0.0f), collisionGroup, collisionMask);
        }
    }
}
