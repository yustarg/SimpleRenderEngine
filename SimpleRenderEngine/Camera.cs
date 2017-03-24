using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRenderEngine
{
    public class Camera
    {
        public Vector4 Position { get; set; }
        public Vector4 Target { get; set; }
        public Vector4 Up { get; set; }
        public float Pitch { get; set; }
        public float Yaw { get; set; }
        public float fov { get; set; }
        public float aspect { get; set; }
        public float zn { get; set; }
        public float zf { get; set; }


        public Camera(float fov, float aspect, float zn, float zf)
        {
            this.fov = fov;
            this.aspect = aspect;
            this.zn = zn;
            this.zf = zf;
        }

        // 计算观察矩阵
        public Matrix4x4 LookAt()
        {
            Matrix4x4 view = new Matrix4x4();
            Vector4 xaxis, yaxis, zaxis;

            zaxis = Target - Position;
            zaxis.Normalize();
            xaxis = Vector4.Cross(Up, zaxis);
            xaxis.Normalize();
            yaxis = Vector4.Cross(zaxis, xaxis);
            yaxis.Normalize();

            view.M[0, 0] = xaxis.X;
            view.M[1, 0] = xaxis.Y;
            view.M[2, 0] = xaxis.Z;
            view.M[3, 0] = -Vector4.Dot(xaxis, Position);

            view.M[0, 1] = yaxis.X;
            view.M[1, 1] = yaxis.Y;
            view.M[2, 1] = yaxis.Z;
            view.M[3, 1] = -Vector4.Dot(yaxis, Position);

            view.M[0, 2] = zaxis.X;
            view.M[1, 2] = zaxis.Y;
            view.M[2, 2] = zaxis.Z;
            view.M[3, 2] = -Vector4.Dot(zaxis, Position);

            view.M[0, 3] = view.M[1, 3] = view.M[2, 3] = 0.0f;
            view.M[3, 3] = 1.0f;

            return view;
        }

        public Matrix4x4 FPSView()
        {
            float pitch = this.Pitch * (float)Math.PI / 180;
            float yaw = this.Yaw * (float)Math.PI / 180;

            float cosPitch = (float)Math.Cos(pitch);
            float sinPitch = (float)Math.Sin(pitch);
            float cosYaw = (float)Math.Cos(yaw);
            float sinYaw = (float)Math.Sin(yaw);
 
            Vector4 xaxis = new Vector4(cosYaw, 0, -sinYaw, 0);
            Vector4 yaxis = new Vector4(sinYaw * sinPitch, cosPitch, cosYaw * sinPitch, 0);
            Vector4 zaxis = new Vector4(sinYaw * cosPitch, -sinPitch, cosPitch * cosYaw, 0);
 
            Matrix4x4 view = new Matrix4x4();

            view.M[0, 0] = xaxis.X;
            view.M[1, 0] = xaxis.Y;
            view.M[2, 0] = xaxis.Z;
            view.M[3, 0] = -Vector4.Dot(xaxis, Position);

            view.M[0, 1] = yaxis.X;
            view.M[1, 1] = yaxis.Y;
            view.M[2, 1] = yaxis.Z;
            view.M[3, 1] = -Vector4.Dot(yaxis, Position);

            view.M[0, 2] = zaxis.X;
            view.M[1, 2] = zaxis.Y;
            view.M[2, 2] = zaxis.Z;
            view.M[3, 2] = -Vector4.Dot(zaxis, Position);

            view.M[0, 3] = view.M[1, 3] = view.M[2, 3] = 0.0f;
            view.M[3, 3] = 1.0f;

            return view;
        }

        public Matrix4x4 Perspective() 
        {
            Matrix4x4 proj = new Matrix4x4();
            float fax = 1.0f / (float)Math.Tan(this.fov * 0.5f);
            proj.SetZero();
	        proj.M[0, 0] = (float)(fax / aspect);
	        proj.M[1, 1] = (float)(fax);
	        proj.M[2, 2] = zf / (zf - zn);
	        proj.M[3, 2] = - zn * zf / (zf - zn);
	        proj.M[2, 3] = 1;
            return proj;
        }
    }
}
