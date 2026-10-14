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
        /// Converts a TextureFilterMode to OpenGL TextureMinFilter.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static int ToGL(TextureFilterMode mode) => mode switch
        {
            TextureFilterMode.Nearest => (int)TextureMinFilter.Nearest,
            TextureFilterMode.Linear => (int)TextureMinFilter.Linear,
            TextureFilterMode.MipmapNearest => (int)TextureMinFilter.NearestMipmapNearest,
            TextureFilterMode.MipmapLinear => (int)TextureMinFilter.LinearMipmapLinear,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        internal static DrawBufferMode ToDrawBuffer(RenderBufferMode access) => access switch
        {
            RenderBufferMode.None => DrawBufferMode.None,
            RenderBufferMode.FrontLeft => DrawBufferMode.FrontLeft,
            RenderBufferMode.FrontRight => DrawBufferMode.FrontRight,
            RenderBufferMode.BackLeft => DrawBufferMode.BackLeft,
            RenderBufferMode.BackRight => DrawBufferMode.BackRight,
            RenderBufferMode.Left => DrawBufferMode.Left,
            RenderBufferMode.Right => DrawBufferMode.Right,
            RenderBufferMode.Color0 => DrawBufferMode.ColorAttachment0,
            RenderBufferMode.Color1 => DrawBufferMode.ColorAttachment1,
            RenderBufferMode.Color2 => DrawBufferMode.ColorAttachment2,
            RenderBufferMode.Color3 => DrawBufferMode.ColorAttachment3,
            RenderBufferMode.Color4 => DrawBufferMode.ColorAttachment4,
            RenderBufferMode.Color5 => DrawBufferMode.ColorAttachment5,
            RenderBufferMode.Color6 => DrawBufferMode.ColorAttachment6,
            RenderBufferMode.Color7 => DrawBufferMode.ColorAttachment7,
            RenderBufferMode.Color8 => DrawBufferMode.ColorAttachment8,
            RenderBufferMode.Color9 => DrawBufferMode.ColorAttachment9,
            RenderBufferMode.Color10 => DrawBufferMode.ColorAttachment10,
            RenderBufferMode.Color11 => DrawBufferMode.ColorAttachment11,
            RenderBufferMode.Color12 => DrawBufferMode.ColorAttachment12,
            RenderBufferMode.Color13 => DrawBufferMode.ColorAttachment13,
            RenderBufferMode.Color14 => DrawBufferMode.ColorAttachment14,
            RenderBufferMode.Color15 => DrawBufferMode.ColorAttachment15,
            RenderBufferMode.Color16 => DrawBufferMode.ColorAttachment16,
            RenderBufferMode.Color17 => DrawBufferMode.ColorAttachment17,
            RenderBufferMode.Color18 => DrawBufferMode.ColorAttachment18,
            RenderBufferMode.Color19 => DrawBufferMode.ColorAttachment19,
            RenderBufferMode.Color20 => DrawBufferMode.ColorAttachment20,
            RenderBufferMode.Color21 => DrawBufferMode.ColorAttachment21,
            RenderBufferMode.Color22 => DrawBufferMode.ColorAttachment22,
            RenderBufferMode.Color23 => DrawBufferMode.ColorAttachment23,
            RenderBufferMode.Color24 => DrawBufferMode.ColorAttachment24,
            RenderBufferMode.Color25 => DrawBufferMode.ColorAttachment25,
            RenderBufferMode.Color26 => DrawBufferMode.ColorAttachment26,
            RenderBufferMode.Color27 => DrawBufferMode.ColorAttachment27,
            RenderBufferMode.Color28 => DrawBufferMode.ColorAttachment28,
            RenderBufferMode.Color29 => DrawBufferMode.ColorAttachment29,
            RenderBufferMode.Color30 => DrawBufferMode.ColorAttachment30,
            RenderBufferMode.Color31 => DrawBufferMode.ColorAttachment31,
            _ => throw new ArgumentOutOfRangeException(nameof(access), access, null)
        };

        internal static ReadBufferMode ToReadBuffer(RenderBufferMode access) => access switch
        {
            RenderBufferMode.None => ReadBufferMode.None,
            RenderBufferMode.FrontLeft => ReadBufferMode.FrontLeft,
            RenderBufferMode.FrontRight => ReadBufferMode.FrontRight,
            RenderBufferMode.BackLeft => ReadBufferMode.BackLeft,
            RenderBufferMode.BackRight => ReadBufferMode.BackRight,
            RenderBufferMode.Left => ReadBufferMode.Left,
            RenderBufferMode.Right => ReadBufferMode.Right,
            RenderBufferMode.Color0 => ReadBufferMode.ColorAttachment0,
            RenderBufferMode.Color1 => ReadBufferMode.ColorAttachment1,
            RenderBufferMode.Color2 => ReadBufferMode.ColorAttachment2,
            RenderBufferMode.Color3 => ReadBufferMode.ColorAttachment3,
            RenderBufferMode.Color4 => ReadBufferMode.ColorAttachment4,
            RenderBufferMode.Color5 => ReadBufferMode.ColorAttachment5,
            RenderBufferMode.Color6 => ReadBufferMode.ColorAttachment6,
            RenderBufferMode.Color7 => ReadBufferMode.ColorAttachment7,
            RenderBufferMode.Color8 => ReadBufferMode.ColorAttachment8,
            RenderBufferMode.Color9 => ReadBufferMode.ColorAttachment9,
            RenderBufferMode.Color10 => ReadBufferMode.ColorAttachment10,
            RenderBufferMode.Color11 => ReadBufferMode.ColorAttachment11,
            RenderBufferMode.Color12 => ReadBufferMode.ColorAttachment12,
            RenderBufferMode.Color13 => ReadBufferMode.ColorAttachment13,
            RenderBufferMode.Color14 => ReadBufferMode.ColorAttachment14,
            RenderBufferMode.Color15 => ReadBufferMode.ColorAttachment15,
            _ => throw new ArgumentOutOfRangeException(nameof(access), access, null)
        };

        /// <summary>
        /// Converts a TextureWrapMode to OpenGL TextureWrapMode.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static int ToGL(TextureWrapMode mode) => mode switch
        {
            TextureWrapMode.ClampToEdge => (int)TextureWrapMode.ClampToEdge,
            TextureWrapMode.Repeat => (int)TextureWrapMode.Repeat,
            TextureWrapMode.MirroredRepeat => (int)TextureWrapMode.MirroredRepeat,
            TextureWrapMode.ClampToBorder => (int)TextureWrapMode.ClampToBorder,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        /// <summary>
        /// Converts a ColorFormatLayout to OpenGL PixelFormat.
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static PixelFormat ToGL(ColorFormatLayout hint) => hint switch
        {
            ColorFormatLayout.R => PixelFormat.Red,
            ColorFormatLayout.RG => PixelFormat.Rg,
            ColorFormatLayout.RGB => PixelFormat.Rgb,
            ColorFormatLayout.RGBA => PixelFormat.Rgba,
            ColorFormatLayout.Depth => PixelFormat.DepthComponent,
            ColorFormatLayout.DepthStencil => PixelFormat.DepthStencil,
            _ => throw new ArgumentOutOfRangeException(nameof(hint), hint, null)
        };

        /// <summary>
        /// Converts a ColorFormatHint to OpenGL PixelInternalFormat.
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static PixelInternalFormat ToGL(ColorFormatHint hint) => hint switch
        {
            ColorFormatHint.R8 => PixelInternalFormat.R8,
            ColorFormatHint.RG8 => PixelInternalFormat.Rg8,
            ColorFormatHint.RGB8 => PixelInternalFormat.Rgb8,
            ColorFormatHint.RGBA8 => PixelInternalFormat.Rgba8,

            ColorFormatHint.R16F => PixelInternalFormat.R16f,
            ColorFormatHint.RG16F => PixelInternalFormat.Rg16f,
            ColorFormatHint.RGB16F => PixelInternalFormat.Rgb16f,
            ColorFormatHint.RGBA16F => PixelInternalFormat.Rgba16f,

            ColorFormatHint.R32F => PixelInternalFormat.R32f,
            ColorFormatHint.RG32F => PixelInternalFormat.Rg32f,
            ColorFormatHint.RGB32F => PixelInternalFormat.Rgb32f,
            ColorFormatHint.RGBA32F => PixelInternalFormat.Rgba32f,

            ColorFormatHint.RGBA8UI => PixelInternalFormat.Rgba8ui,
            ColorFormatHint.RGB10A2 => PixelInternalFormat.Rgb10A2,

            ColorFormatHint.Depth16 => PixelInternalFormat.DepthComponent16,
            ColorFormatHint.Depth24 => PixelInternalFormat.DepthComponent24,
            ColorFormatHint.Depth32 => PixelInternalFormat.DepthComponent32,
            ColorFormatHint.Depth24Stencil8 => PixelInternalFormat.Depth24Stencil8,
            ColorFormatHint.Depth32FStencil8 => PixelInternalFormat.Depth32fStencil8,

            ColorFormatHint.RGBA => PixelInternalFormat.Rgba,
            ColorFormatHint.RGB => PixelInternalFormat.Rgb,
            ColorFormatHint.Depth => PixelInternalFormat.DepthComponent,
            ColorFormatHint.DepthStencil => PixelInternalFormat.DepthStencil,

            _ => throw new ArgumentOutOfRangeException(nameof(hint), hint, null)
        };

        /// <summary>
        /// Converts a ColorFormatType to OpenGL PixelType.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static PixelType ToGL(ColorFormatType type) => type switch
        {
            ColorFormatType.UnsignedByte => PixelType.UnsignedByte,
            ColorFormatType.UnsignedShort => PixelType.UnsignedShort,
            ColorFormatType.UnsignedInt => PixelType.UnsignedInt,
            ColorFormatType.UnsignedInt24_8 => PixelType.UnsignedInt248,
            ColorFormatType.Float => PixelType.Float,
            ColorFormatType.HalfFloat => PixelType.HalfFloat,
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
        /// Converts ClearFlags to OpenGL ClearBufferMask.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        internal static ClearBufferMask ToGL(ClearFlags flags)
        {
            ClearBufferMask mask = 0;

            if ((flags & ClearFlags.Color) != 0)
                mask |= ClearBufferMask.ColorBufferBit;

            if ((flags & ClearFlags.Depth) != 0)
                mask |= ClearBufferMask.DepthBufferBit;

            if ((flags & ClearFlags.Stencil) != 0)
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
        /// Maps RenderDataTypes to OpenGL VertexAttribPointerType.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static VertexAttribPointerType GetVertexAttribPointerType(RenderDataTypes type) => type switch
        {
            RenderDataTypes.Byte => VertexAttribPointerType.Byte,
            RenderDataTypes.UnsignedByte => VertexAttribPointerType.UnsignedByte,
            RenderDataTypes.Short => VertexAttribPointerType.Short,
            RenderDataTypes.UnsignedShort => VertexAttribPointerType.UnsignedShort,
            RenderDataTypes.Int => VertexAttribPointerType.Int,
            RenderDataTypes.UnsignedInt => VertexAttribPointerType.UnsignedInt,
            RenderDataTypes.Float => VertexAttribPointerType.Float,
            RenderDataTypes.Double => VertexAttribPointerType.Double,
            RenderDataTypes.Fixed => VertexAttribPointerType.Fixed,
            RenderDataTypes.HalfFloat => VertexAttribPointerType.HalfFloat,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        /// <summary>
        /// Maps PrimitiveTypes to OpenGL PrimitiveType.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static PrimitiveType ToPrimitiveType(PrimitiveTypes type) => type switch
        {
            PrimitiveTypes.Points => PrimitiveType.Points,
            PrimitiveTypes.Lines => PrimitiveType.Lines,
            PrimitiveTypes.LineLoop => PrimitiveType.LineLoop,
            PrimitiveTypes.LineStrip => PrimitiveType.LineStrip,
            PrimitiveTypes.Triangles => PrimitiveType.Triangles,
            PrimitiveTypes.TriangleStrip => PrimitiveType.TriangleStrip,
            PrimitiveTypes.TriangleFan => PrimitiveType.TriangleFan,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        /// <summary>
        /// Maps PrimitiveTypes to OpenGL BeginMode.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static BeginMode ToBeginMode(PrimitiveTypes type) => type switch
        {
            PrimitiveTypes.Points => BeginMode.Points,
            PrimitiveTypes.Lines => BeginMode.Lines,
            PrimitiveTypes.LineLoop => BeginMode.LineLoop,
            PrimitiveTypes.LineStrip => BeginMode.LineStrip,
            PrimitiveTypes.Triangles => BeginMode.Triangles,
            PrimitiveTypes.TriangleStrip => BeginMode.TriangleStrip,
            PrimitiveTypes.TriangleFan => BeginMode.TriangleFan,
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
