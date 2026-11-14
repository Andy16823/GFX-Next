using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Materials
{
    public class MaterialScope : IDisposable
    {
        private readonly IMaterial _material;
        private readonly IRenderDevice _renderDevice;
        private bool _disposed;

        public MaterialScope(IMaterial material, IRenderDevice renderDevice)
        {
            _material = material;
            _renderDevice = renderDevice;
            _material.Use(_renderDevice);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _material.Disable(_renderDevice);
                _disposed = true;
            }
        }
    }
}
