using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics.Behaviors3D
{
    /// <summary>
    /// Represents a 3D convex hull collider
    /// </summary>
    public class ConvexHullCollider : CollisionBehavior
    {
        /// <summary>
        /// Creates a new 3D convex hull collider
        /// </summary>
        /// <param name="physicsHandler"></param>
        public ConvexHullCollider(PhysicsHandler physicsHandler) : base(physicsHandler)
        {

        }

        /// <summary>
        /// Creates a convex hull collider with the given mass, collision group and collision mask
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="file"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateCollider(float mass, String file, int collisionGroup = -1, int collisionMask = -1)
        {
            var scale = Parent.Transform.Scale;
            var btStartTransform = LibGFX.Core.Utils.GetBtTransform(Parent);

            var collisionShape = BtCollisionShapeBuilder.BuildConvexHull(file, (System.Numerics.Vector3)scale);
            collisionShape.CalculateLocalInertia(0f);

            Collider = new CollisionObject();
            Collider.UserObject = Parent;
            Collider.CollisionShape = collisionShape;
            Collider.WorldTransform = btStartTransform;
            PhysicsHandler.ManageElement(this, collisionGroup, collisionMask);
        }

        /// <summary>
        /// Gets the physics object
        /// </summary>
        /// <returns></returns>
        public override object GetPhysicsObject()
        {
            return this.Collider;
        }
    }
}
