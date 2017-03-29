using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    // 原始三角形只存顶点的索引
    public class Triangle
    {
        public int AIndex { get; set; }
        public int BIndex { get; set; }
        public int CIndex { get; set; }

        public Triangle(int a, int b, int c)
        {
            this.AIndex = a;
            this.BIndex = b;
            this.CIndex = c;
        }
    }

    public class VertexTriangle
    {
        public Vertex VertexA { get; set; }
        public Vertex VertexB { get; set; }
        public Vertex VertexC { get; set; }
        public float Weight1 { get; set; }
        public float Weight2 { get; set; }

        public VertexTriangle(Vertex a, Vertex b, Vertex c)
        {
            this.VertexA = a;
            this.VertexB = b;
            this.VertexC = c;
        }

        public void CalWeight(Vector4 p1, Vector4 p2, Vector4 p3, Vector4 p)
        {
            int a = (int)(p2.X - p1.X);
            int b = (int)(p3.X - p1.X);
            int c = (int)(p2.Y - p1.Y);
            int d = (int)(p3.Y - p1.Y);
            int m = (int)(p.X - p1.X);
            int n = (int)(p.Y - p1.Y);
            this.Weight1 = (float)(b * n - d * m) / (float)(b * c - a * d);
            this.Weight2 = (float)(a * n - c * m) / (float)(a * d - b * c);
        }

        public Color4 GetInterColor()
        {
            byte r = (byte)((1 - Weight1 - Weight2) * (float)VertexA.Color.X + Weight1 * (float)VertexB.Color.X + Weight2 * (float)VertexC.Color.X);
            byte g = (byte)((1 - Weight1 - Weight2) * (float)VertexA.Color.Y + Weight1 * (float)VertexB.Color.Y + Weight2 * (float)VertexC.Color.Y);
            byte b = (byte)((1 - Weight1 - Weight2) * (float)VertexA.Color.Z + Weight1 * (float)VertexB.Color.Z + Weight2 * (float)VertexC.Color.Z);
            return new Color4(r, g, b);
        }

        public float GetInterValue(float a, float b, float c)
        {
            return ((1 - Weight1 - Weight2) * a + Weight1 * b + Weight2 * c);
        }
    }
}
