using BulletSharp;
using LibGFX.Core;
using LibGFX.Graphics;
using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics
{
    /// <summary>
    /// Represents the result of a raycasting hit.
    /// </summary>
    public struct HitResult
    {
        public bool hit;
        public Vector3 rayStart;
        public Vector3 rayEnd;
        public CollisionObject collisionObject;
        public GameElement hitElement;
        public Vector3 hitLocation;
    }

    public class Raycast
    {
        private static Vector4 GetRayStart(PerspectiveCamera camera, Viewport vp, int posX, int posY)
        {
            var projectionMatrix = camera.GetProjectionMatrix(vp);
            var viewMatrix = camera.GetViewMatrix();

            float x = ((float)posX / (float)vp.Width) * 2.0f - 1.0f;
            float y = 1.0f - ((float)posY / (float)vp.Height) * 2.0f;
            var lRayStart_NDC = new Vector4(x, y, -1.0f, 1.0f);

            // Faster way (just one inverse)
            var M = (viewMatrix * projectionMatrix).Inverted();
            var lRayStart_world = lRayStart_NDC * M;
            lRayStart_world /= lRayStart_world.W;
            //glm::vec4 lRayEnd_world   = M * lRayEnd_NDC  ; lRayEnd_world  /=lRayEnd_world.w;

            return lRayStart_world;
        }

        private static Vector4 GetRayEnd(Camera camera, Viewport vp, int posX, int posY)
        {
            var projectionMatrix = camera.GetProjectionMatrix(vp);
            var viewMatrix = camera.GetViewMatrix();
                
            float x = ((float)posX / (float)vp.Width) * 2.0f - 1.0f;
            float y = 1.0f - ((float)posY / (float)vp.Height) * 2.0f;
            var lRayEnd_NDC = new Vector4(x, y, 0.0f, 1.0f);

            // Faster way (just one inverse)
            var M = (viewMatrix * projectionMatrix).Inverted();
            var lRayEnd_world = lRayEnd_NDC * M;
            lRayEnd_world /= lRayEnd_world.W;

            return lRayEnd_world;
        }

        private static Vector3 GetRayDir(Vector4 start, Vector4 end)
        {
            var lRayDir_world = (start - end).Xyz;
            lRayDir_world = lRayDir_world.Normalized();
            return lRayDir_world;
        }

        public static HitResult PerformRaycastFromScreen(PerspectiveCamera camera, Viewport viewport, PhysicsHandler3D physicHandler, int posX, int posY)
        {
            HitResult result = new HitResult();
            var btStart = GetRayStart(camera, viewport, posX, posY);
            var btEnd = GetRayEnd(camera, viewport, posX, posY);
            var direction = GetRayDir(btStart, btEnd);
            var out_end = btStart.Xyz - (direction * 1000.0f);

            var _start = (System.Numerics.Vector3) btStart.Xyz;
            var _end = (System.Numerics.Vector3) out_end;

            result.rayStart = btStart.Xyz;
            result.rayEnd = out_end;

            using (var cb = new ClosestRayResultCallback(ref  _start, ref _end))
            {
                physicHandler.PhysicsWorld.RayTest(_start, _end, cb);
                if (cb.HasHit)
                {
                    result.hit = true;
                    result.collisionObject = cb.CollisionObject;
                    result.hitLocation = (Vector3) cb.HitPointWorld;
                    result.hitElement = (GameElement)cb.CollisionObject.UserObject;
                }
            }
            return result;
        }
    }
}
