using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
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
    public class MSAARenderTarget2D : IRenderTarget
    {
        public int TextureFbo { get; set; } = -1;
        public int TextureId { get; set; } = -1;
        public int Width { get; set; }
        public int Height { get; set; }
        public int FramebufferId { get; set; } = -1;
        public int ColorAttachmentId { get; set; } = -1;
        public int DepthAttachmentId { get; set; } = -1;
        public int Samples { get; set; } = 0;

        public MSAARenderTarget2D(int width, int height, int samples = 0)
        {
            this.Width = width;
            this.Height = height;
            this.Samples = samples;
        }

        public bool HasRenderBuffer()
        {
            return this.DepthAttachmentId != -1;
        }

        public void Create()
        {
            // Create the Texture Framebuffer for the sampling result
            this.TextureFbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, this.TextureFbo);

            this.TextureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, this.TextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, this.Width, this.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, this.TextureId, 0);

            // Create the main Framebuffer for rendering
            this.FramebufferId = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, this.FramebufferId);

            if (this.Samples == 0)
            {
                this.ColorAttachmentId = GL.GenRenderbuffer();
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, this.ColorAttachmentId);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Rgba8, this.Width, this.Height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, RenderbufferTarget.Renderbuffer, this.ColorAttachmentId);

                this.DepthAttachmentId = GL.GenRenderbuffer();
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, this.DepthAttachmentId);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Depth24Stencil8, this.Width, this.Height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, this.DepthAttachmentId);
            }
            else
            {
                this.ColorAttachmentId = GL.GenRenderbuffer();
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, this.ColorAttachmentId);
                GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, this.Samples, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Rgba8, this.Width, this.Height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, RenderbufferTarget.Renderbuffer, this.ColorAttachmentId);

                this.DepthAttachmentId = GL.GenRenderbuffer();
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, this.DepthAttachmentId);
                GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, this.Samples, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Depth24Stencil8, this.Width, this.Height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, this.DepthAttachmentId);
            }

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Failed to create framebuffer for render target.");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }

        public void Resize(int width, int height)
        {
            if (this.Width == width && this.Height == height)
            {
                return;
            }

            this.Width = width;
            this.Height = height;

            // Resize the texture
            GL.BindTexture(TextureTarget.Texture2D, this.TextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            if (this.Samples == 0)
            {
                // Resize the color Attachment
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, this.ColorAttachmentId);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Rgba8, width, height);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

                // Resize the depth attachment buffer
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, this.DepthAttachmentId);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Depth24Stencil8, width, height);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            }
            else
            {
                // Resize the color Attachment
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, this.ColorAttachmentId);
                GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, this.Samples, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Rgba8, width, height);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

                // Resize the depth attachment buffer
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, this.DepthAttachmentId);
                GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, this.Samples, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Depth24Stencil8, width, height);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            }
        }

        public void ResolveMultisample()
        {
            if (this.Samples == 0)
            {
                return;
            }
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, this.FramebufferId);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, this.TextureFbo);
            GL.BlitFramebuffer(0, 0, this.Width, this.Height, 0, 0, this.Width, this.Height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public byte[] GetPixelData()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, this.FramebufferId);
            byte[] pixelData = new byte[this.Width * this.Height * 4];
            GL.ReadPixels(0, 0, this.Width, this.Height, PixelFormat.Rgba, PixelType.UnsignedByte, pixelData);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            return pixelData;
        }

        public void Dispose()
        {
            GL.DeleteTexture(this.TextureId);
            GL.DeleteRenderbuffer(this.DepthAttachmentId);
            GL.DeleteRenderbuffer(this.ColorAttachmentId);
            GL.DeleteFramebuffer(this.FramebufferId);
        }

        
    }
}
