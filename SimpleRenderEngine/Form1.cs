using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleRenderEngine
{
    public partial class Form1 : Form
    {
        private const float PI = 3.1415926f; 
        
        private Bitmap bmp;
        private Device device;
        private Scene scene;
        
        public Form1()
        {
            InitializeComponent();
            InitSettings();
            InitScene();
        }

        private void InitScene()
        {
            bmp = new Bitmap(this.Width, this.Height);
            device = new Device(bmp);
            scene = new Scene();
            scene.renderState = Scene.RenderState.WireFrame;
            InitCamera();
            InitMesh();
            InitLight();
        }

        private void InitMesh()
        {
            scene.mesh = new Mesh("Cube", @"E:\SimpleRenderEngine\SimpleRenderEngine\SimpleRenderEngine\textures\background.png");
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
                new Vertex(new Vector4(-1, -1, -1, 1), new Vector4(-1, 0, 0, 1), new Vector4(0, 0, 0, 0), Color.FromArgb(255, 0, 0)),
                new Vertex(new Vector4(-1, -1, -1, 1), new Vector4(0, 0, -1, 1), new Vector4(0, 0, 0, 0), Color.FromArgb(255, 0, 0)),
                
                new Vertex(new Vector4(1, -1, -1, 1), new Vector4(0, -1, 0, 1), new Vector4(1, 0, 0, 0), Color.FromArgb(0, 255, 0)),
                new Vertex(new Vector4(1, -1, -1, 1), new Vector4(1, 0, 0, 1),  new Vector4(1, 0, 0, 0), Color.FromArgb(0, 255, 0)),
                new Vertex(new Vector4(1, -1, -1, 1), new Vector4(0, 0, -1, 1), new Vector4(1, 0, 0, 0), Color.FromArgb(0, 255, 0)),
                  
                new Vertex(new Vector4(1, 1, -1, 1), new Vector4(0, 1, 0, 1), new Vector4(1, 1, 0, 0), Color.FromArgb(0, 0, 255)),
                new Vertex(new Vector4(1, 1, -1, 1), new Vector4(1, 0, 0, 1), new Vector4(1, 1, 0, 0), Color.FromArgb(0, 0, 255)),
                new Vertex(new Vector4(1, 1, -1, 1), new Vector4(0, 0, -1, 1), new Vector4(1, 1, 0, 0), Color.FromArgb(0, 0, 255)),

                new Vertex(new Vector4(-1, 1, -1, 1), new Vector4(0, 1, 0, 1), new Vector4(0, 1, 0, 0), Color.FromArgb(255, 0, 0)),
                new Vertex(new Vector4(-1, 1, -1, 1), new Vector4(-1, 0, 0, 1), new Vector4(0, 1, 0, 0), Color.FromArgb(255, 0, 0)),
                new Vertex(new Vector4(-1, 1, -1, 1), new Vector4(0, 0, -1, 1), new Vector4(0, 1, 0, 0), Color.FromArgb(255, 0, 0)),
                
                new Vertex(new Vector4(-1, -1, 1, 1), new Vector4(0, -1, 0, 1), new Vector4(0, 0, 0, 0), Color.FromArgb(0, 255, 0)),
                new Vertex(new Vector4(-1, -1, 1, 1), new Vector4(-1, 0, 0, 1), new Vector4(0, 0, 0, 0), Color.FromArgb(0, 255, 0)),
                new Vertex(new Vector4(-1, -1, 1, 1), new Vector4(0, 0, 1, 1), new Vector4(0, 0, 0, 0), Color.FromArgb(0, 255, 0)),
                
                new Vertex(new Vector4(1, -1, 1, 1), new Vector4(0, -1, 0, 1), new Vector4(1, 0, 0, 0), Color.FromArgb(0, 0, 255)),
                new Vertex(new Vector4(1, -1, 1, 1), new Vector4(1, 0, 0, 1), new Vector4(1, 0, 0, 0), Color.FromArgb(0, 0, 255)),
                new Vertex(new Vector4(1, -1, 1, 1), new Vector4(0, 0, 1, 1), new Vector4(1, 0, 0, 0), Color.FromArgb(0, 0, 255)),
                
                new Vertex(new Vector4(1, 1, 1, 1), new Vector4(0, 1, 0, 1), new Vector4(1, 1, 0, 0), Color.FromArgb(255, 0, 0)),
                new Vertex(new Vector4(1, 1, 1, 1), new Vector4(1, 0, 0, 1), new Vector4(1, 1, 0, 0), Color.FromArgb(255, 0, 0)),
                new Vertex(new Vector4(1, 1, 1, 1), new Vector4(0, 0, 1, 1), new Vector4(1, 1, 0, 0), Color.FromArgb(255, 0, 0)),
                
                new Vertex(new Vector4(-1, 1, 1, 1), new Vector4(0, 1, 0, 1), new Vector4(0, 1, 0, 0), Color.FromArgb(255, 255, 0)),
                new Vertex(new Vector4(-1, 1, 1, 1), new Vector4(-1, 0, 0, 1), new Vector4(0, 1, 0, 0), Color.FromArgb(255, 255, 0)),
                new Vertex(new Vector4(-1, 1, 1, 1), new Vector4(0, 0, 1, 1), new Vector4(0, 1, 0, 0), Color.FromArgb(255, 255, 0)),
            };

            scene.mesh.SetVertices(vertices);
        }

        private void InitCamera()
        {
            scene.camera = new Camera(PI * 0.5f, (float)this.Width / (float)this.Height, 1.0f, 500.0f);
            scene.camera.Position = new Vector4(5, -5, -2, 1);
            scene.camera.Target = new Vector4(0, 0, 1, 1);
        }

        private void InitLight()
        {
            scene.light = new DirectionalLight(new Vector4(-2, 2, 0, 1), Color.FromArgb(255, 255, 255));
            scene.light.IsEnable = false;
        }

        private void InitSettings()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true);
        }

        private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            device.Render(scene);
            Rectangle rg = new Rectangle(0, 0, this.Width, this.Height);
            g.DrawImage(bmp, rg);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Paint += new PaintEventHandler(Form1_Paint); // 自己手动添加处理事件
        }

        // Wire Frame
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked)
            {
                this.scene.renderState = Scene.RenderState.WireFrame;
                this.Invalidate();
            }
        }

        // Gouraud Shading
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton2.Checked)
            {
                this.scene.renderState = Scene.RenderState.GouraudShading;
                this.Invalidate();
            }
        }

        // Texture Mapping
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton3.Checked)
            {
                this.scene.renderState = Scene.RenderState.TextureMapping;
                this.Invalidate();
            }
        }

        // Lighting
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.hScrollBar1.Visible = checkBox1.Checked;
            this.hScrollBar1.Value = (int)(0.2f * this.hScrollBar1.Maximum);
            this.scene.light.IsEnable = checkBox1.Checked;
            this.Invalidate();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            float ratio = (float)this.hScrollBar1.Value / (float)this.hScrollBar1.Maximum;
            this.scene.light.Kd = ratio;
            this.Invalidate();
        }
    }
}
