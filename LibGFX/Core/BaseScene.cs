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

        public virtual Layer? FindLayer(string name)
        {
            return this.Layers.FirstOrDefault(layer => layer.Name == name);
        }

        public virtual GameElement? FindElement(string name)
        {
            foreach (var layer in Layers)
            {
                var element = layer.FindElement(name);
                if (element != null)
                {  
                    return element; 
                }
            }
            return null;
        }

        public virtual GameElement? FindElement(string layerName, string name)
        {
            var layer = this.FindLayer(layerName);
            if(layer != null)
            {
                var element = layer.FindElement(name);
                if(element != null)
                {
                    return element;
                }
            }
            return null;
        }

        public abstract void Init(Viewport viewport, IRenderDevice renderer);
        public abstract void Render(Viewport viewport, IRenderDevice renderer, Camera camera);
        public abstract void Update();
        public abstract void DisposeScene(IRenderDevice renderer);
    }
}
