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
        /// <summary>
        /// The width of the render target in pixels.
        /// </summary>
        public int Width => _width;

        /// <summary>
        /// The height of the render target in pixels.
        /// </summary>
        public int Height => _height;

        /// <summary>
        /// The border width of the render target in pixels.
        /// </summary>
        public int Border => _border;

        /// <summary>
        /// Indicates if the render target has an associated renderbuffer for depth/stencil.
        /// </summary>
        public bool HasRenderBuffer => _renderbufferId != 0;

        /// <summary>
        /// The unique identifier for the framebuffer associated with this render target.
        /// </summary>
        public int RenderTargetId => _framebufferID;

        /// <summary>
        /// The unique identifier for the texture associated with this render target.
        /// </summary>
        public int TextureId => _textureId;

        /// <summary>
        /// The unique identifier for the renderbuffer associated with this render target, if any.
        /// </summary>
        public int RenderBufferId => _renderbufferId;

        private int _renderbufferId;
        private int _framebufferID;
        private int _textureId;
        private int _width;
        private int _height;
        private int _border;
        private RenderTargetDescriptor _descriptor;

        /// <summary>
        /// Initializes a new instance of the RenderTarget2D class with the specified descriptor.
        /// </summary>
        /// <param name="descriptor"></param>
        public RenderTarget2D(RenderTargetDescriptor descriptor)
        {
            _descriptor = descriptor;
            _width = descriptor.Width;
            _height = descriptor.Height;
            _border = descriptor.Border;
        }

        /// <summary>
        /// Creates the render target on the specified render device.
        /// </summary>
        /// <param name="renderer"></param>
        /// <exception cref="Exception"></exception>
        public void Create(IRenderDevice renderer)
        {
            this._framebufferID = renderer.GenFramebuffer();
            renderer.BindFramebuffer(RenderFlags.GFXFramebufferTarget.Framebuffer, this._framebufferID);

            this._textureId = renderer.GenTexture();
            renderer.BindTexture(RenderFlags.GFXTextureTarget.Texture2D, this._textureId);
            renderer.TexImage2D(RenderFlags.GFXTextureTarget.Texture2D, 0, _descriptor.Format, _width, _height, _border, _descriptor.Layout, _descriptor.Type, 0);
            renderer.TexParameter(RenderFlags.GFXTextureTarget.Texture2D, RenderFlags.GFXTextureParameterName.TextureMinFilter, _descriptor.FilterMode);
            renderer.TexParameter(RenderFlags.GFXTextureTarget.Texture2D, RenderFlags.GFXTextureParameterName.TextureMagFilter, _descriptor.FilterMode);
            renderer.TexParameter(RenderFlags.GFXTextureTarget.Texture2D, RenderFlags.GFXTextureParameterName.TextureWrapS, _descriptor.WrapS);
            renderer.TexParameter(RenderFlags.GFXTextureTarget.Texture2D, RenderFlags.GFXTextureParameterName.TextureWrapT, _descriptor.WrapT);

            if (_descriptor.WrapS == RenderFlags.TextureWrapMode.ClampToBorder || _descriptor.WrapT == RenderFlags.TextureWrapMode.ClampToBorder)
            {
                renderer.TexParameter(RenderFlags.GFXTextureTarget.Texture2D, RenderFlags.GFXTextureParameterName.TextureBorderColor, _descriptor.BorderColor);
            }
            renderer.FramebufferTexture2D(RenderFlags.GFXFramebufferTarget.Framebuffer, _descriptor.AttachmentPoint, RenderFlags.GFXTextureTarget.Texture2D, this._textureId, 0);
            renderer.DrawBufferMode(_descriptor.DrawBufferMode);
            renderer.DrawBufferMode(_descriptor.ReadBufferMode);

            if ((_descriptor.UseDepth || _descriptor.UseStencil) && !_descriptor.IsDepthTexture)
            {
                this._renderbufferId = renderer.GenRenderbuffer();
                var depthFormat = Utils.GetBestDepthStencilFormat(_descriptor.UseDepth, _descriptor.UseStencil);
                renderer.BindRenderbuffer(RenderFlags.GFXRenderbufferTarget.Renderbuffer, this._renderbufferId);
                if (_descriptor.Samples > 0)
                {
                    renderer.RenderbufferStorageMultisample(RenderFlags.GFXRenderbufferTarget.Renderbuffer, _descriptor.Samples, depthFormat, _width, _height);
                }
                else
                {
                    renderer.RenderbufferStorage(RenderFlags.GFXRenderbufferTarget.Renderbuffer, depthFormat, _width, _height);
                }

                renderer.FramebufferRenderbuffer(RenderFlags.GFXFramebufferTarget.Framebuffer, RenderFlags.GFXFramebufferAttachment.DepthStencil, RenderFlags.GFXRenderbufferTarget.Renderbuffer, this._renderbufferId);
            }

            if (renderer.CheckFramebufferStatus(RenderFlags.GFXFramebufferTarget.Framebuffer) != RenderFlags.GFXFramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Failed to create framebuffer for render target.");
            }

            renderer.BindFramebuffer(RenderFlags.GFXFramebufferTarget.Framebuffer, 0);
            renderer.BindTexture(RenderFlags.GFXTextureTarget.Texture2D, 0);
            renderer.BindRenderbuffer(RenderFlags.GFXRenderbufferTarget.Renderbuffer, 0);
        }

        /// <summary>
        /// Resizes the render target to the specified width and height.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Resize(IRenderDevice renderer, int width, int height)
        {
            if (width == _width && height == _height)
                return;

            _width = width;
            _height = height;

            renderer.BindTexture(RenderFlags.GFXTextureTarget.Texture2D, this._textureId);
            renderer.TexImage2D(RenderFlags.GFXTextureTarget.Texture2D, 0, _descriptor.Format, width, height, _border, _descriptor.Layout, _descriptor.Type, 0);
            renderer.BindTexture(RenderFlags.GFXTextureTarget.Texture2D, 0);

            if (this.HasRenderBuffer)
            {
                var depthFormat = Utils.GetBestDepthStencilFormat(_descriptor.UseDepth, _descriptor.UseStencil);
                renderer.BindRenderbuffer(RenderFlags.GFXRenderbufferTarget.Renderbuffer, this._renderbufferId);
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

        /// <summary>
        /// Disposes the render target from the specified render device.
        /// </summary>
        /// <param name="renderer"></param>
        public void Dispose(IRenderDevice renderer)
        {
            if (this._textureId != 0)
            {
                renderer.DeleteTexture(this._textureId);
                this._textureId = 0;
            }
            if (this._renderbufferId != 0)
            {
                renderer.DeleteRenderbuffer(this._renderbufferId);
                this._renderbufferId = 0;
            }
            if (this._framebufferID != 0)
            {
                renderer.DeleteFramebuffer(this._framebufferID);
                this._framebufferID = 0;
            }
        }
    }
}
