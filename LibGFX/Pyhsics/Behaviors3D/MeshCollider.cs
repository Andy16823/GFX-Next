using BulletSharp;
using BulletSharp.SoftBody;
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

namespace LibGFX.Pyhsics.Behaviors3D
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

            Assimp.AssimpContext importer = new Assimp.AssimpContext();
            var model = importer.ImportFile(file, Assimp.PostProcessPreset.TargetRealTimeQuality | Assimp.PostProcessSteps.PreTransformVertices);
            var compoundShape = new CompoundShape();

            var scale = Parent.Transform.Scale;
            var btStartTransform = LibGFX.Core.Utils.GetBtTransform(Parent);

            foreach (var mesh in model.Meshes)
            {
                int[] indicies = mesh.GetIndices();
                float[] verticies = mesh.Vertices.SelectMany(v => new float[] { v.X, v.Y, v.Z }).ToArray();

                TriangleIndexVertexArray triangle = new TriangleIndexVertexArray(indicies, verticies);
                BvhTriangleMeshShape shape = new BvhTriangleMeshShape(triangle, true);

                compoundShape.AddChildShape(System.Numerics.Matrix4x4.Identity, shape);
            }

            compoundShape.LocalScaling = (System.Numerics.Vector3) scale;
            compoundShape.CalculateLocalInertia(0f);

            Collider = new CollisionObject();
            Collider.UserObject = Parent;
            Collider.CollisionShape = compoundShape;
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
