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
        private Edge[] ET;
        private Edge AEL;
        
        private Bitmap bmp;

        public Device(Bitmap bmp)
        {
            this.bmp = bmp;
            ET = new Edge[bmp.Height];
            for (int i = 0; i < bmp.Height; i++)
            {
                ET[i] = new Edge();
            }
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

        public void DrawPoint(Vector4 point, Color c)
        {
            if (point.X >= 0 && point.Y >= 0 && point.X < bmp.Width && point.Y < bmp.Height)
            {
                PutPixel((int)point.X, (int)point.Y, c);
                //Console.WriteLine(point);
            }
        }

        /*
        // Bresenham 画线算法
        public void DrawLine(Vector4 point0, Vector4 point1, Color color)
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
        */

        // DDA 画线算法
        public void DrawLine(Vector4 point0, Vector4 point1, Color color)
        {
            int x0 = (int)point0.X;
            int y0 = (int)point0.Y;
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;

            int dx = x1 - x0;
            int dy = y1 - y0;
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));
            float xInc = (float)dx / (float)steps;
            float yInc = (float)dy / (float)steps;

            float x = x0;
            float y = y0;
            for (int i = 1; i <= steps; i++)
            {
                DrawPoint(new Vector4((int)x, (int)y, 0, 0), Color.FromArgb(1, 0, 0));
                x += xInc;
                y += yInc;
            }
        }

        public void DrawTriangle(Vector4 p1, Vector4 p2, Vector4 p3, Color color)
        {
            Vector4[] vertices = new Vector4[3];
            vertices[0] = p1;
            vertices[1] = p2;
            vertices[2] = p3;
            for (int i = 0; i < vertices.Length; i++)
            {
                for (int j = i + 1; j < vertices.Length; j++)
                {
                    if (vertices[i].Y != vertices[j].Y)
                    {
                        int x1 = (int)vertices[i].X;
                        int y1 = (int)vertices[i].Y;
                        int x2 = (int)vertices[j].X;
                        int y2 = (int)vertices[j].Y;
                        int ymin = y1 > y2 ? y2 : y1;
                        int ymax = y1 > y2 ? y1 : y2;
                        float x = y1 > y2 ? x2 : x1;
                        float dx = (x1 - x2) * 1.0f / (y1 - y2);
                        Edge e = new Edge();
                        e.yMax = ymax;
                        e.x = x;
                        e.deltaX = dx;
                        InsertEdge(ref ET[ymin].nextEdge, e);
                    }
                }
            }

            // 置空活动边表
            AEL = new Edge();
            for (int i = 0; i < this.bmp.Height; i++)
            {
 //               if (ET[i].nextEdge != null)
                {
                    while (ET[i].nextEdge != null)
                    {
                        InsertEdge(ref AEL, ET[i].nextEdge);
                        ET[i].nextEdge = ET[i].nextEdge.nextEdge;
                    }

                    while (AEL.nextEdge != null && AEL.nextEdge.nextEdge != null)
                    {
                        for (int x = (int)AEL.nextEdge.x; x < (int)AEL.nextEdge.nextEdge.x; x++)
                        {
                            DrawPoint(new Vector4(x, i, 0, 0), Color.FromArgb(1, 0, 0));
                        }
                        AEL.nextEdge = AEL.nextEdge.nextEdge;
                    }

                    Edge p = AEL;
                    while (p.nextEdge != null)
                    {
                        if (p.nextEdge.yMax == i)
                        {
                            Edge pDelete = p.nextEdge;
                            p.nextEdge = pDelete.nextEdge;
                            pDelete.nextEdge = null;
                        }
                        else
                        {
                            p = p.nextEdge;
                        }
                    }

                    p = AEL;
                    while (p.nextEdge != null)
                    {
                        p.nextEdge.x += p.nextEdge.deltaX;
                        p = p.nextEdge;
                    }
                }
            }
        }

        void InsertEdge(ref Edge root, Edge e)
        {
            Edge newEdge = new Edge(e);
            Edge previous;
            Edge current;

            current = root; 
            previous = null;

            // 查找插入的位置  
            while (current != null && (current.x < newEdge.x || (current.x == newEdge.x && current.deltaX < newEdge.deltaX)))
            {
                previous = current;
                current = current.nextEdge;
            }

            newEdge.nextEdge = current;
            if (previous == null)
                root = newEdge;
            else
                previous.nextEdge = newEdge;  
        }

        void ProcessScanLine(int y, Vector4 pa, Vector4 pb, Vector4 pc, Vector4 pd, Color color)
        {

        }

        public void Render(Camera camera, params Mesh[] meshes)
        {
            Matrix4x4 model = new Matrix4x4();
            Matrix4x4 view = camera.LookAt();
            Matrix4x4 projection = camera.Perspective();

            foreach (Mesh mesh in meshes)
            {
                Matrix4x4 matrixMVP = model * view * projection;

                foreach (var triangle in mesh.triangles)
                {
                    var vertexA = mesh.Vertices[triangle.AIndex];
                    var vertexB = mesh.Vertices[triangle.BIndex];
                    var vertexC = mesh.Vertices[triangle.CIndex];

                    var pixelA = Project(vertexA, matrixMVP);
                    var pixelB = Project(vertexB, matrixMVP);
                    var pixelC = Project(vertexC, matrixMVP);

                    // 画线框
                    //DrawLine(pixelA, pixelB, Color.FromArgb(1, 0, 0));
                    //DrawLine(pixelB, pixelC, Color.FromArgb(1, 0, 0));
                    //DrawLine(pixelC, pixelA, Color.FromArgb(1, 0, 0));
                    
                    // 填充三角形
                    DrawTriangle(pixelA, pixelB, pixelC, Color.FromArgb(1, 0, 0));
                }
            }
        }
    }
}
