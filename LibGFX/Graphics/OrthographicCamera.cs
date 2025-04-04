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
        /// Creates a new instance of the OrthographicCamera class with the specified position and scale.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        public OrthographicCamera(Vector2 position, Vector2 scale)
        {
            this.Near = -1.0f;
            this.Far = 1.0f;
            this.Transform = new Transform(position, scale);
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
            float cameraAspectRatio = Transform.Scale.X / Transform.Scale.Y;

            if (screenAspectRatio > cameraAspectRatio)
            {
                return screenHeight / Transform.Scale.Y;
            }
            else
            {
                return screenWidth / Transform.Scale.X;
            }
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
    }
}
