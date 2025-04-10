using BulletSharp;
using LibGFX.Core;
using LibGFX.Core.GameElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics.Behaviors3D
{
    public class MeshCollider : CollisionBehavior
    {
        /// <summary>
        /// Creates a new 3D box collider
        /// </summary>
        /// <param name="physicsHandler"></param>
        public MeshCollider(PhysicsHandler physicsHandler) : base(physicsHandler)
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
            var model = (Model)this.Parent;
            if(model.Meshes.Count == 0)
            {
                throw new Exception("Model has no meshes");
            }


            var compoundShape = new CompoundShape();

            foreach (var mesh in model.Meshes)
            {
                var indices = mesh.Indices.ToArray();
                var vertices = mesh.Vertices.SelectMany(v => new float[] { v.Position.X, v.Position.Y, v.Position.Z }).ToArray();
                var triangleShape = new BvhTriangleMeshShape(new TriangleIndexVertexArray(indices, vertices), true);

                var meshTransform = Utils.GetBtTransform((System.Numerics.Vector3) mesh.LocalTranslation, (System.Numerics.Quaternion) mesh.LocalRotation);

                compoundShape.AddChildShape(meshTransform, triangleShape);
            }

            var btStartTransform = Utils.GetBtTransform(model, this.Offset);
            compoundShape.CalculateLocalInertia(mass);

            Collider = new CollisionObject();
            Collider.UserObject = model;
            Collider.CollisionShape = compoundShape;
            Collider.WorldTransform = btStartTransform;
            Collider.CollisionShape.LocalScaling = (System.Numerics.Vector3) model.Transform.Scale;
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
