using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Math
{
    public struct Plane
    {
        public Vector3 Normal;
        public float D;

        public Plane(float a, float b, float c, float d)
        {
            Normal = new Vector3(a, b, c);
            D = d;
        }

        public Plane(Vector3 normal, float d)
        {
            Normal = normal;
            D = d;
        }

        public float DistanceToPoint(Vector3 point)
        {
            return Vector3.Dot(Normal, point) + D;
        }
    }
}
