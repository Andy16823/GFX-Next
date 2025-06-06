using BulletSharp;
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
        /// <summary>
        /// Converts a RenderFlags.TextureFilterMode to OpenGL TextureMinFilter.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int ToGLMinFilter(RenderFlags.TextureFilterMode mode) => mode switch
        {
            RenderFlags.TextureFilterMode.Nearest => (int)OpenTK.Graphics.OpenGL4.TextureMinFilter.Nearest,
            RenderFlags.TextureFilterMode.Linear => (int)OpenTK.Graphics.OpenGL4.TextureMinFilter.Linear,
            RenderFlags.TextureFilterMode.MipmapNearest => (int)OpenTK.Graphics.OpenGL4.TextureMinFilter.NearestMipmapNearest,
            RenderFlags.TextureFilterMode.MipmapLinear => (int)OpenTK.Graphics.OpenGL4.TextureMinFilter.LinearMipmapLinear,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        /// <summary>
        /// Converts a RenderFlags.TextureFilterMode to OpenGL TextureMagFilter.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int ToGLMagFilter(RenderFlags.TextureFilterMode mode) => mode switch
        {
            RenderFlags.TextureFilterMode.Nearest => (int)OpenTK.Graphics.OpenGL4.TextureMagFilter.Nearest,
            RenderFlags.TextureFilterMode.Linear => (int)OpenTK.Graphics.OpenGL4.TextureMagFilter.Linear,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        /// <summary>
        /// Converts a RenderFlags.TextureWrapMode to OpenGL TextureWrapMode.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int ToGL(RenderFlags.TextureWrapMode mode) => mode switch
        {
            RenderFlags.TextureWrapMode.ClampToEdge => (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.ClampToEdge,
            RenderFlags.TextureWrapMode.Repeat => (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat,
            RenderFlags.TextureWrapMode.MirroredRepeat => (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.MirroredRepeat,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        /// <summary>
        /// Converts a RenderFlags.ColorFormatLayout to OpenGL PixelFormat.
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static PixelFormat ToGL(RenderFlags.ColorFormatLayout hint) => hint switch
        {
            RenderFlags.ColorFormatLayout.R => PixelFormat.Red,
            RenderFlags.ColorFormatLayout.RG => PixelFormat.Rg,
            RenderFlags.ColorFormatLayout.RGB => PixelFormat.Rgb,
            RenderFlags.ColorFormatLayout.RGBA => PixelFormat.Rgba,
            RenderFlags.ColorFormatLayout.Depth => PixelFormat.DepthComponent,
            RenderFlags.ColorFormatLayout.DepthStencil => PixelFormat.DepthStencil,
            _ => throw new ArgumentOutOfRangeException(nameof(hint), hint, null)
        };

        /// <summary>
        /// Converts a RenderFlags.ColorFormatHint to OpenGL PixelInternalFormat.
        /// </summary>
        /// <param name="hint"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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

            RenderFlags.ColorFormatHint.RGBA => PixelInternalFormat.Rgba,
            RenderFlags.ColorFormatHint.RGB => PixelInternalFormat.Rgb,
            RenderFlags.ColorFormatHint.Depth => PixelInternalFormat.DepthComponent,
            RenderFlags.ColorFormatHint.DepthStencil => PixelInternalFormat.DepthStencil,

            _ => throw new ArgumentOutOfRangeException(nameof(hint), hint, null)
        };

        /// <summary>
        /// Converts a RenderFlags.ColorFormatType to OpenGL PixelType.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static PixelType ToGL(RenderFlags.ColorFormatType type) => type switch
        {
            RenderFlags.ColorFormatType.UnsignedByte => PixelType.UnsignedByte,
            RenderFlags.ColorFormatType.UnsignedShort => PixelType.UnsignedShort,
            RenderFlags.ColorFormatType.UnsignedInt => PixelType.UnsignedInt,
            RenderFlags.ColorFormatType.UnsignedInt24_8 => PixelType.UnsignedInt248,
            RenderFlags.ColorFormatType.Float => PixelType.Float,
            RenderFlags.ColorFormatType.HalfFloat => PixelType.HalfFloat,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)

        };

        /// <summary>
        /// Converts RenderFlags.ClearFlags to OpenGL ClearBufferMask.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the best depth-stencil format based on the specified flags.
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="stencil"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static RenderbufferStorage GetBestDepthStencilFormat(bool depth, bool stencil)
        {
            return (depth, stencil) switch
            {
                (true, true) => RenderbufferStorage.Depth24Stencil8,
                (true, false) => RenderbufferStorage.DepthComponent24,
                (false, true) => RenderbufferStorage.StencilIndex8,
                _ => throw new ArgumentException("At least one of depth or stencil must be true.")
            };
        }
    }
}
