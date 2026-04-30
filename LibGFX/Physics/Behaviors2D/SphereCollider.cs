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
    /// Represents a 2D sphere collider
    /// </summary>
    public class SphereCollider : CollisionBehavior
    {
        /// <summary>
        /// Creates a new 2D sphere collider
        /// </summary>
        /// <param name="physicsHandler"></param>
        public SphereCollider(PhysicsHandler physicsHandler) : base(physicsHandler)
        {
        }

        /// <summary>
        /// Creates a 2D sphere collider with the given mass and radius
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="radius"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateCollider(float mass, float radius, int collisionGroup = -1, int collisionMask = -1)
        {
            var btStartTransform = Utils.GetBtTransform(Parent, this.Offset);

            var shape = new Convex2DShape(new SphereShape(radius));
            shape.CalculateLocalInertia(mass);

            Collider = new CollisionObject();
            Collider.UserObject = Parent;
            Collider.CollisionShape = shape;
            Collider.WorldTransform = btStartTransform;
            Collider.CollisionShape.LocalScaling = (System.Numerics.Vector3)Parent.Transform.Scale;
            PhysicsHandler.ManageElement(this, collisionGroup, collisionMask);
        }

        /// <summary>
        /// Returns a clone of the SphereCollider
        /// </summary>
        /// <returns></returns>
        public override SphereCollider Clone()
        {
            var clone = new SphereCollider(this.PhysicsHandler);
            clone.Offset = this.Offset;
            return clone;
        }
    }
}
