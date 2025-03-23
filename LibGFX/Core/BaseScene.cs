using LibGFX.Graphics;
using LibGFX.Pyhsics;
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
        /// <summary>
        /// The Layers of the scene
        /// </summary>
        public List<Layer> Layers { get; set; }

        /// <summary>
        /// The physics handler of the scene
        /// </summary>
        public PhysicsHandler PhysicsHandler { get; set; }

        /// <summary>
        /// Creates a new scene
        /// </summary>
        protected BaseScene()
        {
            this.Layers = new List<Layer>();    
        }

        /// <summary>
        /// Finds a layer by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual Layer? FindLayer(string name)
        {
            return this.Layers.FirstOrDefault(layer => layer.Name == name);
        }

        /// <summary>
        /// Finds an element by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Finds an element by name and layer name
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Initializes the scene
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public abstract void Init(Viewport viewport, IRenderDevice renderer);

        /// <summary>
        /// Renders the scene
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public abstract void Render(Viewport viewport, IRenderDevice renderer, Camera camera);

        /// <summary>
        /// Updates the scene
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Disposes the scene
        /// </summary>
        /// <param name="renderer"></param>
        public abstract void DisposeScene(IRenderDevice renderer);

        /// <summary>
        /// Updates the physics of the scene
        /// </summary>
        public abstract void UpdatePhysics();
    }
}
