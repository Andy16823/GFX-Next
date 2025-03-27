using LibGFX.Graphics;
using LibGFX.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.UI
{
    public abstract class Control
    {
        public String Name { get; set; }
        public Transform Transform { get; set; }
        public RenderTarget RenderTarget { get; set; }

        public abstract void Init(IRenderDevice renderer, Canvas canvas);
        public abstract void Update(Canvas canvas);
        public abstract void Render(IRenderDevice renderer, Canvas canvas);
        public abstract void Dispose(IRenderDevice renderer, Canvas canvas);
    }
}
