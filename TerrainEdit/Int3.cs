using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TerrainEdit
{
    public struct Int3
    {
        public int X;
        public int Y;
        public int Z;
        public Int3(int x, int y, int z)
        {
            X = x; Y = y; Z = z;
        }
        public override string ToString()
        {
            return "(X: " + X + ", Y: " + Y + " Z: " + Z + ")";
        }
        public static explicit operator Int3(Vector3 a)
        {
            return new Int3(Mathf.FloorToInt(a.x), Mathf.FloorToInt(a.y), Mathf.FloorToInt(a.z));
        }
        public static implicit operator Vector3(Int3 a)
        {
            return new Vector3(a.X, a.Y, a.Z);
        }
        public static Int3 operator +(Int3 a, Int3 b)
        {
            return new Int3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Int3 operator -(Int3 a, Int3 b)
        {
            return new Int3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        public static Int3 operator -(Int3 a)
        {
            return new Int3(-a.X, -a.Y, -a.Z);
        }
        public static Int3 operator *(Int3 a, int mult)
        {
            return new Int3(a.X * mult, a.Y * mult, a.Z * mult);
        }
        public static Int3 operator *(int mult, Int3 a)
        {
            return new Int3(a.X * mult, a.Y * mult, a.Z * mult);
        }
        public static Int3 operator /(Int3 a, int mult)
        {
            return new Int3(a.X / mult, a.Y / mult, a.Z / mult);
        }
    }
}