using LibGFX.Assets;
using LibGFX.Core;
using LibGFX.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LibGFX.Graphics.Enviroment
{
    public interface IEnviroment : IAsset, ISerialization
    {
        public Transform Transform { get; set; }
        public void Render(IRenderDevice renderer, Camera camera, Viewport viewport);
    }
}
