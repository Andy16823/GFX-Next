using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.RenderTarget
{
    public class RenderTargetScope : IDisposable
    {
        private readonly IRenderDevice _renderDevice;
        private int _previousFramebuffer;
        private bool _disposed;

        public RenderTargetScope(IRenderTarget renderTarget, IRenderDevice renderDevice)
        {
            _renderDevice = renderDevice;
            _previousFramebuffer = _renderDevice.GetCurrentRenderTargetID();
            _renderDevice.BindRenderTarget(renderTarget);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _renderDevice.BindRenderTarget(_previousFramebuffer);
                _disposed = true;
            }
        }
    }
}
