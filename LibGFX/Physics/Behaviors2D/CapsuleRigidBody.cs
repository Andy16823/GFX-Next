using BulletSharp;
using LibGFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Physics.Behaviors2D
{
    /// <summary>
    /// Represents a 2D capsule rigid body
    /// </summary>
    public class CapsuleRigidBody : RigidBodyBehavior
    {

        /// <summary>
        /// Creates a new 2D capsule rigid body
        /// </summary>
        /// <param name="physicsHandler"></param>
        public CapsuleRigidBody(PhysicsHandler physicsHandler) : base(physicsHandler)
        {
        }

        /// <summary>
        /// Creates a 2D capsule rigid body with the given mass, radius, and height
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="radius"></param>
        /// <param name="height"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateRigidBody(float mass, float radius, float height, int collisionGroup = -1, int collisionMask = -1)
        {
            CapsuleShape capsuleShape = new CapsuleShape(radius, height);
            var shape = new Convex2DShape(capsuleShape);

            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, shape, shape.CalculateLocalInertia(mass));
            var btStartTransform = Utils.GetBtTransform(Parent, this.Offset);

            info.MotionState = new DefaultMotionState(btStartTransform);
            RigidBody = new RigidBody(info);
            RigidBody.UserObject = Parent;
            RigidBody.AngularFactor = new System.Numerics.Vector3(0, 0, 1);
            RigidBody.LinearFactor = new System.Numerics.Vector3(1, 1, 0);
            RigidBody.ApplyGravity();
            RigidBody.CollisionShape.LocalScaling = (System.Numerics.Vector3)Parent.Transform.Scale;

            PhysicsHandler.ManageElement(this, collisionGroup, collisionMask);
            info.Dispose();
        }

        /// <summary>
        /// Returns a clone of the CapsuleRigidBody
        /// </summary>
        /// <returns></returns>
        public override CapsuleRigidBody Clone()
        {
            var clone = new CapsuleRigidBody(this.PhysicsHandler);
            clone.Offset = this.Offset;
            return clone;
        }
    }
}
