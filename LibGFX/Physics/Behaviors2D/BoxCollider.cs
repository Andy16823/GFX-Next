using BulletSharp;
using LibGFX.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Physics.Behaviors2D
{
    /// <summary>
    /// Represents a 2D box collider behavior
    /// </summary>
    public class BoxCollider : CollisionBehavior
    {
        /// <summary>
        /// Creates a new 2D box collider
        /// </summary>
        /// <param name="physicsHandler"></param>
        public BoxCollider(PhysicsHandler physicsHandler) : base(physicsHandler)
        {
        }

        /// <summary>
        /// Creates a 2D box collider with the given mass
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateCollider(float mass, int collisionGroup = -1, int collisionMask = -1)
        {
            this.CreateCollider(mass, new Vector3(0.5f, 0.5f, 0.5f), collisionGroup, collisionMask);
        }

        /// <summary>
        /// Creates a 2D box collider with the given mass and half extends
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="halfExtends"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateCollider(float mass, Vector3 halfExtends, int collisionGroup = -1, int collisionMask = -1)
        {
            var btStartTransform = Utils.GetBtTransform(Parent, (OpenTK.Mathematics.Vector3)this.Offset);
            var shape = new Box2DShape((System.Numerics.Vector3) halfExtends);
            shape.CalculateLocalInertia(mass);

            Collider = new CollisionObject();
            Collider.UserObject = Parent;
            Collider.CollisionShape = shape;
            Collider.WorldTransform = btStartTransform;
            Collider.CollisionShape.LocalScaling = (System.Numerics.Vector3)Parent.Transform.Scale;
            PhysicsHandler.ManageElement(this, collisionGroup, collisionMask);
        }

        /// <summary>
        /// Returns a clone of the BoxCollider
        /// </summary>
        /// <returns></returns>
        public override BoxCollider Clone()
        {
            var clone = new BoxCollider(this.PhysicsHandler);
            clone.Offset = this.Offset;
            return clone;
        }

    }
}
