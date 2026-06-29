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
    /// Represents a render target that only contains a depth texture.
    /// Useful for shadow mapping and depth pre-pass techniques.
    /// </summary>
    public class DepthOnlyRenderTarget : IRenderTarget
    {
        public int FramebufferId { get; set; } = -1;
        public int DepthTextureId { get; set; } = -1;
        public int Width { get; set; }
        public int Height { get; set; }

        public DepthOnlyRenderTarget(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public void Create()
        {
            this.FramebufferId = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, this.FramebufferId);

            this.DepthTextureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, this.DepthTextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, Width, Height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            float[] borderColor = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, this.DepthTextureId, 0);
            GL.DrawBuffer(OpenTK.Graphics.OpenGL4.DrawBufferMode.None);
            GL.ReadBuffer(OpenTK.Graphics.OpenGL4.ReadBufferMode.None);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Failed to create framebuffer for depth render target.");
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Dispose()
        {
            GL.DeleteFramebuffer(this.FramebufferId);
            GL.DeleteTexture(this.DepthTextureId);
        }

        public byte[] GetPixelData()
        {
            throw new NotImplementedException();
        }

        public void Resize(int width, int height)
        {
            if (this.Width == width && this.Height == height)
            {
                return;
            }
            this.Width = width;
            this.Height = height;
            GL.BindTexture(TextureTarget.Texture2D, this.DepthTextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}
