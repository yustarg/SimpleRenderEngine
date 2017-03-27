using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SimpleRenderEngine
{
    public class MathUtil
    {
        public const float PI = 3.1415926f; 
        
        public static float Clamp01(float t)
        {
            if (t < 0) return 0;
            if (t > 1) return 1;
            return t;
        }
        
        // 计算插值：t 为 [0, 1] 之间的数值
        public static float Interp(float x1, float x2, float t) 
        {
            t = Clamp01(t);
            return x1 + (x2 - x1) * t; 
        }
        
        // 矢量插值，t取值 [0, 1]
        public static Vector4 Vector4Interp(Vector4 x1, Vector4 x2, float t)
        {
            t = Clamp01(t);
            Vector4 val = new Vector4();
            val.X = Interp(x1.X, x2.X, t);
            val.Y = Interp(x1.Y, x2.Y, t);
            val.Z = Interp(x1.Z, x2.Z, t);
            val.W = 1.0f;
            return val;
        }

        public static Vector4 Vector4Divide(Vector4 x1, float t)
        {
            Vector4 val = new Vector4();
            val.X = x1.X / t;
            val.Y = x1.Y / t;
            val.Z = x1.Z / t;
            val.W = 1.0f;
            return val;
        }

        public static Color ColorInterp(Color c1, Color c2, float t)
        {
            t = Clamp01(t);
            byte a = (byte)Interp(c1.A, c2.A, t);
            byte r = (byte)Interp(c1.R, c2.R, t);
            byte g = (byte)Interp(c1.G, c2.G, t);
            byte b = (byte)Interp(c1.B, c2.B, t);
            return Color.FromArgb(a, r, g, b);
        }

        public static Color AddColor(Color c1, Color c2)
        {
            int r = Math.Min(c1.R + c2.R, 255);
            int g = Math.Min(c1.G + c2.G, 255);
            int b = Math.Min(c1.B + c2.B, 255);
            return Color.FromArgb(r, g, b);
        }

        public static Color MulColor(Color c1, float t)
        {
            int r = Math.Min((int)(c1.R * t), 255);
            int g = Math.Min((int)(c1.G * t), 255);
            int b = Math.Min((int)(c1.B * t), 255);
            return Color.FromArgb(r, g, b);
        }
    }
}
