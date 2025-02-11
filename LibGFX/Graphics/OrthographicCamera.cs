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
    public class OrthographicCamera : Camera
    {
        public OrthographicCamera(Vector2 position, Vector2 scale)
        {
            this.Near = -1.0f;
            this.Far = 1.0f;
            this.Position = new Vector3(position);
            this.Scale = new Vector3(scale);
        }

        public float CalculateScreenCorrection(float screenWidth, float screenHeight)
        {
            return System.Math.Min(screenWidth / Scale.X, screenHeight / Scale.Y);
        }

        public override Matrix4 GetProjectionMatrix(Viewport viewport)
        {
            float correction = this.CalculateScreenCorrection(viewport.Width, viewport.Height);

            float halfWidth = (viewport.Width / 2) / correction;
            float halfHeight = (viewport.Height / 2) / correction;

            float left = this.Position.X - halfWidth;
            float right = this.Position.X + halfWidth;
            float bottom = this.Position.Y - halfHeight;
            float top = this.Position.Y + halfHeight;

            return Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, Near, Far);
        }

        public override Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 0f), new Vector3(0f, 1f, 0f));
        }
    }
}
