using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    public class ScanLine
    {
        private Device device;
        private Edge[] ET;
        private Edge AEL;
        private int height;
        private int minY;
        private int maxY;

        public ScanLine(Device device)
        {
            this.device = device;
            this.height = device.GetHeight();
            this.ET = new Edge[device.GetHeight()];
            for (int i = 0; i < device.GetHeight(); i++)
            {
                this.ET[i] = new Edge();
            }
        }

        // 需要屏幕坐标
        public void ProcessScanLine(Vertex[] vertices, Matrix4x4 mvp, DirectionalLight light)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                for (int j = i + 1; j < vertices.Length; j++)
                {
                    Vector4 screen1 = this.device.Project(vertices[i].Position, mvp);
                    Vector4 screen2 = this.device.Project(vertices[j].Position, mvp);
                    if (screen1.Y != screen2.Y)
                    {
                        int x1 = (int)screen1.X;
                        int y1 = (int)screen1.Y;
                        int x2 = (int)screen2.X;
                        int y2 = (int)screen2.Y;
                        int ymin = y1 > y2 ? y2 : y1;
                        int ymax = y1 > y2 ? y1 : y2;
                        float x = y1 > y2 ? x2 : x1;
                        float dx = (x1 - x2) * 1.0f / (y1 - y2);
                        Edge e = new Edge();
                        e.yMax = ymax;
                        e.x = x;
                        e.deltaX = dx;
                        e.v1 = y1 > y2 ? vertices[j] : vertices[i];
                        e.v2 = y1 > y2 ? vertices[i] : vertices[j];
                        InsertEdge(ref ET[ymin].nextEdge, e);
                    }
                }
            }

            // 置空活动边表
            AEL = new Edge();
            for (int i = 0; i < this.height; i++)
            {
                //if (ET[i].nextEdge != null)
                {
                    // 将边表的边插入活动边表，并删除边表里的边
                    while (ET[i].nextEdge != null)
                    {
                        InsertEdge(ref AEL, ET[i].nextEdge);
                        ET[i].nextEdge = ET[i].nextEdge.nextEdge;
                    }
                    
                    if (AEL.nextEdge == null) continue;

                    // 填充扫描线
                    Edge a1 = (Edge)AEL.nextEdge.Clone();
                    Edge a2 = (Edge)AEL.nextEdge.nextEdge.Clone();

                    Vector4 screenA1V1 = this.device.Project(a1.v1.Position, mvp);
                    Vector4 screenA1V2 = this.device.Project(a1.v2.Position, mvp);
                    Vector4 screenA2V1 = this.device.Project(a2.v1.Position, mvp);
                    Vector4 screenA2V2 = this.device.Project(a2.v2.Position, mvp);
                    float r1 = (float)(i - (int)screenA1V1.Y) / (float)(screenA1V2.Y - screenA1V1.Y);
                    float r2 = (float)(i - (int)screenA2V1.Y) / (float)(screenA2V2.Y - screenA2V1.Y);
                    Color c1 = MathUtil.ColorInterp(a1.v1.Color, a1.v2.Color, r1);
                    Color c2 = MathUtil.ColorInterp(a2.v1.Color, a2.v2.Color, r2);
                    float z1 = MathUtil.Interp(screenA1V1.Z, screenA1V2.Z, r1);
                    float z2 = MathUtil.Interp(screenA2V1.Z, screenA2V2.Z, r2);

                    float nDotLA1V1 = light.ComputeNDotL(a1.v1.Position, a1.v1.Normal);   // 这里暂时默认模型空间就是世界空间
                    float nDotLA1V2 = light.ComputeNDotL(a1.v2.Position, a1.v2.Normal);
                    float nDotLA2V1 = light.ComputeNDotL(a2.v1.Position, a2.v1.Normal);
                    float nDotLA2V2 = light.ComputeNDotL(a2.v2.Position, a2.v2.Normal);
                    float nDotL1 = MathUtil.Interp(nDotLA1V1, nDotLA1V2, r1);
                    float nDotL2 = MathUtil.Interp(nDotLA2V1, nDotLA2V2, r2);

                    while (a1 != null && a2 != null)
                    {
                        for (int x = (int)AEL.nextEdge.x; x < (int)AEL.nextEdge.nextEdge.x; x++)
                        {
                            float r3 = (float)(x - (int)AEL.nextEdge.x) / (AEL.nextEdge.nextEdge.x - AEL.nextEdge.x);
                            Color c3 = MathUtil.ColorInterp(c1, c2, r3);
                            float z = MathUtil.Interp(z1, z2, r3);
                            float nDotL = MathUtil.Interp(nDotL1, nDotL2, r3);
                            //this.device.DrawPoint(new Vector4(x, i, z, 0), MathUtil.AddColor(MathUtil.MulColor(c3, nDotL), DirectionalLight.AmbientColor));
                            this.device.DrawPoint(new Vector4(x, i, z, 0), light.GetFinalLightColor(nDotL));
                        }
                        if (a2.nextEdge != null && a2.nextEdge.nextEdge != null)
                        {
                            a1 = (Edge)a2.nextEdge.Clone();
                            a2 = (Edge)a1.nextEdge.Clone();
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    // 删除y=yMax-1的边
                    Edge p = AEL;
                    while (p.nextEdge != null)
                    {
                        if (p.nextEdge.yMax - 1 == i)
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
            Edge newEdge = (Edge)e.Clone();
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

    }
}
