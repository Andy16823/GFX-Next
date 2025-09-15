using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public interface IRenderTarget
    {
        public abstract int RenderTargetId { get; }
        public void Create(IRenderDevice renderer);
        public void Resize(IRenderDevice renderer, int width, int height);
        public void Dispose(IRenderDevice renderer);
    }
}
