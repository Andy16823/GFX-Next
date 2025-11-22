using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents a 2D render target such as a framebuffer with a texture attachment.
    /// and depth attachment. Used for off-screen rendering without multisampling.
    /// </summary>
    public class RenderTarget2D : IRenderTarget
    {
        public int FramebufferId { get; set; } = -1;
        public int TextureId { get; set; } = -1;
        public int DepthAttachmentId { get; set; } = -1;
        public int Width { get; set; }
        public int Height { get; set; }

        public void Dispose(IRenderDevice renderer)
        {
            if(this.FramebufferId != -1)
            {
                renderer.DeleteFramebuffer(this.FramebufferId);
                this.FramebufferId = -1;
            }
            if(this.TextureId != -1)
            {
                renderer.DeleteTexture(this.TextureId);
                this.TextureId = -1;
            }
            if(this.DepthAttachmentId != -1)
            {
                renderer.DeleteRenderbuffer(this.DepthAttachmentId);
                this.DepthAttachmentId = -1;
            }
        }
    }
}
