using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    public class HodgmanClip
    {
        public enum Boundary { Left, Right, Bottom, Top, Behind, Front };
        private Device device;

        private List<Vertex> outputList;

        public HodgmanClip(Device device)
        {
            this.outputList = new List<Vertex>();
            this.device = device;
        }

        public List<Vertex> GetOutputList()
        {
            return this.outputList;
        }

        // 求交点, 返回的pos是clip空间下坐标
        Vertex Intersect(Vertex v1, Vertex v2, Boundary b, Vector4 wMin, Vector4 wMax)
        {
            Vertex iPt = new Vertex();
            float m1 = 0, m2 = 0, m3 = 0, m4 = 0, m5 = 0, m6 = 0;
            Vector4 p1 = v1.Position;
            Vector4 p2 = v2.Position;
            if (p1.X != p2.X) { m1 = (wMin.X - p1.X) / (p2.X - p1.X); m2 = (wMax.X - p1.X) / (p2.X - p1.X); }
            if (p1.Y != p2.Y) { m3 = (wMin.Y - p1.Y) / (p2.Y - p1.Y); m4 = (wMax.Y - p1.Y) / (p2.Y - p1.Y); }
            if (p1.Z != p2.Z) { m5 = (wMin.Z - p1.Z) / (p2.Z - p1.Z); m6 = (wMax.Z - p1.Z) / (p2.Z - p1.Z); }
            Vector4 pos = new Vector4();
            Color col = Color.FromArgb(255, 255, 255);
            Vector4 normal = new Vector4();
            Vector4 uv = new Vector4();
            switch (b)
            {
                case Boundary.Left:
                    pos.X = wMin.X;
                    pos.Y = p1.Y + (p2.Y - p1.Y) * m1;
                    pos.Z = p1.Z + (p2.Z - p1.Z) * m1;
                    pos.W = 1;
                    col = MathUtil.ColorInterp(v1.Color, v2.Color, m1);
                    normal = MathUtil.Vector4Interp(v1.Normal, v2.Normal, m1);
                    uv = MathUtil.Vector4Interp(v1.UV, v2.UV, m1);                  
                    break;
                case Boundary.Right:
                    pos.X = wMax.X;
                    pos.Y = p1.Y + (p2.Y - p1.Y) * m2;
                    pos.Z = p1.Z + (p2.Z - p1.Z) * m2;
                    pos.W = 1;
                    col = MathUtil.ColorInterp(v1.Color, v2.Color, m2);
                    normal = MathUtil.Vector4Interp(v1.Normal, v2.Normal, m2);
                    uv = MathUtil.Vector4Interp(v1.UV, v2.UV, m2);
                    break;
                case Boundary.Bottom:
                    pos.Y = wMin.Y;
                    pos.X = p1.X + (p2.X - p1.X) * m3;
                    pos.Z = p1.Z + (p2.Z - p1.Z) * m3;
                    pos.W = 1;
                    col = MathUtil.ColorInterp(v1.Color, v2.Color, m3);
                    normal = MathUtil.Vector4Interp(v1.Normal, v2.Normal, m3);
                    uv = MathUtil.Vector4Interp(v1.UV, v2.UV, m3);
                    break;
                case Boundary.Top:
                    pos.Y = wMax.Y;
                    pos.X = p1.X + (p2.X - p1.X) * m4;
                    pos.Z = p1.Z + (p2.Z - p1.Z) * m4;
                    pos.W = 1;
                    col = MathUtil.ColorInterp(v1.Color, v2.Color, m4);
                    normal = MathUtil.Vector4Interp(v1.Normal, v2.Normal, m4);
                    uv = MathUtil.Vector4Interp(v1.UV, v2.UV, m4);
                    break;
                case Boundary.Behind:
                    pos.Z = wMin.Z;
                    pos.X = p1.X + (p2.X - p1.X) * m5;
                    pos.Y = p1.Y + (p2.Y - p1.Y) * m5;
                    pos.W = 1;
                    col = MathUtil.ColorInterp(v1.Color, v2.Color, m5);
                    normal = MathUtil.Vector4Interp(v1.Normal, v2.Normal, m5);
                    uv = MathUtil.Vector4Interp(v1.UV, v2.UV, m5);
                    break;
                case Boundary.Front:
                    pos.Z = wMax.Z;
                    pos.X = p1.X + (p2.X - p1.X) * m6;
                    pos.Y = p1.Y + (p2.Y - p1.Y) * m6;
                    pos.W = 1;
                    col = MathUtil.ColorInterp(v1.Color, v2.Color, m5);
                    normal = MathUtil.Vector4Interp(v1.Normal, v2.Normal, m5);
                    uv = MathUtil.Vector4Interp(v1.UV, v2.UV, m5);
                    break;
            }

            iPt.Position = pos;
            iPt.Normal = normal;
            iPt.UV = uv;
            iPt.Color = col;
            return iPt;
        }

        bool Inside(Vector4 p, Boundary b, Vector4 wMin, Vector4 wMax)
        {
            bool flag = true;
            switch (b)
            {
                case Boundary.Left:
                    if (p.X < wMin.X)
                        flag = false;
                    break;
                case Boundary.Right:
                    if (p.X > wMax.X)
                        flag = false;
                    break;
                case Boundary.Bottom:
                    if (p.Y < wMin.Y)
                        flag = false;
                    break;
                case Boundary.Top:
                    if (p.Y > wMax.Y)
                        flag = false;
                    break;
                case Boundary.Behind:
                    if (p.Z < wMin.Z)
                        flag = false;
                    break;
                case Boundary.Front:
                    if (p.Z > wMax.Z)
                        flag = false;
                    break;
            }
            return flag;
        }

        // 需要clip空间下的坐标（除以w分量）
        public void HodgmanPolygonClip(Boundary b, Vector4 wMin, Vector4 wMax, Vertex[] pIn)
        {
            Vertex s = pIn[pIn.Length - 1];
            for (int i = 0; i < pIn.Length; i++)
            {
                s.Position = this.device.ClipSpace(s.Position);
                pIn[i].Position = this.device.ClipSpace(pIn[i].Position);
                if (!Inside(s.Position, b, wMin, wMax) && Inside(pIn[i].Position, b, wMin, wMax))
                {
                    this.outputList.Add(Intersect(s, pIn[i], b, wMin, wMax));
                    this.outputList.Add(pIn[i]);
                }
                else if (Inside(s.Position, b, wMin, wMax) && Inside(pIn[i].Position, b, wMin, wMax))
                {
                    this.outputList.Add(pIn[i]);
                }
                else if (Inside(s.Position, b, wMin, wMax) && (!Inside(pIn[i].Position, b, wMin, wMax)))
                {
                    this.outputList.Add(Intersect(s, pIn[i], b, wMin, wMax));
                }
                s = pIn[i];
            }
        }
    }
}
