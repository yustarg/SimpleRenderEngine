using System;
using System.Collections.Generic;
using System.Drawing;
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

        public Color Map(float tu, float tv)
        {
            int u = Math.Abs((int) ((1 - tu) * width) % width);
            int v = Math.Abs((int) ((1 - tv) * height) % height);      
            return this.bitmap.GetPixel(u, v);
        }
    }
}
