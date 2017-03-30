using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
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
        private Rectangle rt;
        private PixelFormat pixelFormat;

        public Form1()
        {
            InitializeComponent();
            InitSettings();
            InitScene();
        }

        private void InitScene()
        {
            bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height, PixelFormat.Format24bppRgb);
            device = new Device(bmp);
            scene = new Scene(this.ClientSize.Width, this.ClientSize.Height);
            this.rt = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            this.pixelFormat = bmp.PixelFormat;
        }

        private void InitSettings()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
        }

        private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs pe)
        {
            BitmapData data = this.bmp.LockBits(rt, ImageLockMode.ReadWrite, this.pixelFormat);
            this.device.Clear(data);
            device.Render(scene, data);
            this.bmp.UnlockBits(data);
            g = pe.Graphics;
            g.DrawImage(this.bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Paint += new PaintEventHandler(Form1_Paint);
            this.MouseWheel += new MouseEventHandler(Form1_OnMouseWheel);
            this.MouseMove += new MouseEventHandler(Form1_OnMouseMove);
        }

        private const float MoveSpeed = 0.5f;
        private const float RotateSpeed = 5f * (float)Math.PI / 180f;
        private int mouseX = 0;
        
        private void Form1_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                float oriX = this.scene.camera.Position.X;
                float oriZ = this.scene.camera.Position.Z;
                float oriY = this.scene.camera.Position.Y;
                if (e.X - mouseX > 0)
                {
                    float newX = (float)(oriX * Math.Cos(-RotateSpeed) - oriZ * Math.Sin(-RotateSpeed));
                    float newZ = (float)(oriX * Math.Sin(-RotateSpeed) + oriZ * Math.Cos(-RotateSpeed));
                    this.scene.UpdateCameraPos(new Vector4(newX, oriY, newZ, 1));
                }
                else
                {
                    float newX = (float)(oriX * Math.Cos(RotateSpeed) - oriZ * Math.Sin(RotateSpeed));
                    float newZ = (float)(oriX * Math.Sin(RotateSpeed) + oriZ * Math.Cos(RotateSpeed));
                    this.scene.UpdateCameraPos(new Vector4(newX, oriY, newZ, 1));
                }
                mouseX = e.X;
                this.Invalidate();
            }
        }

        private void Form1_OnMouseWheel(object sender, MouseEventArgs e)
        {
            float oriX = this.scene.camera.Position.X;
            float oriY = this.scene.camera.Position.Y;
            float oriZ = this.scene.camera.Position.Z;
            Vector4 dir = this.scene.camera.Position.Normalized;
            float x = Vector4.Dot(dir, new Vector4(1, 0, 0, 1)) * MoveSpeed * 0.5f;
            float y = Vector4.Dot(dir, new Vector4(0, 1, 0, 1)) * MoveSpeed * 0.5f;
            float z = Vector4.Dot(dir, new Vector4(0, 0, 1, 1)) * MoveSpeed * 0.5f;

            if (e.Delta < 0)
            {
                this.scene.UpdateCameraPos(new Vector4(oriX + x, oriY + y, oriZ + z, 1));
            }
            else
            {
                this.scene.UpdateCameraPos(new Vector4(oriX - x, oriY - y, oriZ - z, 1));
            }
            this.Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            float oriX = this.scene.camera.Position.X;
            float oriY = this.scene.camera.Position.Y;
            float oriZ = this.scene.camera.Position.Z;
            if (keyData == Keys.W)
            {
                this.scene.UpdateCameraPos(new Vector4(oriX, oriY + MoveSpeed, oriZ, 1));
            }
            else if (keyData == Keys.S)
            {
                this.scene.UpdateCameraPos(new Vector4(oriX, oriY - MoveSpeed, oriZ, 1));
            }
            else if (keyData == Keys.A)
            {
                float newX = (float)(oriX * Math.Cos(-RotateSpeed) - oriZ * Math.Sin(-RotateSpeed));
                float newZ = (float)(oriX * Math.Sin(-RotateSpeed) + oriZ * Math.Cos(-RotateSpeed));
                this.scene.UpdateCameraPos(new Vector4(newX, oriY, newZ, 1));
            }
            else if (keyData == Keys.D)
            {
                float newX = (float)(oriX * Math.Cos(RotateSpeed) - oriZ * Math.Sin(RotateSpeed));
                float newZ = (float)(oriX * Math.Sin(RotateSpeed) + oriZ * Math.Cos(RotateSpeed));
                this.scene.UpdateCameraPos(new Vector4(newX, oriY, newZ, 1));
            }
            this.Invalidate();
            return true;
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
                BitmapData bmData = this.scene.mesh.texture.LockBits();
                this.Invalidate();
                this.scene.mesh.texture.UnlockBits(bmData);
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
