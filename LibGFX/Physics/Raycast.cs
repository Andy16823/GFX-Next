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

namespace LibGFX.Physics
{
    public class Raycast
    {
        public static HitResult PerformRaycast(Ray ray, PhysicsHandler physicsHandler, float maxDistance = 1000.0f)
        {
            //HitResult result = new HitResult();
            //var rayStart = (System.Numerics.Vector3)ray.Origin;
            //var rayEnd = (System.Numerics.Vector3)(ray.Origin + ray.Direction * maxDistance);

            //result.rayStart = ray.Origin;
            //result.rayEnd = ray.Origin + ray.Direction * maxDistance;
            //using (var cb = new ClosestRayResultCallback(ref rayStart, ref rayEnd))
            //{
            //    physicsHandler.PhysicsWorld.RayTest(rayStart, rayEnd, cb);
            //    if (cb.HasHit)
            //    {
            //        result.hit = true;
            //        result.hitLocation = (Vector3)cb.HitPointWorld;
            //        result.hitElement = (GameElement)cb.CollisionObject.UserObject;
            //    }
            //}
            //return result;
            return physicsHandler.RayTest(ray.Origin, ray.Origin + ray.Direction * maxDistance);
        }

        [Obsolete("Use PerformRaycast with Ray parameter instead.")]
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

        [Obsolete("Use PerformRaycast with Ray parameter instead.")]
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

        [Obsolete("Use PerformRaycast with Ray parameter instead.")]
        private static Vector3 GetRayDir(Vector4 start, Vector4 end)
        {
            var lRayDir_world = (start - end).Xyz;
            lRayDir_world = lRayDir_world.Normalized();
            return lRayDir_world;
        }

        [Obsolete("Use PerformRaycast with Ray parameter instead.")]
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
                    result.hitLocation = (Vector3) cb.HitPointWorld;
                    result.hitElement = (GameElement)cb.CollisionObject.UserObject;
                }
            }
            return result;
        }
    }
}
