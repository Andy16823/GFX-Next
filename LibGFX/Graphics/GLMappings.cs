using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Class containing mappings to OpenGL attributes
    /// </summary>
    public class GLMappings
    {
        public static int ToGLMinFilter(RenderFlags.TextureFilterMode mode) => mode switch
        {
            RenderFlags.TextureFilterMode.Nearest => (int)OpenTK.Graphics.OpenGL4.TextureMinFilter.Nearest,
            RenderFlags.TextureFilterMode.Linear => (int)OpenTK.Graphics.OpenGL4.TextureMinFilter.Linear,
            RenderFlags.TextureFilterMode.MipmapNearest => (int)OpenTK.Graphics.OpenGL4.TextureMinFilter.NearestMipmapNearest,
            RenderFlags.TextureFilterMode.MipmapLinear => (int)OpenTK.Graphics.OpenGL4.TextureMinFilter.LinearMipmapLinear,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        public static int ToGLMagFilter(RenderFlags.TextureFilterMode mode) => mode switch
        {
            RenderFlags.TextureFilterMode.Nearest => (int)OpenTK.Graphics.OpenGL4.TextureMagFilter.Nearest,
            RenderFlags.TextureFilterMode.Linear => (int)OpenTK.Graphics.OpenGL4.TextureMagFilter.Linear,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        public static int ToGL(RenderFlags.TextureWrapMode mode) => mode switch
        {
            RenderFlags.TextureWrapMode.ClampToEdge => (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.ClampToEdge,
            RenderFlags.TextureWrapMode.Repeat => (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat,
            RenderFlags.TextureWrapMode.MirroredRepeat => (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.MirroredRepeat,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        public static int ToGL(RenderFlags.PixelFormatHint hint) => hint switch
        {
            RenderFlags.PixelFormatHint.R => (int)OpenTK.Graphics.OpenGL4.PixelFormat.Red,
            RenderFlags.PixelFormatHint.RG => (int)OpenTK.Graphics.OpenGL4.PixelFormat.Rg,
            RenderFlags.PixelFormatHint.RGB => (int)OpenTK.Graphics.OpenGL4.PixelFormat.Rgb,
            RenderFlags.PixelFormatHint.RGBA => (int)OpenTK.Graphics.OpenGL4.PixelFormat.Rgba,
            RenderFlags.PixelFormatHint.Depth => (int)OpenTK.Graphics.OpenGL4.PixelFormat.DepthComponent,
            RenderFlags.PixelFormatHint.DepthStencil => (int)OpenTK.Graphics.OpenGL4.PixelFormat.DepthStencil,
            _ => throw new ArgumentOutOfRangeException(nameof(hint), hint, null)
        };

        public static PixelInternalFormat ToGL(RenderFlags.ColorFormatHint hint) => hint switch
        {
            RenderFlags.ColorFormatHint.R8 => PixelInternalFormat.R8,
            RenderFlags.ColorFormatHint.RG8 => PixelInternalFormat.Rg8,
            RenderFlags.ColorFormatHint.RGB8 => PixelInternalFormat.Rgb8,
            RenderFlags.ColorFormatHint.RGBA8 => PixelInternalFormat.Rgba8,

            RenderFlags.ColorFormatHint.R16F => PixelInternalFormat.R16f,
            RenderFlags.ColorFormatHint.RG16F => PixelInternalFormat.Rg16f,
            RenderFlags.ColorFormatHint.RGB16F => PixelInternalFormat.Rgb16f,
            RenderFlags.ColorFormatHint.RGBA16F => PixelInternalFormat.Rgba16f,

            RenderFlags.ColorFormatHint.R32F => PixelInternalFormat.R32f,
            RenderFlags.ColorFormatHint.RG32F => PixelInternalFormat.Rg32f,
            RenderFlags.ColorFormatHint.RGB32F => PixelInternalFormat.Rgb32f,
            RenderFlags.ColorFormatHint.RGBA32F => PixelInternalFormat.Rgba32f,

            RenderFlags.ColorFormatHint.RGBA8UI => PixelInternalFormat.Rgba8ui,
            RenderFlags.ColorFormatHint.RGB10A2 => PixelInternalFormat.Rgb10A2,

            RenderFlags.ColorFormatHint.Depth16 => PixelInternalFormat.DepthComponent16,
            RenderFlags.ColorFormatHint.Depth24 => PixelInternalFormat.DepthComponent24,
            RenderFlags.ColorFormatHint.Depth32 => PixelInternalFormat.DepthComponent32,
            RenderFlags.ColorFormatHint.Depth24Stencil8 => PixelInternalFormat.Depth24Stencil8,
            RenderFlags.ColorFormatHint.Depth32FStencil8 => PixelInternalFormat.Depth32fStencil8,

            _ => throw new ArgumentOutOfRangeException(nameof(hint), hint, null)
        };

        public static int ToGL(RenderFlags.ColorFormatType type) => type switch
        {
            RenderFlags.ColorFormatType.UnsignedByte => (int)OpenTK.Graphics.OpenGL4.PixelType.UnsignedByte,
            RenderFlags.ColorFormatType.UnsignedShort => (int)OpenTK.Graphics.OpenGL4.PixelType.UnsignedShort,
            RenderFlags.ColorFormatType.UnsignedInt => (int)OpenTK.Graphics.OpenGL4.PixelType.UnsignedInt,
            RenderFlags.ColorFormatType.UnsignedInt24_8 => (int)OpenTK.Graphics.OpenGL4.PixelType.UnsignedInt248,
            RenderFlags.ColorFormatType.Float => (int)OpenTK.Graphics.OpenGL4.PixelType.Float,
            RenderFlags.ColorFormatType.HalfFloat => (int)OpenTK.Graphics.OpenGL4.PixelType.HalfFloat,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)

        };

        public static ClearBufferMask ToGL(RenderFlags.ClearFlags flags)
        {
            ClearBufferMask mask = 0;

            if ((flags & RenderFlags.ClearFlags.Color) != 0)
                mask |= ClearBufferMask.ColorBufferBit;

            if ((flags & RenderFlags.ClearFlags.Depth) != 0)
                mask |= ClearBufferMask.DepthBufferBit;

            if ((flags & RenderFlags.ClearFlags.Stencil) != 0)
                mask |= ClearBufferMask.StencilBufferBit;

            return mask;
        }

    }
}
