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
        private BitmapData bmData;
        private int height;
        private int width;
        private ScanLine scanLine;
        private HodgmanClip clip;
        private Vector4 wMin;   // 裁剪空间(-1, -1, -1)
        private Vector4 wMax;   // 裁剪空间(1, 1, 1)
        private Scene scene;
        private readonly float[] depthBuffer;

        public Device(Bitmap bmp)
        {
            this.bmp = bmp;
            this.height = bmp.Height;
            this.width = bmp.Width;
            this.scanLine = new ScanLine(this);
            this.wMin = new Vector4(-1, -1, -1, 1);
            this.wMax = new Vector4(1, 1, 1, 1);
            this.depthBuffer = new float[bmp.Width * bmp.Height];
        }

        public int GetHeight()
        {
            return this.height;
        }

        public int GetWidth()
        {
            return this.width;
        }

        public void Clear(BitmapData data)
        {
            // clear depth buffer
            for (int index = 0; index < depthBuffer.Length; index++)
            {
                depthBuffer[index] = float.MaxValue;
            }
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
        }

        public void PutPixel(int x, int y, float z, Color4 color)
        {
            int index = (x + y * GetWidth());          
            if (depthBuffer[index] < z) return;
            depthBuffer[index] = z;
            unsafe
            {
                byte* ptr = (byte*)(this.bmData.Scan0);
                byte* row = ptr + (y * this.bmData.Stride);
                row[x * 3] = color.Z;
                row[x * 3 + 1] = color.Y;
                row[x * 3 + 2] = color.X;
            }
        }

        public Vector4 Project(Vector4 coord, Matrix4x4 mvp)
        {
            Vector4 point = mvp.Apply(coord);
            Vector4 viewPort = Homogenize(point);
            return viewPort;
        }

        public Vector4 ClipSpace(Vector4 x, Matrix4x4 mvp)
        {
            Vector4 val = mvp.Apply(x);
            float rhw = 1.0f / val.W;
            val.X = val.X * rhw;
            val.Y = val.Y * rhw;
            val.Z = val.Z * rhw;
            val.W = val.W; // val.W = 1;
            return val;
        }

        public Vector4 ViewPort(Vector4 x)
        {
            Vector4 val = new Vector4();
            val.X = (1.0f + x.X) * GetWidth() * 0.5f;
            val.Y = (1.0f - x.Y) * GetHeight() * 0.5f;
            val.Z = x.Z;
            val.W = 1.0f;
            return val;
        }

        // 归一化，得到屏幕坐标
        public Vector4 Homogenize(Vector4 x)
        {
            Vector4 val = new Vector4();
            float rhw = 1.0f / x.W;
            val.X = (1.0f + x.X * rhw) * GetWidth() * 0.5f;
            val.Y = (1.0f - x.Y * rhw) * GetHeight() * 0.5f;
            val.Z = x.Z * rhw;
            val.W = 1.0f;
            return val;
        }

        public void DrawPoint(Vector4 point, Color4 c)
        {
            //if (point.X >= 0 && point.Y >= 0 && point.X <= GetWidth() && point.Y <= GetHeight())
            {
                if (point.X == GetWidth()) point.X = point.X - 1;
                if (point.Y == GetHeight()) point.Y = point.Y - 1;
                PutPixel((int)point.X, (int)point.Y, point.Z, c);
            }
        }

        public Color4 Tex2D(float u, float v, Texture texture)
        {
            int x = Math.Abs((int)((1 - u) * texture.GetWidth()) % texture.GetWidth());
            int y = Math.Abs((int)((1 - v) * texture.GetHeight()) % texture.GetHeight());

            int r = 0;
            int g = 0;
            int b = 0;
        
            unsafe
            {
                byte* ptr = (byte*)(texture.GetBmData().Scan0);
                byte* row = ptr + (y * texture.GetBmData().Stride);
                b = row[x * 3];
                g = row[x * 3 + 1];
                r = row[x * 3 + 2];
            }

            return new Color4((byte)r, (byte)g, (byte)b);
        }

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
                Color4 vertexColor = new Color4(0, 0, 0);
                Color4 lightColor = new Color4(0, 0, 0);
                if (scene.light.IsEnable)
                {
                    Color4 c1 = scene.light.GetFinalLightColor(nDotL1);
                    Color4 c2 = scene.light.GetFinalLightColor(nDotL2);
                    lightColor = MathUtil.ColorInterp(c1, c2, ratio);
                }else
                {
                    vertexColor = MathUtil.ColorInterp(v1.Color, v2.Color, ratio);
                }
                float z = MathUtil.Interp(z1, z2, ratio);
                DrawPoint(new Vector4((int)x, (int)y, z, 0), vertexColor + lightColor);
                x += xInc;
                y += yInc;
            }
        }

        public void DrawTriangle(VertexTriangle vt, Scene scene)
        {
            Vector4 normal = vt.VertexA.Normal;
            Vector4 dir = scene.camera.GetDir();
            float dot = Vector4.Dot(dir, normal);
            if (dot > 0) return;
            this.scanLine.ProcessScanLine(vt, scene);
        }

        /*
        public void DrawTriangle(VertexTriangle vt, Scene scene)
        {
            Vector4 normal = vt.VertexA.Normal;
            Vector4 dir = scene.camera.GetDir();
            float dot = Vector4.Dot(dir, normal);
            if (dot > 0) return;

            List<Vertex> vList = new List<Vertex>();
            vList.Add(vt.VertexA);
            vList.Add(vt.VertexB);
            vList.Add(vt.VertexC);
            vList.Sort();

            //Vector4 p1 = this.ViewPort(vList[0].ClipSpacePosition);
            //Vector4 p2 = this.ViewPort(vList[1].ClipSpacePosition);
            //Vector4 p3 = this.ViewPort(vList[2].ClipSpacePosition);   
            Vector4 p1 = vList[0].ScreenSpacePosition;
            Vector4 p2 = vList[1].ScreenSpacePosition;
            Vector4 p3 = vList[2].ScreenSpacePosition;       

            //if (p1.Y > p2.Y)
            //{
            //    var temp = p2;
            //    p2 = p1;
            //    p1 = temp;
            //}

            //if (p2.Y > p3.Y)
            //{
            //    var temp = p2;
            //    p2 = p3;
            //    p3 = temp;
            //}

            //if (p1.Y > p2.Y)
            //{
            //    var temp = p2;
            //    p2 = p1;
            //    p1 = temp;
            //}

            float dP1P2, dP1P3, dP2P3;
            if (p2.Y - p1.Y > 0)
                dP1P2 = (p2.X - p1.X) / (p2.Y - p1.Y);
            else
                dP1P2 = 0;

            if (p3.Y - p1.Y > 0)
                dP1P3 = (p3.X - p1.X) / (p3.Y - p1.Y);
            else
                dP1P3 = 0;

            if (p3.Y - p2.Y > 0)
                dP2P3 = (p3.X - p2.X) / (p3.Y - p2.Y);
            else
                dP2P3 = 0;

            if(dP1P2 == 0)
            {
                if (p1.X > p2.X)
                {
                    var temp = vList[0];
                    vList[0] = vList[1];
                    vList[1] = temp;
                }
                for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    //this.scanLine.ProcessScanLineAd(y, p1, p3, p2, p3, scene);
                    this.scanLine.ProcessScanLineAd(y, vList[0], vList[2], vList[1], vList[2], scene);
                }
            }
            else
            {
                if (dP1P2 > dP1P3)
                {
                    for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                    {
                        if (y < p2.Y)
                        {
                            //this.scanLine.ProcessScanLineAd(y, p1, p3, p1, p2, scene);
                            this.scanLine.ProcessScanLineAd(y, vList[0], vList[2], vList[0], vList[1], scene);
                        }
                        else
                        {
                            //this.scanLine.ProcessScanLineAd(y, p1, p3, p2, p3, scene);
                            this.scanLine.ProcessScanLineAd(y, vList[0], vList[2], vList[1], vList[2], scene);
                        }
                    }
                }
                else
                {
                    for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                    {
                        if (y < p2.Y)
                        {
                            //this.scanLine.ProcessScanLineAd(y, p1, p2, p1, p3, scene);
                            this.scanLine.ProcessScanLineAd(y, vList[0], vList[1], vList[0], vList[2], scene);
                        }
                        else
                        {
                            //this.scanLine.ProcessScanLineAd(y, p2, p3, p1, p3, scene);
                            this.scanLine.ProcessScanLineAd(y, vList[1], vList[2], vList[0], vList[2], scene);
                        }
                    }
                }
            }
        }
        */

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
            Matrix4x4 projection = this.scene.camera.Perspective();
            return model * view * projection;
        }

        public void Render(Scene scene, BitmapData bmData)
        {
            this.scene = scene;
            this.bmData = bmData;
            Matrix4x4 matrixMVP = this.GetMvpMatrix();

            foreach (var triangle in scene.mesh.triangles)
            {
                Vertex vertexA = scene.mesh.Vertices[triangle.AIndex];
                Vertex vertexB = scene.mesh.Vertices[triangle.BIndex];
                Vertex vertexC = scene.mesh.Vertices[triangle.CIndex];

                List<Vertex> pIn = new List<Vertex>();
 
                vertexA.ClipSpacePosition = this.ClipSpace(vertexA.Position, matrixMVP);
                vertexB.ClipSpacePosition = this.ClipSpace(vertexB.Position, matrixMVP);
                vertexC.ClipSpacePosition = this.ClipSpace(vertexC.Position, matrixMVP);
                vertexA.ScreenSpacePosition = this.ViewPort(vertexA.ClipSpacePosition);
                vertexB.ScreenSpacePosition = this.ViewPort(vertexB.ClipSpacePosition);
                vertexC.ScreenSpacePosition = this.ViewPort(vertexC.ClipSpacePosition);
               
                pIn.Add(vertexA);
                pIn.Add(vertexB);
                pIn.Add(vertexC);

                for (int i = 0; i < 6; i++)
                {
                    if (pIn.Count == 0) break;
                    clip = new HodgmanClip(this);
                    clip.HodgmanPolygonClip((HodgmanClip.Boundary)i, wMin, wMax, pIn.ToArray());
                    pIn = clip.GetOutputList();
                }
                List<VertexTriangle> vtList = this.MakeTriangle(pIn);

                if (scene.renderState == Scene.RenderState.WireFrame)
                {       
                    // 画线框, 需要vertex的normal,pos,color
                    //DrawLine(vertexA, vertexB, this.ViewPort(vertexA.ClipSpacePosition), this.ViewPort(vertexB.ClipSpacePosition), scene);
                    //DrawLine(vertexB, vertexC, this.ViewPort(vertexB.ClipSpacePosition), this.ViewPort(vertexC.ClipSpacePosition), scene);
                    //DrawLine(vertexC, vertexA, this.ViewPort(vertexC.ClipSpacePosition), this.ViewPort(vertexA.ClipSpacePosition), scene);
                    //DrawLine(vertexA, vertexB, pixelA, pixelB, scene);
                    //DrawLine(vertexB, vertexC, pixelB, pixelC, scene);

                    //Vertex start = pIn[pIn.Count - 1];
                    //for (int i = 0; i < pIn.Count; i++)
                    //{
                    //    Vector4 viewPortA = this.ViewPort(start.ClipSpacePosition);
                    //    Vector4 viewPortB = this.ViewPort(pIn[i].ClipSpacePosition);
                    //    DrawLine(start, pIn[i], viewPortA, viewPortB, scene);
                    //    start = pIn[i];
                    //}
                    for (int i = 0; i < vtList.Count; i++)
                    {
                        Vertex vertex1 = vtList[i].VertexA;
                        Vertex vertex2 = vtList[i].VertexB;
                        Vertex vertex3 = vtList[i].VertexC;
                        Vector4 viewPort1 = this.ViewPort(vertex1.ClipSpacePosition);
                        Vector4 viewPort2 = this.ViewPort(vertex2.ClipSpacePosition);
                        Vector4 viewPort3 = this.ViewPort(vertex3.ClipSpacePosition);
                        DrawLine(vertex1, vertex2, viewPort1, viewPort2, scene);
                        DrawLine(vertex2, vertex3, viewPort2, viewPort3, scene);
                        DrawLine(vertex3, vertex1, viewPort3, viewPort1, scene);
                    }
                }
                else
                {        
                    // 填充三角形                  
                    for (int i = 0; i < vtList.Count; i++)
                    {
                        //DrawTriangle(vtList[i], scene);
                        DrawTriangle(vtList[i], scene);
                    }
                }
            }
        }

        private List<VertexTriangle> MakeTriangle(List<Vertex> input)
        {
            List<VertexTriangle> temp = new List<VertexTriangle>();
            for (int i = 0; i < input.Count - 2; i++)
            {
                VertexTriangle vt = new VertexTriangle(input[0], input[i + 1], input[i + 2]);
                temp.Add(vt);
            }
            return temp;
        }
    }
}
