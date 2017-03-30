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
            this.mesh = new Mesh("Cube");
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
                new Vertex(new Vector4(-1, -1, -1, 1), new Vector4(0, -1, 0, 1), new Vector4(0, 0, 0, 0), new Color4(255, 0, 0)),
                new Vertex(new Vector4(-1, -1, -1, 1), new Vector4(-1, 0, 0, 1), new Vector4(1, 0, 0, 0), new Color4(255, 0, 0)),
                new Vertex(new Vector4(-1, -1, -1, 1), new Vector4(0, 0, -1, 1), new Vector4(0, 0, 0, 0), new Color4(255, 0, 0)),
                
                new Vertex(new Vector4(1, -1, -1, 1), new Vector4(0, -1, 0, 1), new Vector4(1, 0, 0, 0), new Color4(0, 255, 0)),
                new Vertex(new Vector4(1, -1, -1, 1), new Vector4(1, 0, 0, 1),  new Vector4(0, 0, 0, 0), new Color4(0, 255, 0)),
                new Vertex(new Vector4(1, -1, -1, 1), new Vector4(0, 0, -1, 1), new Vector4(1, 0, 0, 0), new Color4(0, 255, 0)),
                  
                new Vertex(new Vector4(1, 1, -1, 1), new Vector4(0, 1, 0, 1), new Vector4(1, 0, 0, 0), new Color4(0, 0, 255)),
                new Vertex(new Vector4(1, 1, -1, 1), new Vector4(1, 0, 0, 1), new Vector4(0, 1, 0, 0), new Color4(0, 0, 255)),
                new Vertex(new Vector4(1, 1, -1, 1), new Vector4(0, 0, -1, 1), new Vector4(1, 1, 0, 0), new Color4(0, 0, 255)),

                new Vertex(new Vector4(-1, 1, -1, 1), new Vector4(0, 1, 0, 1), new Vector4(0, 0, 0, 0), new Color4(255, 0, 0)),
                new Vertex(new Vector4(-1, 1, -1, 1), new Vector4(-1, 0, 0, 1), new Vector4(1, 1, 0, 0), new Color4(255, 0, 0)),
                new Vertex(new Vector4(-1, 1, -1, 1), new Vector4(0, 0, -1, 1), new Vector4(0, 1, 0, 0), new Color4(255, 0, 0)),
                
                new Vertex(new Vector4(-1, -1, 1, 1), new Vector4(0, -1, 0, 1), new Vector4(0, 1, 0, 0), new Color4(0, 255, 0)),
                new Vertex(new Vector4(-1, -1, 1, 1), new Vector4(-1, 0, 0, 1), new Vector4(0, 0, 0, 0), new Color4(0, 255, 0)),
                new Vertex(new Vector4(-1, -1, 1, 1), new Vector4(0, 0, 1, 1), new Vector4(0, 0, 0, 0), new Color4(0, 255, 0)),
                
                new Vertex(new Vector4(1, -1, 1, 1), new Vector4(0, -1, 0, 1), new Vector4(1, 1, 0, 0), new Color4(0, 0, 255)),
                new Vertex(new Vector4(1, -1, 1, 1), new Vector4(1, 0, 0, 1), new Vector4(1, 0, 0, 0), new Color4(0, 0, 255)),
                new Vertex(new Vector4(1, -1, 1, 1), new Vector4(0, 0, 1, 1), new Vector4(1, 0, 0, 0), new Color4(0, 0, 255)),
                
                new Vertex(new Vector4(1, 1, 1, 1), new Vector4(0, 1, 0, 1), new Vector4(1, 1, 0, 0), new Color4(255, 0, 0)),
                new Vertex(new Vector4(1, 1, 1, 1), new Vector4(1, 0, 0, 1), new Vector4(1, 1, 0, 0), new Color4(255, 0, 0)),
                new Vertex(new Vector4(1, 1, 1, 1), new Vector4(0, 0, 1, 1), new Vector4(1, 1, 0, 0), new Color4(255, 0, 0)),
                
                new Vertex(new Vector4(-1, 1, 1, 1), new Vector4(0, 1, 0, 1), new Vector4(0, 1, 0, 0), new Color4(255, 255, 0)),
                new Vertex(new Vector4(-1, 1, 1, 1), new Vector4(-1, 0, 0, 1), new Vector4(0, 1, 0, 0), new Color4(255, 255, 0)),
                new Vertex(new Vector4(-1, 1, 1, 1), new Vector4(0, 0, 1, 1), new Vector4(0, 1, 0, 0), new Color4(255, 255, 0)),
            };

            this.mesh.SetVertices(vertices);
        }

        private void InitCamera(int width, int height)
        {
            this.camera = new Camera(MathUtil.PI * 0.3f, (float)width / (float)height, 1.0f, 500.0f);
            this.camera.Position = new Vector4(0, 5, 5, 1);
            this.camera.Target = new Vector4(0, 0, 0, 1);
            this.camera.Up = new Vector4(0, 1, 0, 1);
            this.camera.Pitch = 0;
            this.camera.Yaw = 0;
        }

        private void InitLight()
        {
            this.light = new DirectionalLight(new Vector4(-5, 5, 5, 1), new Color4(255, 255, 255));
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

        public Matrix4x4 GetMvpMatrix()
        {
            Matrix4x4 translate = new Matrix4x4();
            translate.SetTranslate(0, 0, 0);
            Matrix4x4 scale = new Matrix4x4();
            scale.SetScale(1, 1, 1);
            Matrix4x4 rotate = new Matrix4x4();
            rotate.SetRotate(0, 0, 0);
            Matrix4x4 model = scale * rotate * translate;
            Matrix4x4 view = this.camera.LookAt();
            Matrix4x4 projection = this.camera.Perspective();
            return model * view * projection;
        }
    }
}
