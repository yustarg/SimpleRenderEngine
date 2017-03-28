using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{   
    // rgba 0-255
    public  class Color4
    {
        public byte X { get; set; }
        public byte Y { get; set; }
        public byte Z { get; set; }
        public byte A { get; set; }

        public Color4(byte red, byte green, byte blue)
        {
            this.X = red;
            this.Y = green;
            this.Z = blue;
            this.A = 255;
        }

        public static Color4 operator *(Color4 c1, Color4 c2)
        {
            float r = (c1.X / 255f) * (c2.X / 255f);
            float g = (c1.Y / 255f) * (c2.Y / 255f);
            float b = (c1.Z / 255f) * (c2.Z / 255f);
            return new Color4((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        public static Color4 operator +(Color4 c1, Color4 c2)
        {
            byte r = (byte)Math.Min(c1.X + c2.X, 255);
            byte g = (byte)Math.Min(c1.Y + c2.Y, 255);
            byte b = (byte)Math.Min(c1.Z + c2.Z, 255);
            return new Color4(r, g, b);
        }
    }
}
