using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Math
{
    /// <summary>
    /// Frustum data structure
    /// </summary>
    public struct Frustum
    {
        /// <summary>
        /// Left Plane of the frustum
        /// </summary>
        public Plane Left { get; set; }

        /// <summary>
        /// Right Plane of the frustum
        /// </summary>
        public Plane Right { get; set; }

        /// <summary>
        /// Bottom Plane of the frustum
        /// </summary>
        public Plane Bottom { get; set; }

        /// <summary>
        /// Top Plane of the frustum
        /// </summary>
        public Plane Top { get; set; }

        /// <summary>
        /// Near Plane of the frustum
        /// </summary>
        public Plane Near { get; set; }

        /// <summary>
        /// Far Plane of the frustum
        /// </summary>
        public Plane Far { get; set; }

        /// <summary>
        /// Returns the six planes of the frustum in the order: Left, Right, Bottom, Top, Near, Far.
        /// </summary>
        /// <returns></returns>
        public Plane[] GetPlanes()
        {
            return new Plane[] { Left, Right, Bottom, Top, Near, Far };
        }

        /// <summary>
        /// Returns a new frustum with all planes normalized so that each plane's normal vector has unit length.
        /// </summary>
        /// <remarks>Normalizing the planes of a frustum is essential for accurate geometric calculations,
        /// such as intersection tests and distance measurements, in 3D graphics applications.</remarks>
        /// <param name="frustum">The frustum whose planes are to be normalized.</param>
        /// <returns>A new Frustum instance with each plane's normal vector normalized to a length of one.</returns>
        public static Frustum Normalized(Frustum frustum)
        {
            var planes = frustum.GetPlanes();
            for (int i = 0; i < planes.Length; i++)
            {
                Plane plane = planes[i];
                float length = plane.Normal.Length;
                if (length > 0)
                {
                    planes[i] = new Plane(plane.Normal / length, plane.D / length);
                }
            }

            return new Frustum
            {
                Left = planes[0],
                Right = planes[1],
                Bottom = planes[2],
                Top = planes[3],
                Near = planes[4],
                Far = planes[5]
            };
        }

        /// <summary>
        /// Determines whether the specified axis-aligned bounding box (AABB) intersects with or is contained within the
        /// given view frustum.
        /// </summary>
        /// <remarks>This method checks each plane of the frustum to determine if the AABB is outside any
        /// of them. If the AABB is outside at least one plane, it does not intersect the frustum.</remarks>
        /// <param name="frustum">The view frustum to test against. Defines the visible region in 3D space using six planes.</param>
        /// <param name="min">The minimum corner of the axis-aligned bounding box, representing the lowest X, Y, and Z coordinates.</param>
        /// <param name="max">The maximum corner of the axis-aligned bounding box, representing the highest X, Y, and Z coordinates.</param>
        /// <returns>true if the AABB intersects with or is contained within the frustum; otherwise, false.</returns>
        public static bool IntersectsAABB(Frustum frustum, Vector3 min, Vector3 max)
        {
            var planes = frustum.GetPlanes();
            foreach (var plane in planes)
            {
                Vector3 positiveVertex = new Vector3(
                    plane.Normal.X >= 0 ? max.X : min.X,
                    plane.Normal.Y >= 0 ? max.Y : min.Y,
                    plane.Normal.Z >= 0 ? max.Z : min.Z
                );
                if (Vector3.Dot(plane.Normal, positiveVertex) + plane.D < 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether a specified point is contained within the given frustum.
        /// </summary>
        /// <remarks>This method evaluates the point against all six planes of the frustum. If the point
        /// lies on the negative side of any plane, it is considered outside the frustum.</remarks>
        /// <param name="frustum">The frustum to check against, which defines the viewing volume in 3D space.</param>
        /// <param name="point">The point in 3D space to test for containment within the frustum.</param>
        /// <returns>true if the point is within the frustum; otherwise, false.</returns>
        public static bool ContainsPoint(Frustum frustum, Vector3 point)
        {
            var planes = frustum.GetPlanes();
            foreach (var plane in planes)
            {
                if (Vector3.Dot(plane.Normal, point) + plane.D < 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether the specified axis-aligned bounding box (AABB) is entirely contained within the given
        /// frustum.
        /// </summary>
        /// <remarks>This method checks each plane of the frustum to determine if the bounding box lies
        /// entirely within all planes. If the box is outside any plane, the method returns false. This is commonly used
        /// in view frustum culling for 3D graphics.</remarks>
        /// <param name="frustum">The frustum that defines the viewing volume to test against.</param>
        /// <param name="min">The minimum corner of the axis-aligned bounding box, representing the lowest X, Y, and Z coordinates.</param>
        /// <param name="max">The maximum corner of the axis-aligned bounding box, representing the highest X, Y, and Z coordinates.</param>
        /// <returns>true if the AABB is completely contained within the frustum; otherwise, false.</returns>
        public static bool ContainsAABB(Frustum frustum, Vector3 min, Vector3 max)
        {
            var planes = frustum.GetPlanes();
            foreach (var plane in planes)
            {
                Vector3 positiveVertex = new Vector3(
                    plane.Normal.X >= 0 ? max.X : min.X,
                    plane.Normal.Y >= 0 ? max.Y : min.Y,
                    plane.Normal.Z >= 0 ? max.Z : min.Z
                );
                if (Vector3.Dot(plane.Normal, positiveVertex) + plane.D < 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether a sphere, specified by its center and radius, is fully contained within the given
        /// frustum.
        /// </summary>
        /// <remarks>The method evaluates the sphere against each plane of the frustum. If any plane
        /// excludes the sphere, the method returns false. This check is useful for culling or visibility determination
        /// in 3D graphics applications.</remarks>
        /// <param name="frustum">The frustum that defines the viewing volume against which the sphere is tested for containment.</param>
        /// <param name="center">The center point of the sphere to check for containment within the frustum.</param>
        /// <param name="radius">The radius of the sphere, which influences the containment check against the frustum's planes. Must be
        /// non-negative.</param>
        /// <returns>true if the sphere is completely contained within the frustum; otherwise, false.</returns>
        public static bool ContainsSphere(Frustum frustum, Vector3 center, float radius)
        {
            var planes = frustum.GetPlanes();
            foreach (var plane in planes)
            {
                if (Vector3.Dot(plane.Normal, center) + plane.D < -radius)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
