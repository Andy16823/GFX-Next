using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    /// <summary>
    /// Represents a layer in the scene
    /// </summary>
    public class Layer
    {
        /// <summary>
        /// The name of the layer
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Determines if the layer is visible
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Determines if the layer is enabled
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The elements of the layer
        /// </summary>
        public List<GameElement> Elements { get; set; }

        /// <summary>
        /// Creates a new layer
        /// </summary>
        /// <param name="name"></param>
        public Layer(String name)
        {
            this.Name = name;
            this.Elements = new List<GameElement>();
        }

        /// <summary>
        /// Initializes the layer
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            this.Elements.ForEach(e =>
            {
                e.Init(scene, viewport, renderer);
            });
        }

        /// <summary>
        /// Renders the layer
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public void RenderLayer(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            if (this.Visible)
            {
                this.Elements.ForEach(e => {
                    e.Render(scene, viewport, renderer, camera); 
                });
            }
        }

        /// <summary>
        /// Updates the layer
        /// </summary>
        /// <param name="scene"></param>
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

        /// <summary>
        /// Disposes the layer
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="renderer"></param>
        public void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            this.Elements.ForEach(e =>
            {
                e.Dispose(scene, renderer);
            });
        }

        /// <summary>
        /// Finds an element by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameElement? FindElement(String name)
        {
            return this.Elements.FirstOrDefault(e => e.Name == name);
        }
    }
}
