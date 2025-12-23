using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.PostProcessing
{
    public interface IPostProcessFilter
    {
        RenderTarget2D RenderTarget { get; }
        void Apply(PostProcessStack stack, IRenderDevice renderer, int sourceTexture);
        void Init(PostProcessStack stack, Viewport viewport, IRenderDevice renderer);
        void Dispose(PostProcessStack stack, IRenderDevice renderer);
        void Resize(Viewport viewport, IRenderDevice renderer);
    }
}
