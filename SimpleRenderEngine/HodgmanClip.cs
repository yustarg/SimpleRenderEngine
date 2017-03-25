using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    public class Boundary
    {
        public Vector4 vertA;
        public Vector4 vertB;

        public Boundary(Vector4 a, Vector4 b)
        {
            this.vertA = a;
            this.vertB = b;
        }
    }
    
    public class HodgmanClip
    {
        private Boundary[] boundaries;

        public HodgmanClip()
        {
            //this.boundaries = new Boundary[4];
            //this.boundaries[0] = new Boundary(new Vector4(0, 0, 0, 0), new Vector4(width, 0, 0, 0));                        //下
            //this.boundaries[1] = new Boundary(new Vector4(width, 0, 0, 0), new Vector4(width, height, 0, 0));               //右
            //this.boundaries[2] = new Boundary(new Vector4(width, height, 0, 0), new Vector4(0, height, 0, 0));              //上
            //this.boundaries[3] = new Boundary(new Vector4(0, height, 0, 0), new Vector4(0, 0, 0, 0));                       //左
        }

        // 求交点
        private Vertex Intersect(Boundary boundary, Vertex s, Vertex p)
        {
            Vertex inter = new Vertex();
            Vector4 pos = new Vector4();
            if (boundary.vertA.Y == boundary.vertB.Y)
            {
                pos.Y = boundary.vertA.Y;
                pos.X = s.Position.X;
            }
            else
            { 
            
            }
            return inter;
        }

        // 是否在内侧
        private bool Inside()
        {
            return true;
        }

        // 输出顶点
        private void Output()
        { 
        
        }

        public void HodgmanPolygonClip()
        { 
            
        }
    }
}
