using BulletSharp;
using BulletSharp.SoftBody;
using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Math;
using OpenTK.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BulletSharp.Dbvt;

namespace LibGFX.Physics.Behaviors3D
{
    /// <summary>
    /// Represents a 3D mesh collider
    /// This collider creates an static mesh collider from the mesh of an 3D model File
    /// This MeshCollider works with an non-uniform scale
    /// </summary>
    public class MeshCollider : CollisionBehavior
    {
        /// <summary>
        /// Creates a new 3D mesh collider
        /// </summary>
        /// <param name="physicsHandler"></param>
        public MeshCollider(PhysicsHandler physicsHandler) : base(physicsHandler)
        {

        }

        /// <summary>
        /// Creates a mesh collider with the given mass, collision group and collision mask
        /// </summary>
        /// <param name="mass"></param>
        /// <param name="file"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public void CreateCollider(float mass, String file, int collisionGroup = -1, int collisionMask = -1)
        {
            var scale = Parent.Transform.Scale;
            var btStartTransform = LibGFX.Core.Utils.GetBtTransform(Parent);

            var compoundShape = BtCollisionShapeBuilder.BuildMeshShape(file, (System.Numerics.Vector3) scale);
            compoundShape.CalculateLocalInertia(0f);

            Collider = new CollisionObject();
            Collider.UserObject = Parent;
            Collider.CollisionShape = compoundShape;
            Collider.WorldTransform = btStartTransform;
            PhysicsHandler.ManageElement(this, collisionGroup, collisionMask);
        }
    }
}
