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
    /// <summary>
    /// Represents a 3D capsule rigid body
    /// </summary>
    public class CapsuleRigidBody : RigidBodyBehavior
    {
        /// <summary>
        /// Creates a new 3D capsule rigid body
        /// </summary>
        /// <param name="physicsHandler"></param>
        public CapsuleRigidBody(PhysicsHandler physicsHandler) : base(physicsHandler)
        {

        }

        /// <summary>
        /// Creates a capsule rigid body with the given mass, radius, height, collision group and collision mask
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="radius"></param>
        /// <param name="height"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateRigidBody(float mass, float radius = 0.5f, float height = 1.0f, int collisionGroup = -1, int collisionMask = -1)
        {
            var halfExtends = new System.Numerics.Vector3(0.5f, 0.5f, 0.5f);
            var element = this.Parent;

            CapsuleShape capsuleShape = new CapsuleShape(radius, height);
            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, capsuleShape, capsuleShape.CalculateLocalInertia(mass));
            var btStartTransform = Utils.GetBtTransform(element, this.Offset);

            info.MotionState = new DefaultMotionState(btStartTransform);
            RigidBody = new RigidBody(info);
            RigidBody.UserObject = element;
            RigidBody.ApplyGravity();
            RigidBody.CollisionShape.LocalScaling = (System.Numerics.Vector3)element.Transform.Scale;

            PhysicsHandler.ManageElement(this, collisionGroup, collisionMask);
            info.Dispose();
        }

        /// <summary>
        /// Gets the physics object
        /// </summary>
        /// <returns></returns>
        public override object GetPhysicsObject()
        {
            return RigidBody;
        }
    }
}
