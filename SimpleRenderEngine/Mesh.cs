﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    public class Mesh
    {
        public string Name { get; set; }
        public Vertex[] Vertices { get; private set; }
        public Triangle[] triangles { get; set; }
        public Texture texture;
        public Matrix4x4 rotation { get; set; }

        public Mesh(string name)
        {
            Name = name;
            texture = new Texture(@"..\..\textures\background1.jpg", 512, 512);//706, 530);
            this.rotation = new Matrix4x4();
        }

        public void SetVertices(Vertex[] vertices)
        {
            this.Vertices = vertices;
            this.MakeTriangles();
        }

        public void MakeTriangles()
        {
            triangles = new Triangle[] {
                // 正面
                new Triangle(2, 5, 8),
                new Triangle(2, 8, 11),
                // 右面
                new Triangle(4, 16, 7),
                new Triangle(16, 19, 7),
                // 左面
                new Triangle(13, 1, 10),
                new Triangle(13, 10, 22),
                // 背面
                new Triangle(17, 14, 23),
                new Triangle(17, 23, 20),
                // 上面
                new Triangle(9, 6, 18),
                new Triangle(9, 18, 21),
                // 下面
                new Triangle(12, 15, 3),
                new Triangle(12, 3, 0)
            };
        }
    }

    public class Vertex
    {
        public Vector4 Position { get; set; }
        public Vector4 ClipSpacePosition { get; set; }
        public Vector4 ScreenSpacePosition { get; set; }
        public Vector4 Normal { get; set; }
        public Vector4 UV { get; set; }
        public Color4 Color { get; set; }

        public Vertex() { }
        
        public Vertex(Vector4 pos, Vector4 normal, Vector4 uv, Color4 col)
        {
            this.Position = pos;
            this.Normal = normal;
            this.UV = uv;
            this.Color = col;
        }
    }
}
