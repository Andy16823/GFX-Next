using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Interface for render targets such as framebuffers and the default framebuffer (screen).
    /// </summary>
    public interface IRenderTarget
    {
        /// <summary>
        /// The unique identifier for the render target.
        /// </summary>
        public abstract int FramebufferId { get; set; }

        /// <summary>
        /// Disposes the render target from the given render device.
        /// </summary>
        /// <param name="renderer"></param>
        public abstract void Dispose(IRenderDevice renderer);
    }
}
