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
        public abstract int RenderTargetId { get; }

        /// <summary>
        /// Creates the render target on the given render device.
        /// </summary>
        /// <param name="renderer"></param>
        public void Create(IRenderDevice renderer);

        /// <summary>
        /// Resizes the render target to the given width and height.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Resize(IRenderDevice renderer, int width, int height);

        /// <summary>
        /// Disposes the render target from the given render device.
        /// </summary>
        /// <param name="renderer"></param>
        public void Dispose(IRenderDevice renderer);
    }
}
