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
        /// Sets the near and far planes of the camera
        /// </summary>
        /// <param name="near"></param>
        /// <param name="far"></param>
        public void SetNearFar(float near, float far)
        {
            this.Near = near;
            this.Far = far;
        }

        /// <summary>
        /// Gets the near and far planes of the camera
        /// </summary>
        /// <returns></returns>
        public (float, float) GetNearFar()
        {
            return (this.Near, this.Far);
        }

        /// <summary>
        /// Clones the camera
        /// </summary>
        /// <returns></returns>
        public Camera Clone()
        {
            var clone = new PerspectiveCamera();
            clone.Transform.Position = this.Transform.Position;
            clone.Transform.Rotation = this.Transform.Rotation;
            clone.Transform.Scale = this.Transform.Scale;
            clone.Fov = this.Fov;
            clone.Near = this.Near;
            clone.Far = this.Far;
            return clone;
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
