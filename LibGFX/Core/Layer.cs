using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                    if (e.Visible)
                    {
                        e.Render(scene, viewport, renderer, camera);
                    }
                });
            }
        }

        /// <summary>
        /// Renders the shadows of the layer
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public void RenderShadows(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            if (this.Visible)
            {
                this.Elements.ForEach(e =>
                {
                    if (e.CastShadows && e.Visible)
                    {
                        e.RenderShadow(scene, viewport, renderer);
                    }
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

        /// <summary>
        /// Finds an element by ID using its hash code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GameElement? FindElementByID(int id)
        {
            return this.Elements.FirstOrDefault(e => e.ID.GetHashCode() == id);
        }

        /// <summary>
        /// Finds an element by ID as a string representation of the GUID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GameElement? FindElementByID(String id)
        {
            return this.Elements.FirstOrDefault(e => e.ID.ToString() == id);
        }

        /// <summary>
        /// Finds an element by a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public GameElement? FindElement(Func<GameElement, bool> predicate)
        {
            return this.Elements.FirstOrDefault(predicate);
        }

        /// <summary>
        /// Finds an element by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GameElement? FindElement(Guid id)
        {
            return this.Elements.FirstOrDefault(e => e.ID == id);
        }

        /// <summary>
        /// Finds all elements with a specific tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public ICollection<GameElement> FindElementsWithTag(String tag)
        {
            return this.Elements.Where(e => e.Tags.Contains(tag)).ToList();
        }

        /// <summary>
        /// Finds all elements that match a specific predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public ICollection<GameElement> FindElements(Func<GameElement, bool> predicate)
        {
            return this.Elements.Where(predicate).ToList();
        }

        /// <summary>
        /// Finds all elements with a specific behavior type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ICollection<GameElement> FindElementsWithBehaviors<T>() where T : IGameBehavior
        {
            return this.Elements.Where(e => e.Behaviors.Any(b => b is T)).ToList();
        }

        /// <summary>
        /// Finds all elements of a specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ICollection<GameElement> FindElements<T>() where T : GameElement
        {
            return this.Elements.Where(e => e is T).ToList();
        }

        /// <summary>
        /// Tries to add an element to the layer
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool TryRemoveElement(GameElement element)
        {
            if (this.Elements.Contains(element))
            {
                this.Elements.Remove(element);
                return true;
            }
            return false;
        }
    }
}
