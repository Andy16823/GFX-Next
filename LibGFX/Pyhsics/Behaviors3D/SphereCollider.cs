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
    /// Represents a 3D sphere collider
    /// </summary>
    public class SphereCollider : CollisionBehavior
    {
        /// <summary>
        /// Creates a new 3D sphere collider
        /// </summary>
        /// <param name="physicsHandler"></param>
        public SphereCollider(PhysicsHandler physicsHandler) : base(physicsHandler)
        {

        }

        /// <summary>
        /// Creates a sphere collider with the given mass, radius, collision group and collision mask
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="radius"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateCollider(float mass, float radius = 0.5f, int collisionGroup = -1, int collisionMask = -1)
        {
            var element = this.Parent;
            var btStartTransform = Utils.GetBtTransform(element, this.Offset);

            SphereShape shape = new SphereShape(radius);
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
