using BulletSharp;
using LibGFX.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Physics
{
    /// <summary>
    /// Hit result of a raycast operation.
    /// </summary>
    public struct HitResult
    {
        public bool hit;
        public Vector3 rayStart;
        public Vector3 rayEnd;
        public GameElement hitElement;
        public Vector3 hitLocation;
        public Vector3 hitNormal;
        public int hitTriangleIndex;
        public float hitDistance;
    }
}
