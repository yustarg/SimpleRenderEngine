using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    public class Texture
    {
        private Bitmap bitmap;
        private int width;
        private int height;
        private Rectangle rt;
        private PixelFormat pixelFormat;
        
        public Texture(string filename, int width, int height)
        {
            this.width = width;
            this.height = height;
            this.bitmap = new Bitmap(filename);
            this.rt = new Rectangle(0, 0, width, height);
            this.pixelFormat = this.bitmap.PixelFormat;
        }

        public Color Map(float tu, float tv)
        {
            int x = Math.Abs((int) ((1 - tu) * width) % width);
            int y = Math.Abs((int) ((1 - tv) * height) % height);

            int r = 0;
            int g = 0;
            int b = 0;
            BitmapData data = this.bitmap.LockBits(rt, ImageLockMode.ReadWrite, this.pixelFormat);
            unsafe
            {
                byte* ptr = (byte*)(data.Scan0);
                byte* row = ptr + (y * data.Stride);
                b = row[x * 3];
                g = row[x * 3 + 1];
                r = row[x * 3 + 2];
            }
            this.bitmap.UnlockBits(data);

            return Color.FromArgb(r, g, b);
        }
    }
}
