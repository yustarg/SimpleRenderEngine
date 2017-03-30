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
        public static float Clamp01(float t)
        {
            if (t.CompareTo(0) < 0) return 0;
            else if (t.CompareTo(1) > 0) return 1;
            return t;    
        }

        // 计算插值：t 为 [0, 1] 之间的数值
        public static float Interp(float x1, float x2, float t) 
        {
            return x1 + (x2 - x1) * t; 
        }

        public static byte Interp(byte x1, byte x2, float t)
        {
            return (byte)(x1 + (x2 - x1) * t);
        }
        
        // 矢量插值，t取值 [0, 1]
        public static Vector4 Vector4Interp(Vector4 x1, Vector4 x2, float t)
        {
            //t = Clamp01(t);
            Vector4 val = new Vector4();
            val.X = Interp(x1.X, x2.X, t);
            val.Y = Interp(x1.Y, x2.Y, t);
            val.Z = Interp(x1.Z, x2.Z, t);
            val.W = 1.0f;
            return val;
        }

        public static Color4 ColorInterp(Color4 c1, Color4 c2, float t)
        {
            byte r = Interp(c1.X, c2.X, t);
            byte g = Interp(c1.Y, c2.Y, t);
            byte b = Interp(c1.Z, c2.Z, t);
            return new Color4(r, g, b);
        }
    }
}
