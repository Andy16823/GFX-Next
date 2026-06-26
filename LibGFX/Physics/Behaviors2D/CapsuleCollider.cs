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
    /// Represents a 2D capsule collider
    /// </summary>
    public class CapsuleCollider : CollisionBehavior
    {

        /// <summary>
        /// Creates a new 2D capsule collider
        /// </summary>
        /// <param name="physicsHandler"></param>
        public CapsuleCollider(PhysicsHandler physicsHandler) : base(physicsHandler)
        {
        }

        /// <summary>
        /// Creates a 2D capsule collider with the given mass, radius, and height
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="radius"></param>
        /// <param name="height"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateCollider(float mass, float radius, float height, int collisionGroup = -1, int collisionMask = -1)
        {
            var btStartTransform = Utils.GetBtTransform(Parent, this.Offset);

            var shape = new Convex2DShape(new CapsuleShape(radius, height));
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
