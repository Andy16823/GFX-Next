using BulletSharp;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Renderer.OpenGL
{
    /// <summary>
    /// Class containing mappings to OpenGL attributes
    /// </summary>
    internal class GLMappings
    {
        /// <summary>
        /// Converts a GFXTextureFilterMode to OpenGL TextureMinFilter.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static int ToGL(GFXTextureFilterMode mode) => mode switch
        {
            GFXTextureFilterMode.Nearest => (int)TextureMinFilter.Nearest,
            GFXTextureFilterMode.Linear => (int)TextureMinFilter.Linear,
            GFXTextureFilterMode.MipmapNearest => (int)TextureMinFilter.NearestMipmapNearest,
            GFXTextureFilterMode.MipmapLinear => (int)TextureMinFilter.LinearMipmapLinear,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        internal static DrawBufferMode ToDrawBuffer(GFXRenderBufferMode access) => access switch
        {
            GFXRenderBufferMode.None => DrawBufferMode.None,
            GFXRenderBufferMode.FrontLeft => DrawBufferMode.FrontLeft,
            GFXRenderBufferMode.FrontRight => DrawBufferMode.FrontRight,
            GFXRenderBufferMode.BackLeft => DrawBufferMode.BackLeft,
            GFXRenderBufferMode.BackRight => DrawBufferMode.BackRight,
            GFXRenderBufferMode.Left => DrawBufferMode.Left,
            GFXRenderBufferMode.Right => DrawBufferMode.Right,
            GFXRenderBufferMode.Color0 => DrawBufferMode.ColorAttachment0,
            GFXRenderBufferMode.Color1 => DrawBufferMode.ColorAttachment1,
            GFXRenderBufferMode.Color2 => DrawBufferMode.ColorAttachment2,
            GFXRenderBufferMode.Color3 => DrawBufferMode.ColorAttachment3,
            GFXRenderBufferMode.Color4 => DrawBufferMode.ColorAttachment4,
            GFXRenderBufferMode.Color5 => DrawBufferMode.ColorAttachment5,
            GFXRenderBufferMode.Color6 => DrawBufferMode.ColorAttachment6,
            GFXRenderBufferMode.Color7 => DrawBufferMode.ColorAttachment7,
            GFXRenderBufferMode.Color8 => DrawBufferMode.ColorAttachment8,
            GFXRenderBufferMode.Color9 => DrawBufferMode.ColorAttachment9,
            GFXRenderBufferMode.Color10 => DrawBufferMode.ColorAttachment10,
            GFXRenderBufferMode.Color11 => DrawBufferMode.ColorAttachment11,
            GFXRenderBufferMode.Color12 => DrawBufferMode.ColorAttachment12,
            GFXRenderBufferMode.Color13 => DrawBufferMode.ColorAttachment13,
            GFXRenderBufferMode.Color14 => DrawBufferMode.ColorAttachment14,
            GFXRenderBufferMode.Color15 => DrawBufferMode.ColorAttachment15,
            GFXRenderBufferMode.Color16 => DrawBufferMode.ColorAttachment16,
            GFXRenderBufferMode.Color17 => DrawBufferMode.ColorAttachment17,
            GFXRenderBufferMode.Color18 => DrawBufferMode.ColorAttachment18,
            GFXRenderBufferMode.Color19 => DrawBufferMode.ColorAttachment19,
            GFXRenderBufferMode.Color20 => DrawBufferMode.ColorAttachment20,
            GFXRenderBufferMode.Color21 => DrawBufferMode.ColorAttachment21,
            GFXRenderBufferMode.Color22 => DrawBufferMode.ColorAttachment22,
            GFXRenderBufferMode.Color23 => DrawBufferMode.ColorAttachment23,
            GFXRenderBufferMode.Color24 => DrawBufferMode.ColorAttachment24,
            GFXRenderBufferMode.Color25 => DrawBufferMode.ColorAttachment25,
            GFXRenderBufferMode.Color26 => DrawBufferMode.ColorAttachment26,
            GFXRenderBufferMode.Color27 => DrawBufferMode.ColorAttachment27,
            GFXRenderBufferMode.Color28 => DrawBufferMode.ColorAttachment28,
            GFXRenderBufferMode.Color29 => DrawBufferMode.ColorAttachment29,
            GFXRenderBufferMode.Color30 => DrawBufferMode.ColorAttachment30,
            GFXRenderBufferMode.Color31 => DrawBufferMode.ColorAttachment31,
            _ => throw new ArgumentOutOfRangeException(nameof(access), access, null)
        };

        internal static ReadBufferMode ToReadBuffer(GFXRenderBufferMode access) => access switch
        {
            GFXRenderBufferMode.None => ReadBufferMode.None,
            GFXRenderBufferMode.FrontLeft => ReadBufferMode.FrontLeft,
            GFXRenderBufferMode.FrontRight => ReadBufferMode.FrontRight,
            GFXRenderBufferMode.BackLeft => ReadBufferMode.BackLeft,
            GFXRenderBufferMode.BackRight => ReadBufferMode.BackRight,
            GFXRenderBufferMode.Left => ReadBufferMode.Left,
            GFXRenderBufferMode.Right => ReadBufferMode.Right,
            GFXRenderBufferMode.Color0 => ReadBufferMode.ColorAttachment0,
            GFXRenderBufferMode.Color1 => ReadBufferMode.ColorAttachment1,
            GFXRenderBufferMode.Color2 => ReadBufferMode.ColorAttachment2,
            GFXRenderBufferMode.Color3 => ReadBufferMode.ColorAttachment3,
            GFXRenderBufferMode.Color4 => ReadBufferMode.ColorAttachment4,
            GFXRenderBufferMode.Color5 => ReadBufferMode.ColorAttachment5,
            GFXRenderBufferMode.Color6 => ReadBufferMode.ColorAttachment6,
            GFXRenderBufferMode.Color7 => ReadBufferMode.ColorAttachment7,
            GFXRenderBufferMode.Color8 => ReadBufferMode.ColorAttachment8,
            GFXRenderBufferMode.Color9 => ReadBufferMode.ColorAttachment9,
            GFXRenderBufferMode.Color10 => ReadBufferMode.ColorAttachment10,
            GFXRenderBufferMode.Color11 => ReadBufferMode.ColorAttachment11,
            GFXRenderBufferMode.Color12 => ReadBufferMode.ColorAttachment12,
            GFXRenderBufferMode.Color13 => ReadBufferMode.ColorAttachment13,
            GFXRenderBufferMode.Color14 => ReadBufferMode.ColorAttachment14,
            GFXRenderBufferMode.Color15 => ReadBufferMode.ColorAttachment15,
            _ => throw new ArgumentOutOfRangeException(nameof(access), access, null)
        };

        /// <summary>
        /// Converts a GFXTextureWrapMode to OpenGL GFXTextureWrapMode.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static int ToGL(GFXTextureWrapMode mode) => mode switch
        {
            GFXTextureWrapMode.ClampToEdge => (int)GFXTextureWrapMode.ClampToEdge,
            GFXTextureWrapMode.Repeat => (int)GFXTextureWrapMode.Repeat,
            GFXTextureWrapMode.MirroredRepeat => (int)GFXTextureWrapMode.MirroredRepeat,
            GFXTextureWrapMode.ClampToBorder => (int)GFXTextureWrapMode.ClampToBorder,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        /// <summary>
        /// Converts a GFXColorFormatLayout to OpenGL PixelFormat.
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static PixelFormat ToGL(GFXColorFormatLayout hint) => hint switch
        {
            GFXColorFormatLayout.R => PixelFormat.Red,
            GFXColorFormatLayout.RG => PixelFormat.Rg,
            GFXColorFormatLayout.RGB => PixelFormat.Rgb,
            GFXColorFormatLayout.RGBA => PixelFormat.Rgba,
            GFXColorFormatLayout.Depth => PixelFormat.DepthComponent,
            GFXColorFormatLayout.DepthStencil => PixelFormat.DepthStencil,
            _ => throw new ArgumentOutOfRangeException(nameof(hint), hint, null)
        };

        /// <summary>
        /// Converts a GFXColorFormatHint to OpenGL PixelInternalFormat.
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static PixelInternalFormat ToGL(GFXColorFormatHint hint) => hint switch
        {
            GFXColorFormatHint.R8 => PixelInternalFormat.R8,
            GFXColorFormatHint.RG8 => PixelInternalFormat.Rg8,
            GFXColorFormatHint.RGB8 => PixelInternalFormat.Rgb8,
            GFXColorFormatHint.RGBA8 => PixelInternalFormat.Rgba8,

            GFXColorFormatHint.R16F => PixelInternalFormat.R16f,
            GFXColorFormatHint.RG16F => PixelInternalFormat.Rg16f,
            GFXColorFormatHint.RGB16F => PixelInternalFormat.Rgb16f,
            GFXColorFormatHint.RGBA16F => PixelInternalFormat.Rgba16f,

            GFXColorFormatHint.R32F => PixelInternalFormat.R32f,
            GFXColorFormatHint.RG32F => PixelInternalFormat.Rg32f,
            GFXColorFormatHint.RGB32F => PixelInternalFormat.Rgb32f,
            GFXColorFormatHint.RGBA32F => PixelInternalFormat.Rgba32f,

            GFXColorFormatHint.RGBA8UI => PixelInternalFormat.Rgba8ui,
            GFXColorFormatHint.RGB10A2 => PixelInternalFormat.Rgb10A2,

            GFXColorFormatHint.Depth16 => PixelInternalFormat.DepthComponent16,
            GFXColorFormatHint.Depth24 => PixelInternalFormat.DepthComponent24,
            GFXColorFormatHint.Depth32 => PixelInternalFormat.DepthComponent32,
            GFXColorFormatHint.Depth24Stencil8 => PixelInternalFormat.Depth24Stencil8,
            GFXColorFormatHint.Depth32FStencil8 => PixelInternalFormat.Depth32fStencil8,

            GFXColorFormatHint.RGBA => PixelInternalFormat.Rgba,
            GFXColorFormatHint.RGB => PixelInternalFormat.Rgb,
            GFXColorFormatHint.Depth => PixelInternalFormat.DepthComponent,
            GFXColorFormatHint.DepthStencil => PixelInternalFormat.DepthStencil,

            _ => throw new ArgumentOutOfRangeException(nameof(hint), hint, null)
        };

        /// <summary>
        /// Converts a GFXColorFormatType to OpenGL PixelType.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static PixelType ToGL(GFXColorFormatType type) => type switch
        {
            GFXColorFormatType.UnsignedByte => PixelType.UnsignedByte,
            GFXColorFormatType.UnsignedShort => PixelType.UnsignedShort,
            GFXColorFormatType.UnsignedInt => PixelType.UnsignedInt,
            GFXColorFormatType.UnsignedInt24_8 => PixelType.UnsignedInt248,
            GFXColorFormatType.Float => PixelType.Float,
            GFXColorFormatType.HalfFloat => PixelType.HalfFloat,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        internal static FramebufferAttachment ToGL(GFXFramebufferAttachment attachment) => attachment switch
        {
            GFXFramebufferAttachment.Color0 => FramebufferAttachment.ColorAttachment0,
            GFXFramebufferAttachment.Color1 => FramebufferAttachment.ColorAttachment1,
            GFXFramebufferAttachment.Color2 => FramebufferAttachment.ColorAttachment2,
            GFXFramebufferAttachment.Color3 => FramebufferAttachment.ColorAttachment3,
            GFXFramebufferAttachment.Color4 => FramebufferAttachment.ColorAttachment4,
            GFXFramebufferAttachment.Color5 => FramebufferAttachment.ColorAttachment5,
            GFXFramebufferAttachment.Color6 => FramebufferAttachment.ColorAttachment6,
            GFXFramebufferAttachment.Color7 => FramebufferAttachment.ColorAttachment7,
            GFXFramebufferAttachment.Depth => FramebufferAttachment.DepthAttachment,
            GFXFramebufferAttachment.DepthStencil => FramebufferAttachment.DepthStencilAttachment,
            GFXFramebufferAttachment.Stencil => FramebufferAttachment.StencilAttachment,
            _ => throw new ArgumentOutOfRangeException(nameof(attachment), attachment, null)
        };

        /// <summary>
        /// Converts GFXClearFlags to OpenGL ClearBufferMask.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        internal static ClearBufferMask ToGL(GFXClearFlags flags)
        {
            ClearBufferMask mask = 0;

            if ((flags & GFXClearFlags.Color) != 0)
                mask |= ClearBufferMask.ColorBufferBit;

            if ((flags & GFXClearFlags.Depth) != 0)
                mask |= ClearBufferMask.DepthBufferBit;

            if ((flags & GFXClearFlags.Stencil) != 0)
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
        internal static RenderbufferStorage GetBestDepthStencilFormat(bool depth, bool stencil)
        {
            return (depth, stencil) switch
            {
                (true, true) => RenderbufferStorage.Depth24Stencil8,
                (true, false) => RenderbufferStorage.DepthComponent24,
                (false, true) => RenderbufferStorage.StencilIndex8,
                _ => throw new ArgumentException("At least one of depth or stencil must be true.")
            };
        }

        /// <summary>
        /// Maps GFXRenderDataTypes to OpenGL VertexAttribPointerType.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static VertexAttribPointerType GetVertexAttribPointerType(GFXRenderDataTypes type) => type switch
        {
            GFXRenderDataTypes.Byte => VertexAttribPointerType.Byte,
            GFXRenderDataTypes.UnsignedByte => VertexAttribPointerType.UnsignedByte,
            GFXRenderDataTypes.Short => VertexAttribPointerType.Short,
            GFXRenderDataTypes.UnsignedShort => VertexAttribPointerType.UnsignedShort,
            GFXRenderDataTypes.Int => VertexAttribPointerType.Int,
            GFXRenderDataTypes.UnsignedInt => VertexAttribPointerType.UnsignedInt,
            GFXRenderDataTypes.Float => VertexAttribPointerType.Float,
            GFXRenderDataTypes.Double => VertexAttribPointerType.Double,
            GFXRenderDataTypes.Fixed => VertexAttribPointerType.Fixed,
            GFXRenderDataTypes.HalfFloat => VertexAttribPointerType.HalfFloat,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        /// <summary>
        /// Maps GFXPrimitiveTypes to OpenGL PrimitiveType.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static PrimitiveType ToPrimitiveType(GFXPrimitiveTypes type) => type switch
        {
            GFXPrimitiveTypes.Points => PrimitiveType.Points,
            GFXPrimitiveTypes.Lines => PrimitiveType.Lines,
            GFXPrimitiveTypes.LineLoop => PrimitiveType.LineLoop,
            GFXPrimitiveTypes.LineStrip => PrimitiveType.LineStrip,
            GFXPrimitiveTypes.Triangles => PrimitiveType.Triangles,
            GFXPrimitiveTypes.TriangleStrip => PrimitiveType.TriangleStrip,
            GFXPrimitiveTypes.TriangleFan => PrimitiveType.TriangleFan,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        /// <summary>
        /// Maps GFXPrimitiveTypes to OpenGL BeginMode.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static BeginMode ToBeginMode(GFXPrimitiveTypes type) => type switch
        {
            GFXPrimitiveTypes.Points => BeginMode.Points,
            GFXPrimitiveTypes.Lines => BeginMode.Lines,
            GFXPrimitiveTypes.LineLoop => BeginMode.LineLoop,
            GFXPrimitiveTypes.LineStrip => BeginMode.LineStrip,
            GFXPrimitiveTypes.Triangles => BeginMode.Triangles,
            GFXPrimitiveTypes.TriangleStrip => BeginMode.TriangleStrip,
            GFXPrimitiveTypes.TriangleFan => BeginMode.TriangleFan,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        internal static FramebufferTarget ToFramebufferTarget(GFXFramebufferTarget target) => target switch
        {
            GFXFramebufferTarget.ReadFramebuffer => FramebufferTarget.ReadFramebuffer,
            GFXFramebufferTarget.DrawFramebuffer => FramebufferTarget.DrawFramebuffer,
            GFXFramebufferTarget.Framebuffer => FramebufferTarget.Framebuffer,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };

        internal static TextureTarget ToTextureTarget(GFXTextureTarget target) => target switch
        {
            GFXTextureTarget.Texture1D => TextureTarget.Texture1D,
            GFXTextureTarget.Texture2D => TextureTarget.Texture2D,
            GFXTextureTarget.Texture3D => TextureTarget.Texture3D,
            GFXTextureTarget.TextureCubeMap => TextureTarget.TextureCubeMap,
            GFXTextureTarget.Texture1DArray => TextureTarget.Texture1DArray,
            GFXTextureTarget.Texture2DArray => TextureTarget.Texture2DArray,
            GFXTextureTarget.TextureRectangle => TextureTarget.TextureRectangle,
            GFXTextureTarget.TextureCubeMapArray => TextureTarget.TextureCubeMapArray,
            GFXTextureTarget.TextureBuffer => TextureTarget.TextureBuffer,
            GFXTextureTarget.Texture2DMultisample => TextureTarget.Texture2DMultisample,
            GFXTextureTarget.Texture2DMultisampleArray => TextureTarget.Texture2DMultisampleArray,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };

        internal static TextureParameterName ToTextureParameterName(GFXTextureParameterName target) => target switch
        {
            GFXTextureParameterName.TextureMinFilter => TextureParameterName.TextureMinFilter,
            GFXTextureParameterName.TextureMagFilter => TextureParameterName.TextureMagFilter,
            GFXTextureParameterName.TextureWrapS => TextureParameterName.TextureWrapS,
            GFXTextureParameterName.TextureWrapT => TextureParameterName.TextureWrapT,
            GFXTextureParameterName.TextureWrapR => TextureParameterName.TextureWrapR,
            GFXTextureParameterName.TextureBorderColor => TextureParameterName.TextureBorderColor,
            GFXTextureParameterName.TextureBaseLevel => TextureParameterName.TextureBaseLevel,
            GFXTextureParameterName.TextureMaxLevel => TextureParameterName.TextureMaxLevel,
            GFXTextureParameterName.TextureLodBias => TextureParameterName.TextureLodBias,
            GFXTextureParameterName.TextureCompareMode => TextureParameterName.TextureCompareMode,
            GFXTextureParameterName.TextureCompareFunc => TextureParameterName.TextureCompareFunc,
            GFXTextureParameterName.GenerateMipmap => TextureParameterName.GenerateMipmap,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };

        internal static RenderbufferTarget ToRenderbufferTarget(GFXRenderbufferTarget target) => target switch
        {
            GFXRenderbufferTarget.Renderbuffer => RenderbufferTarget.Renderbuffer,
            GFXRenderbufferTarget.RenderbufferExt => RenderbufferTarget.RenderbufferExt,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };

        internal static RenderbufferStorage ToRenderBufferStorage(GFXRenderbufferStorage target) => target switch
        {
            GFXRenderbufferStorage.R3G3B2 => RenderbufferStorage.R3G3B2,
            GFXRenderbufferStorage.Alpha4 => RenderbufferStorage.Alpha4,
            GFXRenderbufferStorage.Alpha8 => RenderbufferStorage.Alpha8,
            GFXRenderbufferStorage.Alpha12 => RenderbufferStorage.Alpha12,
            GFXRenderbufferStorage.Alpha16 => RenderbufferStorage.Alpha16,
            GFXRenderbufferStorage.Rgb4 => RenderbufferStorage.Rgb4,
            GFXRenderbufferStorage.Rgb5 => RenderbufferStorage.Rgb5,
            GFXRenderbufferStorage.Rgb8 => RenderbufferStorage.Rgb8,
            GFXRenderbufferStorage.Rgb10 => RenderbufferStorage.Rgb10,
            GFXRenderbufferStorage.Rgb12 => RenderbufferStorage.Rgb12,
            GFXRenderbufferStorage.Rgb16 => RenderbufferStorage.Rgb16,
            GFXRenderbufferStorage.Rgba2 => RenderbufferStorage.Rgba2,
            GFXRenderbufferStorage.Rgba4 => RenderbufferStorage.Rgba4,
            GFXRenderbufferStorage.Rgba8 => RenderbufferStorage.Rgba8,
            GFXRenderbufferStorage.Rgb10A2 => RenderbufferStorage.Rgb10A2,
            GFXRenderbufferStorage.Rgba12 => RenderbufferStorage.Rgba12,
            GFXRenderbufferStorage.Rgba16 => RenderbufferStorage.Rgba16,
            GFXRenderbufferStorage.DepthComponent16 => RenderbufferStorage.DepthComponent16,
            GFXRenderbufferStorage.DepthComponent24 => RenderbufferStorage.DepthComponent24,
            GFXRenderbufferStorage.DepthComponent32 => RenderbufferStorage.DepthComponent32,
            GFXRenderbufferStorage.R8 => RenderbufferStorage.R8,
            GFXRenderbufferStorage.R16 => RenderbufferStorage.R16,
            GFXRenderbufferStorage.Rg8 => RenderbufferStorage.Rg8,
            GFXRenderbufferStorage.Rg16 => RenderbufferStorage.Rg16,
            GFXRenderbufferStorage.R16f => RenderbufferStorage.R16f,
            GFXRenderbufferStorage.R32f => RenderbufferStorage.R32f,
            GFXRenderbufferStorage.Rg16f => RenderbufferStorage.Rg16f,
            GFXRenderbufferStorage.Rg32f => RenderbufferStorage.Rg32f,
            GFXRenderbufferStorage.R8i => RenderbufferStorage.R8i,
            GFXRenderbufferStorage.R8ui => RenderbufferStorage.R8ui,
            GFXRenderbufferStorage.R16i => RenderbufferStorage.R16i,
            GFXRenderbufferStorage.R16ui => RenderbufferStorage.R16ui,
            GFXRenderbufferStorage.R32i => RenderbufferStorage.R32i,
            GFXRenderbufferStorage.R32ui => RenderbufferStorage.R32ui,
            GFXRenderbufferStorage.Rg8i => RenderbufferStorage.Rg8i,
            GFXRenderbufferStorage.Rg8ui => RenderbufferStorage.Rg8ui,
            GFXRenderbufferStorage.Rg16i => RenderbufferStorage.Rg16i,
            GFXRenderbufferStorage.Rg16ui => RenderbufferStorage.Rg16ui,
            GFXRenderbufferStorage.Rg32i => RenderbufferStorage.Rg32i,
            GFXRenderbufferStorage.Rg32ui => RenderbufferStorage.Rg32ui,
            GFXRenderbufferStorage.Rgba32f => RenderbufferStorage.Rgba32f,
            GFXRenderbufferStorage.Rgb32f => RenderbufferStorage.Rgb32f,
            GFXRenderbufferStorage.Rgba16f => RenderbufferStorage.Rgba16f,
            GFXRenderbufferStorage.Rgb16f => RenderbufferStorage.Rgb16f,
            GFXRenderbufferStorage.Depth24Stencil8 => RenderbufferStorage.Depth24Stencil8,
            GFXRenderbufferStorage.R11fG11fB10f => RenderbufferStorage.R11fG11fB10f,
            GFXRenderbufferStorage.Rgb9E5 => RenderbufferStorage.Rgb9E5,
            GFXRenderbufferStorage.Srgb8 => RenderbufferStorage.Srgb8,
            GFXRenderbufferStorage.Srgb8Alpha8 => RenderbufferStorage.Srgb8Alpha8,
            GFXRenderbufferStorage.DepthComponent32f => RenderbufferStorage.DepthComponent32f,
            GFXRenderbufferStorage.Depth32fStencil8 => RenderbufferStorage.Depth32fStencil8,
            GFXRenderbufferStorage.StencilIndex1 => RenderbufferStorage.StencilIndex1,
            GFXRenderbufferStorage.StencilIndex1Ext => RenderbufferStorage.StencilIndex1Ext,
            GFXRenderbufferStorage.StencilIndex4 => RenderbufferStorage.StencilIndex4,
            GFXRenderbufferStorage.StencilIndex4Ext => RenderbufferStorage.StencilIndex4Ext,
            GFXRenderbufferStorage.StencilIndex8 => RenderbufferStorage.StencilIndex8,
            GFXRenderbufferStorage.StencilIndex8Ext => RenderbufferStorage.StencilIndex8Ext,
            GFXRenderbufferStorage.StencilIndex16 => RenderbufferStorage.StencilIndex16,
            GFXRenderbufferStorage.StencilIndex16Ext => RenderbufferStorage.StencilIndex16Ext,
            GFXRenderbufferStorage.Rgba32ui => RenderbufferStorage.Rgba32ui,
            GFXRenderbufferStorage.Rgb32ui => RenderbufferStorage.Rgb32ui,
            GFXRenderbufferStorage.Rgba16ui => RenderbufferStorage.Rgba16ui,
            GFXRenderbufferStorage.Rgb16ui => RenderbufferStorage.Rgb16ui,
            GFXRenderbufferStorage.Rgba8ui => RenderbufferStorage.Rgba8ui,
            GFXRenderbufferStorage.Rgb8ui => RenderbufferStorage.Rgb8ui,
            GFXRenderbufferStorage.Rgba32i => RenderbufferStorage.Rgba32i,
            GFXRenderbufferStorage.Rgb32i => RenderbufferStorage.Rgb32i,
            GFXRenderbufferStorage.Rgba16i => RenderbufferStorage.Rgba16i,
            GFXRenderbufferStorage.Rgb16i => RenderbufferStorage.Rgb16i,
            GFXRenderbufferStorage.Rgba8i => RenderbufferStorage.Rgba8i,
            GFXRenderbufferStorage.Rgb8i => RenderbufferStorage.Rgb8i,
            GFXRenderbufferStorage.Rgb10A2ui => RenderbufferStorage.Rgb10A2ui,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };

        internal static FramebufferErrorCode ToFramebufferErrorCode(GFXFramebufferErrorCode code) => code switch
        {
            GFXFramebufferErrorCode.FramebufferComplete => FramebufferErrorCode.FramebufferComplete,
            GFXFramebufferErrorCode.FramebufferUndefined => FramebufferErrorCode.FramebufferUndefined,
            GFXFramebufferErrorCode.FramebufferIncompleteAttachment => FramebufferErrorCode.FramebufferIncompleteAttachment,
            GFXFramebufferErrorCode.FramebufferIncompleteMissingAttachment => FramebufferErrorCode.FramebufferIncompleteMissingAttachment,
            GFXFramebufferErrorCode.FramebufferIncompleteDrawBuffer => FramebufferErrorCode.FramebufferIncompleteDrawBuffer,
            GFXFramebufferErrorCode.FramebufferIncompleteReadBuffer => FramebufferErrorCode.FramebufferIncompleteReadBuffer,
            GFXFramebufferErrorCode.FramebufferUnsupported => FramebufferErrorCode.FramebufferUnsupported,
            GFXFramebufferErrorCode.FramebufferIncompleteMultisample => FramebufferErrorCode.FramebufferIncompleteMultisample,
            GFXFramebufferErrorCode.FramebufferIncompleteLayerTargets => FramebufferErrorCode.FramebufferIncompleteLayerTargets,
            _ => throw new ArgumentOutOfRangeException(nameof(code), code, null)
        };

        internal static GFXFramebufferErrorCode ToGFXFramebufferErrorCode(FramebufferErrorCode code) => code switch
        {
            FramebufferErrorCode.FramebufferComplete => GFXFramebufferErrorCode.FramebufferComplete,
            FramebufferErrorCode.FramebufferUndefined => GFXFramebufferErrorCode.FramebufferUndefined,
            FramebufferErrorCode.FramebufferIncompleteAttachment => GFXFramebufferErrorCode.FramebufferIncompleteAttachment,
            FramebufferErrorCode.FramebufferIncompleteMissingAttachment => GFXFramebufferErrorCode.FramebufferIncompleteMissingAttachment,
            FramebufferErrorCode.FramebufferIncompleteDrawBuffer => GFXFramebufferErrorCode.FramebufferIncompleteDrawBuffer,
            FramebufferErrorCode.FramebufferIncompleteReadBuffer => GFXFramebufferErrorCode.FramebufferIncompleteReadBuffer,
            FramebufferErrorCode.FramebufferUnsupported => GFXFramebufferErrorCode.FramebufferUnsupported,
            FramebufferErrorCode.FramebufferIncompleteMultisample => GFXFramebufferErrorCode.FramebufferIncompleteMultisample,
            FramebufferErrorCode.FramebufferIncompleteLayerTargets => GFXFramebufferErrorCode.FramebufferIncompleteLayerTargets,
            _ => throw new ArgumentOutOfRangeException(nameof(code), code, null)
        };

        internal static BufferTarget ToBufferTarget(GFXBufferTarget target) => target switch
        {
            GFXBufferTarget.ArrayBuffer => BufferTarget.ArrayBuffer,
            GFXBufferTarget.AtomicCounterBuffer => BufferTarget.AtomicCounterBuffer,
            GFXBufferTarget.CopyReadBuffer => BufferTarget.CopyReadBuffer,
            GFXBufferTarget.CopyWriteBuffer => BufferTarget.CopyWriteBuffer,
            GFXBufferTarget.DispatchIndirectBuffer => BufferTarget.DispatchIndirectBuffer,
            GFXBufferTarget.DrawIndirectBuffer => BufferTarget.DrawIndirectBuffer,
            GFXBufferTarget.ElementArrayBuffer => BufferTarget.ElementArrayBuffer,
            GFXBufferTarget.PixelPackBuffer => BufferTarget.PixelPackBuffer,
            GFXBufferTarget.PixelUnpackBuffer => BufferTarget.PixelUnpackBuffer,
            GFXBufferTarget.QueryBuffer => BufferTarget.QueryBuffer,
            GFXBufferTarget.ShaderStorageBuffer => BufferTarget.ShaderStorageBuffer,
            GFXBufferTarget.TextureBuffer => BufferTarget.TextureBuffer,
            GFXBufferTarget.TransformFeedbackBuffer => BufferTarget.TransformFeedbackBuffer,
            GFXBufferTarget.UniformBuffer => BufferTarget.UniformBuffer,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };

        internal static BufferRangeTarget ToBufferRangeTarget(GFXBufferTarget target) => target switch
        {
            GFXBufferTarget.AtomicCounterBuffer => BufferRangeTarget.AtomicCounterBuffer,
            GFXBufferTarget.ShaderStorageBuffer => BufferRangeTarget.ShaderStorageBuffer,
            GFXBufferTarget.TransformFeedbackBuffer => BufferRangeTarget.TransformFeedbackBuffer,
            GFXBufferTarget.UniformBuffer => BufferRangeTarget.UniformBuffer,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };

        internal static BufferUsageHint ToBufferUsageHint(GFXBufferUsageHint hint) => hint switch
        {
            GFXBufferUsageHint.StreamDraw => BufferUsageHint.StreamDraw,
            GFXBufferUsageHint.StreamRead => BufferUsageHint.StreamRead,
            GFXBufferUsageHint.StreamCopy => BufferUsageHint.StreamCopy,
            GFXBufferUsageHint.StaticDraw => BufferUsageHint.StaticDraw,
            GFXBufferUsageHint.StaticRead => BufferUsageHint.StaticRead,
            GFXBufferUsageHint.StaticCopy => BufferUsageHint.StaticCopy,
            GFXBufferUsageHint.DynamicDraw => BufferUsageHint.DynamicDraw,
            GFXBufferUsageHint.DynamicRead => BufferUsageHint.DynamicRead,
            GFXBufferUsageHint.DynamicCopy => BufferUsageHint.DynamicCopy,
            _ => throw new ArgumentOutOfRangeException(nameof(hint), hint, null)
        };

    }
}
