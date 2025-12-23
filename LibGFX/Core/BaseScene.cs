using LibGFX.Core.GameElements;
using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using LibGFX.Physics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LibGFX.Core.BaseScene;

namespace LibGFX.Core
{
    /// <summary>
    /// Event args for the enque event
    /// </summary>
    public struct EnqueEventArgs
    {
        public BaseScene Scene { get; set; }
        public GameElement Element { get; set; }
        public Dictionary<string, object>? ExtraData { get; set; }
    }

    /// <summary>
    /// Entry for the enque system
    /// </summary>
    public struct EnqueEntry
    {
        public string LayerName { get; set; }
        public GameElement Element { get; set; }
        public EnqueEvent Event { get; set; }
        public Dictionary<string, object>? ExtraData { get; set; }
    }

    /// <summary>
    /// Base class for creating a scene
    /// </summary>
    public abstract class BaseScene
    {
        /// <summary>
        /// Gets the render target associated with this instance.
        /// </summary>
        public abstract IRenderTarget RenderTarget { get; }

        /// <summary>
        /// The Layers of the scene
        /// </summary>
        public virtual List<Layer> Layers { get; set; }

        /// <summary>
        /// The physics handler of the scene
        /// </summary>
        public virtual PhysicsHandler PhysicsHandler { get; set; }

        /// <summary>
        /// The light handler of the scene
        /// </summary>
        public virtual ILightManager LightManager { get; set; }

        /// <summary>
        /// The render stats of the scene. 
        /// It contain the fps, delta time.
        /// </summary>
        public virtual RenderStats RenderStats { get; set; }

        /// <summary>
        /// List of scene behaviors
        /// </summary>
        public List<ISceneBehavior> SceneBehaviors { get; set; }

        /// <summary>
        /// Entries to be enqueued at the end of the update cycle
        /// This allows to safely add elements to the scene without modifying the scene during update or render
        /// </summary>
        public List<EnqueEntry> EnqueEntries { get; set; } = new List<EnqueEntry>();

        /// <summary>
        /// Event that is triggered when an element is enqueued
        /// </summary>
        /// <param name="args"></param>
        public delegate void EnqueEvent(EnqueEventArgs args);

        private readonly List<Action<Viewport, IRenderDevice, Camera>> _renderActions = new();

        /// <summary>
        /// Creates a new scene
        /// </summary>
        protected BaseScene()
        {
            this.Layers = new List<Layer>(); 
            this.RenderStats = new RenderStats();
            this.SceneBehaviors = new List<ISceneBehavior>();
        }

        /// <summary>
        /// Adds a render action to be performed during the render phase
        /// </summary>
        /// <param name="action"></param>
        public void AddRenderAction(Action<Viewport, IRenderDevice, Camera> action)
        {
            _renderActions.Add(action);
        }

        /// <summary>
        /// Processes and executes all render actions added to the scene and then clears the list
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public void ProcessRenderActions(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            foreach (var action in _renderActions)
            {
                action(viewport, renderer, camera);
            }
            _renderActions.Clear();
        }

        /// <summary>
        /// Tries to add a layer to the scene
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool AddGameElement(String layerName, GameElement element)
        {
            if(element == null || String.IsNullOrEmpty(layerName))
            {
                return false;
            }

            var layer = this.FindLayer(layerName);
            if (layer != null)
            {
                layer.Elements.Add(element); 
                return true;
            }

            return false;
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
        /// Gets all elements in the scene
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GameElement> GetAllElements()
        {
            List<GameElement> elements = new List<GameElement>();
            foreach (var layer in Layers)
            {
                elements.AddRange(layer.Elements);
            }
            return elements;
        }

        /// <summary>
        /// Executes an action for each element in the scene
        /// </summary>
        /// <param name="action"></param>
        public void ForEachElement(Action<GameElement> action)
        {
            foreach (var layer in Layers)
            {
                foreach (var element in layer.Elements)
                {
                    action(element);
                }
            }
        }

        /// <summary>
        /// Finds an element by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GameElement? GetElementByID(String id)
        {
            foreach (var layer in Layers)
            {
                var element = layer.FindElementByID(id);
                if (element != null)
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds an element by ID where the ID is an integer hash code of the GUID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GameElement? GetElementByID(int id)
        {
            foreach (var layer in Layers)
            {
                var element = layer.FindElementByID(id);
                if (element != null)
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds an element by a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public GameElement? FindElement(Func<GameElement, bool> predicate)
        {
            foreach (var layer in Layers)
            {
                var element = layer.FindElement(predicate);
                if (element != null)
                {
                    return element;
                }
            }
            return null;
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
                return layer.FindElement(name);
            }
            return null;
        }

        /// <summary>
        /// Finds all elements with a specific type
        /// Uses parallel processing to speed up the search
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual ICollection<GameElement> FindElements<T>() where T : GameElement
        {
            List<GameElement> elements = new List<GameElement>();
            this.Layers.AsParallel().ForAll(layer =>
            {
                elements.AddRange(layer.FindElements<T>());
            });
            return elements;
        }

        /// <summary>
        /// Finds all elements that match a specific predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual ICollection<GameElement> FindElements(Func<GameElement, bool> predicate)
        {
            List<GameElement> elements = new List<GameElement>();
            this.Layers.AsParallel().ForAll(layer =>
            {
                elements.AddRange(layer.FindElements(predicate));
            });
            return elements;
        }

        /// <summary>
        /// Finds all elements with a specific behavior
        /// Uses parallel processing to speed up the search
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual ICollection<GameElement> FindElementsWithBehaviors<T>() where T : IGameBehavior
        {
            List<GameElement> elements = new List<GameElement>();

            this.Layers.AsParallel().ForAll(layer =>
            {
                elements.AddRange(layer.FindElementsWithBehaviors<T>());
            });

            return elements;
        }

        /// <summary>
        /// Finds all elements with a specific tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public virtual ICollection<GameElement> FindElementsWithTag(String tag)
        {
            List<GameElement> elements = new List<GameElement>();
            this.Layers.AsParallel().ForAll(layer =>
            {
                elements.AddRange(layer.FindElementsWithTag(tag));
            });
            return elements;
        }

        /// <summary>
        /// Removes an element from the scene
        /// </summary>
        /// <param name="element"></param>
        public virtual void RemoveElement(GameElement element)
        {
            this.Layers.AsParallel().ForAll(layer =>
            {
                layer.TryRemoveElement(element);
            });
        }

        /// <summary>
        /// Clears all elements in the scene
        /// </summary>
        public virtual void ClearElements()
        {
            foreach (var layer in this.Layers)
            {
                layer.Elements.Clear();
            }
        }

        /// <summary>
        /// Adds a scene behavior to the scene
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public virtual T AddSceneBehavior<T>(ISceneBehavior behavior) where T : ISceneBehavior
        {
            this.SceneBehaviors.Add(behavior);
            return (T)behavior;
        }

        /// <summary>
        /// Gets a scene behavior from the scene
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T? GetSceneBehavior<T>() where T : ISceneBehavior
        {
            return this.SceneBehaviors.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Removes a scene behavior from the scene
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public virtual void RemoveSceneBehavior<T>() where T : ISceneBehavior
        {
            var behavior = this.GetSceneBehavior<T>();
            if (behavior != null)
            {
                this.SceneBehaviors.Remove(behavior);
            }
        }

        /// <summary>
        /// Enqueues elements to be added to the scene at the end of the update cycle
        /// </summary>
        public void EnqueElements()
        {
            foreach (var entry in EnqueEntries)
            {
                // Skip null elements
                if (entry.Element == null)
                {
                    continue;
                }
                // Trigger the event before adding the element to the scene
                entry.Event?.Invoke(new EnqueEventArgs() { Scene = this, Element = entry.Element, ExtraData = entry.ExtraData });

                // Add the element to the scene
                if (!string.IsNullOrEmpty(entry.LayerName) && entry.Element != null)
                {
                    AddGameElement(entry.LayerName, entry.Element);
                }
            }
            // Clear the entries after processing
            EnqueEntries.Clear();
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
        /// Renders the shadow maps for the scene
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public abstract void RenderShadowMaps(Viewport viewport, IRenderDevice renderer, Camera camera);

        /// <summary>
        /// Updates the scene
        /// </summary>
        public abstract void Update(float dt);

        /// <summary>
        /// Disposes the scene
        /// </summary>
        /// <param name="renderer"></param>
        public abstract void DisposeScene(IRenderDevice renderer);

        /// <summary>
        /// Updates the physics of the scene
        /// </summary>
        public abstract void UpdatePhysics(float dt);
    }
}
