using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    public class Layer
    {
        public String Name { get; set; }
        public bool Visible { get; set; } = true;
        public bool Enabled { get; set; } = true;
        public List<GameElement> Elements { get; set; }

        public Layer(String name)
        {
            this.Name = name;
            this.Elements = new List<GameElement>();
        }

        public void RenderLayer(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            if (this.Visible)
            {
                this.Elements.ForEach(e => {
                    e.Render(scene, viewport, renderer, camera); 
                });
            }
        }

        public void Update(BaseScene scene)
        {
            if (this.Enabled)
            {
                this.Elements.ForEach(e =>
                {
                    e.Update(scene);
                });
            }
        }

        public void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            this.Elements.ForEach(e =>
            {
                e.Dispose(scene, renderer);
            });
        }

        public GameElement? FindElement(String name)
        {
            return this.Elements.FirstOrDefault(e => e.Name == name);
        }
    }
}
