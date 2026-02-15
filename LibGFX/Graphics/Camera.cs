using LibGFX.Core;
using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public abstract class Camera
    {
        /// <summary>
        /// The current camera
        /// </summary>
        public static Camera Current { get; set; }

        /// <summary>
        /// The near plane of the camera
        /// </summary>
        public float Near { get; set; }

        /// <summary>
        /// The far plane of the camera
        /// </summary>
        public float Far { get; set; }

        /// <summary>
        /// The resolution of the camera
        /// </summary>
        public Vector2 Resolution { get; set; }

        /// <summary>
        /// The axis-aligned bounding box (AABB) of the camera, used for frustum culling
        /// </summary>
        public AABB AABB { get; set; }

        /// <summary>
        /// The transform of the camera, which includes position, rotation, and scale
        /// </summary>
        public Transform Transform { get; set; } = new Transform();

        /// <summary>
        /// Gets the view matrix of the camera
        /// </summary>
        /// <returns></returns>
        public abstract Matrix4 GetViewMatrix();

        /// <summary>
        /// Gets the projection matrix of the camera
        /// </summary>
        /// <param name="viewport"></param>
        /// <returns></returns>
        public abstract Matrix4 GetProjectionMatrix(Viewport viewport);

        /// <summary>
        /// Computes the axis-aligned bounding box (AABB) of the camera based on its position and orientation
        /// </summary>
        public abstract void ComputeAABB();

        /// <summary>
        /// Checks if a point is in the frustum of the camera
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        [Obsolete("Use Frustum specific methods for point containment checks.")]
        public abstract bool IsPointInFrustum(Viewport viewport, Vector3 point);

        /// <summary>
        /// Checks if a axis-aligned bounding box (AABB) is in the frustum of the camera
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [Obsolete("Use Frustum specific methods for AABB containment checks.")]
        public abstract bool IsAABBInFrustum(Viewport viewport, Vector3 min, Vector3 max);

        /// <summary>
        /// Looks at a target point in 3D space
        /// </summary>
        /// <param name="target"></param>
        public abstract void LookAt(Vector3 target);

        /// <summary>
        /// Gets the aspect ratio of the camera
        /// </summary>
        /// <returns></returns>
        public float GetAspectRatio()
        {
            return Resolution.X / Resolution.Y;
        }

        /// <summary>
        /// Sets the camera as the current camera
        /// </summary>
        public void SetAsCurrent()
        {
            Current = this;
        }

        /// <summary>
        /// Calculates and returns the view frustum for the camera using the specified viewport and normalization
        /// option.
        /// </summary>
        /// <remarks>This method combines the camera's view and projection matrices to derive the frustum
        /// planes. Normalizing the planes can improve the accuracy of subsequent geometric calculations, such as
        /// intersection tests.</remarks>
        /// <param name="viewport">The viewport that defines the visible area of the scene and is used to compute the projection matrix.</param>
        /// <param name="normalize">true to normalize the frustum planes to unit length; otherwise, false.</param>
        /// <returns>A Frustum object representing the six planes (left, right, bottom, top, near, and far) that define the
        /// camera's view frustum.</returns>
        public Frustum GetFrustum(Viewport viewport, bool normalize = true)
        {
            var viewMatrix = this.GetViewMatrix();
            var projectionMatrix = this.GetProjectionMatrix(viewport);
            var vp = viewMatrix * projectionMatrix;

            // Left
            var left = new Plane(
                vp.M14 + vp.M11,
                vp.M24 + vp.M21,
                vp.M34 + vp.M31,
                vp.M44 + vp.M41
            );

            // Right
            var right = new Plane(
                vp.M14 - vp.M11,
                vp.M24 - vp.M21,
                vp.M34 - vp.M31,
                vp.M44 - vp.M41
            );

            // Bottom
            var bottom = new Plane(
                vp.M14 + vp.M12,
                vp.M24 + vp.M22,
                vp.M34 + vp.M32,
                vp.M44 + vp.M42
            );

            // Top
            var top = new Plane(
                vp.M14 - vp.M12,
                vp.M24 - vp.M22,
                vp.M34 - vp.M32,
                vp.M44 - vp.M42
            );

            // Near
            var near = new Plane(
                vp.M13,
                vp.M23,
                vp.M33,
                vp.M43
            );

            // Far
            var far = new Plane(
                vp.M14 - vp.M13,
                vp.M24 - vp.M23,
                vp.M34 - vp.M33,
                vp.M44 - vp.M43
            );

            var Frustum = new Frustum()
            {
                Left = left,
                Right = right,
                Bottom = bottom,
                Top = top,
                Near = near,
                Far = far
            };

            if (normalize)
            {
                return Frustum.Normalized(Frustum);
            }

            return Frustum;
        }
    }
}
