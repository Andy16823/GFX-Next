using LibGFX.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    public abstract class BaseScene
    {
        public List<Layer> Layers { get; set; }

        protected BaseScene()
        {
            this.Layers = new List<Layer>();    
        }

        public abstract void Init(Viewport viewport, IRenderDevice renderer);
        public abstract void Render(Viewport viewport, IRenderDevice renderer, Camera camera);
        public abstract void Update();
        public abstract void DisposeScene(IRenderDevice renderer);
    }
}
