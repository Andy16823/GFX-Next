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
        //private RenderTarget2D _bufferB; // Reserved for future use (ping-pong buffering)

        public PostProcessStack()
        {
            Filter = new List<IPostProcessFilter>();
        }

        public void Init(Viewport viewport, IRenderDevice renderer)
        {
            int width = (int)viewport.Width;
            int height = (int)viewport.Height;
            _bufferA = renderer.CreateRenderTarget2D(width, height);
            //_bufferB = renderer.CreateRenderTarget2D(width, height);
            this.Filter.ForEach(f => f.Init(this, viewport, renderer));
        }

        public void Apply(IRenderDevice renderer, int sourceTexture)
        {
            
            renderer.BindRenderTarget(_bufferA);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);

            // Draw render target texture to screen quad with each filter
            renderer.DrawRenderTarget(sourceTexture);

            this.Filter.ForEach(f =>
            {
                f.Apply(this, renderer, sourceTexture);
            });

            renderer.UnbindRenderTarget();
        }

        public void Dispose(IRenderDevice renderer)
        {
            _bufferA.Dispose(renderer);
            //_bufferB.Dispose(renderer);
            this.Filter.ForEach(f => f.Dispose(this, renderer));
        }

        public void SetViewport(Viewport viewport, IRenderDevice renderer)
        {
            int width = (int)viewport.Width;
            int height = (int)viewport.Height;
            renderer.ResizeRenderTarget(_bufferA, width, height);
            //renderer.ResizeRenderTarget(_bufferB, width, height);
        }
    }
}
