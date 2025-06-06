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
            MirroredRepeat
        }

        /// <summary>
        /// Hints for color formats used in textures and render targets.
        /// </summary>
        public enum ColorFormatHint
        {
            // 8-Bit pro Kanal
            R8,
            RG8,
            RGB8,
            RGBA8,

            // 16-Bit Gleitkomma
            R16F,
            RG16F,
            RGB16F,
            RGBA16F,

            // 32-Bit Gleitkomma
            R32F,
            RG32F,
            RGB32F,
            RGBA32F,

            // Integer Formate
            RGBA8UI,
            RGB10A2,
            Depth16,
            Depth24,
            Depth32,
            Depth24Stencil8,
            Depth32FStencil8,

            // Others
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

    }
}
