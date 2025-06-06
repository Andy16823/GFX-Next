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
