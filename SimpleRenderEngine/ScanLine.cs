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
        public void ProcessScanLine(Vertex[] vertices, Scene scene)
        {
            int yMin = this.height;
            int yMax = 0;

            for (int i = 0; i < vertices.Length; i++)
            {
                for (int j = i + 1; j < vertices.Length; j++)
                {
                    //Vector4 screen1 = this.device.Project(vertices[i].Position, mvp);
                    //Vector4 screen2 = this.device.Project(vertices[j].Position, mvp);
                    Vector4 screen1 = this.device.ViewPort(vertices[i].ClipSpacePosition);
                    Vector4 screen2 = this.device.ViewPort(vertices[j].ClipSpacePosition);

                    if ((int)screen1.Y != (int)screen2.Y)
                    {
                        if (screen1.Y > yMax) yMax = (int)screen1.Y;
                        if (screen2.Y > yMax) yMax = (int)screen2.Y;
                        if (screen1.Y < yMin) yMin = (int)screen1.Y;
                        if (screen2.Y < yMin) yMin = (int)screen2.Y;
                        if (yMax > this.height) yMax = this.height;
                        if (yMin < 0) yMin = 0;

                        int x1 = (int)screen1.X;
                        int y1 = (int)screen1.Y;
                        int x2 = (int)screen2.X;
                        int y2 = (int)screen2.Y;
                        int ymin = y1 > y2 ? y2 : y1;
                        if (ymin < 0) ymin = 0;
                        int ymax = y1 > y2 ? y1 : y2;
                        if (ymax > this.height) ymax = this.height;
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
            for (int i = yMin; i < yMax; i++)
            {
                // 将边表的边插入活动边表，并删除边表里的边
                while (ET[i].nextEdge != null)
                {
                    InsertEdge(ref AEL.nextEdge, ET[i].nextEdge);
                    ET[i].nextEdge = ET[i].nextEdge.nextEdge;
                }
                    
                if (AEL.nextEdge == null) continue;

                // 填充扫描线
                Edge a1 = (Edge)AEL.nextEdge.Clone();
                Edge a2 = (Edge)AEL.nextEdge.nextEdge.Clone();

                // 插值
                Vector4 screenA1V1 = this.device.ViewPort(a1.v1.ClipSpacePosition); //this.device.Project(a1.v1.Position, mvp);
                Vector4 screenA1V2 = this.device.ViewPort(a1.v2.ClipSpacePosition);//this.device.Project(a1.v2.Position, mvp);
                Vector4 screenA2V1 = this.device.ViewPort(a2.v1.ClipSpacePosition);//this.device.Project(a2.v1.Position, mvp);
                Vector4 screenA2V2 = this.device.ViewPort(a2.v2.ClipSpacePosition);//this.device.Project(a2.v2.Position, mvp);
                float r1 = (float)(i - (int)screenA1V1.Y) / (float)(screenA1V2.Y - screenA1V1.Y);
                float r2 = (float)(i - (int)screenA2V1.Y) / (float)(screenA2V2.Y - screenA2V1.Y);
                Color c1 = MathUtil.ColorInterp(a1.v1.Color, a1.v2.Color, r1);
                Color c2 = MathUtil.ColorInterp(a2.v1.Color, a2.v2.Color, r2);
                float z1 = MathUtil.Interp(screenA1V1.Z, screenA1V2.Z, r1);
                float z2 = MathUtil.Interp(screenA2V1.Z, screenA2V2.Z, r2);

                float nDotLA1V1 = scene.light.ComputeNDotL(a1.v1.Position, a1.v1.Normal);   // 这里暂时默认模型空间就是世界空间
                float nDotLA1V2 = scene.light.ComputeNDotL(a1.v2.Position, a1.v2.Normal);
                float nDotLA2V1 = scene.light.ComputeNDotL(a2.v1.Position, a2.v1.Normal);
                float nDotLA2V2 = scene.light.ComputeNDotL(a2.v2.Position, a2.v2.Normal);
                float nDotL1 = MathUtil.Interp(nDotLA1V1, nDotLA1V2, r1);
                float nDotL2 = MathUtil.Interp(nDotLA2V1, nDotLA2V2, r2);

                float u1 = 0;
                float v1 = 0;
                float u2 = 0;
                float v2 = 0;
                //float w11 = mvp.Apply(a1.v1.Position).W;
                //float w12 = mvp.Apply(a1.v2.Position).W;
                //float w21 = mvp.Apply(a2.v1.Position).W;
                //float w22 = mvp.Apply(a2.v2.Position).W;
                float w11 = a1.v1.ClipSpacePosition.W;
                float w12 = a1.v2.ClipSpacePosition.W;
                float w21 = a2.v1.ClipSpacePosition.W;
                float w22 = a2.v2.ClipSpacePosition.W;
                float w1 = 0;
                float w2 = 0;
                if (scene.renderState == Scene.RenderState.TextureMapping)
                {
                    u1 = MathUtil.Interp(a1.v1.UV.X / w11, a1.v2.UV.X / w12, r1);
                    v1 = MathUtil.Interp(a1.v1.UV.Y / w11, a1.v2.UV.Y / w12, r1);
                    u2 = MathUtil.Interp(a2.v1.UV.X / w21, a2.v2.UV.X / w22, r2);
                    v2 = MathUtil.Interp(a2.v1.UV.Y / w21, a2.v2.UV.Y / w22, r2);
                    w1 = MathUtil.Interp(1 / w11, 1 / w12, r1);
                    w2 = MathUtil.Interp(1 / w21, 1 / w22, r2);
                }

                while (a1 != null && a2 != null)
                {
                    for (int x = (int)AEL.nextEdge.x; x < (int)AEL.nextEdge.nextEdge.x; x++)
                    {
                        float r3 = MathUtil.Clamp01((float)(x - a1.x) / (a2.x - a1.x));
                        Color c3 = MathUtil.ColorInterp(c1, c2, r3);
                        float z = MathUtil.Interp(z1, z2, r3);
                        float nDotL = MathUtil.Interp(nDotL1, nDotL2, r3);
                        Color final = Color.FromArgb(255, 255, 255);
                        if (scene.renderState == Scene.RenderState.GouraudShading)
                        {
                            final = scene.light.IsEnable ? scene.light.GetFinalLightColor(nDotL) : c3;                            
                        }
                        else if (scene.renderState == Scene.RenderState.TextureMapping)
                        {
                            float w = MathUtil.Interp(w1, w2, r3);
                            float u3 = MathUtil.Interp(u1, u2, r3) / w;
                            float v3 = MathUtil.Interp(v1, v2, r3) / w;
                            if (scene.light.IsEnable)
                            {
                                final = MathUtil.MulColor(scene.light.GetDiffuseColor(nDotL), this.device.Tex2D(u3, v3, scene.mesh.texture));
                                final = MathUtil.AddColor(final, DirectionalLight.AmbientColor);
                            }
                            else
                            {
                                final = this.device.Tex2D(u3, v3, scene.mesh.texture);
                            }
                        }
                        this.device.DrawPoint(new Vector4(x, i, z, 0), final);
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
            if (previous == null) root = newEdge;
            else previous.nextEdge = newEdge;  
        }

    }
}
