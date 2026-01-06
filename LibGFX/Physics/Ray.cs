using LibGFX.Graphics;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Physics
{
    /// <summary>
    /// Represents a ray in three-dimensional space, defined by an origin point and a direction vector.
    /// </summary>
    /// <remarks>A ray is commonly used in graphics and physics calculations to represent a line with a
    /// specific starting point and direction, extending infinitely. The direction vector should be normalized to ensure
    /// consistent behavior in intersection and projection operations.</remarks>
    public struct Ray
    {
        public Vector3 Origin;
        public Vector3 Direction;
        public Ray(Vector3 origin, Vector3 direction)
        {
            this.Origin = origin;
            this.Direction = direction;
        }

        public static Ray FromScreenPoint(PerspectiveCamera camera, Viewport viewport, float mouseX, float mouseY)
        {
            // 1. Mauskoordinaten normalisieren (NDC: -1 bis +1)
            float x = (2.0f * mouseX) / viewport.Width - 1.0f;
            float y = 1.0f - (2.0f * mouseY) / viewport.Height; // ACHTUNG: Y-Flip!

            // 2. NDC → Clip Space
            Vector4 rayClip = new Vector4(x, y, -1.0f, 1.0f);

            // 3. Clip Space → Eye (Camera) Space
            Matrix4 invProjection = Matrix4.Invert(camera.GetProjectionMatrix(viewport));
            Vector4 rayEye = rayClip * invProjection;
            rayEye = new Vector4(rayEye.X, rayEye.Y, -1.0f, 0.0f);

            // 4. Eye → World Space
            Matrix4 invView = Matrix4.Invert(camera.GetViewMatrix());
            Vector4 rayWorld4 = rayEye * invView;
            Vector3 rayDirWorld = Vector3.Normalize(rayWorld4.Xyz);

            // 5. Ray origin ist Kamera-Position (im World Space)
            Vector3 rayOrigin = camera.Transform.Position;

            // 6. Erstelle den Ray
            return new Ray(rayOrigin, rayDirWorld);
        }
    }
}
