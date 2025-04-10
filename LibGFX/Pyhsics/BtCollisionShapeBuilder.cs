using Assimp;
using BulletSharp;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics
{
    internal class BtCollisionShapeBuilder
    {
        public static CollisionShape BuildMeshShape(string filePath, System.Numerics.Vector3 scale)
        {
            var importer = new AssimpContext();
            var model = importer.ImportFile(filePath,
                PostProcessPreset.TargetRealTimeQuality |
                PostProcessSteps.PreTransformVertices);

            var compoundShape = new CompoundShape();

            foreach (var mesh in model.Meshes)
            {
                int[] indices = mesh.GetIndices();
                float[] vertices = mesh.Vertices.SelectMany(v => new float[] { v.X, v.Y, v.Z }).ToArray();

                var triangleArray = new TriangleIndexVertexArray(indices, vertices);
                var triangleShape = new BvhTriangleMeshShape(triangleArray, true);

                // Child transform is identity because the mesh is pre-transformed
                compoundShape.AddChildShape(System.Numerics.Matrix4x4.Identity, triangleShape);
            }

            // Apply global non-uniform scale safely
            compoundShape.LocalScaling = scale;

            return compoundShape;
        }


        public static ConvexHullShape BuildConvexHull(string filePath, System.Numerics.Vector3 scale)
        {
            var importer = new AssimpContext();
            var model = importer.ImportFile(filePath,
                PostProcessPreset.TargetRealTimeQuality |
                PostProcessSteps.PreTransformVertices);

            var allVertices = model.Meshes
                .SelectMany(mesh => mesh.Vertices)
                .Select(v => new Vector3(v.X * scale.X, v.Y * scale.Y, v.Z * scale.Z))
                .ToList();

            var convexShape = new ConvexHullShape();

            foreach (var v in allVertices)
            {
                convexShape.AddPoint(v);
            }
                
            convexShape.OptimizeConvexHull();
            convexShape.RecalcLocalAabb();

            return convexShape;
        }

    }
}
