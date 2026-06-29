using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents an orthographic camera.
    /// </summary>
    public class OrthographicCamera : Camera
    {
        /// <summary>
        /// Creates a new instance of the OrthographicCamera class with the specified position and resolution.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="resolution"></param>
        public OrthographicCamera(Vector2 position, Vector2 resolution)
        {
            this.Near = -1.0f;
            this.Far = 1.0f;
            this.Transform.Position = new Vector3(position.X, position.Y, 0f);
            this.Resolution = resolution;
        }

        /// <summary>
        /// Calculates the screen correction factor based on the screen width and height.
        /// </summary>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        /// <returns></returns>
        public float CalculateScreenCorrection(float screenWidth, float screenHeight)
        {
            //return System.Math.Min(screenWidth / Transform.Scale.X, screenHeight / Transform.Scale.Y);
            float screenAspectRatio = screenWidth / screenHeight;
            float cameraAspectRatio = Resolution.X / Resolution.Y;

            if (screenAspectRatio > cameraAspectRatio)
            {
                return screenHeight / Resolution.Y;
            }
            else
            {
                return screenWidth / Resolution.X;
            }
        }

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
        /// Gets the projection matrix of the camera based on the viewport dimensions.
        /// </summary>
        /// <param name="viewport"></param>
        /// <returns></returns>
        public override Matrix4 GetProjectionMatrix(Viewport viewport)
        {
            float correction = this.CalculateScreenCorrection(viewport.Width, viewport.Height);

            float halfWidth = (viewport.Width / 2) / correction;
            float halfHeight = (viewport.Height / 2) / correction;

            float left = this.Transform.Position.X - halfWidth;
            float right = this.Transform.Position.X + halfWidth;
            float bottom = this.Transform.Position.Y - halfHeight;
            float top = this.Transform.Position.Y + halfHeight;

            return Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, Near, Far);
        }

        /// <summary>
        /// Gets the view matrix of the camera.
        /// </summary>
        /// <returns></returns>
        public override Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 0f), new Vector3(0f, 1f, 0f));
        }

        /// <summary>
        /// Looks at a target point in 3D space by setting the camera's position to the target.
        /// </summary>
        /// <param name="target"></param>
        public override void LookAt(Vector3 target)
        {
            this.Transform.Position = target;
        }

        /// <summary>
        /// Converts screen coordinates to world coordinates in 2D space.
        /// </summary>
        /// <param name="screenX"></param>
        /// <param name="screenY"></param>
        /// <param name="viewport"></param>
        /// <returns></returns>
        public Vector2 ScreenToWorld2D(float screenX, float screenY, Viewport viewport)
        {
            float ndcX = (screenX / viewport.Width) * 2 - 1;
            float ndcY = 1 - (screenY / viewport.Height) * 2; // OpenGL's Y-axis is inverted
            float correction = this.CalculateScreenCorrection(viewport.Width, viewport.Height);

            var clipCoordinates = new Vector4(ndcX, ndcY, 0, 1);
            var viewMatrix = this.GetViewMatrix();
            var projectionMatrix = this.GetProjectionMatrix(viewport);
            var inverseProjectionView = Matrix4.Invert(viewMatrix * projectionMatrix);

            var worldCoordinates = Vector4.TransformRow(clipCoordinates, inverseProjectionView);

            if (worldCoordinates.W != 0.0f)
            {
                worldCoordinates.X /= worldCoordinates.W;
                worldCoordinates.Y /= worldCoordinates.W;
            }

            return new Vector2(worldCoordinates.X, worldCoordinates.Y);
        }
    }
}
