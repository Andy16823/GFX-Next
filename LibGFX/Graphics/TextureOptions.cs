using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Struct representing options for texture configuration.
    /// </summary>
    public struct TextureOptions
    {
        public RenderFlags.TextureFilterMode MinFilter;
        public RenderFlags.TextureFilterMode MagFilter;
        public RenderFlags.TextureWrapMode WrapS;
        public RenderFlags.TextureWrapMode WrapT;
        public bool GenerateMipmaps;

        public static readonly TextureOptions Default = new TextureOptions
        {
            MinFilter = RenderFlags.TextureFilterMode.Linear,
            MagFilter = RenderFlags.TextureFilterMode.Linear,
            WrapS = RenderFlags.TextureWrapMode.Repeat,
            WrapT = RenderFlags.TextureWrapMode.Repeat,
            GenerateMipmaps = false
        };

        public static readonly TextureOptions PixelPerfect = new TextureOptions
        {
            MinFilter = RenderFlags.TextureFilterMode.Nearest,
            MagFilter = RenderFlags.TextureFilterMode.Nearest,
            WrapS = RenderFlags.TextureWrapMode.ClampToEdge,
            WrapT = RenderFlags.TextureWrapMode.ClampToEdge,
            GenerateMipmaps = false
        };

        public static readonly TextureOptions Mipmapped = new TextureOptions
        {
            MinFilter = RenderFlags.TextureFilterMode.MipmapLinear,
            MagFilter = RenderFlags.TextureFilterMode.Linear,
            WrapS = RenderFlags.TextureWrapMode.Repeat,
            WrapT = RenderFlags.TextureWrapMode.Repeat,
            GenerateMipmaps = true
        };
    }
}
