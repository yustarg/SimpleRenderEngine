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
        private Color4 final;

        public ScanLine(Device device)
        {
            this.device = device;
            this.height = device.GetHeight();
            this.ET = new Edge[device.GetHeight()];
            for (int i = 0; i < device.GetHeight(); i++)
            {
                this.ET[i] = new Edge();
            }
            final = new Color4(255, 255, 255);
        }

        #region 简单scanline
        public void ProcessScanLineAd(int y, Vertex v1, Vertex v2, Vertex v3, Vertex v4, Scene scene)
        {
            var pa = v1.ScreenSpacePosition;
            var pb = v2.ScreenSpacePosition;
            var pc = v3.ScreenSpacePosition;
            var pd = v4.ScreenSpacePosition;
            var gradient1 = pa.Y != pb.Y ? (y - pa.Y) / (pb.Y - pa.Y) : 1;
            var gradient2 = pc.Y != pd.Y ? (y - pc.Y) / (pd.Y - pc.Y) : 1;

            int sx = (int)MathUtil.Interp(pa.X, pb.X, gradient1);
            int ex = (int)MathUtil.Interp(pc.X, pd.X, gradient2);

            float z1 = MathUtil.Interp(pa.Z, pb.Z, gradient1);
            float z2 = MathUtil.Interp(pc.Z, pd.Z, gradient2);

            for (var x = sx; x < ex; x++)
            {
                float gradient = (x - sx) / (float)(ex - sx);
                var z = MathUtil.Interp(z1, z2, gradient);

                if (scene.renderState == Scene.RenderState.GouraudShading)
                {
                    if (scene.light.IsEnable)
                    {
                        float nDotLA1V1 = scene.light.ComputeNDotL(v1.Position, v1.Normal);
                        float nDotLA1V2 = scene.light.ComputeNDotL(v2.Position, v2.Normal);
                        float nDotLA2V1 = scene.light.ComputeNDotL(v3.Position, v3.Normal);
                        float nDotLA2V2 = scene.light.ComputeNDotL(v4.Position, v4.Normal);
                        float nDotL1 = MathUtil.Interp(nDotLA1V1, nDotLA1V2, gradient1);
                        float nDotL2 = MathUtil.Interp(nDotLA2V1, nDotLA2V2, gradient2);
                        float nDotL = MathUtil.Interp(nDotL1, nDotL2, gradient);
                        final = scene.light.GetFinalLightColor(nDotL);
                    }
                    else
                    {
                        Color4 c1 = MathUtil.ColorInterp(v1.Color, v2.Color, gradient1);
                        Color4 c2 = MathUtil.ColorInterp(v3.Color, v4.Color, gradient2);
                        Color4 c3 = MathUtil.ColorInterp(c1, c2, gradient);
                        //vt.CalWeight(s1, s2, s3, new Vector4(x, i, 0, 0));
                        //Color4 c3 = vt.GetInterColor();
                        final = c3;
                    }
                }
                else if (scene.renderState == Scene.RenderState.TextureMapping)
                {
                    float w11 = v1.ClipSpacePosition.W;
                    float w12 = v2.ClipSpacePosition.W;
                    float w21 = v3.ClipSpacePosition.W;
                    float w22 = v4.ClipSpacePosition.W;

                    float uu1 = MathUtil.Interp(v1.UV.X / w11, v2.UV.X / w12, gradient1);
                    float vv1 = MathUtil.Interp(v1.UV.Y / w11, v2.UV.Y / w12, gradient1);
                    float uu2 = MathUtil.Interp(v3.UV.X / w21, v4.UV.X / w22, gradient2);
                    float vv2 = MathUtil.Interp(v3.UV.Y / w21, v4.UV.Y / w22, gradient2);
                    float w1 = MathUtil.Interp(1 / w11, 1 / w12, gradient1);
                    float w2 = MathUtil.Interp(1 / w21, 1 / w22, gradient2);

                    float w = MathUtil.Interp(w1, w2, gradient);
                    float uu3 = MathUtil.Interp(uu1, uu2, gradient) / w;
                    float vv3 = MathUtil.Interp(vv1, vv2, gradient) / w;

                    final = this.device.Tex2D(uu3, vv3, scene.mesh.texture);
                    //final = this.device.Tex2D(u / w, v / w, scene.mesh.texture);
                }
                this.device.DrawPoint(new Vector4(x, y, z, 0), final);
            }
        }
        #endregion

        // 需要屏幕坐标
        public void ProcessScanLine(VertexTriangle vt, Scene scene)
        {
            int yMin = this.height;
            int yMax = 0;

            Vertex[] vertices = new Vertex[3];
            vertices[0] = vt.VertexA;
            vertices[1] = vt.VertexB;
            vertices[2] = vt.VertexC;

            for (int i = 0; i < vertices.Length; i++)
            {
                for (int j = i + 1; j < vertices.Length; j++)
                {
                    Vector4 screen1 = vertices[i].ScreenSpacePosition;
                    Vector4 screen2 = vertices[j].ScreenSpacePosition;
                    //Vector4 screen1 = this.device.ViewPort(vertices[i].ClipSpacePosition);
                    //Vector4 screen2 = this.device.ViewPort(vertices[j].ClipSpacePosition);

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
                //Vector4 screenA1V1 = this.device.ViewPort(a1.v1.ClipSpacePosition); //this.device.Project(a1.v1.Position, mvp);
                //Vector4 screenA1V2 = this.device.ViewPort(a1.v2.ClipSpacePosition);//this.device.Project(a1.v2.Position, mvp);
                //Vector4 screenA2V1 = this.device.ViewPort(a2.v1.ClipSpacePosition);//this.device.Project(a2.v1.Position, mvp);
                //Vector4 screenA2V2 = this.device.ViewPort(a2.v2.ClipSpacePosition);//this.device.Project(a2.v2.Position, mvp);
                Vector4 screenA1V1 = a1.v1.ScreenSpacePosition;
                Vector4 screenA1V2 = a1.v2.ScreenSpacePosition;
                Vector4 screenA2V1 = a2.v1.ScreenSpacePosition;
                Vector4 screenA2V2 = a2.v2.ScreenSpacePosition;
                float r1 = (float)(i - (int)screenA1V1.Y) / (float)(screenA1V2.Y - screenA1V1.Y);
                float r2 = (float)(i - (int)screenA2V1.Y) / (float)(screenA2V2.Y - screenA2V1.Y);
                float z1 = MathUtil.Interp(screenA1V1.Z, screenA1V2.Z, r1);
                float z2 = MathUtil.Interp(screenA2V1.Z, screenA2V2.Z, r2);

                float nDotL1 = 0, nDotL2 = 0;
                if (scene.light.IsEnable)
                {
                    float nDotLA1V1 = scene.light.ComputeNDotL(a1.v1.Position, a1.v1.Normal);
                    float nDotLA1V2 = scene.light.ComputeNDotL(a1.v2.Position, a1.v2.Normal);
                    float nDotLA2V1 = scene.light.ComputeNDotL(a2.v1.Position, a2.v1.Normal);
                    float nDotLA2V2 = scene.light.ComputeNDotL(a2.v2.Position, a2.v2.Normal);
                    nDotL1 = MathUtil.Interp(nDotLA1V1, nDotLA1V2, r1);
                    nDotL2 = MathUtil.Interp(nDotLA2V1, nDotLA2V2, r2);
                }
                Color4 c1 = MathUtil.ColorInterp(a1.v1.Color, a1.v2.Color, r1);
                Color4 c2 = MathUtil.ColorInterp(a2.v1.Color, a2.v2.Color, r2);
                Color4 c3 = null;

                float u1 = 0, v1 = 0, u2 = 0, v2 = 0, w1 = 0, w2 = 0;
                if (scene.renderState == Scene.RenderState.TextureMapping)
                {
                    float w11 = a1.v1.ClipSpacePosition.W;
                    float w12 = a1.v2.ClipSpacePosition.W;
                    float w21 = a2.v1.ClipSpacePosition.W;
                    float w22 = a2.v2.ClipSpacePosition.W;
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
                        //float r3 = MathUtil.Clamp01((float)(x - a1.x) / (a2.x - a1.x)); 
                        float r3 = (float)(x - Math.Floor(a1.x)) / (a2.x - a1.x);    
                        float z = MathUtil.Interp(z1, z2, r3);
                        if (scene.renderState == Scene.RenderState.GouraudShading)
                        {
                            if (scene.light.IsEnable)
                            {
                                float nDotL = MathUtil.Interp(nDotL1, nDotL2, r3);
                                //final = scene.light.GetFinalLightColor(nDotL);
                                c3 = MathUtil.ColorInterp(c1, c2, r3);
                                final = c3 * scene.light.GetDiffuseColor(nDotL) + DirectionalLight.AmbientColor;
                            }
                            else
                            {
                                c3 = MathUtil.ColorInterp(c1, c2, r3);
                                //vt.CalWeight(s1, s2, s3, new Vector4(x, i, 0, 0));
                                //Color4 c3 = vt.GetInterColor();
                                final = c3;
                            }
                        }
                        else if (scene.renderState == Scene.RenderState.TextureMapping)
                        {
                            //float w11 = mvp.Apply(a1.v1.Position).W;
                            //float w12 = mvp.Apply(a1.v2.Position).W;
                            //float w21 = mvp.Apply(a2.v1.Position).W;
                            //float w22 = mvp.Apply(a2.v2.Position).W;
                            //vt.CalWeight(s1, s2, s3, new Vector4(x, i, 0, 0));
                            //float weight1 = vt.Weight1;
                            //float weight2 = vt.Weight2;
                     
                            //float u = vt.GetInterValue(vt.VertexA.UV.X / vt.VertexA.ClipSpacePosition.W,
                            //                            vt.VertexB.UV.X / vt.VertexB.ClipSpacePosition.W,
                            //                            vt.VertexC.UV.X / vt.VertexC.ClipSpacePosition.W);
                            //float v = vt.GetInterValue(vt.VertexA.UV.Y / vt.VertexA.ClipSpacePosition.W,
                            //                            vt.VertexB.UV.Y / vt.VertexB.ClipSpacePosition.W,
                            //                            vt.VertexC.UV.Y / vt.VertexC.ClipSpacePosition.W);
                            //float w = vt.GetInterValue(1f / vt.VertexA.ClipSpacePosition.W, 
                            //                            1f / vt.VertexB.ClipSpacePosition.W,
                            //                            1f / vt.VertexC.ClipSpacePosition.W);

                           
                            float w = MathUtil.Interp(w1, w2, r3);
                            float u3 = MathUtil.Interp(u1 / w, u2 / w, r3);
                            float v3 = MathUtil.Interp(v1 / w, v2 / w, r3);
                            /*if (scene.light.IsEnable)
                            {
                                float nDotLA1V1 = scene.light.ComputeNDotL(a1.v1.Position, a1.v1.Normal);
                                float nDotLA1V2 = scene.light.ComputeNDotL(a1.v2.Position, a1.v2.Normal);
                                float nDotLA2V1 = scene.light.ComputeNDotL(a2.v1.Position, a2.v1.Normal);
                                float nDotLA2V2 = scene.light.ComputeNDotL(a2.v2.Position, a2.v2.Normal);
                                float nDotL1 = MathUtil.Interp(nDotLA1V1, nDotLA1V2, r1);
                                float nDotL2 = MathUtil.Interp(nDotLA2V1, nDotLA2V2, r2);
                                float nDotL = MathUtil.Interp(nDotL1, nDotL2, r3);
                                final = this.device.Tex2D(u3, v3, scene.mesh.texture) * scene.light.GetDiffuseColor(nDotL);
                                final += DirectionalLight.AmbientColor;
                            }*/
                            final = this.device.Tex2D(u3, v3, scene.mesh.texture);
                            //final = this.device.Tex2D(u / w, v / w, scene.mesh.texture);
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
