using System;
using System.Collections.Generic;
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
    }
}
