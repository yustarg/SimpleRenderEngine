using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SimpleRenderEngine
{
    public class DirectionalLight
    {
        public static Color AmbientColor = Color.FromArgb(0, 50, 0);
        private const int MAXKD = 5;

        public Vector4 LightPos { get; set; }
        public Color LightColor { get; set; }
        private float kd;
        public float Kd 
        {
            get 
            {
                return this.kd;
            } 
            set
            {
                this.kd = value * MAXKD; 
            }
        }

        public DirectionalLight(Vector4 pos, Color color)
        {
            this.kd = 1;
            this.LightPos = pos;
            this.LightColor = color;
        }

        public float ComputeNDotL(Vector4 pos, Vector4 normal)
        {
            var lightDirection = this.LightPos - pos;

            normal.Normalize();
            lightDirection.Normalize();

            return Math.Max(0, Vector4.Dot(normal, lightDirection));
        }

        public Color GetFinalLightColor(float nDotL)
        {
            Color diffuse = MathUtil.MulColor(MathUtil.MulColor(LightColor, nDotL), Kd);
            Color final = MathUtil.AddColor(diffuse, DirectionalLight.AmbientColor);
            return final;
        }
    }
}
