using BulletSharp;
using LibGFX.Core;
using LibGFX.Graphics;
using LibGFX.Pyhsics.Behaviors;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics.Behaviors3D
{
    public class BoxRigidBody : RigidBody3D
    {
        public BoxRigidBody(PhysicsHandler physicsHandler) : base(physicsHandler)
        {
            
        }

        public void CreateRigidBody(float mass, int collisionGroup = -1, int collisionMask = -1)
        {
            var halfExtends = new System.Numerics.Vector3(0.5f, 0.5f, 0.5f);
            var element = this.Parent;

            BoxShape boxShape = new BoxShape(halfExtends);
            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, boxShape, boxShape.CalculateLocalInertia(mass));
            var btStartTransform = Utils.GetBtTransform(element, Vector3.Zero);

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
