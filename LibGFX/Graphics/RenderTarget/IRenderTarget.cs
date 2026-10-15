
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
    }
}
