using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    public class Vector4
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public Vector4 Normalized {
            get {
                Vector4 n = new Vector4(this.X, this.Y, this.Z, this.W);
                n.Normalize();
                return n;
            }
        }

        public Vector4() { }

        public Vector4(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public static Vector4 operator +(Vector4 a, Vector4 b)
        {
            return new Vector4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, 1);
        }

        public static Vector4 operator -(Vector4 a, Vector4 b)
        {
            return new Vector4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, 1);
        }

        public static Vector4 operator /(Vector4 a, float t)
        {
            return new Vector4(a.X / t, a.Y / t, a.Z / t, 1);
        }

        public static Vector4 operator *(Vector4 a, float t)
        {
            return new Vector4(a.X * t, a.Y * t, a.Z * t, 1); ;
        }

        public static float Dot(Vector4 a, Vector4 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static Vector4 Cross(Vector4 a, Vector4 b)
        {
            float m1, m2, m3;
            m1 = a.Y * b.Z - a.Z * b.Y;
            m2 = a.Z * b.X - a.X * b.Z;
            m3 = a.X * b.Y - a.Y * b.X;
            return new Vector4(m1, m2, m3, 1f);
        }

        public float Length() 
        {
            float sq = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
	        return (float)Math.Sqrt(sq);
        }

        // 矢量归一化
        public void Normalize()
        {
            float length = this.Length();
            if (length != 0.0f)
            {
                float inv = 1.0f / length;
                this.X *= inv;
                this.Y *= inv;
                this.Z *= inv;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("x = {0}, y = {1}, z = {2}, w = {3}", this.X, this.Y, this.Z, this.W );
            return sb.ToString();
        }
    }
}
