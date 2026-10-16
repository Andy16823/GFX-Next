using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public enum GFXCullMode
    {
        Front,
        Back,
        None,
        FrontAndBack
    }

    /// <summary>
    /// Flags to specify which buffers to clear before rendering.
    /// </summary>
    [Flags] 
    public enum GFXClearFlags
    {
        None = 0,
        Color = 1,
        Depth = 2,
        Stencil = 4,
    }

    /// <summary>
    /// Texture filtering modes used for texture sampling.
    /// </summary>
    public enum GFXTextureFilterMode
    {
        Nearest,
        Linear,
        MipmapNearest,
        MipmapLinear
    }

    /// <summary>
    /// Texture wrapping modes used to determine how textures are sampled outside their normal range.
    /// </summary>
    public enum GFXTextureWrapMode
    {
        ClampToEdge,
        Repeat,
        MirroredRepeat,
        ClampToBorder
    }

    /// <summary>
    /// Hints for color formats used in textures and render targets.
    /// </summary>
    public enum GFXColorFormatHint
    {
        R8,
        RG8,
        RGB8,
        RGBA8,
        R16F,
        RG16F,
        RGB16F,
        RGBA16F,
        R32F,
        RG32F,
        RGB32F,
        RGBA32F,
        RGBA8UI,
        RGB10A2,
        Depth16,
        Depth24,
        Depth32,
        Depth24Stencil8,
        Depth32FStencil8,
        RGB,
        RGBA,
        Depth,
        DepthStencil
    }

    /// <summary>
    /// Data types used for rendering operations, specifying the type of data being processed.
    /// </summary>
    public enum GFXRenderDataTypes
    {
        Byte,
        UnsignedByte,
        Short,
        UnsignedShort,
        Int,
        UnsignedInt,
        Float,
        Double,
        HalfFloat,
        Fixed
    }

    /// <summary>
    /// Primitive types used for rendering geometric shapes.
    /// </summary>
    public enum GFXPrimitiveTypes
    {
        Points,
        Lines,
        LineLoop,
        LineStrip,
        Triangles,
        TriangleStrip,
        TriangleFan
    }

    public enum GFXBufferTarget
    {
        ArrayBuffer,
        AtomicCounterBuffer,
        CopyReadBuffer,
        CopyWriteBuffer,
        DispatchIndirectBuffer,
        DrawIndirectBuffer,
        ElementArrayBuffer,
        PixelPackBuffer,
        PixelUnpackBuffer,
        QueryBuffer,
        ShaderStorageBuffer,
        TextureBuffer,
        TransformFeedbackBuffer,
        UniformBuffer
    }

    public enum GFXBufferUsageHint
    {
        StreamDraw,
        StreamRead,
        StreamCopy,
        StaticDraw,
        StaticRead,
        StaticCopy,
        DynamicDraw,
        DynamicRead,
        DynamicCopy
    }   

    [Flags]
    public enum GFXMemoryBarrierFlags
    {
        VertexAttribArrayBarrierBit = 1,
        ElementArrayBarrierBit = 2,
        UniformBarrierBit = 4,
        TextureFetchBarrierBit = 8,
        ShaderImageAccessBarrierBit = 32,
        CommandBarrierBit = 64,
        PixelBufferBarrierBit = 128,
        TextureUpdateBarrierBit = 256,
        BufferUpdateBarrierBit = 512,
        FramebufferBarrierBit = 1024,
        TransformFeedbackBarrierBit = 2048,
        AtomicCounterBarrierBit = 4096,
        ShaderStorageBarrierBit = 8192,
        ClientMappedBufferBarrierBit = 16384,
        QueryBufferBarrierBit = 32768,
        AllBarrierBits = -1
    }

    [Flags]
    public enum GFXMapBufferAccessMask 
    {
        MapReadBit = 1,
        MapReadBitExt = 1,
        MapWriteBit = 2,
        MapWriteBitExt = 2,
        MapInvalidateRangeBit = 4,
        MapInvalidateRangeBitExt = 4,
        MapInvalidateBufferBit = 8,
        MapInvalidateBufferBitExt = 8,
        MapFlushExplicitBit = 0x10,
        MapFlushExplicitBitExt = 0x10,
        MapUnsynchronizedBit = 0x20,
        MapUnsynchronizedBitExt = 0x20,
        MapPersistentBit = 0x40,
        MapPersistentBitExt = 0x40,
        MapCoherentBit = 0x80,
        MapCoherentBitExt = 0x80
    }
}

