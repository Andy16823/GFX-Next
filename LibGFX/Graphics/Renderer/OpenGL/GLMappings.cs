using BulletSharp;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: Change to ToDrawBuffer()
namespace LibGFX.Graphics.Renderer.OpenGL
{
    /// <summary>
    /// Class containing mappings to OpenGL attributes
    /// </summary>
    internal class GLMappings
    {
        /// <summary>
        /// Converts a RenderFlags.TextureFilterMode to OpenGL TextureMinFilter.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static int ToGL(RenderFlags.TextureFilterMode mode) => mode switch
        {
            RenderFlags.TextureFilterMode.Nearest => (int)TextureMinFilter.Nearest,
            RenderFlags.TextureFilterMode.Linear => (int)TextureMinFilter.Linear,
            RenderFlags.TextureFilterMode.MipmapNearest => (int)TextureMinFilter.NearestMipmapNearest,
            RenderFlags.TextureFilterMode.MipmapLinear => (int)TextureMinFilter.LinearMipmapLinear,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        internal static DrawBufferMode ToDrawBuffer(RenderFlags.RenderBufferMode access) => access switch
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

        internal static ReadBufferMode ToReadBuffer(RenderFlags.RenderBufferMode access) => access switch
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
        internal static int ToGL(RenderFlags.TextureWrapMode mode) => mode switch
        {
            RenderFlags.TextureWrapMode.ClampToEdge => (int)TextureWrapMode.ClampToEdge,
            RenderFlags.TextureWrapMode.Repeat => (int)TextureWrapMode.Repeat,
            RenderFlags.TextureWrapMode.MirroredRepeat => (int)TextureWrapMode.MirroredRepeat,
            RenderFlags.TextureWrapMode.ClampToBorder => (int)TextureWrapMode.ClampToBorder,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        /// <summary>
        /// Converts a RenderFlags.ColorFormatLayout to OpenGL PixelFormat.
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static PixelFormat ToGL(RenderFlags.ColorFormatLayout hint) => hint switch
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
        internal static PixelInternalFormat ToGL(RenderFlags.ColorFormatHint hint) => hint switch
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
        internal static PixelType ToGL(RenderFlags.ColorFormatType type) => type switch
        {
            RenderFlags.ColorFormatType.UnsignedByte => PixelType.UnsignedByte,
            RenderFlags.ColorFormatType.UnsignedShort => PixelType.UnsignedShort,
            RenderFlags.ColorFormatType.UnsignedInt => PixelType.UnsignedInt,
            RenderFlags.ColorFormatType.UnsignedInt24_8 => PixelType.UnsignedInt248,
            RenderFlags.ColorFormatType.Float => PixelType.Float,
            RenderFlags.ColorFormatType.HalfFloat => PixelType.HalfFloat,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        internal static FramebufferAttachment ToGL(RenderFlags.GFXFramebufferAttachment attachment) => attachment switch
        {
            RenderFlags.GFXFramebufferAttachment.Color0 => FramebufferAttachment.ColorAttachment0,
            RenderFlags.GFXFramebufferAttachment.Color1 => FramebufferAttachment.ColorAttachment1,
            RenderFlags.GFXFramebufferAttachment.Color2 => FramebufferAttachment.ColorAttachment2,
            RenderFlags.GFXFramebufferAttachment.Color3 => FramebufferAttachment.ColorAttachment3,
            RenderFlags.GFXFramebufferAttachment.Color4 => FramebufferAttachment.ColorAttachment4,
            RenderFlags.GFXFramebufferAttachment.Color5 => FramebufferAttachment.ColorAttachment5,
            RenderFlags.GFXFramebufferAttachment.Color6 => FramebufferAttachment.ColorAttachment6,
            RenderFlags.GFXFramebufferAttachment.Color7 => FramebufferAttachment.ColorAttachment7,
            RenderFlags.GFXFramebufferAttachment.Depth => FramebufferAttachment.DepthAttachment,
            RenderFlags.GFXFramebufferAttachment.DepthStencil => FramebufferAttachment.DepthStencilAttachment,
            RenderFlags.GFXFramebufferAttachment.Stencil => FramebufferAttachment.StencilAttachment,
            _ => throw new ArgumentOutOfRangeException(nameof(attachment), attachment, null)
        };

        /// <summary>
        /// Converts RenderFlags.ClearFlags to OpenGL ClearBufferMask.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        internal static ClearBufferMask ToGL(RenderFlags.ClearFlags flags)
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
        /// Maps RenderFlags.RenderDataTypes to OpenGL VertexAttribPointerType.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static VertexAttribPointerType GetVertexAttribPointerType(RenderFlags.RenderDataTypes type) => type switch
        {
            RenderFlags.RenderDataTypes.Byte => VertexAttribPointerType.Byte,
            RenderFlags.RenderDataTypes.UnsignedByte => VertexAttribPointerType.UnsignedByte,
            RenderFlags.RenderDataTypes.Short => VertexAttribPointerType.Short,
            RenderFlags.RenderDataTypes.UnsignedShort => VertexAttribPointerType.UnsignedShort,
            RenderFlags.RenderDataTypes.Int => VertexAttribPointerType.Int,
            RenderFlags.RenderDataTypes.UnsignedInt => VertexAttribPointerType.UnsignedInt,
            RenderFlags.RenderDataTypes.Float => VertexAttribPointerType.Float,
            RenderFlags.RenderDataTypes.Double => VertexAttribPointerType.Double,
            RenderFlags.RenderDataTypes.Fixed => VertexAttribPointerType.Fixed,
            RenderFlags.RenderDataTypes.HalfFloat => VertexAttribPointerType.HalfFloat,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        /// <summary>
        /// Maps RenderFlags.PrimitiveTypes to OpenGL PrimitiveType.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static PrimitiveType ToPrimitiveType(RenderFlags.PrimitiveTypes type) => type switch
        {
            RenderFlags.PrimitiveTypes.Points => PrimitiveType.Points,
            RenderFlags.PrimitiveTypes.Lines => PrimitiveType.Lines,
            RenderFlags.PrimitiveTypes.LineLoop => PrimitiveType.LineLoop,
            RenderFlags.PrimitiveTypes.LineStrip => PrimitiveType.LineStrip,
            RenderFlags.PrimitiveTypes.Triangles => PrimitiveType.Triangles,
            RenderFlags.PrimitiveTypes.TriangleStrip => PrimitiveType.TriangleStrip,
            RenderFlags.PrimitiveTypes.TriangleFan => PrimitiveType.TriangleFan,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        /// <summary>
        /// Maps RenderFlags.PrimitiveTypes to OpenGL BeginMode.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static BeginMode ToBeginMode(RenderFlags.PrimitiveTypes type) => type switch
        {
            RenderFlags.PrimitiveTypes.Points => BeginMode.Points,
            RenderFlags.PrimitiveTypes.Lines => BeginMode.Lines,
            RenderFlags.PrimitiveTypes.LineLoop => BeginMode.LineLoop,
            RenderFlags.PrimitiveTypes.LineStrip => BeginMode.LineStrip,
            RenderFlags.PrimitiveTypes.Triangles => BeginMode.Triangles,
            RenderFlags.PrimitiveTypes.TriangleStrip => BeginMode.TriangleStrip,
            RenderFlags.PrimitiveTypes.TriangleFan => BeginMode.TriangleFan,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        internal static FramebufferTarget ToFramebufferTarget(RenderFlags.GFXFramebufferTarget target) => target switch
        {
            RenderFlags.GFXFramebufferTarget.ReadFramebuffer => FramebufferTarget.ReadFramebuffer,
            RenderFlags.GFXFramebufferTarget.DrawFramebuffer => FramebufferTarget.DrawFramebuffer,
            RenderFlags.GFXFramebufferTarget.Framebuffer => FramebufferTarget.Framebuffer,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };

        internal static TextureTarget ToTextureTarget(RenderFlags.GFXTextureTarget target) => target switch
        {
            RenderFlags.GFXTextureTarget.Texture1D => TextureTarget.Texture1D,
            RenderFlags.GFXTextureTarget.Texture2D => TextureTarget.Texture2D,
            RenderFlags.GFXTextureTarget.Texture3D => TextureTarget.Texture3D,
            RenderFlags.GFXTextureTarget.TextureCubeMap => TextureTarget.TextureCubeMap,
            RenderFlags.GFXTextureTarget.Texture1DArray => TextureTarget.Texture1DArray,
            RenderFlags.GFXTextureTarget.Texture2DArray => TextureTarget.Texture2DArray,
            RenderFlags.GFXTextureTarget.TextureRectangle => TextureTarget.TextureRectangle,
            RenderFlags.GFXTextureTarget.TextureCubeMapArray => TextureTarget.TextureCubeMapArray,
            RenderFlags.GFXTextureTarget.TextureBuffer => TextureTarget.TextureBuffer,
            RenderFlags.GFXTextureTarget.Texture2DMultisample => TextureTarget.Texture2DMultisample,
            RenderFlags.GFXTextureTarget.Texture2DMultisampleArray => TextureTarget.Texture2DMultisampleArray,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };

        internal static TextureParameterName ToTextureParameterName(RenderFlags.GFXTextureParameterName target) => target switch
        {
            RenderFlags.GFXTextureParameterName.TextureMinFilter => TextureParameterName.TextureMinFilter,
            RenderFlags.GFXTextureParameterName.TextureMagFilter => TextureParameterName.TextureMagFilter,
            RenderFlags.GFXTextureParameterName.TextureWrapS => TextureParameterName.TextureWrapS,
            RenderFlags.GFXTextureParameterName.TextureWrapT => TextureParameterName.TextureWrapT,
            RenderFlags.GFXTextureParameterName.TextureWrapR => TextureParameterName.TextureWrapR,
            RenderFlags.GFXTextureParameterName.TextureBorderColor => TextureParameterName.TextureBorderColor,
            RenderFlags.GFXTextureParameterName.TextureBaseLevel => TextureParameterName.TextureBaseLevel,
            RenderFlags.GFXTextureParameterName.TextureMaxLevel => TextureParameterName.TextureMaxLevel,
            RenderFlags.GFXTextureParameterName.TextureLodBias => TextureParameterName.TextureLodBias,
            RenderFlags.GFXTextureParameterName.TextureCompareMode => TextureParameterName.TextureCompareMode,
            RenderFlags.GFXTextureParameterName.TextureCompareFunc => TextureParameterName.TextureCompareFunc,
            RenderFlags.GFXTextureParameterName.GenerateMipmap => TextureParameterName.GenerateMipmap,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };

        internal static RenderbufferTarget ToRenderbufferTarget(RenderFlags.GFXRenderbufferTarget target) => target switch
        {
            RenderFlags.GFXRenderbufferTarget.Renderbuffer => RenderbufferTarget.Renderbuffer,
            RenderFlags.GFXRenderbufferTarget.RenderbufferExt => RenderbufferTarget.RenderbufferExt,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };

        internal static RenderbufferStorage ToRenderBufferStorage(RenderFlags.GFXRenderbufferStorage target) => target switch
        {
            RenderFlags.GFXRenderbufferStorage.R3G3B2 => RenderbufferStorage.R3G3B2,
            RenderFlags.GFXRenderbufferStorage.Alpha4 => RenderbufferStorage.Alpha4,
            RenderFlags.GFXRenderbufferStorage.Alpha8 => RenderbufferStorage.Alpha8,
            RenderFlags.GFXRenderbufferStorage.Alpha12 => RenderbufferStorage.Alpha12,
            RenderFlags.GFXRenderbufferStorage.Alpha16 => RenderbufferStorage.Alpha16,
            RenderFlags.GFXRenderbufferStorage.Rgb4 => RenderbufferStorage.Rgb4,
            RenderFlags.GFXRenderbufferStorage.Rgb5 => RenderbufferStorage.Rgb5,
            RenderFlags.GFXRenderbufferStorage.Rgb8 => RenderbufferStorage.Rgb8,
            RenderFlags.GFXRenderbufferStorage.Rgb10 => RenderbufferStorage.Rgb10,
            RenderFlags.GFXRenderbufferStorage.Rgb12 => RenderbufferStorage.Rgb12,
            RenderFlags.GFXRenderbufferStorage.Rgb16 => RenderbufferStorage.Rgb16,
            RenderFlags.GFXRenderbufferStorage.Rgba2 => RenderbufferStorage.Rgba2,
            RenderFlags.GFXRenderbufferStorage.Rgba4 => RenderbufferStorage.Rgba4,
            RenderFlags.GFXRenderbufferStorage.Rgba8 => RenderbufferStorage.Rgba8,
            RenderFlags.GFXRenderbufferStorage.Rgb10A2 => RenderbufferStorage.Rgb10A2,
            RenderFlags.GFXRenderbufferStorage.Rgba12 => RenderbufferStorage.Rgba12,
            RenderFlags.GFXRenderbufferStorage.Rgba16 => RenderbufferStorage.Rgba16,
            RenderFlags.GFXRenderbufferStorage.DepthComponent16 => RenderbufferStorage.DepthComponent16,
            RenderFlags.GFXRenderbufferStorage.DepthComponent24 => RenderbufferStorage.DepthComponent24,
            RenderFlags.GFXRenderbufferStorage.DepthComponent32 => RenderbufferStorage.DepthComponent32,
            RenderFlags.GFXRenderbufferStorage.R8 => RenderbufferStorage.R8,
            RenderFlags.GFXRenderbufferStorage.R16 => RenderbufferStorage.R16,
            RenderFlags.GFXRenderbufferStorage.Rg8 => RenderbufferStorage.Rg8,
            RenderFlags.GFXRenderbufferStorage.Rg16 => RenderbufferStorage.Rg16,
            RenderFlags.GFXRenderbufferStorage.R16f => RenderbufferStorage.R16f,
            RenderFlags.GFXRenderbufferStorage.R32f => RenderbufferStorage.R32f,
            RenderFlags.GFXRenderbufferStorage.Rg16f => RenderbufferStorage.Rg16f,
            RenderFlags.GFXRenderbufferStorage.Rg32f => RenderbufferStorage.Rg32f,
            RenderFlags.GFXRenderbufferStorage.R8i => RenderbufferStorage.R8i,
            RenderFlags.GFXRenderbufferStorage.R8ui => RenderbufferStorage.R8ui,
            RenderFlags.GFXRenderbufferStorage.R16i => RenderbufferStorage.R16i,
            RenderFlags.GFXRenderbufferStorage.R16ui => RenderbufferStorage.R16ui,
            RenderFlags.GFXRenderbufferStorage.R32i => RenderbufferStorage.R32i,
            RenderFlags.GFXRenderbufferStorage.R32ui => RenderbufferStorage.R32ui,
            RenderFlags.GFXRenderbufferStorage.Rg8i => RenderbufferStorage.Rg8i,
            RenderFlags.GFXRenderbufferStorage.Rg8ui => RenderbufferStorage.Rg8ui,
            RenderFlags.GFXRenderbufferStorage.Rg16i => RenderbufferStorage.Rg16i,
            RenderFlags.GFXRenderbufferStorage.Rg16ui => RenderbufferStorage.Rg16ui,
            RenderFlags.GFXRenderbufferStorage.Rg32i => RenderbufferStorage.Rg32i,
            RenderFlags.GFXRenderbufferStorage.Rg32ui => RenderbufferStorage.Rg32ui,
            RenderFlags.GFXRenderbufferStorage.Rgba32f => RenderbufferStorage.Rgba32f,
            RenderFlags.GFXRenderbufferStorage.Rgb32f => RenderbufferStorage.Rgb32f,
            RenderFlags.GFXRenderbufferStorage.Rgba16f => RenderbufferStorage.Rgba16f,
            RenderFlags.GFXRenderbufferStorage.Rgb16f => RenderbufferStorage.Rgb16f,
            RenderFlags.GFXRenderbufferStorage.Depth24Stencil8 => RenderbufferStorage.Depth24Stencil8,
            RenderFlags.GFXRenderbufferStorage.R11fG11fB10f => RenderbufferStorage.R11fG11fB10f,
            RenderFlags.GFXRenderbufferStorage.Rgb9E5 => RenderbufferStorage.Rgb9E5,
            RenderFlags.GFXRenderbufferStorage.Srgb8 => RenderbufferStorage.Srgb8,
            RenderFlags.GFXRenderbufferStorage.Srgb8Alpha8 => RenderbufferStorage.Srgb8Alpha8,
            RenderFlags.GFXRenderbufferStorage.DepthComponent32f => RenderbufferStorage.DepthComponent32f,
            RenderFlags.GFXRenderbufferStorage.Depth32fStencil8 => RenderbufferStorage.Depth32fStencil8,
            RenderFlags.GFXRenderbufferStorage.StencilIndex1 => RenderbufferStorage.StencilIndex1,
            RenderFlags.GFXRenderbufferStorage.StencilIndex1Ext => RenderbufferStorage.StencilIndex1Ext,
            RenderFlags.GFXRenderbufferStorage.StencilIndex4 => RenderbufferStorage.StencilIndex4,
            RenderFlags.GFXRenderbufferStorage.StencilIndex4Ext => RenderbufferStorage.StencilIndex4Ext,
            RenderFlags.GFXRenderbufferStorage.StencilIndex8 => RenderbufferStorage.StencilIndex8,
            RenderFlags.GFXRenderbufferStorage.StencilIndex8Ext => RenderbufferStorage.StencilIndex8Ext,
            RenderFlags.GFXRenderbufferStorage.StencilIndex16 => RenderbufferStorage.StencilIndex16,
            RenderFlags.GFXRenderbufferStorage.StencilIndex16Ext => RenderbufferStorage.StencilIndex16Ext,
            RenderFlags.GFXRenderbufferStorage.Rgba32ui => RenderbufferStorage.Rgba32ui,
            RenderFlags.GFXRenderbufferStorage.Rgb32ui => RenderbufferStorage.Rgb32ui,
            RenderFlags.GFXRenderbufferStorage.Rgba16ui => RenderbufferStorage.Rgba16ui,
            RenderFlags.GFXRenderbufferStorage.Rgb16ui => RenderbufferStorage.Rgb16ui,
            RenderFlags.GFXRenderbufferStorage.Rgba8ui => RenderbufferStorage.Rgba8ui,
            RenderFlags.GFXRenderbufferStorage.Rgb8ui => RenderbufferStorage.Rgb8ui,
            RenderFlags.GFXRenderbufferStorage.Rgba32i => RenderbufferStorage.Rgba32i,
            RenderFlags.GFXRenderbufferStorage.Rgb32i => RenderbufferStorage.Rgb32i,
            RenderFlags.GFXRenderbufferStorage.Rgba16i => RenderbufferStorage.Rgba16i,
            RenderFlags.GFXRenderbufferStorage.Rgb16i => RenderbufferStorage.Rgb16i,
            RenderFlags.GFXRenderbufferStorage.Rgba8i => RenderbufferStorage.Rgba8i,
            RenderFlags.GFXRenderbufferStorage.Rgb8i => RenderbufferStorage.Rgb8i,
            RenderFlags.GFXRenderbufferStorage.Rgb10A2ui => RenderbufferStorage.Rgb10A2ui,
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        };

        internal static FramebufferErrorCode ToFramebufferErrorCode(RenderFlags.GFXFramebufferErrorCode code) => code switch
        {
            RenderFlags.GFXFramebufferErrorCode.FramebufferComplete => FramebufferErrorCode.FramebufferComplete,
            RenderFlags.GFXFramebufferErrorCode.FramebufferUndefined => FramebufferErrorCode.FramebufferUndefined,
            RenderFlags.GFXFramebufferErrorCode.FramebufferIncompleteAttachment => FramebufferErrorCode.FramebufferIncompleteAttachment,
            RenderFlags.GFXFramebufferErrorCode.FramebufferIncompleteMissingAttachment => FramebufferErrorCode.FramebufferIncompleteMissingAttachment,
            RenderFlags.GFXFramebufferErrorCode.FramebufferIncompleteDrawBuffer => FramebufferErrorCode.FramebufferIncompleteDrawBuffer,
            RenderFlags.GFXFramebufferErrorCode.FramebufferIncompleteReadBuffer => FramebufferErrorCode.FramebufferIncompleteReadBuffer,
            RenderFlags.GFXFramebufferErrorCode.FramebufferUnsupported => FramebufferErrorCode.FramebufferUnsupported,
            RenderFlags.GFXFramebufferErrorCode.FramebufferIncompleteMultisample => FramebufferErrorCode.FramebufferIncompleteMultisample,
            RenderFlags.GFXFramebufferErrorCode.FramebufferIncompleteLayerTargets => FramebufferErrorCode.FramebufferIncompleteLayerTargets,
            _ => throw new ArgumentOutOfRangeException(nameof(code), code, null)
        };

        internal static RenderFlags.GFXFramebufferErrorCode ToGFXFramebufferErrorCode(FramebufferErrorCode code) => code switch
        {
            FramebufferErrorCode.FramebufferComplete => RenderFlags.GFXFramebufferErrorCode.FramebufferComplete,
            FramebufferErrorCode.FramebufferUndefined => RenderFlags.GFXFramebufferErrorCode.FramebufferUndefined,
            FramebufferErrorCode.FramebufferIncompleteAttachment => RenderFlags.GFXFramebufferErrorCode.FramebufferIncompleteAttachment,
            FramebufferErrorCode.FramebufferIncompleteMissingAttachment => RenderFlags.GFXFramebufferErrorCode.FramebufferIncompleteMissingAttachment,
            FramebufferErrorCode.FramebufferIncompleteDrawBuffer => RenderFlags.GFXFramebufferErrorCode.FramebufferIncompleteDrawBuffer,
            FramebufferErrorCode.FramebufferIncompleteReadBuffer => RenderFlags.GFXFramebufferErrorCode.FramebufferIncompleteReadBuffer,
            FramebufferErrorCode.FramebufferUnsupported => RenderFlags.GFXFramebufferErrorCode.FramebufferUnsupported,
            FramebufferErrorCode.FramebufferIncompleteMultisample => RenderFlags.GFXFramebufferErrorCode.FramebufferIncompleteMultisample,
            FramebufferErrorCode.FramebufferIncompleteLayerTargets => RenderFlags.GFXFramebufferErrorCode.FramebufferIncompleteLayerTargets,
            _ => throw new ArgumentOutOfRangeException(nameof(code), code, null)
        };

    }
}
