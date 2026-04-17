using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

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

        public RenderTarget2D(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public void Create()
        {
            this.FramebufferId = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferId);

            this.TextureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, TextureId, 0);

            this.DepthAttachmentId = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthAttachmentId);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Depth24Stencil8, Width, Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, DepthAttachmentId);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Failed to create framebuffer for render target.");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }

        public void Dispose()
        {
            GL.DeleteFramebuffer(this.FramebufferId);
            GL.DeleteTexture(this.TextureId);
            GL.DeleteRenderbuffer(this.DepthAttachmentId);
        }

        public byte[] GetPixelData()
        {
            throw new NotImplementedException();
        }

        public void Resize(int width, int height)
        {
            if (Width == width && Height == height)
            {
                return;
            }

            Width = width;
            Height = height;

            // Resize the texture
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // Resize the depth attachment buffer
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthAttachmentId);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Depth24Stencil8, width, height);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }
    }
}
