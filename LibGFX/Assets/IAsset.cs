using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets
{
    public interface IAsset
    {
        bool IsInitialized { get; }

        void Init(IRenderDevice renderer);

        void Dispose(IRenderDevice renderer);

        void FreeCPUResources();
    }
}
