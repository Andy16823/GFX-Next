using BulletSharp;
using LibGFX.Core;
using LibGFX.Physics.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Physics.Behaviors3D
{
    /// <summary>
    /// Represents a 3D sphere rigid body
    /// </summary>
    public class SphereRigidBody : RigidBodyBehavior
    {
        /// <summary>
        /// Creates a new 3D sphere rigid body
        /// </summary>
        /// <param name="physicsHandler"></param>
        public SphereRigidBody(PhysicsHandler physicsHandler) : base(physicsHandler)
        {

        }

        /// <summary>
        /// Creates a sphere rigid body with the given mass, radius, collision group and collision mask
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="radius"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
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

        /// <summary>
        /// Returns a clone of the SphereRigidBody
        /// </summary>
        /// <returns></returns>
        public override SphereRigidBody Clone()
        {
            var clone = new SphereRigidBody(this.PhysicsHandler);
            clone.Offset = this.Offset;
            return clone;
        }
    }
}
