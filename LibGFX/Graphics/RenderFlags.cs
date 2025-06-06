using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public class RenderFlags
    {
        [Flags] 
        public enum ClearFlags
        {
            None = 0,
            Color = 1,
            Depth = 2,
            Stencil = 4,
        }

        public enum TextureFilterMode
        {
            Nearest,
            Linear,
            MipmapNearest,
            MipmapLinear
        }

        public enum TextureWrapMode
        {
            ClampToEdge,
            Repeat,
            MirroredRepeat
        }

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
            Depth32FStencil8
        }

        public enum PixelFormatHint
        {
            R,
            RG,
            RGB,
            RGBA,
            Depth,
            DepthStencil
        }

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
