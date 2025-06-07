using BulletSharp;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Class containing mappings to OpenGL attributes
    /// </summary>
    public class GLMappings
    {
        /// <summary>
        /// Converts a RenderFlags.TextureFilterMode to OpenGL TextureMinFilter.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int ToGLMinFilter(RenderFlags.TextureFilterMode mode) => mode switch
        {
            RenderFlags.TextureFilterMode.Nearest => (int)OpenTK.Graphics.OpenGL4.TextureMinFilter.Nearest,
            RenderFlags.TextureFilterMode.Linear => (int)OpenTK.Graphics.OpenGL4.TextureMinFilter.Linear,
            RenderFlags.TextureFilterMode.MipmapNearest => (int)OpenTK.Graphics.OpenGL4.TextureMinFilter.NearestMipmapNearest,
            RenderFlags.TextureFilterMode.MipmapLinear => (int)OpenTK.Graphics.OpenGL4.TextureMinFilter.LinearMipmapLinear,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        /// <summary>
        /// Converts a RenderFlags.TextureFilterMode to OpenGL TextureMagFilter.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int ToGLMagFilter(RenderFlags.TextureFilterMode mode) => mode switch
        {
            RenderFlags.TextureFilterMode.Nearest => (int)OpenTK.Graphics.OpenGL4.TextureMagFilter.Nearest,
            RenderFlags.TextureFilterMode.Linear => (int)OpenTK.Graphics.OpenGL4.TextureMagFilter.Linear,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        public static DrawBufferMode ToDrawBuffer(RenderFlags.RenderBufferMode access) => access switch
        {
            RenderFlags.RenderBufferMode.None => DrawBufferMode.None,
            RenderFlags.RenderBufferMode.FrontLeft => DrawBufferMode.FrontLeft,
            RenderFlags.RenderBufferMode.FrontRight => DrawBufferMode.FrontRight,
            RenderFlags.RenderBufferMode.BackLeft => DrawBufferMode.BackLeft,
            RenderFlags.RenderBufferMode.BackRight => DrawBufferMode.BackRight,
            RenderFlags.RenderBufferMode.Left => DrawBufferMode.Left,
            RenderFlags.RenderBufferMode.Right => DrawBufferMode.Right,
            RenderFlags.RenderBufferMode.Color0 => DrawBufferMode.ColorAttachment0,
            RenderFlags.RenderBufferMode.Color1 => DrawBufferMode.ColorAttachment1,
            RenderFlags.RenderBufferMode.Color2 => DrawBufferMode.ColorAttachment2,
            RenderFlags.RenderBufferMode.Color3 => DrawBufferMode.ColorAttachment3,
            RenderFlags.RenderBufferMode.Color4 => DrawBufferMode.ColorAttachment4,
            RenderFlags.RenderBufferMode.Color5 => DrawBufferMode.ColorAttachment5,
            RenderFlags.RenderBufferMode.Color6 => DrawBufferMode.ColorAttachment6,
            RenderFlags.RenderBufferMode.Color7 => DrawBufferMode.ColorAttachment7,
            RenderFlags.RenderBufferMode.Color8 => DrawBufferMode.ColorAttachment8,
            RenderFlags.RenderBufferMode.Color9 => DrawBufferMode.ColorAttachment9,
            RenderFlags.RenderBufferMode.Color10 => DrawBufferMode.ColorAttachment10,
            RenderFlags.RenderBufferMode.Color11 => DrawBufferMode.ColorAttachment11,
            RenderFlags.RenderBufferMode.Color12 => DrawBufferMode.ColorAttachment12,
            RenderFlags.RenderBufferMode.Color13 => DrawBufferMode.ColorAttachment13,
            RenderFlags.RenderBufferMode.Color14 => DrawBufferMode.ColorAttachment14,
            RenderFlags.RenderBufferMode.Color15 => DrawBufferMode.ColorAttachment15,
            RenderFlags.RenderBufferMode.Color16 => DrawBufferMode.ColorAttachment16,
            RenderFlags.RenderBufferMode.Color17 => DrawBufferMode.ColorAttachment17,
            RenderFlags.RenderBufferMode.Color18 => DrawBufferMode.ColorAttachment18,
            RenderFlags.RenderBufferMode.Color19 => DrawBufferMode.ColorAttachment19,
            RenderFlags.RenderBufferMode.Color20 => DrawBufferMode.ColorAttachment20,
            RenderFlags.RenderBufferMode.Color21 => DrawBufferMode.ColorAttachment21,
            RenderFlags.RenderBufferMode.Color22 => DrawBufferMode.ColorAttachment22,
            RenderFlags.RenderBufferMode.Color23 => DrawBufferMode.ColorAttachment23,
            RenderFlags.RenderBufferMode.Color24 => DrawBufferMode.ColorAttachment24,
            RenderFlags.RenderBufferMode.Color25 => DrawBufferMode.ColorAttachment25,
            RenderFlags.RenderBufferMode.Color26 => DrawBufferMode.ColorAttachment26,
            RenderFlags.RenderBufferMode.Color27 => DrawBufferMode.ColorAttachment27,
            RenderFlags.RenderBufferMode.Color28 => DrawBufferMode.ColorAttachment28,
            RenderFlags.RenderBufferMode.Color29 => DrawBufferMode.ColorAttachment29,
            RenderFlags.RenderBufferMode.Color30 => DrawBufferMode.ColorAttachment30,
            RenderFlags.RenderBufferMode.Color31 => DrawBufferMode.ColorAttachment31,
            _ => throw new ArgumentOutOfRangeException(nameof(access), access, null)
        };

        public static ReadBufferMode ToReadBuffer(RenderFlags.RenderBufferMode access) => access switch
        {
            RenderFlags.RenderBufferMode.None => ReadBufferMode.None,
            RenderFlags.RenderBufferMode.FrontLeft => ReadBufferMode.FrontLeft,
            RenderFlags.RenderBufferMode.FrontRight => ReadBufferMode.FrontRight,
            RenderFlags.RenderBufferMode.BackLeft => ReadBufferMode.BackLeft,
            RenderFlags.RenderBufferMode.BackRight => ReadBufferMode.BackRight,
            RenderFlags.RenderBufferMode.Left => ReadBufferMode.Left,
            RenderFlags.RenderBufferMode.Right => ReadBufferMode.Right,
            RenderFlags.RenderBufferMode.Color0 => ReadBufferMode.ColorAttachment0,
            RenderFlags.RenderBufferMode.Color1 => ReadBufferMode.ColorAttachment1,
            RenderFlags.RenderBufferMode.Color2 => ReadBufferMode.ColorAttachment2,
            RenderFlags.RenderBufferMode.Color3 => ReadBufferMode.ColorAttachment3,
            RenderFlags.RenderBufferMode.Color4 => ReadBufferMode.ColorAttachment4,
            RenderFlags.RenderBufferMode.Color5 => ReadBufferMode.ColorAttachment5,
            RenderFlags.RenderBufferMode.Color6 => ReadBufferMode.ColorAttachment6,
            RenderFlags.RenderBufferMode.Color7 => ReadBufferMode.ColorAttachment7,
            RenderFlags.RenderBufferMode.Color8 => ReadBufferMode.ColorAttachment8,
            RenderFlags.RenderBufferMode.Color9 => ReadBufferMode.ColorAttachment9,
            RenderFlags.RenderBufferMode.Color10 => ReadBufferMode.ColorAttachment10,
            RenderFlags.RenderBufferMode.Color11 => ReadBufferMode.ColorAttachment11,
            RenderFlags.RenderBufferMode.Color12 => ReadBufferMode.ColorAttachment12,
            RenderFlags.RenderBufferMode.Color13 => ReadBufferMode.ColorAttachment13,
            RenderFlags.RenderBufferMode.Color14 => ReadBufferMode.ColorAttachment14,
            RenderFlags.RenderBufferMode.Color15 => ReadBufferMode.ColorAttachment15,
            _ => throw new ArgumentOutOfRangeException(nameof(access), access, null)
        };

        /// <summary>
        /// Converts a RenderFlags.TextureWrapMode to OpenGL TextureWrapMode.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int ToGL(RenderFlags.TextureWrapMode mode) => mode switch
        {
            RenderFlags.TextureWrapMode.ClampToEdge => (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.ClampToEdge,
            RenderFlags.TextureWrapMode.Repeat => (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat,
            RenderFlags.TextureWrapMode.MirroredRepeat => (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.MirroredRepeat,
            RenderFlags.TextureWrapMode.ClampToBorder => (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.ClampToBorder,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        /// <summary>
        /// Converts a RenderFlags.ColorFormatLayout to OpenGL PixelFormat.
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static PixelFormat ToGL(RenderFlags.ColorFormatLayout hint) => hint switch
        {
            RenderFlags.ColorFormatLayout.R => PixelFormat.Red,
            RenderFlags.ColorFormatLayout.RG => PixelFormat.Rg,
            RenderFlags.ColorFormatLayout.RGB => PixelFormat.Rgb,
            RenderFlags.ColorFormatLayout.RGBA => PixelFormat.Rgba,
            RenderFlags.ColorFormatLayout.Depth => PixelFormat.DepthComponent,
            RenderFlags.ColorFormatLayout.DepthStencil => PixelFormat.DepthStencil,
            _ => throw new ArgumentOutOfRangeException(nameof(hint), hint, null)
        };

        /// <summary>
        /// Converts a RenderFlags.ColorFormatHint to OpenGL PixelInternalFormat.
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static PixelInternalFormat ToGL(RenderFlags.ColorFormatHint hint) => hint switch
        {
            RenderFlags.ColorFormatHint.R8 => PixelInternalFormat.R8,
            RenderFlags.ColorFormatHint.RG8 => PixelInternalFormat.Rg8,
            RenderFlags.ColorFormatHint.RGB8 => PixelInternalFormat.Rgb8,
            RenderFlags.ColorFormatHint.RGBA8 => PixelInternalFormat.Rgba8,

            RenderFlags.ColorFormatHint.R16F => PixelInternalFormat.R16f,
            RenderFlags.ColorFormatHint.RG16F => PixelInternalFormat.Rg16f,
            RenderFlags.ColorFormatHint.RGB16F => PixelInternalFormat.Rgb16f,
            RenderFlags.ColorFormatHint.RGBA16F => PixelInternalFormat.Rgba16f,

            RenderFlags.ColorFormatHint.R32F => PixelInternalFormat.R32f,
            RenderFlags.ColorFormatHint.RG32F => PixelInternalFormat.Rg32f,
            RenderFlags.ColorFormatHint.RGB32F => PixelInternalFormat.Rgb32f,
            RenderFlags.ColorFormatHint.RGBA32F => PixelInternalFormat.Rgba32f,

            RenderFlags.ColorFormatHint.RGBA8UI => PixelInternalFormat.Rgba8ui,
            RenderFlags.ColorFormatHint.RGB10A2 => PixelInternalFormat.Rgb10A2,

            RenderFlags.ColorFormatHint.Depth16 => PixelInternalFormat.DepthComponent16,
            RenderFlags.ColorFormatHint.Depth24 => PixelInternalFormat.DepthComponent24,
            RenderFlags.ColorFormatHint.Depth32 => PixelInternalFormat.DepthComponent32,
            RenderFlags.ColorFormatHint.Depth24Stencil8 => PixelInternalFormat.Depth24Stencil8,
            RenderFlags.ColorFormatHint.Depth32FStencil8 => PixelInternalFormat.Depth32fStencil8,

            RenderFlags.ColorFormatHint.RGBA => PixelInternalFormat.Rgba,
            RenderFlags.ColorFormatHint.RGB => PixelInternalFormat.Rgb,
            RenderFlags.ColorFormatHint.Depth => PixelInternalFormat.DepthComponent,
            RenderFlags.ColorFormatHint.DepthStencil => PixelInternalFormat.DepthStencil,

            _ => throw new ArgumentOutOfRangeException(nameof(hint), hint, null)
        };

        /// <summary>
        /// Converts a RenderFlags.ColorFormatType to OpenGL PixelType.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static PixelType ToGL(RenderFlags.ColorFormatType type) => type switch
        {
            RenderFlags.ColorFormatType.UnsignedByte => PixelType.UnsignedByte,
            RenderFlags.ColorFormatType.UnsignedShort => PixelType.UnsignedShort,
            RenderFlags.ColorFormatType.UnsignedInt => PixelType.UnsignedInt,
            RenderFlags.ColorFormatType.UnsignedInt24_8 => PixelType.UnsignedInt248,
            RenderFlags.ColorFormatType.Float => PixelType.Float,
            RenderFlags.ColorFormatType.HalfFloat => PixelType.HalfFloat,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        public static FramebufferAttachment ToGL(RenderFlags.RenderAttachmentPoint attachment) => attachment switch
        {
            RenderFlags.RenderAttachmentPoint.Color0 => FramebufferAttachment.ColorAttachment0,
            RenderFlags.RenderAttachmentPoint.Color1 => FramebufferAttachment.ColorAttachment1,
            RenderFlags.RenderAttachmentPoint.Color2 => FramebufferAttachment.ColorAttachment2,
            RenderFlags.RenderAttachmentPoint.Color3 => FramebufferAttachment.ColorAttachment3,
            RenderFlags.RenderAttachmentPoint.Color4 => FramebufferAttachment.ColorAttachment4,
            RenderFlags.RenderAttachmentPoint.Color5 => FramebufferAttachment.ColorAttachment5,
            RenderFlags.RenderAttachmentPoint.Color6 => FramebufferAttachment.ColorAttachment6,
            RenderFlags.RenderAttachmentPoint.Color7 => FramebufferAttachment.ColorAttachment7,
            RenderFlags.RenderAttachmentPoint.Depth => FramebufferAttachment.DepthAttachment,
            RenderFlags.RenderAttachmentPoint.DepthStencil => FramebufferAttachment.DepthStencilAttachment,
            RenderFlags.RenderAttachmentPoint.Stencil => FramebufferAttachment.StencilAttachment,
            _ => throw new ArgumentOutOfRangeException(nameof(attachment), attachment, null)
        };

        /// <summary>
        /// Converts RenderFlags.ClearFlags to OpenGL ClearBufferMask.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static ClearBufferMask ToGL(RenderFlags.ClearFlags flags)
        {
            ClearBufferMask mask = 0;

            if ((flags & RenderFlags.ClearFlags.Color) != 0)
                mask |= ClearBufferMask.ColorBufferBit;

            if ((flags & RenderFlags.ClearFlags.Depth) != 0)
                mask |= ClearBufferMask.DepthBufferBit;

            if ((flags & RenderFlags.ClearFlags.Stencil) != 0)
                mask |= ClearBufferMask.StencilBufferBit;

            return mask;
        }

        /// <summary>
        /// Returns the best depth-stencil format based on the specified flags.
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="stencil"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static RenderbufferStorage GetBestDepthStencilFormat(bool depth, bool stencil)
        {
            return (depth, stencil) switch
            {
                (true, true) => RenderbufferStorage.Depth24Stencil8,
                (true, false) => RenderbufferStorage.DepthComponent24,
                (false, true) => RenderbufferStorage.StencilIndex8,
                _ => throw new ArgumentException("At least one of depth or stencil must be true.")
            };
        }
    }
}
