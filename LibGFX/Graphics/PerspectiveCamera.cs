using Assimp;
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
            Vector3 direction = new Vector3(0f);
            direction.X = (float)(-System.Math.Sin(Math.Math.ToRadians(this.Transform.Rotation.Y)) * System.Math.Cos(Math.Math.ToRadians(this.Transform.Rotation.X)));
            direction.Y = (float)System.Math.Sin(Math.Math.ToRadians(this.Transform.Rotation.X));
            direction.Z = (float)(-System.Math.Cos(Math.Math.ToRadians(this.Transform.Rotation.Y)) * System.Math.Cos(Math.Math.ToRadians(this.Transform.Rotation.X)));
            return direction.Normalized();
        }

        /// <summary>
        /// Gets the aspect ratio of the camera
        /// </summary>
        /// <returns></returns>
        public float GetAspectRatio()
        {
            return this.Transform.Scale.X / this.Transform.Scale.Y;
        }
    }
}
