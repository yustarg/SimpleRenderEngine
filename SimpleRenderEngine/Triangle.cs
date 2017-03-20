using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    // 三角形只存顶点的索引
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
}
