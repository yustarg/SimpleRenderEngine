using System.Collections.Generic;
using System.Drawing;


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
            Vector4 p1 = v1.ClipSpacePosition;
            Vector4 p2 = v2.ClipSpacePosition;
            if (p1.X != p2.X) { m1 = (wMin.X - p1.X) / (p2.X - p1.X); m2 = (wMax.X - p1.X) / (p2.X - p1.X); }
            if (p1.Y != p2.Y) { m3 = (wMin.Y - p1.Y) / (p2.Y - p1.Y); m4 = (wMax.Y - p1.Y) / (p2.Y - p1.Y); }
            if (p1.Z != p2.Z) { m5 = (wMin.Z - p1.Z) / (p2.Z - p1.Z); m6 = (wMax.Z - p1.Z) / (p2.Z - p1.Z); }
            Vector4 clipPos = new Vector4();
            Vector4 pos = new Vector4();
            Color4 col = new Color4(255, 255, 255);
            Vector4 normal = new Vector4();
            Vector4 uv = new Vector4();
            switch (b)
            {
                case Boundary.Left:
                    clipPos.X = wMin.X;
                    clipPos.Y = p1.Y + (p2.Y - p1.Y) * m1;
                    clipPos.Z = p1.Z + (p2.Z - p1.Z) * m1;
                    clipPos.W = p1.W + (p2.W - p1.W) * m1;
                    col = MathUtil.ColorInterp(v1.Color, v2.Color, m1);
                    normal = MathUtil.Vector4Interp(v1.Normal, v2.Normal, m1);
                    break;
                case Boundary.Right:
                    clipPos.X = wMax.X;
                    clipPos.Y = p1.Y + (p2.Y - p1.Y) * m2;
                    clipPos.Z = p1.Z + (p2.Z - p1.Z) * m2;
                    clipPos.W = p1.W + (p2.W - p1.W) * m2;
                    col = MathUtil.ColorInterp(v1.Color, v2.Color, m2);
                    normal = MathUtil.Vector4Interp(v1.Normal, v2.Normal, m2);
                    break;
                case Boundary.Bottom:
                    clipPos.Y = wMin.Y;
                    clipPos.X = p1.X + (p2.X - p1.X) * m3;
                    clipPos.Z = p1.Z + (p2.Z - p1.Z) * m3;
                    clipPos.W = p1.W + (p2.W - p1.W) * m3;
                    col = MathUtil.ColorInterp(v1.Color, v2.Color, m3);
                    normal = MathUtil.Vector4Interp(v1.Normal, v2.Normal, m3);
                    break;
                case Boundary.Top:
                    clipPos.Y = wMax.Y;
                    clipPos.X = p1.X + (p2.X - p1.X) * m4;
                    clipPos.Z = p1.Z + (p2.Z - p1.Z) * m4;
                    clipPos.W = p1.W + (p2.W - p1.W) * m4;
                    col = MathUtil.ColorInterp(v1.Color, v2.Color, m4);
                    normal = MathUtil.Vector4Interp(v1.Normal, v2.Normal, m4);
                    break;
                case Boundary.Behind:
                    clipPos.Z = wMin.Z;
                    clipPos.X = p1.X + (p2.X - p1.X) * m5;
                    clipPos.Y = p1.Y + (p2.Y - p1.Y) * m5;
                    clipPos.W = p1.W + (p2.W - p1.W) * m5;
                    col = MathUtil.ColorInterp(v1.Color, v2.Color, m5);
                    normal = MathUtil.Vector4Interp(v1.Normal, v2.Normal, m5);  
                    break;
                case Boundary.Front:
                    clipPos.Z = wMax.Z;
                    clipPos.X = p1.X + (p2.X - p1.X) * m6;
                    clipPos.Y = p1.Y + (p2.Y - p1.Y) * m6;
                    clipPos.W = p1.W + (p2.W - p1.W) * m6;
                    col = MathUtil.ColorInterp(v1.Color, v2.Color, m6);
                    normal = MathUtil.Vector4Interp(v1.Normal, v2.Normal, m6);
                    break;
            }

            iPt.Position = pos;
            iPt.ClipSpacePosition = clipPos;
            iPt.ScreenSpacePosition = this.device.ViewPort(clipPos);
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
                if (!Inside(s.ClipSpacePosition, b, wMin, wMax) && Inside(pIn[i].ClipSpacePosition, b, wMin, wMax))
                {
                    this.outputList.Add(Intersect(s, pIn[i], b, wMin, wMax));
                    this.outputList.Add(pIn[i]);
                }
                else if (Inside(s.ClipSpacePosition, b, wMin, wMax) && Inside(pIn[i].ClipSpacePosition, b, wMin, wMax))
                {
                    this.outputList.Add(pIn[i]);
                }
                else if (Inside(s.ClipSpacePosition, b, wMin, wMax) && (!Inside(pIn[i].ClipSpacePosition, b, wMin, wMax)))
                {
                    this.outputList.Add(Intersect(s, pIn[i], b, wMin, wMax));
                }
                s = pIn[i];
            }
        }
    }
}
