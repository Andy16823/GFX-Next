using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.PostProcessing
{
    public class PostProcessStack
    {
        public RenderTarget2D RenderTarget => _bufferA;
        public List<IPostProcessFilter> Filter { get; set; }
        private RenderTarget2D _bufferA;

        public PostProcessStack()
        {
            Filter = new List<IPostProcessFilter>();
        }

        public void Init(Viewport viewport, IRenderDevice renderer)
        {
            int width = (int)viewport.Width;
            int height = (int)viewport.Height;
            _bufferA = new RenderTarget2D(width, height);
            _bufferA.Create();
            this.Filter.ForEach(f => f.Init(this, viewport, renderer));
        }

        public void Apply(IRenderDevice renderer, int sourceTexture)
        {
            // Apply each filter in sequence
            int lastTexture = sourceTexture;
            this.Filter.ForEach(f =>
            {
                f.Apply(this, renderer, lastTexture);
                lastTexture = f.RenderTarget.TextureId;
            });

            // Finally, render the result to the main buffer
            renderer.BindRenderTarget(_bufferA);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);
            renderer.DrawRenderTarget(lastTexture, _bufferA.FramebufferId);
            renderer.UnbindRenderTarget();
        }

        public void Dispose(IRenderDevice renderer)
        {
            _bufferA.Dispose();
            this.Filter.ForEach(f => f.Dispose(this, renderer));
        }

        public void SetViewport(Viewport viewport, IRenderDevice renderer)
        {
            int width = (int)viewport.Width;
            int height = (int)viewport.Height;
            _bufferA.Resize(width, height);
            this.Filter.ForEach(f => f.Resize(viewport, renderer));
        }

        public void Update(float deltaTime)
        {
            this.Filter.ForEach(f => f.Update(deltaTime));
        }
    }
}
