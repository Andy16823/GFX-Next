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
    /// Represents a render target in the graphics rendering pipeline, including its texture, framebuffer, and render buffer IDs.
    /// </summary>
    public class RenderTarget2D : IRenderTarget
    {
        private int _width;
        private int _height;
        private int _border;
        private RenderTargetDescriptor _descriptor;

        public int TextureID;
        public int FramebufferID;
        public int RenderBufferID;

        public int Width => _width;
        public int Height => _height;
        public int Border => _border;

        public bool HasRenderBuffer => RenderBufferID != 0;

        public RenderTarget2D(RenderTargetDescriptor descriptor)
        {
            _descriptor = descriptor;
            _width = descriptor.Width;
            _height = descriptor.Height;
            _border = descriptor.Border;
        }

        public void Create(IRenderDevice renderer)
        {
            this.FramebufferID = renderer.GenFramebuffer();
            renderer.BindFramebuffer(RenderFlags.GFXFramebufferTarget.Framebuffer, this.FramebufferID);

            this.TextureID = renderer.GenTexture();
            renderer.BindTexture(RenderFlags.GFXTextureTarget.Texture2D, this.TextureID);
            renderer.TexImage2D(RenderFlags.GFXTextureTarget.Texture2D, 0, _descriptor.Format, _width, _height, _border, _descriptor.Layout, _descriptor.Type, 0);
            renderer.TexParameter(RenderFlags.GFXTextureTarget.Texture2D, RenderFlags.GFXTextureParameterName.TextureMinFilter, _descriptor.FilterMode);
            renderer.TexParameter(RenderFlags.GFXTextureTarget.Texture2D, RenderFlags.GFXTextureParameterName.TextureMagFilter, _descriptor.FilterMode);
            renderer.TexParameter(RenderFlags.GFXTextureTarget.Texture2D, RenderFlags.GFXTextureParameterName.TextureWrapS, _descriptor.WrapS);
            renderer.TexParameter(RenderFlags.GFXTextureTarget.Texture2D, RenderFlags.GFXTextureParameterName.TextureWrapT, _descriptor.WrapT);

            if (_descriptor.WrapS == RenderFlags.TextureWrapMode.ClampToBorder || _descriptor.WrapT == RenderFlags.TextureWrapMode.ClampToBorder)
            {
                renderer.TexParameter(RenderFlags.GFXTextureTarget.Texture2D, RenderFlags.GFXTextureParameterName.TextureBorderColor, _descriptor.BorderColor);
            }
            renderer.FramebufferTexture2D(RenderFlags.GFXFramebufferTarget.Framebuffer, _descriptor.AttachmentPoint, RenderFlags.GFXTextureTarget.Texture2D, this.TextureID, 0);
            renderer.DrawBufferMode(_descriptor.DrawBufferMode);
            renderer.DrawBufferMode(_descriptor.ReadBufferMode);

            if ((_descriptor.UseDepth || _descriptor.UseStencil) && !_descriptor.IsDepthTexture)
            {
                this.RenderBufferID = renderer.GenRenderbuffer();
                var depthFormat = Utils.GetBestDepthStencilFormat(_descriptor.UseDepth, _descriptor.UseStencil);
                renderer.BindRenderbuffer(RenderFlags.GFXRenderbufferTarget.Renderbuffer, this.RenderBufferID);
                if (_descriptor.Samples > 0)
                {
                    renderer.RenderbufferStorageMultisample(RenderFlags.GFXRenderbufferTarget.Renderbuffer, _descriptor.Samples, depthFormat, _width, _height);
                }
                else
                {
                    renderer.RenderbufferStorage(RenderFlags.GFXRenderbufferTarget.Renderbuffer, depthFormat, _width, _height);
                }

                renderer.FramebufferRenderbuffer(RenderFlags.GFXFramebufferTarget.Framebuffer, RenderFlags.GFXFramebufferAttachment.DepthStencil, RenderFlags.GFXRenderbufferTarget.Renderbuffer, this.RenderBufferID);
            }

            if (renderer.CheckFramebufferStatus(RenderFlags.GFXFramebufferTarget.Framebuffer) != RenderFlags.GFXFramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Failed to create framebuffer for render target.");
            }

            renderer.BindFramebuffer(RenderFlags.GFXFramebufferTarget.Framebuffer, 0);
            renderer.BindTexture(RenderFlags.GFXTextureTarget.Texture2D, 0);
            renderer.BindRenderbuffer(RenderFlags.GFXRenderbufferTarget.Renderbuffer, 0);
        }

        public void Resize(IRenderDevice renderer, int width, int height)
        {
            if (width == _width && height == _height)
                return;

            _width = width;
            _height = height;

            renderer.BindTexture(RenderFlags.GFXTextureTarget.Texture2D, this.TextureID);
            renderer.TexImage2D(RenderFlags.GFXTextureTarget.Texture2D, 0, _descriptor.Format, width, height, _border, _descriptor.Layout, _descriptor.Type, 0);
            renderer.BindTexture(RenderFlags.GFXTextureTarget.Texture2D, 0);

            if (this.HasRenderBuffer)
            {
                var depthFormat = Utils.GetBestDepthStencilFormat(_descriptor.UseDepth, _descriptor.UseStencil);
                renderer.BindRenderbuffer(RenderFlags.GFXRenderbufferTarget.Renderbuffer, this.RenderBufferID);
                if (_descriptor.Samples > 0)
                {
                    renderer.RenderbufferStorageMultisample(RenderFlags.GFXRenderbufferTarget.Renderbuffer, _descriptor.Samples, depthFormat, width, height);
                }
                else
                {
                    renderer.RenderbufferStorage(RenderFlags.GFXRenderbufferTarget.Renderbuffer, depthFormat, width, height);
                }
                renderer.BindRenderbuffer(RenderFlags.GFXRenderbufferTarget.Renderbuffer, 0);
            }
        }

        public void Dispose(IRenderDevice renderer)
        {
            if (this.TextureID != 0)
            {
                renderer.DeleteTexture(this.TextureID);
                this.TextureID = 0;
            }
            if (this.RenderBufferID != 0)
            {
                renderer.DeleteRenderbuffer(this.RenderBufferID);
                this.RenderBufferID = 0;
            }
            if (this.FramebufferID != 0)
            {
                renderer.DeleteFramebuffer(this.FramebufferID);
                this.FramebufferID = 0;
            }
        }
    }
}
