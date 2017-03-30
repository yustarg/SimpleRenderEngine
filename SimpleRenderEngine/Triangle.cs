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
        //public Vertex VertexA { get; set; }
        //public Vertex VertexB { get; set; }
        //public Vertex VertexC { get; set; }
        public Vertex[] Vertices { get; set; }
        public float Weight1 { get; set; }
        public float Weight2 { get; set; }
        
        // 做三角形重心插值时的二元一次方程组系数
        private int a, b, c, d, dn1, dn2;

        public VertexTriangle(Vertex a, Vertex b, Vertex c)
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
        }

        public void CalWeight(Vector4 p)
        {
            Vector4 p1 = this.Vertices[0].ScreenSpacePosition;            
            int m = (int)(p.X - p1.X);
            int n = (int)(p.Y - p1.Y);
            this.Weight1 = (float)(b * n - d * m) / (float)dn1;
            this.Weight2 = (float)(a * n - c * m) / (float)dn2;
        }

        //public Color4 GetInterColor()
        //{
        //    byte r = (byte)((1 - Weight1 - Weight2) * (float)VertexA.Color.X + Weight1 * (float)VertexB.Color.X + Weight2 * (float)VertexC.Color.X);
        //    byte g = (byte)((1 - Weight1 - Weight2) * (float)VertexA.Color.Y + Weight1 * (float)VertexB.Color.Y + Weight2 * (float)VertexC.Color.Y);
        //    byte b = (byte)((1 - Weight1 - Weight2) * (float)VertexA.Color.Z + Weight1 * (float)VertexB.Color.Z + Weight2 * (float)VertexC.Color.Z);
        //    return new Color4(r, g, b);
        //}

        public float GetInterValue(float a, float b, float c)
        {
            return ((1 - Weight1 - Weight2) * a + Weight1 * b + Weight2 * c);
        }

        public Vector4 GetInterUV()
        {
            float u = GetInterValue(Vertices[0].UV.X / Vertices[0].ClipSpacePosition.W, Vertices[1].UV.X / Vertices[1].ClipSpacePosition.W, Vertices[2].UV.X / Vertices[2].ClipSpacePosition.W);
            float v = GetInterValue(Vertices[0].UV.Y / Vertices[0].ClipSpacePosition.W, Vertices[1].UV.Y / Vertices[1].ClipSpacePosition.W, Vertices[2].UV.Y / Vertices[2].ClipSpacePosition.W);
            float w = GetInterValue(1f / Vertices[0].ClipSpacePosition.W, 1f / Vertices[1].ClipSpacePosition.W, 1f / Vertices[2].ClipSpacePosition.W);
            return new Vector4(u / w, v / w, 0, 0);
        }
    }
}
