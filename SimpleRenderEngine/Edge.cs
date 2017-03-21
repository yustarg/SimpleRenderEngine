using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    public class Edge
    {
        public int yMax;                // 边的上端点的y坐标
        public float x;                 // AEL中表示当前扫描线与边的交点的x坐标， ET中的值为边的下端点的x坐标
        public float deltaX;            // 边的斜率的倒数
        public Edge nextEdge;           // 指向下一条边

        public Edge() { }

        public Edge(Edge e)
        {
            this.yMax = e.yMax;
            this.x = e.x;
            this.deltaX = e.deltaX;
            this.nextEdge = e.nextEdge;
        }
    }
}
