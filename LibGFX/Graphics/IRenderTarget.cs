using LibGFX.Core;
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
        /// Creates the render target resources.
        /// </summary>
        public void Create();

        /// <summary>
        /// Free the render target resources from the specified render device.
        /// </summary>
        public void Dispose();

        /// <summary>
        /// Gets the pixel data from the render target. This is typically used for reading back the rendered image for saving to disk or processing on the CPU.
        /// </summary>
        /// <returns></returns>
        public byte[] GetPixelData();

        /// <summary>
        /// Resizes the render target to the specified width and height. This is typically called when the window is resized.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Resize(int width, int height);
    }
}
