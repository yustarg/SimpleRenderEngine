using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    class Device
    {
        private Bitmap bmp;

        public Device(Bitmap bmp)
        {
            this.bmp = bmp;
        }

        public void PutPixel(int x, int y, Color color)
        {
            this.bmp.SetPixel(x, y, color);
        }

        public Vector4 Project(Vector4 coord, Matrix4x4 mvp)
        {
            Vector4 point = mvp.Apply(coord);
            Vector4 after = this.Homogenize(point);
            return after;
        }

        public void DrawPoint(Vector4 point)
        {
            if (point.X >= 0 && point.Y >= 0 && point.X < bmp.Width && point.Y < bmp.Height)
            {
                PutPixel((int)point.X, (int)point.Y, Color.FromArgb(1, 0, 0));
                //Console.WriteLine(point);
            }
        }

        // Bresenham 画线算法
        public void DrawLine(Vector4 point0, Vector4 point1)
        {
            int x0 = (int)point0.X;
            int y0 = (int)point0.Y;
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = (x0 < x1) ? 1 : -1;
            int sy = (y0 < y1) ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                DrawPoint(new Vector4(x0, y0, 0, 0));

                if ((x0 == x1) && (y0 == y1)) break;
                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
        }

        public void Render(Camera camera, params Mesh[] meshes)
        {
            Matrix4x4 model = new Matrix4x4();
            Matrix4x4 view = camera.LookAt();
            Matrix4x4 projection = camera.Perspective();

            foreach (Mesh mesh in meshes)
            {
                var matrixMVP = model * view * projection;

                foreach (var triangle in mesh.triangles)
                {
                    var vertexA = mesh.Vertices[triangle.AIndex];
                    var vertexB = mesh.Vertices[triangle.BIndex];
                    var vertexC = mesh.Vertices[triangle.CIndex];

                    var pixelA = Project(vertexA, matrixMVP);
                    var pixelB = Project(vertexB, matrixMVP);
                    var pixelC = Project(vertexC, matrixMVP);

                    DrawLine(pixelA, pixelB);
                    DrawLine(pixelB, pixelC);
                    DrawLine(pixelC, pixelA);
                }
            }
        }

        // 归一化，得到屏幕坐标
        public Vector4 Homogenize(Vector4 x)
        {
            Vector4 val = new Vector4();
            float rhw = 1.0f / x.W;
            val.X = (x.X * rhw + 1.0f) * this.bmp.Width * 0.5f;
	        val.Y = (1.0f - x.Y * rhw) * this.bmp.Height * 0.5f;
            val.Z = x.Z * rhw;
            val.W = 1.0f;
            return val;
        }

    }
}
