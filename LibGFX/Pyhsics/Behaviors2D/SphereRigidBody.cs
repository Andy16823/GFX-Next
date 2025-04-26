using BulletSharp;
using LibGFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics.Behaviors2D
{
    /// <summary>
    /// Represents a 2D sphere rigid body
    /// </summary>
    public class SphereRigidBody : RigidBodyBehavior
    {
        /// <summary>
        /// Creates a new 2D sphere rigid body
        /// </summary>
        /// <param name="physicsHandler"></param>
        public SphereRigidBody(PhysicsHandler physicsHandler) : base(physicsHandler)
        {
        }

        /// <summary>
        /// Creates a 2D sphere rigid body with the given mass and radius
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="radius"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateRigidBody(float mass, float radius, int collisionGroup = -1, int collisionMask = -1)
        {
            var shape = new Convex2DShape(new SphereShape(radius));
            
            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, shape, shape.CalculateLocalInertia(mass));
            var btStartTransform = Utils.GetBtTransform(Parent, this.Offset);

            info.MotionState = new DefaultMotionState(btStartTransform);
            RigidBody = new RigidBody(info);
            RigidBody.UserObject = Parent;
            RigidBody.AngularFactor = new System.Numerics.Vector3(0, 0, 1);
            RigidBody.LinearFactor = new System.Numerics.Vector3(1, 1, 0);
            RigidBody.CollisionShape.LocalScaling = (System.Numerics.Vector3)Parent.Transform.Scale;
            RigidBody.ApplyGravity();

            PhysicsHandler.ManageElement(this, collisionGroup, collisionMask);
            info.Dispose();
        }
    }
}
