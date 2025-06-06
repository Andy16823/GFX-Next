using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public struct RenderTargetDescriptor
    {
        public int Width;
        public int Height;
        public int Border;

        public RenderFlags.ColorFormatHint Format;
        public RenderFlags.ColorFormatLayout Layout;
        public RenderFlags.ColorFormatType Type;

        public bool UseDepth;
        public bool UseStencil;

        public int Samples;
        public RenderFlags.TextureFilterMode FilterMode;

        public static RenderTargetDescriptor Default(int width, int height) => new RenderTargetDescriptor
        {
            Width = width,
            Height = height,
            Border = 0,
            Format = RenderFlags.ColorFormatHint.RGBA,
            Layout = RenderFlags.ColorFormatLayout.RGBA,
            Type = RenderFlags.ColorFormatType.UnsignedByte,
            UseDepth = true,
            UseStencil = true,
            Samples = 0,
            FilterMode = RenderFlags.TextureFilterMode.Linear
        };

        public static RenderTargetDescriptor DepthOnly(int width, int height) => new RenderTargetDescriptor
        {
            Width = width,
            Height = height,
            Border = 0,
            Format = RenderFlags.ColorFormatHint.Depth,
            Layout = RenderFlags.ColorFormatLayout.Depth,
            Type = RenderFlags.ColorFormatType.Float,
            UseDepth = true,
            UseStencil = false,
            Samples = 0,
            FilterMode = RenderFlags.TextureFilterMode.Nearest
        };

    }
}
