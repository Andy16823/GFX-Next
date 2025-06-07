using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents a render target in the graphics rendering pipeline, including its texture, framebuffer, and render buffer IDs.
    /// </summary>
    public struct RenderTarget
    {
        public int TextureID;
        public int FramebufferID;
        public int RenderBufferID;

        public RenderFlags.ColorFormatHint Format;
        public RenderFlags.ColorFormatLayout Layout;
        public RenderFlags.ColorFormatType Type;

        public bool UseDepth;
        public bool UseStencil;

        public bool HasRenderBuffer => RenderBufferID != 0;
    }
}
