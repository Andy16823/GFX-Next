using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Lights
{
    public interface ILightManager
    {
        public void Init(IRenderDevice renderDevice);
        public void BindLights(Viewport viewport, IRenderDevice renderer, Camera camera);
        public void Dispose(IRenderDevice renderDevice);
        public int GetLightCount<T>() where T : Light;
    }
}
