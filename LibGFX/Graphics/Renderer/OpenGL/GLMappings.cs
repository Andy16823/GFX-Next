using OpenTK.Graphics.OpenGL4;

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

        /// <summary>
        /// Converts a GFXTextureWrapMode to OpenGL GFXTextureWrapMode.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static int ToGL(GFXTextureWrapMode mode) => mode switch
        {
            GFXTextureWrapMode.ClampToEdge => (int)TextureWrapMode.ClampToEdge,
            GFXTextureWrapMode.Repeat => (int)TextureWrapMode.Repeat,
            GFXTextureWrapMode.MirroredRepeat => (int)TextureWrapMode.MirroredRepeat,
            GFXTextureWrapMode.ClampToBorder => (int)TextureWrapMode.ClampToBorder,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
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
        /// Derives the OpenGL PixelType (data type of the pixel components) from a GFXColorFormatHint.
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static PixelType ToPixelType(GFXColorFormatHint hint) => hint switch
        {
            GFXColorFormatHint.R8 or
            GFXColorFormatHint.RG8 or
            GFXColorFormatHint.RGB8 or
            GFXColorFormatHint.RGBA8 or
            GFXColorFormatHint.RGBA8UI or
            GFXColorFormatHint.RGB or
            GFXColorFormatHint.RGBA => PixelType.UnsignedByte,

            GFXColorFormatHint.R16F or
            GFXColorFormatHint.RG16F or
            GFXColorFormatHint.RGB16F or
            GFXColorFormatHint.RGBA16F => PixelType.HalfFloat,

            GFXColorFormatHint.R32F or
            GFXColorFormatHint.RG32F or
            GFXColorFormatHint.RGB32F or
            GFXColorFormatHint.RGBA32F or
            GFXColorFormatHint.Depth => PixelType.Float,

            GFXColorFormatHint.RGB10A2 => PixelType.UnsignedInt1010102,

            GFXColorFormatHint.Depth16 => PixelType.UnsignedShort,
            GFXColorFormatHint.Depth24 or
            GFXColorFormatHint.Depth32 => PixelType.UnsignedInt,

            GFXColorFormatHint.Depth24Stencil8 or
            GFXColorFormatHint.DepthStencil => PixelType.UnsignedInt248,
            GFXColorFormatHint.Depth32FStencil8 => PixelType.Float32UnsignedInt248Rev,

            _ => throw new ArgumentOutOfRangeException(nameof(hint), hint, null)
        };

        /// <summary>
        /// Derives the OpenGL PixelFormat (channel layout) from a GFXColorFormatHint.
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static PixelFormat ToPixelFormat(GFXColorFormatHint hint) => hint switch
        {
            GFXColorFormatHint.R8 or
            GFXColorFormatHint.R16F or
            GFXColorFormatHint.R32F => PixelFormat.Red,

            GFXColorFormatHint.RG8 or
            GFXColorFormatHint.RG16F or
            GFXColorFormatHint.RG32F => PixelFormat.Rg,

            GFXColorFormatHint.RGB8 or
            GFXColorFormatHint.RGB16F or
            GFXColorFormatHint.RGB32F or
            GFXColorFormatHint.RGB => PixelFormat.Rgb,

            GFXColorFormatHint.RGBA8 or
            GFXColorFormatHint.RGBA16F or
            GFXColorFormatHint.RGBA32F or
            GFXColorFormatHint.RGB10A2 or
            GFXColorFormatHint.RGBA => PixelFormat.Rgba,

            GFXColorFormatHint.RGBA8UI => PixelFormat.RgbaInteger,

            GFXColorFormatHint.Depth16 or
            GFXColorFormatHint.Depth24 or
            GFXColorFormatHint.Depth32 or
            GFXColorFormatHint.Depth => PixelFormat.DepthComponent,

            GFXColorFormatHint.Depth24Stencil8 or
            GFXColorFormatHint.Depth32FStencil8 or
            GFXColorFormatHint.DepthStencil => PixelFormat.DepthStencil,

            _ => throw new ArgumentOutOfRangeException(nameof(hint), hint, null)
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
