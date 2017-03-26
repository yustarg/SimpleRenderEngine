using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    public class Device
    {
        private Bitmap bmp;
        private BitmapData data;
        private ScanLine scanLine;
        private HodgmanClip clip;
        private Vector4 wMin;   // 裁剪空间(-1, -1, -1)
        private Vector4 wMax;   // 裁剪空间(1, 1, 1)
        private Scene scene;
        private readonly float[] depthBuffer;

        public Device(Bitmap bmp)
        {
            this.bmp = bmp;
            this.scanLine = new ScanLine(this);
            wMin = new Vector4(-1, -1, -1, 1);
            wMax = new Vector4(1, 1, 1, 1);
            depthBuffer = new float[bmp.Width * bmp.Height];
            Clear();
        }

        public void UpdateBmp(Bitmap bmp)
        {
            this.bmp = bmp;
        }

        public int GetHeight()
        {
            return this.bmp.Height;
        }

        public int GetWidth()
        {
            return this.bmp.Width;
        }

        public void Clear()
        {
            // clear depth buffer
            for (int index = 0; index < depthBuffer.Length; index++)
            {
                depthBuffer[index] = float.MaxValue;
            }

            this.data = this.bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                                ImageLockMode.ReadWrite, bmp.PixelFormat); 
            unsafe
            {
                byte* ptr = (byte*)(data.Scan0);
                for (int i = 0; i < data.Height; i++)
                {
                    for (int j = 0; j < data.Width; j++)
                    {
                        // write the logic implementation here 
                        *ptr = 128;
                        *(ptr + 1) = 128;
                        *(ptr + 2) = 128;
                        ptr += 3;
                    }
                    ptr += data.Stride - data.Width * 3;
                }
            }
            this.bmp.UnlockBits(data);    
        }

        public void PutPixel(int x, int y, float z, Color color)
        {
            int index = (x + y * GetWidth());
            
            if (depthBuffer[index] < z) return;

            depthBuffer[index] = z;
            //this.bmp.SetPixel(x, y, color);

            this.data = this.bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                                ImageLockMode.ReadWrite, bmp.PixelFormat);
            unsafe
            {
                byte* ptr = (byte*)(data.Scan0);
                byte* row = ptr + (y * data.Stride);
                row[x * 3] = color.B;
                row[x * 3 + 1] = color.G;
                row[x * 3 + 2] = color.R;
            }
            this.bmp.UnlockBits(data);
        }

        public Vector4 Project(Vector4 coord, Matrix4x4 mvp)
        {
            Vector4 point = mvp.Apply(coord);
            Vector4 viewPort = Homogenize(point);
            return viewPort;
        }

        public Vector4 ClipSpace(Vector4 x)
        {
            Vector4 val = this.GetMvpMatrix().Apply(x);
            float rhw = 1.0f / val.W;
            val.X = val.X * rhw;
            val.Y = val.Y * rhw;
            val.Z = val.Z * rhw;
            val.W = 1.0f;
            return val;
        }

        public Vector4 ViewPort(Vector4 x)
        {
            Vector4 val = new Vector4();
            val.X = (1.0f + x.X) * this.bmp.Width * 0.5f;
            val.Y = (1.0f - x.Y) * this.bmp.Height * 0.5f;
            val.Z = x.Z;
            val.W = 1.0f;
            return val;
        }

        // 归一化，得到屏幕坐标
        public Vector4 Homogenize(Vector4 x)
        {
            Vector4 val = new Vector4();
            float rhw = 1.0f / x.W;
            val.X = (1.0f + x.X * rhw) * this.bmp.Width * 0.5f;
            val.Y = (1.0f - x.Y * rhw) * this.bmp.Height * 0.5f;
            val.Z = x.Z * rhw;
            val.W = 1.0f;
            return val;
        }

        public void DrawPoint(Vector4 point, Color c)
        {
            if (point.X >= 0 && point.Y >= 0 && point.X < bmp.Width && point.Y < bmp.Height)
            {
                PutPixel((int)point.X, (int)point.Y, point.Z, c);
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
        public void DrawLine(Vertex v1, Vertex v2, Vector4 point0, Vector4 point1, Scene scene)
        {   
            int x0 = (int)point0.X;
            int y0 = (int)point0.Y;
            int x1 = (int)point1.X;
            int y1 = (int)point1.Y;
            float z1 = point0.Z;
            float z2 = point1.Z;

            int dx = x1 - x0;
            int dy = y1 - y0;
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));
            float xInc = (float)dx / (float)steps;
            float yInc = (float)dy / (float)steps;

            float x = x0;
            float y = y0;

            float nDotL1 = scene.light.ComputeNDotL(v1.Position, v1.Normal);
            float nDotL2 = scene.light.ComputeNDotL(v2.Position, v2.Normal);
            for (int i = 1; i <= steps; i++)
            {
                float ratio = (float)i / (float)steps;
                Color vertexColor = Color.FromArgb(0, 0, 0);
                Color lightColor = Color.FromArgb(0, 0, 0);
                if (scene.light.IsEnable)
                {
                    Color c1 = scene.light.GetFinalLightColor(nDotL1);
                    Color c2 = scene.light.GetFinalLightColor(nDotL2);
                    lightColor = MathUtil.ColorInterp(c1, c2, ratio);
                }else
                {
                    vertexColor = MathUtil.ColorInterp(v1.Color, v2.Color, ratio);
                }
                float z = MathUtil.Interp(z1, z2, ratio);
                DrawPoint(new Vector4((int)x, (int)y, z, 0), MathUtil.AddColor(vertexColor, lightColor));
                x += xInc;
                y += yInc;
            }
        }

        public void DrawTriangle(Vertex p1, Vertex p2, Vertex p3, Matrix4x4 mvp, Scene scene)
        {
            Vertex[] vertices = new Vertex[3];
            vertices[0] = p1;
            vertices[1] = p2;
            vertices[2] = p3;

            this.scanLine.ProcessScanLine(vertices, mvp, scene);
        }

        public Matrix4x4 GetMvpMatrix()
        {
            Matrix4x4 translate = new Matrix4x4();
            translate.SetTranslate(0, 0, 0);
            Matrix4x4 scale = new Matrix4x4();
            scale.SetScale(1, 1, 1);
            Matrix4x4 rotate = new Matrix4x4();
            rotate.SetRotate(0, 0, 0);
            Matrix4x4 model = scale * rotate * translate;
            Matrix4x4 view = this.scene.camera.LookAt();
            //Matrix4x4 view = scene.camera.FPSView();
            Matrix4x4 projection = this.scene.camera.Perspective();

            return model * view * projection;
        }

        public void Render(Scene scene)
        {
            this.scene = scene;
            Matrix4x4 matrixMVP = GetMvpMatrix();

            foreach (var triangle in scene.mesh.triangles)
            {
                Vertex vertexA = scene.mesh.Vertices[triangle.AIndex];
                Vertex vertexB = scene.mesh.Vertices[triangle.BIndex];
                Vertex vertexC = scene.mesh.Vertices[triangle.CIndex];

                //Vector4 pixelA = Project(vertexA.Position, matrixMVP);
                //Vector4 pixelB = Project(vertexB.Position, matrixMVP);
                //Vector4 pixelC = Project(vertexC.Position, matrixMVP);

                //DrawPoint(Project(scene.light.LightPos, matrixMVP), Color.FromArgb(255, 0, 0));

                List<Vertex> pIn = new List<Vertex>();
                //Vector4 a = matrixMVP.Apply(vertexA.Position);
                //Vector4 b = matrixMVP.Apply(vertexB.Position);
                //Vector4 c = matrixMVP.Apply(vertexC.Position);

                //pIn.Add(MathUtil.Vector4Divide(a, a.W));
                //pIn.Add(MathUtil.Vector4Divide(b, b.W));
                //pIn.Add(MathUtil.Vector4Divide(c, c.W));
                pIn.Add(vertexA);
                pIn.Add(vertexB);
                pIn.Add(vertexC);
                for (int i = 0; i < 6; i++)
                {
                    clip = new HodgmanClip(this);
                    clip.HodgmanPolygonClip((HodgmanClip.Boundary)i, wMin, wMax, pIn.ToArray());
                    pIn = clip.GetOutputList();
                }

                if (scene.renderState == Scene.RenderState.WireFrame)
                {
                    // 画线框, 需要vertex的normal,pos,color
                    //DrawLine(vertexA, vertexB, pixelA, pixelB, scene);
                    //DrawLine(vertexB, vertexC, pixelB, pixelC, scene);
                    //DrawLine(vertexC, vertexA, pixelC, pixelA, scene);
                    Vertex start = pIn[pIn.Count - 1];
                    for (int i = 0; i < pIn.Count; i++)
                    {
                        DrawLine(start, pIn[i], this.ViewPort(start.Position), this.ViewPort(pIn[i].Position), scene);
                        start = pIn[i];
                    }
                }
                else
                {
                    // 填充三角形
                    DrawTriangle(vertexA, vertexB, vertexC, matrixMVP, scene);
                }
            }
        }
    }
}
