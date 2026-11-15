
namespace LibGFX.Graphics.RenderTarget
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
        /// Begins rendering to this render target. This method should be called before any rendering operations that target this render target.
        /// </summary>
        /// <param name="renderDevice">The render device to use for rendering.</param>
        /// <returns>A RenderTargetScope that will restore the previous render target when disposed.</returns>
        public RenderTargetScope Begin(IRenderDevice renderDevice);
    }
}
