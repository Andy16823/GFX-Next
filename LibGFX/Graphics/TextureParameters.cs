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
        public GFXTextureFilterMode MinFilter;
        public GFXTextureFilterMode MagFilter;
        public GFXTextureWrapMode WrapS;
        public GFXTextureWrapMode WrapT;
        public bool GenerateMipmaps;

        public static readonly TextureParameters Default = new TextureParameters
        {
            MinFilter = GFXTextureFilterMode.Linear,
            MagFilter = GFXTextureFilterMode.Linear,
            WrapS = GFXTextureWrapMode.Repeat,
            WrapT = GFXTextureWrapMode.Repeat,
            GenerateMipmaps = false
        };

        public static readonly TextureParameters PixelPerfect = new TextureParameters
        {
            MinFilter = GFXTextureFilterMode.Nearest,
            MagFilter = GFXTextureFilterMode.Nearest,
            WrapS = GFXTextureWrapMode.ClampToEdge,
            WrapT = GFXTextureWrapMode.ClampToEdge,
            GenerateMipmaps = false
        };

        public static readonly TextureParameters Mipmapped = new TextureParameters
        {
            MinFilter = GFXTextureFilterMode.MipmapLinear,
            MagFilter = GFXTextureFilterMode.Linear,
            WrapS = GFXTextureWrapMode.Repeat,
            WrapT = GFXTextureWrapMode.Repeat,
            GenerateMipmaps = true
        };
    }
}
