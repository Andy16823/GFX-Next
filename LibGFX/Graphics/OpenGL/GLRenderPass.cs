using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

namespace LibGFX.Graphics.OpenGL
{
    public class GLRenderPass : IRenderPass
    {
        public RenderPassScope Begin()
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            return new RenderPassScope(this);
        }

        public void End()
        {
            GL.Flush();
        }
    }
}
