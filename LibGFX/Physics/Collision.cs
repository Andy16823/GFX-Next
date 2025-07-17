using LibGFX.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Physics
{
    public enum ElementIndex
    {
        A = 0,
        B = 1
    }

    public struct CollisionPoint
    {
        public Vector3 LocalPointA;
        public Vector3 LocalPointB;
        public Vector3 WorldPointA;
        public Vector3 WorldPointB;
        public Vector3 WorldNormal;
        public float Impulse;
    }

    public struct Collision
    {
        public GameElement GameElement;
        public int Contacts;
        public List<CollisionPoint> ContactPoints;
        public ElementIndex ElementIndex;
    }
}
