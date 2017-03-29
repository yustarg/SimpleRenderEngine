using System;

namespace SimpleRenderEngine
{
    public class DirectionalLight
    {
        public static Color4 AmbientColor = new Color4(50, 0, 0);
        private const float MAXKD = 2f;

        public bool IsEnable { get; set; }
        public Vector4 LightPos { get; set; }
        public Color4 LightColor { get; set; }
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

        public DirectionalLight(Vector4 pos, Color4 color)
        {
            this.kd = MAXKD * 0.2f;
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

        public Color4 GetDiffuseColor(float nDotL)
        {
            return MathUtil.MulColor(MathUtil.MulColor(LightColor, nDotL), Kd);
        }

        public Color4 GetFinalLightColor(float nDotL)
        {
            Color4 diffuse = MathUtil.MulColor(MathUtil.MulColor(LightColor, nDotL), Kd);
            Color4 final = diffuse + DirectionalLight.AmbientColor;
            return final;
        }
    }
}
