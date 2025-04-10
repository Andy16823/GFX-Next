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
    /// <summary>
    /// Represents a 3D box rigid body
    /// </summary>
    public class BoxRigidBody : RigidBodyBehavior
    {
        /// <summary>
        /// Creates a new 3D box rigid body
        /// </summary>
        /// <param name="physicsHandler"></param>
        public BoxRigidBody(PhysicsHandler physicsHandler) : base(physicsHandler)
        {
            
        }

        /// <summary>
        /// Creates a box rigid body with the given mass, collision group and collision mask
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateRigidBody(float mass, int collisionGroup = -1, int collisionMask = -1)
        {
            var halfExtends = new System.Numerics.Vector3(0.5f, 0.5f, 0.5f);
            var element = this.Parent;

            BoxShape boxShape = new BoxShape(halfExtends);
            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(mass, null, boxShape, boxShape.CalculateLocalInertia(mass));
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
