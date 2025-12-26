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
    public struct TextureParameters
    {
        public RenderFlags.TextureFilterMode MinFilter;
        public RenderFlags.TextureFilterMode MagFilter;
        public RenderFlags.TextureWrapMode WrapS;
        public RenderFlags.TextureWrapMode WrapT;
        public bool GenerateMipmaps;

        public static readonly TextureParameters Default = new TextureParameters
        {
            MinFilter = RenderFlags.TextureFilterMode.Linear,
            MagFilter = RenderFlags.TextureFilterMode.Linear,
            WrapS = RenderFlags.TextureWrapMode.Repeat,
            WrapT = RenderFlags.TextureWrapMode.Repeat,
            GenerateMipmaps = false
        };

        public static readonly TextureParameters PixelPerfect = new TextureParameters
        {
            MinFilter = RenderFlags.TextureFilterMode.Nearest,
            MagFilter = RenderFlags.TextureFilterMode.Nearest,
            WrapS = RenderFlags.TextureWrapMode.ClampToEdge,
            WrapT = RenderFlags.TextureWrapMode.ClampToEdge,
            GenerateMipmaps = false
        };

        public static readonly TextureParameters Mipmapped = new TextureParameters
        {
            MinFilter = RenderFlags.TextureFilterMode.MipmapLinear,
            MagFilter = RenderFlags.TextureFilterMode.Linear,
            WrapS = RenderFlags.TextureWrapMode.Repeat,
            WrapT = RenderFlags.TextureWrapMode.Repeat,
            GenerateMipmaps = true
        };
    }
}
