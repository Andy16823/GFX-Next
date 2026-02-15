using LibGFX.Math;
using OpenTK.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents a perspective camera
    /// </summary>
    public class PerspectiveCamera : Camera
    {
        /// <summary>
        /// The field of view of the camera
        /// </summary>
        public float Fov { get; set; } = 45.0f;

        /// <summary>
        /// Creates a new perspective camera
        /// </summary>
        public PerspectiveCamera()
        {
            this.Near = 0.1f;
            this.Far = 1000.0f;
        }

        /// <summary>
        /// Creates a new perspective camera
        /// </summary>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        public PerspectiveCamera(Vector3 position, Vector2 resolution)
        {
            this.Near = 0.1f;
            this.Far = 1000.0f;
            this.Transform.Position = position;
            this.Resolution = resolution;
        }

        /// <summary>
        /// Gets the projection matrix of the camera
        /// </summary>
        /// <param name="viewport"></param>
        /// <returns></returns>
        public override Matrix4 GetProjectionMatrix(Viewport viewport)
        {
            var fov = Math.MathUtils.ToRadians(this.Fov);
            var aspect = this.GetAspectRatio();

            return Matrix4.CreatePerspectiveFieldOfView(fov, aspect, Near, Far);
        }

        /// <summary>
        /// Gets the view matrix of the camera
        /// </summary>
        /// <returns></returns>
        public override Matrix4 GetViewMatrix()
        {
            var front = this.GetCameraFront();
            var frontPosition = this.Transform.Position + front;

            return Matrix4.LookAt(this.Transform.Position, frontPosition, new Vector3(0.0f, 1.0f, 0.0f));
        }

        /// <summary>
        /// Gets the front of the camera
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCameraFront()
        {
            return this.Transform.GetFront();
        }

        /// <summary>
        /// Converts a screen position to a world position
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="viewport"></param>
        /// <param name="sX"></param>
        /// <param name="sY"></param>
        /// <returns></returns>
        public static Vector3 ScreenToWorldPosition3D(PerspectiveCamera camera, Viewport viewport, float sX, float sY)
        {
            var projectionMatrix = camera.GetProjectionMatrix(viewport);
            var viewMatrix = camera.GetViewMatrix();

            float x = ((float)sX / (float)viewport.Width) * 2.0f - 1.0f;
            float y = 1.0f - ((float)sY / (float)viewport.Height) * 2.0f;
            var ndc = new Vector4(x, y, -1.0f, 1.0f);

            // Faster way (just one inverse)
            Matrix4 m = (projectionMatrix * viewMatrix).Inverted();
            Vector4 world = m * ndc;
            world /= world.W;

            return world.Xyz;
        }

        /// <summary>
        /// Looks at the target position
        /// </summary>
        /// <param name="target"></param>
        public override void LookAt(Vector3 target)
        {
            this.Transform.Towards(target);
        }

        /// <summary>
        /// Checks if a point is in the frustum of the camera
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        /// TODO: Remove this method
        [Obsolete("Use Frustum.ContainsPoint instead for better performance")]
        public override bool IsPointInFrustum(Viewport viewport, Vector3 point)
        {
            var projectionMatrix = this.GetProjectionMatrix(viewport);
            var viewMatrix = this.GetViewMatrix();

            Matrix4 viewProjection = viewMatrix * projectionMatrix;
            Vector4 clipSpacePos = new Vector4(point, 1.0f) * viewProjection;

            if (clipSpacePos.W == 0.0f)
                return false;

            clipSpacePos.X /= clipSpacePos.W;
            clipSpacePos.Y /= clipSpacePos.W;
            clipSpacePos.Z /= clipSpacePos.W;

            return
                clipSpacePos.X >= -1.0f && clipSpacePos.X <= 1.0f &&
                clipSpacePos.Y >= -1.0f && clipSpacePos.Y <= 1.0f &&
                clipSpacePos.Z >= -1.0f && clipSpacePos.Z <= 1.0f;
        }

        /// <summary>
        /// Checks if a axis-aligned bounding box (AABB) is in the frustum of the camera
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [Obsolete("Use Frustum.ContainsAABB instead for better performance")]
        public override bool IsAABBInFrustum(Viewport viewport, Vector3 min, Vector3 max)
        {
            var projectionMatrix = GetProjectionMatrix(viewport);
            var viewMatrix = GetViewMatrix();
            Matrix4 viewProjection = viewMatrix * projectionMatrix;

            var planes = ExtractFrustumPlanes(viewProjection);

            return IntersectsFrustum(planes, min, max);
        }

        /// <summary>
        /// Extracts the frustum planes from the view-projection matrix
        /// </summary>
        /// <param name="vp"></param>
        /// <returns></returns>
        [Obsolete("Use Frustum.GetPlanes instead for better performance")]
        public static Plane[] ExtractFrustumPlanes(Matrix4 vp)
        {
            Plane[] planes = new Plane[6];

            // Left
            planes[0] = new Plane(
                vp.M14 + vp.M11,
                vp.M24 + vp.M21,
                vp.M34 + vp.M31,
                vp.M44 + vp.M41
            );

            // Right
            planes[1] = new Plane(
                vp.M14 - vp.M11,
                vp.M24 - vp.M21,
                vp.M34 - vp.M31,
                vp.M44 - vp.M41
            );

            // Bottom
            planes[2] = new Plane(
                vp.M14 + vp.M12,
                vp.M24 + vp.M22,
                vp.M34 + vp.M32,
                vp.M44 + vp.M42
            );

            // Top
            planes[3] = new Plane(
                vp.M14 - vp.M12,
                vp.M24 - vp.M22,
                vp.M34 - vp.M32,
                vp.M44 - vp.M42
            );

            // Near
            planes[4] = new Plane(
                vp.M13,
                vp.M23,
                vp.M33,
                vp.M43
            );

            // Far
            planes[5] = new Plane(
                vp.M14 - vp.M13,
                vp.M24 - vp.M23,
                vp.M34 - vp.M33,
                vp.M44 - vp.M43
            );

            // Normalize planes
            for (int i = 0; i < 6; i++)
            {
                float length = planes[i].Normal.Length;
                planes[i].Normal /= length;
                planes[i].D /= length;
            }

            return planes;
        }

        /// <summary>
        /// Checks if a bounding box intersects with the frustum defined by the planes
        /// </summary>
        /// <param name="planes"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [Obsolete("Use Frustum.ContainsAABB instead for better performance")]
        public static bool IntersectsFrustum(Plane[] planes, Vector3 min, Vector3 max)
        {
            foreach (var plane in planes)
            {
                Vector3 positiveVertex = new Vector3(
                    plane.Normal.X >= 0 ? max.X : min.X,
                    plane.Normal.Y >= 0 ? max.Y : min.Y,
                    plane.Normal.Z >= 0 ? max.Z : min.Z
                );

                if (Vector3.Dot(plane.Normal, positiveVertex) + plane.D < 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Computes the axis-aligned bounding box (AABB) of the camera
        /// </summary>
        public override void ComputeAABB()
        {
            var min = new Vector3(
                this.Transform.Position.X - (this.Transform.Scale.X / 2),
                this.Transform.Position.Y - (this.Transform.Scale.Y / 2),
                Near
            );

            var max = new Vector3(
                this.Transform.Position.X + (this.Transform.Scale.X / 2),
                this.Transform.Position.Y + (this.Transform.Scale.Y / 2),
                Far
            );

            this.AABB = new AABB(min, max);
        }

        /// <summary>
        /// Projects a world position to screen coordinates
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="worldPos"></param>
        /// <param name="viewport"></param>
        /// <returns></returns>
        public static Vector3 WorldToScreen(PerspectiveCamera camera, Vector3 worldPos, Viewport viewport)
        {
            var viewMatrix = camera.GetViewMatrix();
            var projectionMatrix = camera.GetProjectionMatrix(viewport);
            Vector4 clipSpacePos = new Vector4(worldPos, 1.0f) * viewMatrix * projectionMatrix;

            if (clipSpacePos.W == 0)
                clipSpacePos.W = 0.0001f;

            var ndc = new Vector3(clipSpacePos.X, clipSpacePos.Y, clipSpacePos.Z) / clipSpacePos.W;
            float x = ((ndc.X + 1.0f) / 2.0f) * viewport.Width;
            float y = ((1.0f - ndc.Y) / 2.0f) * viewport.Height;

            return new Vector3(x, y, ndc.Z);
        }
    }
}
