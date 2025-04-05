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
    /// Represents a 3D box collider
    /// </summary>
    public class BoxCollider : CollisionBehavior
    {

        /// <summary>
        /// Creates a new 3D box collider
        /// </summary>
        /// <param name="physicsHandler"></param>
        public BoxCollider(PhysicsHandler physicsHandler) : base(physicsHandler)
        {

        }

        /// <summary>
        /// Creates a box collider with the given mass, collision group and collision mask
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateCollider(float mass, int collisionGroup = -1, int collisionMask = -1)
        {
            var halfExtends = new System.Numerics.Vector3(0.5f, 0.5f, 0.5f);
            var element = this.Parent;
            var btStartTransform = Utils.GetBtTransform(element, this.Offset);

            BoxShape boxShape = new BoxShape(halfExtends);
            boxShape.CalculateLocalInertia(mass);

            Collider = new CollisionObject();
            Collider.UserObject = element;
            Collider.CollisionShape = boxShape;
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
