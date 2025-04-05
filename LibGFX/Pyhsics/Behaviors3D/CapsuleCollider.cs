using BulletSharp;
using LibGFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics.Behaviors3D
{
    /// <summary>
    /// Represents a 3D capsule collider
    /// </summary>
    public class CapsuleCollider : CollisionBehavior
    {
        /// <summary>
        /// Creates a new 3D capsule collider
        /// </summary>
        /// <param name="physicsHandler"></param>
        public CapsuleCollider(PhysicsHandler physicsHandler) : base(physicsHandler)
        {

        }

        /// <summary>
        /// Creates a capsule collider with the given mass, radius, height, collision group and collision mask
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="radius"></param>
        /// <param name="height"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateCollider(float mass, float radius = 0.5f, float height = 1.0f, int collisionGroup = -1, int collisionMask = -1)
        {
            var element = this.Parent;
            var btStartTransform = Utils.GetBtTransform(element, this.Offset);

            CapsuleShape shape = new CapsuleShape(radius, height);
            shape.CalculateLocalInertia(mass);

            Collider = new CollisionObject();
            Collider.UserObject = element;
            Collider.CollisionShape = shape;
            Collider.WorldTransform = btStartTransform;
            Collider.CollisionShape.LocalScaling = (System.Numerics.Vector3)element.Transform.Scale;
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
