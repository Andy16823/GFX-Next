namespace LibGFX.Graphics.RenderTarget
{
    /// <summary>
    /// Creates a 2D render target that encapsulates a framebuffer, texture, and optional renderbuffer for depth/stencil.
    /// </summary>
    public class RenderTarget2D : IRenderTarget
    {
        public int FramebufferId { get; set; } = -1;
        public int TextureId { get; set; } = -1;
        public int DepthAttachmentId { get; set; } = -1;
        public int Width { get; internal set; }
        public int Height { get; internal set; }

        public RenderTarget2D(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public byte[] GetPixelData()
        {
            throw new NotImplementedException();
        }
    }
}
