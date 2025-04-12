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
        public PerspectiveCamera(Vector3 position, Vector3 scale)
        {
            this.Near = 0.1f;
            this.Far = 1000.0f;
            this.Transform.Position = position;
            this.Transform.Scale = scale;
        }

        /// <summary>
        /// Gets the projection matrix of the camera
        /// </summary>
        /// <param name="viewport"></param>
        /// <returns></returns>
        public override Matrix4 GetProjectionMatrix(Viewport viewport)
        {
            var fov = Math.Math.ToRadians(this.Fov);
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

        public static Quaternion LookAtQuaternion(Vector3 cameraPosition, Vector3 targetPosition, Vector3 up)
        {
            Vector3 direction = targetPosition - cameraPosition;
            direction = Vector3.Normalize(direction);

            Vector3 front = -Vector3.UnitZ;

            float angle = (float)System.Math.Acos(Vector3.Dot(front, direction));
            Vector3 axis = Vector3.Cross(front, direction);

            if (axis.LengthSquared < float.Epsilon)
            {
                axis = up;
            }
            axis = Vector3.Normalize(axis);

            Quaternion rotation = Quaternion.FromAxisAngle(axis, angle);

            return rotation;
        }

        /// <summary>
        /// Rotates the camera to look at a specific target point.
        /// </summary>
        /// <param name="target">The world-space position to look at.</param>
        public void LookAt(Vector3 target)
        {
            Vector3 direction = Vector3.Normalize(this.Transform.Position - target); // Richtung ZUM Ziel
            Vector3 up = Vector3.UnitY;

            // Prüfen auf Gimbal Lock
            if (MathF.Abs(Vector3.Dot(direction, up)) > 0.999f)
                up = Vector3.UnitZ;

            this.Transform.Rotation = LookRotation(direction, up);
        }

        private static Quaternion LookRotation(Vector3 forward, Vector3 up)
        {
            forward = Vector3.Normalize(forward);
            Vector3 right = Vector3.Normalize(Vector3.Cross(up, forward));
            Vector3 correctedUp = Vector3.Cross(forward, right);

            Matrix3 rotationMatrix = new Matrix3(
                right.X, correctedUp.X, forward.X,
                right.Y, correctedUp.Y, forward.Y,
                right.Z, correctedUp.Z, forward.Z
            );

            return Quaternion.FromMatrix(rotationMatrix);
        }
    }
}
