using LibGFX.Graphics;
using LibGFX.Math;
using Microsoft.VisualBasic.Devices;
using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mesh = LibGFX.Graphics.Mesh;

namespace NewGFXEditor
{
    //public struct MeshRayHit
    //{
    //    public bool Hit;
    //    public float Distance;
    //    public Vector3 Position;
    //    public Vector3 Normal;
    //    public int TriangleIndex;
    //}

    //public struct MeshRay
    //{
    //    public Vector3 Origin;
    //    public Vector3 Direction;

    //    public MeshRay(Vector3 origin, Vector3 direction)
    //    {
    //        Origin = origin;
    //        Direction = direction.Normalized();
    //    }
    //}

    //public class MeshRaycaster
    //{
    //    public static MeshRayHit PerformRaycast(MeshRay ray, Transform transform, AABB aabb, Mesh mesh)
    //    {
    //        var intersectsAABB = MeshRaycaster.IntersectsAABB(ray, aabb, out float tMin, out float tMax);
    //        if (intersectsAABB)
    //        {
    //            return IntersectsMesh(ray, transform, mesh);
    //        }

    //        return new MeshRayHit
    //        {
    //            Hit = false

    //        };
    //    }


    //    public static MeshRayHit IntersectsMesh(MeshRay ray, Transform transform, Mesh mesh)
    //    {
    //        var finalMatrix = mesh.GetTransform() * transform.GetMatrix();
    //        float closestT = float.MaxValue;
    //        MeshRayHit hit = new MeshRayHit { Hit = false };

    //        for (int i = 0; i < mesh.Indices.Count; i += 3)
    //        {
    //            var v0 = Vector3.TransformPosition(mesh.Vertices[mesh.Indices[i]].Position, finalMatrix);
    //            var v1 = Vector3.TransformPosition(mesh.Vertices[mesh.Indices[i + 1]].Position, finalMatrix);
    //            var v2 = Vector3.TransformPosition(mesh.Vertices[mesh.Indices[i + 2]].Position, finalMatrix);
    //            //Debug.WriteLine($"Triangle {i / 3}: v0={v0}, v1={v1}, v2={v2}");
    //            if (RayIntersectsTriangle(ray, v0, v1, v2, out float t, out Vector3 normal))
    //            {
    //                if (t < closestT)
    //                {
    //                    closestT = t;
    //                    hit = new MeshRayHit
    //                    {
    //                        Hit = true,
    //                        Distance = t,
    //                        Position = ray.Origin + ray.Direction * t,
    //                        Normal = normal,
    //                        TriangleIndex = i / 3
    //                    };
    //                }
    //            }
    //        }
    //        return hit;
    //    }

    //    public static bool IntersectsAABB(MeshRay ray, AABB aabb, out float tMin, out float tMax)
    //    {
    //        tMin = float.MinValue;
    //        tMax = float.MaxValue;

    //        for (int i = 0; i < 3; i++)
    //        {
    //            float invD = 1.0f / ray.Direction[i];
    //            float t0 = (aabb.Min[i] - ray.Origin[i]) * invD;
    //            float t1 = (aabb.Max[i] - ray.Origin[i]) * invD;
    //            if (invD < 0.0f)
    //            {
    //                var temp = t0;
    //                t0 = t1;
    //                t1 = temp;
    //            }
    //            tMin = System.Math.Max(tMin, t0);
    //            tMax = System.Math.Min(tMax, t1);
    //            if (tMax < tMin)
    //                return false;
    //        }
    //        return true;
    //    }

    //    public static MeshRay ScreenPointToWorldRay(PerspectiveCamera camera, Viewport viewport, float mouseX, float mouseY)
    //    {
    //        // 1. Mauskoordinaten normalisieren (NDC: -1 bis +1)
    //        float x = (2.0f * mouseX) / viewport.Width - 1.0f;
    //        float y = 1.0f - (2.0f * mouseY) / viewport.Height; // ACHTUNG: Y-Flip!

    //        // 2. NDC → Clip Space
    //        Vector4 rayClip = new Vector4(x, y, -1.0f, 1.0f);

    //        // 3. Clip Space → Eye (SelectedCamera) Space
    //        Matrix4 invProjection = Matrix4.Invert(camera.GetProjectionMatrix(viewport));
    //        Vector4 rayEye = rayClip * invProjection;
    //        rayEye = new Vector4(rayEye.X, rayEye.Y, -1.0f, 0.0f);

    //        // 4. Eye → World Space
    //        Matrix4 invView = Matrix4.Invert(camera.GetViewMatrix());
    //        Vector4 rayWorld4 = rayEye * invView;
    //        Vector3 rayDirWorld = Vector3.Normalize(rayWorld4.Xyz);

    //        // 5. Ray origin ist Kamera-Position (im World Space)
    //        Vector3 rayOrigin = camera.Transform.Position;

    //        // 6. Erstelle den Ray
    //        return new MeshRay(rayOrigin, rayDirWorld);
    //    }

    //    private static bool RayIntersectsTriangle(MeshRay ray, Vector3 v0, Vector3 v1, Vector3 v2, out float t, out Vector3 normal)
    //    {
    //        t = 0;
    //        normal = Vector3.Zero;

    //        const float EPSILON = 1e-5f;
    //        Vector3 edge1 = v1 - v0;
    //        Vector3 edge2 = v2 - v0;
    //        Vector3 h = Vector3.Cross(ray.Direction, edge2);
    //        float a = Vector3.Dot(edge1, h);

    //        if (System.Math.Abs(a) < EPSILON)
    //            return false; // Parallel

    //        float f = 1.0f / a;
    //        Vector3 s = ray.Origin - v0;
    //        float u = f * Vector3.Dot(s, h);
    //        if (u < 0.0f || u > 1.0f)
    //            return false;

    //        Vector3 q = Vector3.Cross(s, edge1);
    //        float v = f * Vector3.Dot(ray.Direction, q);
    //        if (v < 0.0f || u + v > 1.0f)
    //            return false;

    //        t = f * Vector3.Dot(edge2, q);
    //        if (t > EPSILON)
    //        {
    //            normal = Vector3.Normalize(Vector3.Cross(edge1, edge2));
    //            // **Backface-Culling (optional):**
    //            // if (Vector3.Dot(ray.Direction, normal) > 0) return false;

    //            // **Extra Logging:**
    //            //var p = ray.Origin + ray.Direction * t;
    //            //Debug.WriteLine($"TriangleHit: t={t}, u={u}, v={v}, p={p}");
    //            return true;
    //        }
    //        return false;
    //    }
    //}
}
