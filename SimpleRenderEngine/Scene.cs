using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    public class Scene
    {
        public enum RenderState { WireFrame = 0, GouraudShading, TextureMapping }
        public RenderState renderState;
        public DirectionalLight light;
        public Mesh mesh;
        public Camera camera;

        public Scene(int width, int height)
        {
            this.renderState = Scene.RenderState.WireFrame;
            InitCamera(width, height);
            InitMesh();
            InitLight();
        }

        private void InitMesh()
        {
            this.mesh = new Mesh("Cube", @"E:\SimpleRenderEngine\SimpleRenderEngine\SimpleRenderEngine\textures\background.jpg");
            //Vector4[] vertices = new Vector4[8] {
            //    new Vector4(-1, -1, -1, 1),
            //    new Vector4(1, -1, -1, 1),
            //    new Vector4(1, 1, -1, 1),
            //    new Vector4(-1, 1, -1, 1),
            //    new Vector4(-1, -1, 1, 1),
            //    new Vector4(1, -1, 1, 1),
            //    new Vector4(1, 1, 1, 1),
            //    new Vector4(-1, 1, 1, 1),
            //};

            // pos, normal, uv, color  法线方向按上下左右前后排列
            Vertex[] vertices = new Vertex[24] {
                new Vertex(new Vector4(-1, -1, -1, 1), new Vector4(0, -1, 0, 1), new Vector4(0, 0, 0, 0), Color.FromArgb(255, 0, 0)),
                new Vertex(new Vector4(-1, -1, -1, 1), new Vector4(-1, 0, 0, 1), new Vector4(1, 0, 0, 0), Color.FromArgb(255, 0, 0)),
                new Vertex(new Vector4(-1, -1, -1, 1), new Vector4(0, 0, -1, 1), new Vector4(0, 0, 0, 0), Color.FromArgb(255, 0, 0)),
                
                new Vertex(new Vector4(1, -1, -1, 1), new Vector4(0, -1, 0, 1), new Vector4(1, 0, 0, 0), Color.FromArgb(0, 255, 0)),
                new Vertex(new Vector4(1, -1, -1, 1), new Vector4(1, 0, 0, 1),  new Vector4(0, 0, 0, 0), Color.FromArgb(0, 255, 0)),
                new Vertex(new Vector4(1, -1, -1, 1), new Vector4(0, 0, -1, 1), new Vector4(1, 0, 0, 0), Color.FromArgb(0, 255, 0)),
                  
                new Vertex(new Vector4(1, 1, -1, 1), new Vector4(0, 1, 0, 1), new Vector4(1, 0, 0, 0), Color.FromArgb(0, 0, 255)),
                new Vertex(new Vector4(1, 1, -1, 1), new Vector4(1, 0, 0, 1), new Vector4(0, 1, 0, 0), Color.FromArgb(0, 0, 255)),
                new Vertex(new Vector4(1, 1, -1, 1), new Vector4(0, 0, -1, 1), new Vector4(1, 1, 0, 0), Color.FromArgb(0, 0, 255)),

                new Vertex(new Vector4(-1, 1, -1, 1), new Vector4(0, 1, 0, 1), new Vector4(0, 0, 0, 0), Color.FromArgb(255, 0, 0)),
                new Vertex(new Vector4(-1, 1, -1, 1), new Vector4(-1, 0, 0, 1), new Vector4(1, 1, 0, 0), Color.FromArgb(255, 0, 0)),
                new Vertex(new Vector4(-1, 1, -1, 1), new Vector4(0, 0, -1, 1), new Vector4(0, 1, 0, 0), Color.FromArgb(255, 0, 0)),
                
                new Vertex(new Vector4(-1, -1, 1, 1), new Vector4(0, -1, 0, 1), new Vector4(0, 1, 0, 0), Color.FromArgb(0, 255, 0)),
                new Vertex(new Vector4(-1, -1, 1, 1), new Vector4(-1, 0, 0, 1), new Vector4(0, 0, 0, 0), Color.FromArgb(0, 255, 0)),
                new Vertex(new Vector4(-1, -1, 1, 1), new Vector4(0, 0, 1, 1), new Vector4(0, 0, 0, 0), Color.FromArgb(0, 255, 0)),
                
                new Vertex(new Vector4(1, -1, 1, 1), new Vector4(0, -1, 0, 1), new Vector4(1, 1, 0, 0), Color.FromArgb(0, 0, 255)),
                new Vertex(new Vector4(1, -1, 1, 1), new Vector4(1, 0, 0, 1), new Vector4(1, 0, 0, 0), Color.FromArgb(0, 0, 255)),
                new Vertex(new Vector4(1, -1, 1, 1), new Vector4(0, 0, 1, 1), new Vector4(1, 0, 0, 0), Color.FromArgb(0, 0, 255)),
                
                new Vertex(new Vector4(1, 1, 1, 1), new Vector4(0, 1, 0, 1), new Vector4(1, 1, 0, 0), Color.FromArgb(255, 0, 0)),
                new Vertex(new Vector4(1, 1, 1, 1), new Vector4(1, 0, 0, 1), new Vector4(1, 1, 0, 0), Color.FromArgb(255, 0, 0)),
                new Vertex(new Vector4(1, 1, 1, 1), new Vector4(0, 0, 1, 1), new Vector4(1, 1, 0, 0), Color.FromArgb(255, 0, 0)),
                
                new Vertex(new Vector4(-1, 1, 1, 1), new Vector4(0, 1, 0, 1), new Vector4(0, 1, 0, 0), Color.FromArgb(255, 255, 0)),
                new Vertex(new Vector4(-1, 1, 1, 1), new Vector4(-1, 0, 0, 1), new Vector4(0, 1, 0, 0), Color.FromArgb(255, 255, 0)),
                new Vertex(new Vector4(-1, 1, 1, 1), new Vector4(0, 0, 1, 1), new Vector4(0, 1, 0, 0), Color.FromArgb(255, 255, 0)),
            };

            this.mesh.SetVertices(vertices);
        }

        private void InitCamera(int width, int height)
        {
            this.camera = new Camera(MathUtil.PI * 0.5f, (float)width / (float)height, 1.0f, 500.0f);
            this.camera.Position = new Vector4(0, 5, -5, 1);
            this.camera.Target = new Vector4(0, 0, 0, 1);
            this.camera.Up = new Vector4(0, 1, 0, 1);
            this.camera.Pitch = 0;
            this.camera.Yaw = 0;
        }

        private void InitLight()
        {
            this.light = new DirectionalLight(new Vector4(-2, 2, 0, 1), Color.FromArgb(255, 255, 255));
            this.light.IsEnable = false;
        }

        public void UpdateCameraPos(Vector4 pos)
        {
            this.camera.Position = pos;
        }

        public void UpdateCameraRotation(float degree)
        {
            this.camera.Yaw += degree;
        }
    }
}
