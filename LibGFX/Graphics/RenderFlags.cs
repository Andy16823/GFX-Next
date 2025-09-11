using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents various render flags and settings used in the graphics rendering pipeline.
    /// </summary>
    public class RenderFlags
    {
        /// <summary>
        /// Flags to specify which buffers to clear before rendering.
        /// </summary>
        [Flags] 
        public enum ClearFlags
        {
            None = 0,
            Color = 1,
            Depth = 2,
            Stencil = 4,
        }

        /// <summary>
        /// Texture filtering modes used for texture sampling.
        /// </summary>
        public enum TextureFilterMode
        {
            Nearest,
            Linear,
            MipmapNearest,
            MipmapLinear
        }

        /// <summary>
        /// Texture wrapping modes used to determine how textures are sampled outside their normal range.
        /// </summary>
        public enum TextureWrapMode
        {
            ClampToEdge,
            Repeat,
            MirroredRepeat,
            ClampToBorder
        }

        /// <summary>
        /// Hints for color formats used in textures and render targets.
        /// </summary>
        public enum ColorFormatHint
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
        /// Represents the layout of color formats used in textures and render targets.
        /// </summary>
        public enum ColorFormatLayout
        {
            R,
            RG,
            RGB,
            RGBA,
            Depth,
            DepthStencil
        }

        /// <summary>
        /// Represents the type of color format used in textures and render targets.
        /// </summary>
        public enum ColorFormatType
        {
            UnsignedByte,
            Float,
            HalfFloat,
            UnsignedInt24_8,
            UnsignedShort,
            UnsignedInt
        }

        /// <summary>
        /// Attachment points for render targets, specifying where to attach textures or buffers.
        /// </summary>
        public enum RenderAttachmentPoint
        {
            Color0,
            Color1,
            Color2,
            Color3,
            Color4,
            Color5,
            Color6,
            Color7,
            Depth,
            DepthStencil,
            Stencil
        }

        /// <summary>
        /// Modes for rendering buffers, specifying which buffer to use for rendering operations.
        /// </summary>
        public enum RenderBufferMode
        {
            None,
            FrontLeft,
            FrontRight,
            BackLeft,
            BackRight,
            Left,
            Right,
            Color0,
            Color1,
            Color2,
            Color3,
            Color4,
            Color5,
            Color6,
            Color7,
            Color8,
            Color9,
            Color10,
            Color11,
            Color12,
            Color13,
            Color14,
            Color15,
            Color16,
            Color17,
            Color18,
            Color19,
            Color20,
            Color21,
            Color22,
            Color23,
            Color24,
            Color25,
            Color26,
            Color27,
            Color28,
            Color29,
            Color30,
            Color31,
        }

        /// <summary>
        /// Data types used for rendering operations, specifying the type of data being processed.
        /// </summary>
        public enum RenderDataTypes
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
        public enum PrimitiveTypes
        {
            Points,
            Lines,
            LineLoop,
            LineStrip,
            Triangles,
            TriangleStrip,
            TriangleFan
        }
    }
}
