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
        /// Checks if a point is in the frustum of the camera
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract bool IsPointInFrustum(Viewport viewport, Vector3 point);

        /// <summary>
        /// Computes the axis-aligned bounding box (AABB) of the camera based on its position and orientation
        /// </summary>
        public abstract void ComputeAABB();

        /// <summary>
        /// Checks if a axis-aligned bounding box (AABB) is in the frustum of the camera
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
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
    }
}
