namespace LibGFX.Graphics.RenderTarget
{
    /// <summary>
    /// A 2D render target that encapsulates a framebuffer, texture, and optional renderbuffer for depth/stencil.
    /// </summary>
    public class MSAARenderTarget2D : IRenderTarget
    {
        public int TextureFbo { get; set; } = -1;
        public int TextureId { get; set; } = -1;
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public int FramebufferId { get; set; } = -1;
        public int ColorAttachmentId { get; set; } = -1;
        public int DepthAttachmentId { get; set; } = -1;
        public int Samples { get; set; } = 0;

        public MSAARenderTarget2D(int width, int height, int samples = 0)
        {
            this.Width = width;
            this.Height = height;
            this.Samples = samples;
        }

        public bool HasRenderBuffer()
        {
            return this.DepthAttachmentId != -1;
        }        
    }
}
