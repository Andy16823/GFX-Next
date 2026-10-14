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
        public TextureFilterMode MinFilter;
        public TextureFilterMode MagFilter;
        public TextureWrapMode WrapS;
        public TextureWrapMode WrapT;
        public bool GenerateMipmaps;

        public static readonly TextureParameters Default = new TextureParameters
        {
            MinFilter = TextureFilterMode.Linear,
            MagFilter = TextureFilterMode.Linear,
            WrapS = TextureWrapMode.Repeat,
            WrapT = TextureWrapMode.Repeat,
            GenerateMipmaps = false
        };

        public static readonly TextureParameters PixelPerfect = new TextureParameters
        {
            MinFilter = TextureFilterMode.Nearest,
            MagFilter = TextureFilterMode.Nearest,
            WrapS = TextureWrapMode.ClampToEdge,
            WrapT = TextureWrapMode.ClampToEdge,
            GenerateMipmaps = false
        };

        public static readonly TextureParameters Mipmapped = new TextureParameters
        {
            MinFilter = TextureFilterMode.MipmapLinear,
            MagFilter = TextureFilterMode.Linear,
            WrapS = TextureWrapMode.Repeat,
            WrapT = TextureWrapMode.Repeat,
            GenerateMipmaps = true
        };
    }
}
