using LibGFX.Graphics;
using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Physics
{
    /// <summary>
    /// Class for performing raycasts against a mesh without using a physics engine.
    /// </summary>
    public class MeshRaycast
    {
        /// <summary>
        /// Performs a raycast against a mesh, first checking the AABB for a quick rejection.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="transform"></param>
        /// <param name="aabb"></param>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HitResult PerformRaycast(Ray ray, Transform transform, AABB aabb, Mesh mesh)
        {
            var intersectsAABB = IntersectsAABB(ray, aabb, out float tMin, out float tMax);
            if (intersectsAABB)
            {
                return IntersectsMesh(ray, transform, mesh);
            }

            return new HitResult
            {
                hit = false
            };
        }


        /// <summary>
        /// Checks if a ray intersects with a mesh by testing each triangle.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="transform"></param>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HitResult IntersectsMesh(Ray ray, Transform transform, Mesh mesh)
        {
            var finalMatrix = mesh.GetTransform() * transform.GetMatrix();
            float closestT = float.MaxValue;

            HitResult hit = new HitResult { 
                hit = false,
            };

            for (int i = 0; i < mesh.Indices.Count; i += 3)
            {
                var v0 = Vector3.TransformPosition(mesh.Positions[mesh.Indices[i]], finalMatrix);
                var v1 = Vector3.TransformPosition(mesh.Positions[mesh.Indices[i + 1]], finalMatrix);
                var v2 = Vector3.TransformPosition(mesh.Positions[mesh.Indices[i + 2]], finalMatrix);

                if (RayIntersectsTriangle(ray, v0, v1, v2, out float t, out Vector3 normal))
                {
                    if (t < closestT)
                    {
                        closestT = t;
                        hit = new HitResult
                        {
                            hit = true,
                            hitLocation = ray.Origin + ray.Direction * t,
                            hitNormal = normal,
                            hitTriangleIndex = i / 3,
                            rayStart = ray.Origin,
                            rayEnd = ray.Origin + ray.Direction * t,
                            hitDistance = t,
                        };
                    }
                }
            }
            return hit;
        }


        /// <summary>
        /// Checks if a ray intersects with an Axis-Aligned Bounding Box (AABB).
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="aabb"></param>
        /// <param name="tMin"></param>
        /// <param name="tMax"></param>
        /// <returns></returns>
        public static bool IntersectsAABB(Ray ray, AABB aabb, out float tMin, out float tMax)
        {
            tMin = float.MinValue;
            tMax = float.MaxValue;

            for (int i = 0; i < 3; i++)
            {
                float invD = 1.0f / ray.Direction[i];
                float t0 = (aabb.Min[i] - ray.Origin[i]) * invD;
                float t1 = (aabb.Max[i] - ray.Origin[i]) * invD;
                if (invD < 0.0f)
                {
                    var temp = t0;
                    t0 = t1;
                    t1 = temp;
                }
                tMin = System.Math.Max(tMin, t0);
                tMax = System.Math.Min(tMax, t1);
                if (tMax < tMin)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a ray intersects with a triangle using the Möller–Trumbore algorithm.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="t"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        private static bool RayIntersectsTriangle(Ray ray, Vector3 v0, Vector3 v1, Vector3 v2, out float t, out Vector3 normal)
        {
            t = 0;
            normal = Vector3.Zero;

            const float EPSILON = 1e-5f;
            Vector3 edge1 = v1 - v0;
            Vector3 edge2 = v2 - v0;
            Vector3 h = Vector3.Cross(ray.Direction, edge2);
            float a = Vector3.Dot(edge1, h);

            if (System.Math.Abs(a) < EPSILON)
                return false; // Parallel

            float f = 1.0f / a;
            Vector3 s = ray.Origin - v0;
            float u = f * Vector3.Dot(s, h);
            if (u < 0.0f || u > 1.0f)
                return false;

            Vector3 q = Vector3.Cross(s, edge1);
            float v = f * Vector3.Dot(ray.Direction, q);
            if (v < 0.0f || u + v > 1.0f)
                return false;

            t = f * Vector3.Dot(edge2, q);
            if (t > EPSILON)
            {
                normal = Vector3.Normalize(Vector3.Cross(edge1, edge2));
                // **Backface-Culling (optional):**
                // if (Vector3.Dot(ray.Direction, normal) > 0) return false;

                // **Extra Logging:**
                //var p = ray.Origin + ray.Direction * t;
                //Debug.WriteLine($"TriangleHit: t={t}, u={u}, v={v}, p={p}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a world space ray from screen coordinates.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="viewport"></param>
        /// <param name="mouseX"></param>
        /// <param name="mouseY"></param>
        /// <returns></returns>
        [Obsolete("Use Ray.FromScreenPoint instead.")]
        public static Ray ScreenPointToWorldRay(PerspectiveCamera camera, Viewport viewport, float mouseX, float mouseY)
        {
            // 1. Mauskoordinaten normalisieren (NDC: -1 bis +1)
            float x = (2.0f * mouseX) / viewport.Width - 1.0f;
            float y = 1.0f - (2.0f * mouseY) / viewport.Height; // ACHTUNG: Y-Flip!

            // 2. NDC → Clip Space
            Vector4 rayClip = new Vector4(x, y, -1.0f, 1.0f);

            // 3. Clip Space → Eye (Camera) Space
            Matrix4 invProjection = Matrix4.Invert(camera.GetProjectionMatrix(viewport));
            Vector4 rayEye = rayClip * invProjection;
            rayEye = new Vector4(rayEye.X, rayEye.Y, -1.0f, 0.0f);

            // 4. Eye → World Space
            Matrix4 invView = Matrix4.Invert(camera.GetViewMatrix());
            Vector4 rayWorld4 = rayEye * invView;
            Vector3 rayDirWorld = Vector3.Normalize(rayWorld4.Xyz);

            // 5. Ray origin ist Kamera-Position (im World Space)
            Vector3 rayOrigin = camera.Transform.Position;

            // 6. Erstelle den Ray
            return new Ray(rayOrigin, rayDirWorld);
        }
    }
}
