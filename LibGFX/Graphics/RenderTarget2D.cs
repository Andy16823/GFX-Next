using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.Egl;
using LibGFX.Graphics.Renderer.OpenGL;
using LibGFX.Core;

namespace LibGFX.Graphics
{
    /// <summary>
    /// A 2D render target that encapsulates a framebuffer, texture, and optional renderbuffer for depth/stencil.
    /// </summary>
    public class RenderTarget2D : IRenderTarget
    {
        public int TextureFbo { get; set; } = -1;
        public int TextureId { get; set; } = -1;
        public int Width { get; set; }
        public int Height { get; set; }
        public int FramebufferId { get; set; } = -1;
        public int ColorAttachmentId { get; set; } = -1;
        public int DepthAttachmentId { get; set; } = -1;
        public int Samples { get; set; } = 0;

        public RenderTarget2D(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Disposes the render target from the specified render device.
        /// </summary>
        /// <param name="renderer"></param>
        public void Dispose(IRenderDevice renderer)
        {
            if (this.TextureId != -1)
            {
                renderer.DeleteTexture(this.TextureId);
                this.TextureId = -1;
            }
            if (this.DepthAttachmentId != -1)
            {
                renderer.DeleteRenderbuffer(this.DepthAttachmentId);
                this.DepthAttachmentId = -1;
            }
            if (this.FramebufferId != -1)
            {
                renderer.DeleteFramebuffer(this.FramebufferId);
                this.FramebufferId = -1;
            }
        }

        public bool HasRenderBuffer()
        {
            return this.DepthAttachmentId != -1;
        }
    }
}
