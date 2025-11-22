using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents a render target that only contains a depth texture.
    /// Useful for shadow mapping and depth pre-pass techniques.
    /// </summary>
    public class DepthOnlyRenderTarget : IRenderTarget
    {
        public int FramebufferId { get; set; } = -1;
        public int DepthTextureId { get; set; } = -1;
        public int Width { get; set; }
        public int Height { get; set; }


        public void Dispose(IRenderDevice renderer)
        {
            if(this.FramebufferId != -1)
            {
                renderer.DeleteFramebuffer(this.FramebufferId);
                this.FramebufferId = -1;
            }
            if(this.DepthTextureId != -1)
            {
                renderer.DeleteTexture(this.DepthTextureId);
                this.DepthTextureId = -1;
            }
        }
    }
}
