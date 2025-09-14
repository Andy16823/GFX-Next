using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents a descriptor for a render target, including its dimensions, format, and other properties.
    /// </summary>
    public struct RenderTargetDescriptor
    {
        public int Width;
        public int Height;
        public int Border;
        public Vector4 BorderColor;

        public RenderFlags.ColorFormatHint Format; // Internal Format
        public RenderFlags.ColorFormatLayout Layout; // Format
        public RenderFlags.ColorFormatType Type;
        public RenderFlags.GFXFramebufferAttachment AttachmentPoint;

        public bool UseDepth;
        public bool UseStencil;

        public int Samples;
        public RenderFlags.TextureFilterMode FilterMode;
        public RenderFlags.TextureWrapMode WrapS;
        public RenderFlags.TextureWrapMode WrapT;
        public RenderFlags.RenderBufferMode DrawBufferMode;
        public RenderFlags.RenderBufferMode ReadBufferMode;

        public bool IsDepthTexture => AttachmentPoint == RenderFlags.GFXFramebufferAttachment.Depth || AttachmentPoint == RenderFlags.GFXFramebufferAttachment.DepthStencil;

        public static RenderTargetDescriptor Default(int width, int height, int samples = 0) => new RenderTargetDescriptor
        {
            Width = width,
            Height = height,
            Border = 0,
            BorderColor = new Vector4(0, 0, 0, 0),
            Format = RenderFlags.ColorFormatHint.RGBA,
            Layout = RenderFlags.ColorFormatLayout.RGBA,
            Type = RenderFlags.ColorFormatType.UnsignedByte,
            AttachmentPoint = RenderFlags.GFXFramebufferAttachment.Color0,
            UseDepth = true,
            UseStencil = true,
            Samples = 0,
            FilterMode = RenderFlags.TextureFilterMode.Linear,
            WrapS = RenderFlags.TextureWrapMode.ClampToEdge,
            WrapT = RenderFlags.TextureWrapMode.ClampToEdge,
            DrawBufferMode = RenderFlags.RenderBufferMode.Color0,
            ReadBufferMode = RenderFlags.RenderBufferMode.Color0
        };

        public static RenderTargetDescriptor DepthOnly(int width, int height) => new RenderTargetDescriptor
        {
            Width = width,
            Height = height,
            Border = 0,
            BorderColor = new Vector4(1, 1, 1, 1),
            Format = RenderFlags.ColorFormatHint.Depth,
            Layout = RenderFlags.ColorFormatLayout.Depth,
            Type = RenderFlags.ColorFormatType.Float,
            AttachmentPoint = RenderFlags.GFXFramebufferAttachment.Depth,
            UseDepth = true,
            UseStencil = false,
            Samples = 0,
            FilterMode = RenderFlags.TextureFilterMode.Nearest,
            WrapS = RenderFlags.TextureWrapMode.ClampToBorder,
            WrapT = RenderFlags.TextureWrapMode.ClampToBorder,
            DrawBufferMode = RenderFlags.RenderBufferMode.None,
            ReadBufferMode = RenderFlags.RenderBufferMode.None
        };

    }
}
