using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    public class HodgmanClip
    {
        public enum Boundary { Left, Right, Bottom, Top, Behind, Front };

        private List<Vector4> outputList;

        public HodgmanClip()
        {
            this.outputList = new List<Vector4>();
        }

        public List<Vector4> GetOutputList()
        {
            return this.outputList;
        }

        // 求交点
        Vector4 Intersect(Vector4 p1, Vector4 p2, Boundary b, Vector4 wMin, Vector4 wMax)
        {
            Vector4 iPt = new Vector4();
            float m1 = 0, m2 = 0, m3 = 0, m4 = 0, m5 = 0, m6 = 0;
            if (p1.X != p2.X) { m1 = (wMin.X - p1.X) / (p2.X - p1.X); m2 = (wMax.X - p1.X) / (p2.X - p1.X); }
            if (p1.Y != p2.Y) { m3 = (wMin.Y - p1.Y) / (p2.Y - p1.Y); m4 = (wMax.Y - p1.Y) / (p2.Y - p1.Y); }
            if (p1.Z != p2.Z) { m5 = (wMin.Z - p1.Z) / (p2.Z - p1.Z); m6 = (wMax.Z - p1.Z) / (p2.Z - p1.Z); }
            switch (b)
            {
                case Boundary.Left:
                    iPt.X = wMin.X;
                    iPt.Y = p1.Y + (p2.Y - p1.Y) * m1;
                    iPt.Z = p1.Z + (p2.Z - p1.Z) * m1;
                    break;
                case Boundary.Right:
                    iPt.X = wMax.X;
                    iPt.Y = p1.Y + (p2.Y - p1.Y) * m2;
                    iPt.Z = p1.Z + (p2.Z - p1.Z) * m2;
                    break;
                case Boundary.Bottom:
                    iPt.Y = wMin.Y;
                    iPt.X = p1.X + (p2.X - p1.X) * m3;
                    iPt.Z = p1.Z + (p2.Z - p1.Z) * m3;
                    break;
                case Boundary.Top:
                    iPt.Y = wMax.Y;
                    iPt.X = p1.X + (p2.X - p1.X) * m4;
                    iPt.Z = p1.Z + (p2.Z - p1.Z) * m4;
                    break;
                case Boundary.Behind:
                    iPt.Z = wMin.Z;
                    iPt.X = p1.X + (p2.X - p1.X) * m5;
                    iPt.Y = p1.Y + (p2.Y - p1.Y) * m5;
                    break;
                case Boundary.Front:
                    iPt.Z = wMax.Z;
                    iPt.X = p1.X + (p2.X - p1.X) * m6;
                    iPt.Y = p1.Y + (p2.Y - p1.Y) * m6;
                    break;
            }
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

        public void HodgmanPolygonClip(Boundary b, Vector4 wMin, Vector4 wMax, Vector4[] pIn)
        {
            Vector4 s = pIn[pIn.Length - 1];
            for (int i = 0; i < pIn.Length; i++)
            {
                if (!Inside(s, b, wMin, wMax) && Inside(pIn[i], b, wMin, wMax))
                {
                    this.outputList.Add(Intersect(s, pIn[i], b, wMin, wMax));
                    this.outputList.Add(pIn[i]);
                }
                else if (Inside(s, b, wMin, wMax) && Inside(pIn[i], b, wMin, wMax))
                {
                    this.outputList.Add(pIn[i]);
                }
                else if (Inside(s, b, wMin, wMax) && (!Inside(pIn[i], b, wMin, wMax)))
                {
                    this.outputList.Add(Intersect(s, pIn[i], b, wMin, wMax));
                }
                s = pIn[i];
            }
        }
    }
}
