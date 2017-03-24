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
        private Bitmap bmp;
        private Graphics g;
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
            scene = new Scene(this.Width, this.Height);
        }

        private void InitSettings()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true);
        }

        private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs pe)
        {
            g = pe.Graphics;
            g.Clear(Color.LightGreen); 
            device.Render(scene);
            Rectangle rg = new Rectangle(0, 0, this.Width, this.Height);
            g.DrawImage(bmp, rg);
            bmp = new Bitmap(this.Width, this.Height);
            device.UpdateBmp(bmp);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Paint += new PaintEventHandler(Form1_Paint);
            this.MouseMove += new MouseEventHandler(Form1_OnMouseMove);
            this.MouseWheel += new MouseEventHandler(Form1_OnMouseWheel);
        }

        private const float MoveSpeed = 0.5f;
        private const float RotateSpeed = 2f * (float)Math.PI / 180f;
        private int mouseX = 0;
        private int mouseY = 0;
        private void Form1_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                float oriX = this.scene.camera.Position.X;
                float oriZ = this.scene.camera.Position.Z;
                float oriY = this.scene.camera.Position.Y;
                if (e.X - mouseX > 0)
                {
                    float newX = (float)(oriX * Math.Cos(RotateSpeed) - oriZ * Math.Sin(RotateSpeed));
                    float newZ = (float)(oriX * Math.Sin(RotateSpeed) + oriZ * Math.Cos(RotateSpeed));
                    this.scene.UpdateCameraPos(new Vector4(newX, oriY, newZ, 1));
                }
                else
                {
                    float newX = (float)(oriX * Math.Cos(-RotateSpeed) - oriZ * Math.Sin(-RotateSpeed));
                    float newZ = (float)(oriX * Math.Sin(-RotateSpeed) + oriZ * Math.Cos(-RotateSpeed));
                    this.scene.UpdateCameraPos(new Vector4(newX, oriY, newZ, 1));
                }
                mouseX = e.X;
            }    
            this.device.Clear();
            this.Invalidate();
        }

        private void Form1_OnMouseWheel(object sender, MouseEventArgs e)
        {
            float oriX = this.scene.camera.Position.X;
            float oriY = this.scene.camera.Position.Y;
            float oriZ = this.scene.camera.Position.Z;
            if (e.Delta > 0)
            {
                this.scene.UpdateCameraPos(new Vector4(oriX, oriY, oriZ + MoveSpeed, 1));
            }
            else
            {
                this.scene.UpdateCameraPos(new Vector4(oriX, oriY, oriZ - MoveSpeed, 1));
            }
            this.device.Clear();
            this.Invalidate();
        }

        // Wire Frame
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked)
            {
                this.device.Clear();
                this.scene.renderState = Scene.RenderState.WireFrame;
                this.Invalidate();
            }
        }

        // Gouraud Shading
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton2.Checked)
            {
                this.device.Clear(); 
                this.scene.renderState = Scene.RenderState.GouraudShading;
                this.Invalidate();
            }
        }

        // Texture Mapping
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton3.Checked)
            {
                this.device.Clear();
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
            this.device.Clear();
            this.Invalidate();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            float ratio = (float)this.hScrollBar1.Value / (float)this.hScrollBar1.Maximum;
            this.scene.light.Kd = ratio;
            this.device.Clear();
            this.Invalidate();
        }
    }
}
