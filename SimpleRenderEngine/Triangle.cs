using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    // 原始三角形只存顶点的索引
    public struct Triangle
    {
        public int AIndex { get; set; }
        public int BIndex { get; set; }
        public int CIndex { get; set; }

        public Triangle(int a, int b, int c) :this()
        {
            this.AIndex = a;
            this.BIndex = b;
            this.CIndex = c;
        }
    }

    public struct VertexTriangle
    {
        //public Vertex VertexA { get; set; }
        //public Vertex VertexB { get; set; }
        //public Vertex VertexC { get; set; }
        public Vertex[] Vertices { get; set; }
        public float Weight1 { get; set; }
        public float Weight2 { get; set; }
        
        // 做三角形重心插值时的二元一次方程组系数
        // P.x = (1 - u - v) * P1.x + u * P2.x + v * P3.x
        // P.y = (1 - u - v) * P1.y + u * P2.y + v * P3.y
        private int a, b, c, d, dn1, dn2;
        private float u1, v1;
        private float u2, v2;
        private float u3, v3;
        private float w1, w2, w3;

        public VertexTriangle(Vertex a, Vertex b, Vertex c):this()
        {
            this.Vertices = new Vertex[] { a, b, c };   
        }

        public void PreCalWeight()
        {
            Vector4 p1 = this.Vertices[0].ScreenSpacePosition;
            Vector4 p2 = this.Vertices[1].ScreenSpacePosition;
            Vector4 p3 = this.Vertices[2].ScreenSpacePosition;

            a = (int)(p2.X - p1.X);
            b = (int)(p3.X - p1.X);
            c = (int)(p2.Y - p1.Y);
            d = (int)(p3.Y - p1.Y);
            dn1 = (b * c - a * d);
            dn2 = (a * d - b * c);

            u1 = Vertices[0].UV.X / Vertices[0].ClipSpacePosition.W;
            u2 = Vertices[1].UV.X / Vertices[1].ClipSpacePosition.W;
            u3 = Vertices[2].UV.X / Vertices[2].ClipSpacePosition.W;
            v1 = Vertices[0].UV.Y / Vertices[0].ClipSpacePosition.W;
            v2 = Vertices[1].UV.Y / Vertices[1].ClipSpacePosition.W;
            v3 = Vertices[2].UV.Y / Vertices[2].ClipSpacePosition.W;
            w1 = 1f / Vertices[0].ClipSpacePosition.W;
            w2 = 1f / Vertices[1].ClipSpacePosition.W;
            w3 = 1f / Vertices[2].ClipSpacePosition.W;
        }

        // 插值之前必须先算权重
        public void CalWeight(Vector4 p)
        {
            Vector4 p1 = this.Vertices[0].ScreenSpacePosition;            
            int m = (int)(p.X - p1.X);
            int n = (int)(p.Y - p1.Y);
            this.Weight1 = (float)(b * n - d * m) / (float)dn1;
            this.Weight2 = (float)(a * n - c * m) / (float)dn2;
        }

        public float GetInterValue(float a, float b, float c)
        {
            return ((1 - Weight1 - Weight2) * a + Weight1 * b + Weight2 * c);
        }

        public Vector4 GetInterUV()
        {
            float u = GetInterValue(u1, u2, u3);
            float v = GetInterValue(v1, v2, v3);
            float w = GetInterValue(w1, w2, w3);
            return new Vector4(u / w, v / w, 0, 0);
        }
    }
}
