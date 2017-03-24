using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    public class Matrix4x4
    {
        public float[,] M {get; set;}
        public Matrix4x4()
        {
            M = new float[4, 4];
            this.SetIdentity();
        }

        public static Matrix4x4 operator +(Matrix4x4 a, Matrix4x4 b)
        {
            Matrix4x4 t = new Matrix4x4();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    t.M[i, j] = a.M[i, j] + b.M[i, j];
                }
            }
            return t;
        }

        public static Matrix4x4 operator -(Matrix4x4 a, Matrix4x4 b)
        {
            Matrix4x4 t = new Matrix4x4();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    t.M[i, j] = a.M[i, j] - b.M[i, j];
                }
            }
            return t;
        }

        public static Matrix4x4 operator *(Matrix4x4 a, Matrix4x4 b)
        {
            Matrix4x4 t = new Matrix4x4();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    t.M[j, i] = (a.M[j, 0] * b.M[0, i]) +
                        (a.M[j, 1] * b.M[1, i]) +
                        (a.M[j, 2] * b.M[2, i]) +
                        (a.M[j, 3] * b.M[3, i]);
                }
            }
            return t;
        }

        // c = a * f  
        public void Scale(float f) 
        {
	        for (int i = 0; i < 4; i++) 
            {
		        for (int j = 0; j < 4; j++) 
                {
                    this.M[i, j] = this.M[i, j] * f;
                }
	        }
        }

        // y = x * m
        public Vector4 Apply(Vector4 a) 
        {
            Vector4 t = new Vector4();
            float X = a.X, Y = a.Y, Z = a.Z, W = a.W;
	        t.X = X * this.M[0, 0] + Y * this.M[1, 0] + Z * this.M[2, 0] + W * this.M[3, 0];
	        t.Y = X * this.M[0, 1] + Y * this.M[1, 1] + Z * this.M[2, 1] + W * this.M[3, 1];
	        t.Z = X * this.M[0, 2] + Y * this.M[1, 2] + Z * this.M[2, 2] + W * this.M[3, 2];
	        t.W = X * this.M[0, 3] + Y * this.M[1, 3] + Z * this.M[2, 3] + W * this.M[3, 3];
            return t;
        }

        public void SetIdentity()
        {
	        this.M[0, 0] = this.M[1, 1] = this.M[2, 2] = this.M[3, 3] = 1.0f; 
	        this.M[0, 1] = this.M[0, 2] = this.M[0, 3] = 0.0f;
	        this.M[1, 0] = this.M[1, 2] = this.M[1, 3] = 0.0f;
	        this.M[2, 0] = this.M[2, 1] = this.M[2, 3] = 0.0f;
	        this.M[3, 0] = this.M[3, 1] = this.M[3, 2] = 0.0f;
        }

        public void SetZero() 
        {
	        this.M[0, 0] = this.M[0, 1] = this.M[0, 2] = this.M[0, 3] = 0.0f;
	        this.M[1, 0] = this.M[1, 1] = this.M[1, 2] = this.M[1, 3] = 0.0f;
	        this.M[2, 0] = this.M[2, 1] = this.M[2, 2] = this.M[2, 3] = 0.0f;
	        this.M[3, 0] = this.M[3, 1] = this.M[3, 2] = this.M[3, 3] = 0.0f;
        }

        // 平移变换
        public void SetTranslate(float x, float y, float z)
        {
	        this.SetIdentity();
	        this.M[3, 0] = x;
	        this.M[3, 1] = y;
	        this.M[3, 2] = z;
        }

        // 缩放变换
        public void SetScale(float x, float y, float z)
        {
            this.SetIdentity();
	        this.M[0, 0] = x;
	        this.M[1, 1] = y;
	        this.M[2, 2] = z;
        }

        // 旋转矩阵
        public void SetRotate(float x, float y, float z, float theta) 
        {
	        float qsin = (float)Math.Sin(theta * 0.5f);
            float qcos = (float)Math.Cos(theta * 0.5f);
	        Vector4 vec = new Vector4( x, y, z, 1.0f );
	        float w = qcos;
            vec.Normalize();
	        x = vec.X * qsin;
	        y = vec.Y * qsin;
	        z = vec.Z * qsin;
	        this.M[0, 0] = 1 - 2 * y * y - 2 * z * z;
	        this.M[1, 0] = 2 * x * y - 2 * w * z;
	        this.M[2, 0] = 2 * x * z + 2 * w * y;
	        this.M[0, 1] = 2 * x * y + 2 * w * z;
	        this.M[1, 1] = 1 - 2 * x * x - 2 * z * z;
	        this.M[2, 1] = 2 * y * z - 2 * w * x;
	        this.M[0, 2] = 2 * x * z - 2 * w * y;
	        this.M[1, 2] = 2 * y * z + 2 * w * x;
	        this.M[2, 2] = 1 - 2 * x * x - 2 * y * y;
	        this.M[0, 3] = this.M[1, 3] = this.M[2, 3] = 0.0f;
	        this.M[3, 0] = this.M[3, 1] = this.M[3, 2] = 0.0f;	
	        this.M[3, 3] = 1.0f;
        }

        public void SetRotate(float degreeX, float degreeY, float degreeZ)
        {
            degreeX = degreeX * (float)Math.PI / 180;
            degreeY = degreeY * (float)Math.PI / 180;
            degreeZ = degreeZ * (float)Math.PI / 180;
            Matrix4x4 z = new Matrix4x4();
            z.M[0, 0] = (float)Math.Cos(degreeZ);
            z.M[0, 1] = -(float)Math.Sin(degreeZ);
            z.M[1, 0] = (float)Math.Sin(degreeZ);
            z.M[1, 1] = (float)Math.Cos(degreeZ);
            Matrix4x4 x = new Matrix4x4();
            x.M[1, 1] = (float)Math.Cos(degreeX);
            x.M[1, 2] = -(float)Math.Sin(degreeX);
            x.M[2, 1] = (float)Math.Sin(degreeX);
            x.M[2, 2] = (float)Math.Cos(degreeX);
            Matrix4x4 y = new Matrix4x4();
            y.M[0, 0] = (float)Math.Cos(degreeY);
            y.M[0, 2] = (float)Math.Sin(degreeY);
            y.M[2, 0] = -(float)Math.Sin(degreeY);
            y.M[2, 2] = (float)Math.Cos(degreeY);

            this.SetIdentity();
            Matrix4x4 final = y * x * z;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    this.M[i, j] = final.M[i, j];
        }
    }
}
