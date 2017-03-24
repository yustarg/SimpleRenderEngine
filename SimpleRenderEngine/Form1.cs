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
            this.Paint += new PaintEventHandler(Form1_Paint); // 自己手动添加处理事件
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


        private const float MoveSpeed = 0.1f;
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Console.WriteLine("Form1_KeyDown !!!!!" + keyData);
            if (keyData == Keys.W)
            {
                this.scene.UpdateCameraPos(new Vector4(0, 0, MoveSpeed, 0));
            }
            else if (keyData == Keys.A)
            {
                this.scene.UpdateCameraPos(new Vector4(-MoveSpeed, 0, 0, 0));
            }
            else if (keyData == Keys.S)
            {
                this.scene.UpdateCameraPos(new Vector4(0, 0, -MoveSpeed, 0));
            }
            else if (keyData == Keys.D)
            {
                this.scene.UpdateCameraPos(new Vector4(MoveSpeed, 0, 0, 0));
            }
            else if (keyData == Keys.Q)
            {
                this.scene.UpdateCameraPos(new Vector4(0, MoveSpeed, 0, 0));
            }
            else if (keyData == Keys.E)
            {
                this.scene.UpdateCameraPos(new Vector4(0, -MoveSpeed, 0, 0));
            }
            this.device.Clear();
            this.Invalidate();
            return true;
        }

    }
}
