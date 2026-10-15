
namespace LibGFX.Graphics.RenderTarget
{
    /// <summary>
    /// Represents a render target that only contains a depth texture.
    /// Useful for shadow mapping and depth pre-pass techniques.
    /// </summary>
    public class DepthOnlyRenderTarget : IRenderTarget
    {
        public int FramebufferId { get; set; } = -1;
        public int DepthTextureId { get; set; } = -1;
        public int Width { get; internal set; }
        public int Height { get; internal set; }

        public DepthOnlyRenderTarget(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }
    }
}
