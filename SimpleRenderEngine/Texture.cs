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

        public Texture(string filename, int width, int height)
        {
            this.width = width;
            this.height = height;
            this.bitmap = new Bitmap(filename);
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
