using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    public class Mesh
    {
        public string Name { get; set; }
        public Vector4[] Vertices { get; private set; }
        public Triangle[] triangles { get; set; }

        public Mesh(string name)
        {
            Name = name;
        }

        public void SetVertices(Vector4[] vertices)
        {
            this.Vertices = vertices;
            this.MakeTriangles();
        }

        public void MakeTriangles()
        {
            triangles = new Triangle[12] {
                // 正面
                new Triangle(0, 1, 2),
                new Triangle(0, 2, 3),
                // 右面
                new Triangle(1, 5, 2),
                new Triangle(5, 6, 2),
                // 左面
                new Triangle(4, 0, 3),
                new Triangle(4, 3, 7),
                // 背面
                new Triangle(4, 5, 6),
                new Triangle(4, 6, 7),
                // 上面
                new Triangle(2, 6, 3),
                new Triangle(3, 6, 7),
                // 下面
                new Triangle(0, 1, 5),
                new Triangle(0, 5, 4)
            };
        }
    }
}
