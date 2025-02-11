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
    public class PerspectiveCamera : Camera
    {
        public float Fov { get; set; } = 45.0f;

        public PerspectiveCamera()
        {
            this.Near = 0.1f;
            this.Far = 1000.0f;
        }

        public override Matrix4 GetProjectionMatrix(Viewport viewport)
        {
            var fov = Math.Math.ToRadians(this.Fov);
            var aspect = this.GetAspectRatio();

            return Matrix4.CreatePerspectiveFieldOfView(fov, aspect, Near, Far);
        }

        public override Matrix4 GetViewMatrix()
        {
            var front = this.GetCameraFront();
            var frontPosition = this.Position + front;

            return Matrix4.LookAt(this.Position, frontPosition, new Vector3(0.0f, 1.0f, 0.0f));
        }

        public Vector3 GetCameraFront()
        {
            Vector3 direction = new Vector3(0f);
            direction.X = (float)(System.Math.Cos(Math.Math.ToRadians(this.Rotation.Y)) * System.Math.Cos(Math.Math.ToRadians(this.Rotation.X)));
            direction.Y = (float)System.Math.Sin(Math.Math.ToRadians(this.Rotation.X));
            direction.Z = (float)(System.Math.Sin(Math.Math.ToRadians(this.Rotation.Y)) * System.Math.Cos(Math.Math.ToRadians(this.Rotation.X)));
            return direction.Normalized();
        }

        public float GetAspectRatio()
        {
            return this.Scale.X / this.Scale.Y;
        }
    }
}
