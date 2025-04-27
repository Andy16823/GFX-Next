using LibGFX.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LibGFX.Graphics.Enviroment
{
    public interface IEnviroment
    {
        public Transform Transform { get; set; }
        public void Init(IRenderDevice renderer);
        public void Render(IRenderDevice renderer, Camera camera, Viewport viewport);
        public void Dispose(IRenderDevice renderer);
    }
}
