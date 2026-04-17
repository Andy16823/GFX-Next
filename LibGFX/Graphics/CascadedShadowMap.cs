using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LibGFX.Graphics
{
    public class CascadedShadowMap : IRenderTarget
    {
        public int FramebufferId { get; set; }
        public int TextureId { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public int CascadeCount { get; set; }

        public CascadedShadowMap(int width, int height, int cascadeCount)
        {
            this.Width = width;
            this.Height = height;
            this.CascadeCount = cascadeCount;
        }

        public void Create()
        {
            var framebuffer = GL.GenFramebuffer();
            var lightDepthMaps = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, lightDepthMaps);
            GL.TexImage3D(TextureTarget.Texture2DArray, 0, PixelInternalFormat.DepthComponent, this.Width, this.Height, this.CascadeCount, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            float[] borderColor = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureBorderColor, borderColor);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, lightDepthMaps, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception($"Failed to create framebuffer for cascaded shadow map. Status: {status}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Successfully created framebuffer for cascaded shadow map.");
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            this.FramebufferId = framebuffer;
            this.TextureId = lightDepthMaps;
        }

        public void Dispose()
        {
            GL.DeleteFramebuffer(this.FramebufferId);
            GL.DeleteTexture(this.TextureId);
        }

        public byte[] GetPixelData()
        {
            throw new NotImplementedException();
        }

        public void Resize(int width, int height)
        {
            // TODO: Implement resizing logic for cascaded shadow map. This may involve recreating the framebuffer and texture with the new dimensions.
            throw new NotImplementedException();
        }
    }
}
