using LibGFX.Core.GameElements;
using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using LibGFX.Physics;
using Newtonsoft.Json.Linq;
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
    public abstract class BaseScene : IIdentifier, ISerialization
    {
        /// <summary>
        /// Gets or sets the name associated with the object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the unique identifier for this instance.
        /// </summary>
        public Guid ID { get; private set; } = Guid.NewGuid();

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
        /// Entries to be enqueued at the end of the update cycle
        /// This allows to safely add elements to the scene without modifying the scene during update or render
        /// </summary>
        public List<EnqueEntry> EnqueEntries { get; set; } = new List<EnqueEntry>();

        /// <summary>
        /// Event that is triggered when an element is enqueued
        /// </summary>
        /// <param name="args"></param>
        public delegate void EnqueEvent(EnqueEventArgs args);

        /// <summary>
        /// Called when the scene is initializing
        /// </summary>
        public abstract event Action<BaseScene, Viewport, IRenderDevice> OnInitStart;

        /// <summary>
        /// Called after the render target for the scene has been created
        /// </summary>
        public abstract event Action<BaseScene, Viewport, IRenderDevice> AfterRenderTargetCreation;

        /// <summary>
        /// Called when the scene has been initialized
        /// </summary>
        public abstract event Action<BaseScene, Viewport, IRenderDevice> OnInitEnd;

        /// <summary>
        /// Called when the shadow pass starts
        /// </summary>
        public abstract event Action<BaseScene, Viewport, IRenderDevice, Camera> OnShadowPassStart;

        /// <summary>
        /// Called when the shadow pass ends
        /// </summary>
        public abstract event Action<BaseScene, Viewport, IRenderDevice, Camera> OnShadowPassEnd;

        /// <summary>
        /// Called immediately on the start of the render process
        /// </summary>
        public abstract event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderStart;

        /// <summary>
        /// Called after light culling has been performed
        /// </summary>
        public abstract event Action<BaseScene, Viewport, IRenderDevice, Camera> AfterLightCulling;

        /// <summary>
        /// Called before the game elements get rendered
        /// </summary>
        public abstract event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderPassBegin;

        /// <summary>
        /// Called when all game elements have been rendered before the render target blitting
        /// </summary>
        public abstract event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderPassEnd;

        /// <summary>
        /// Called at the end of the render process
        /// </summary>
        public abstract event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderEnd;

        /// <summary>
        /// Called when the scene is updating
        /// </summary>
        public abstract event Action<BaseScene, float> OnUpdateStart;

        /// <summary>
        /// Called when the scene has been updated
        /// </summary>
        public abstract event Action<BaseScene, float> OnUpdateEnd;

        /// <summary>
        /// Called when the physics update starts
        /// </summary>
        public abstract event Action<BaseScene, float> OnPhysicsUpdateStart;

        /// <summary>
        /// Called when the physics update ends
        /// </summary>
        public abstract event Action<BaseScene, float> OnPhysicsUpdateEnd;

        /// <summary>
        /// Called before the scene is disposed
        /// </summary>
        public abstract event Action<BaseScene, IRenderDevice> OnDisposeStart;

        /// <summary>
        /// Called when the scene gets disposed
        /// </summary>
        public abstract event Action<BaseScene, IRenderDevice> OnDispose;

        /// <summary>
        /// Called after the scene has been disposed
        /// </summary>
        public abstract event Action<BaseScene, IRenderDevice> OnDisposeEnd;

        /// <summary>
        /// Creates a new scene
        /// </summary>
        protected BaseScene()
        {
            this.Layers = new List<Layer>(); 
            this.RenderStats = new RenderStats();
        }

        /// <summary>
        /// Adds the specified game element to the first available layer.
        /// </summary>
        /// <param name="element">The game element to add. Cannot be null.</param>
        /// <returns>true if the element was successfully added; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="element"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there are no layers available to add the game element.</exception>
        public bool AddGameElement(GameElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (this.Layers.Count == 0)
            {
                throw new InvalidOperationException("No layers available to add the game element.");
            }
            this.Layers[0].Elements.Add(element);
            return true;
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
        /// Adds a light object to the collection of managed lights.
        /// </summary>
        /// <typeparam name="T">The type of light to add. Must derive from <see cref="Light"/>.</typeparam>
        /// <param name="light">The light instance to add. Cannot be null.</param>
        public abstract void AddLight<T>(T light) where T : Light;

        /// <summary>
        /// Retrieves an instance of a light of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of light to retrieve. Must inherit from <see cref="Light"/>.</typeparam>
        /// <returns>An instance of type <typeparamref name="T"/> representing the requested light.</returns>
        public abstract T GetLight<T>() where T : Light;

        /// <summary>
        /// Removes the specified light from the collection of managed lights.
        /// </summary>
        /// <typeparam name="T">The type of light to remove. Must derive from <see cref="Light"/>.</typeparam>
        /// <param name="light">The light instance to remove from the collection. Cannot be null.</param>
        public abstract void RemoveLight<T>(T light) where T : Light;

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
        /// Releases all resources associated with the scene and its layers using the specified renderer.
        /// </summary>
        /// <remarks>After calling this method, the scene's layers are cleared and cannot be used unless
        /// reinitialized.</remarks>
        /// <param name="renderer">The rendering device used to release resources for each layer. Cannot be null.</param>
        public void FreeScene(IRenderDevice renderer)
        {
            foreach (var layer in Layers)
            {
                layer.Dispose(this, renderer);
            }
            this.Layers.Clear();
        }

        /// <summary>
        /// Initializes all elements in the collection using the specified viewport and render device.
        /// </summary>
        /// <param name="viewport">The viewport that defines the rendering area for the elements.</param>
        /// <param name="renderer">The render device used to initialize the elements.</param>
        public void InitializeElements(Viewport viewport, IRenderDevice renderer)
        {
            foreach (var layer in Layers)
            {
                layer.Init(this, viewport, renderer);
            }
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

        /// <summary>
        /// Serializes the current object to a JSON representation using the specified serialization context.
        /// </summary>
        /// <param name="serializationContext">The context that provides configuration and state information for the serialization process.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized JSON representation of the object.</returns>
        /// <exception cref="NotImplementedException">Thrown in all cases as the method is not yet implemented.</exception>
        public virtual JObject Serialize(SerializationContext serializationContext)
        {
            var layerArray = new JArray();
            foreach(var layer in Layers)
            {
                layerArray.Add(layer.Serialize(serializationContext));
            }

            return new JObject
            {
                ["Type"] = this.GetType().FullName,
                ["Name"] = !String.IsNullOrEmpty(Name) ? this.Name : this.ID.ToString(),
                ["ID"] = this.ID.ToString(),
                ["LightManager"] = LightManager.Serialize(serializationContext),
                ["Layers"] = layerArray,
            };
        }

        /// <summary>
        /// Populates the object's properties and layers from the specified JSON object using the provided serialization
        /// context.
        /// </summary>
        /// <remarks>Existing layers are cleared before new layers are deserialized from the JSON data. If
        /// the 'Layers' property is not present or is null, the object's Layers collection will remain empty.</remarks>
        /// <param name="jObject">The JSON object containing the data to deserialize. Must include 'Name', 'ID', and optionally a 'Layers'
        /// array.</param>
        /// <param name="serializationContext">The context used to control serialization and deserialization behavior, such as type resolution or custom
        /// converters.</param>
        public virtual void Deserialize(JObject jObject, SerializationContext serializationContext)
        {
            this.Name = jObject["Name"]?.ToString() ?? String.Empty;
            this.ID = Guid.Parse(jObject["ID"]?.ToString() ?? Guid.NewGuid().ToString());
        }
    }
}
