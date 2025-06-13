using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Math
{
    /// <summary>
    /// Represents an Axis-Aligned Bounding Box (AABB) in 3D space.
    /// </summary>
    public struct AABB
    {
        /// <summary>
        /// The minimum point of the AABB, which defines one corner of the box.
        /// </summary>
        public Vector3 Min { get; set; }

        /// <summary>
        /// The maximum point of the AABB, which defines the opposite corner from the minimum point.
        /// </summary>
        public Vector3 Max { get; set; }

        /// <summary>
        /// Creates a new Axis-Aligned Bounding Box (AABB) with the specified minimum and maximum points.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public AABB(Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Represents an empty AABB with minimum and maximum points at zero.
        /// </summary>
        public readonly AABB Zero
        {
            get
            {
                return new AABB(Vector3.Zero, Vector3.Zero);
            }
        }

        /// <summary>
        /// Checks if the AABB contains a given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vector3 point)
        {
            return (point.X >= Min.X && point.X <= Max.X) &&
                   (point.Y >= Min.Y && point.Y <= Max.Y) &&
                   (point.Z >= Min.Z && point.Z <= Max.Z);
        }

        /// <summary>
        /// Checks if the AABB intersects with another AABB.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Intersects(AABB other)
        {
            return (Min.X <= other.Max.X && Max.X >= other.Min.X) &&
                   (Min.Y <= other.Max.Y && Max.Y >= other.Min.Y) &&
                   (Min.Z <= other.Max.Z && Max.Z >= other.Min.Z);
        }

        /// <summary>
        /// Transforms the AABB by a given transformation matrix.
        /// </summary>
        /// <param name="aabb"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static AABB TransformAABB(AABB aabb, Matrix4 matrix)
        {
            // Alle 8 Ecken transformieren
            Vector3[] corners = new Vector3[8];
            corners[0] = Vector3.TransformPosition(new Vector3(aabb.Min.X, aabb.Min.Y, aabb.Min.Z), matrix);
            corners[1] = Vector3.TransformPosition(new Vector3(aabb.Max.X, aabb.Min.Y, aabb.Min.Z), matrix);
            corners[2] = Vector3.TransformPosition(new Vector3(aabb.Min.X, aabb.Max.Y, aabb.Min.Z), matrix);
            corners[3] = Vector3.TransformPosition(new Vector3(aabb.Max.X, aabb.Max.Y, aabb.Min.Z), matrix);
            corners[4] = Vector3.TransformPosition(new Vector3(aabb.Min.X, aabb.Min.Y, aabb.Max.Z), matrix);
            corners[5] = Vector3.TransformPosition(new Vector3(aabb.Max.X, aabb.Min.Y, aabb.Max.Z), matrix);
            corners[6] = Vector3.TransformPosition(new Vector3(aabb.Min.X, aabb.Max.Y, aabb.Max.Z), matrix);
            corners[7] = Vector3.TransformPosition(new Vector3(aabb.Max.X, aabb.Max.Y, aabb.Max.Z), matrix);

            Vector3 newMin = new Vector3(float.MaxValue);
            Vector3 newMax = new Vector3(float.MinValue);
            foreach (var c in corners)
            {
                newMin = Vector3.ComponentMin(newMin, c);
                newMax = Vector3.ComponentMax(newMax, c);
            }
            return new AABB(newMin, newMax);
        }
    }
}
