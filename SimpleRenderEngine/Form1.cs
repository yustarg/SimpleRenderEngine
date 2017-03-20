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
        private Camera cam;
        private Mesh cube;
        
        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            bmp = new Bitmap(this.Width, this.Height);
            device = new Device(bmp);
            InitCamera();
            InitMesh();
            InitSettings();
        }

        private void InitMesh()
        {
            cube = new Mesh("Cube");
            Vector4[] vertices = new Vector4[8] {
                new Vector4(-1, -1, -1, 1),
                new Vector4(1, -1, -1, 1),
                new Vector4(1, 1, -1, 1),
                new Vector4(-1, 1, -1, 1),
                new Vector4(-1, -1, 1, 1),
                new Vector4(1, -1, 1, 1),
                new Vector4(1, 1, 1, 1),
                new Vector4(-1, 1, 1, 1),
            };
            cube.SetVertices(vertices);
        }

        private void InitCamera()
        {
            cam = new Camera(PI * 0.5f, (float)this.Width / (float)this.Height, 1.0f, 500.0f);
            cam.Position = new Vector4(0, 0, 5, 1);
            cam.Target = new Vector4(0, 0, 1, 1);
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
            device.Render(cam, cube);
            Rectangle rg = new Rectangle(0, 0, this.Width, this.Height);
            //bmp.SetPixel(450, 450, Color.FromArgb(1, 0, 0));
            g.DrawImage(bmp, rg);
            //this.Refresh();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Paint += new PaintEventHandler(Form1_Paint); // 自己手动添加处理事件
        }
    }
}
