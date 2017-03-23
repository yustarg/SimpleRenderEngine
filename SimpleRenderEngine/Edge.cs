using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    public class Edge : ICloneable
    {
        public int yMax;                // 边的上端点的y坐标
        public float x;                 // AEL中表示当前扫描线与边的交点的x坐标， ET中的值为边的下端点的x坐标
        public float deltaX;            // 边的斜率的倒数
        public Edge nextEdge;           // 指向下一条边

        public Vertex v1;               // 总保存y较小的点
        public Vertex v2;               // 总保存y较大的点

        public Edge() { }

        // 深拷贝
        public Object Clone()
        {
            Edge e = new Edge();
            e.deltaX = this.deltaX;
            e.x = this.x;
            e.yMax = this.yMax;
            e.nextEdge = this.nextEdge;
            e.v1 = this.v1;
            e.v2 = this.v2;
            return e;
        }
    }
}
