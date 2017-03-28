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
        private BitmapData bmData;
        private int width;
        private int height;

        public Texture(string filename, int width, int height)
        {
            this.width = width;
            this.height = height;
            this.bitmap = new Bitmap(filename);
        }

        public BitmapData GetBmData()
        {
            return this.bmData;
        }
        
        public BitmapData LockBits()
        {
            this.bmData = this.bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            return this.bmData;
        }

        public void UnlockBits(BitmapData bmData)
        {
            this.bitmap.UnlockBits(bmData);
        }

        public int GetWidth()
        {
            return this.width;
        }

        public int GetHeight()
        {
            return this.height;
        }
    }
}
