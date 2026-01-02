using LibGFX.Core.GameElements;
using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using LibGFX.Physics;
using Newtonsoft.Json;
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
        /// Gets or sets the collection of entries to be enqueued.
        /// </summary>
        public List<IEnqueEntry> EnqueEntries { get; private set; } = new List<IEnqueEntry>();

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
            this.RenderStats = new RenderStats();
        }

        /// <summary>
        /// Adds a game element to the current collection or scene.
        /// </summary>
        /// <param name="element">The game element to add. Cannot be null.</param>
        public abstract void AddGameElement(GameElement element);

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
        /// Gets all elements in the scene
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<GameElement> GetAllElements();

        /// <summary>
        /// Invokes the specified action for each element in the collection.
        /// </summary>
        /// <remarks>The order in which elements are processed is implementation-dependent. The method
        /// will throw an exception if <paramref name="action"/> is null.</remarks>
        /// <param name="action">The action to perform on each <see cref="GameElement"/> in the collection. Cannot be null.</param>
        public abstract void ForEachElement(Action<GameElement> action);


        /// <summary>
        /// Retrieves the game element associated with the specified unique identifier.
        /// </summary>
        /// <param name="uuid">The unique identifier of the game element to retrieve. Cannot be null or empty.</param>
        /// <returns>The <see cref="GameElement"/> with the specified unique identifier, or <see langword="null"/> if no such
        /// element exists.</returns>
        public abstract GameElement? GetElementByID(String uuid);

        /// <summary>
        /// Finds an element by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract T? FindElement<T>(string name) where T : GameElement;

        /// <summary>
        /// Retrieves a collection of game elements of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of game element to retrieve. Must derive from GameElement.</typeparam>
        /// <returns>A collection containing all game elements of type T. The collection is empty if no elements of the specified
        /// type exist.</returns>
        public abstract ICollection<GameElement> GetElements<T>() where T : GameElement;

        /// <summary>
        /// Finds all elements with a specific behavior
        /// Uses parallel processing to speed up the search
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public abstract ICollection<GameElement> FindElementsWithBehavior<T>() where T : IGameBehavior;

        /// <summary>
        /// Finds all elements with a specific tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public abstract ICollection<GameElement> FindElementsWithTag(String tag);

        /// <summary>
        /// Removes an element from the scene
        /// </summary>
        /// <param name="element"></param>
        public abstract void RemoveElement(GameElement element);

        /// <summary>
        /// Clears all elements in the scene
        /// </summary>
        public abstract void ClearElements();

        /// <summary>
        /// Enqueues elements to be added to the scene at the end of the update cycle
        /// </summary>
        public abstract void EnqueElements();

        /// <summary>
        /// Adds the specified entry to the enqueue queue for processing.
        /// </summary>
        /// <param name="entry">The entry to add to the enqueue queue. Cannot be null.</param>
        public abstract void AddEnqueEntry(IEnqueEntry entry);

        /// <summary>
        /// Releases all resources associated with the scene and its layers using the specified renderer.
        /// </summary>
        /// <remarks>After calling this method, the scene's layers are cleared and cannot be used unless
        /// reinitialized.</remarks>
        /// <param name="renderer">The rendering device used to release resources for each layer. Cannot be null.</param>
        public abstract void FreeScene(IRenderDevice renderer);

        /// <summary>
        /// Initializes all elements in the collection using the specified viewport and render device.
        /// </summary>
        /// <param name="viewport">The viewport that defines the rendering area for the elements.</param>
        /// <param name="renderer">The render device used to initialize the elements.</param>
        public abstract void InitializeElements(Viewport viewport, IRenderDevice renderer);

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
        public virtual void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            // Start writing the JSON object
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(this.GetType().FullName);
            writer.WritePropertyName("Name");
            writer.WriteValue(!String.IsNullOrEmpty(Name) ? this.Name : this.ID.ToString());
            writer.WritePropertyName("ID");
            writer.WriteValue(this.ID.ToString());
            writer.WritePropertyName("LightManager");
            LightManager.Serialize(writer, serializationContext);

            // Invoke the callback for additional serialization
            callback?.Invoke(writer);

            // End writing the JSON object
            writer.WriteEndObject();
        }

        /// <summary>
        /// Deserializes the object from the provided JSON representation using the specified serialization context.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializationContext"></param>
        /// <param name="callback"></param>
        public virtual void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            // Basic properties
            this.Name = obj.Value<string>("Name") ?? this.Name;
            this.ID = Guid.Parse(obj.Value<string>("ID") ?? this.ID.ToString());

            // Light manager
            this.LightManager.Deserialize(obj.Value<JObject>("LightManager")!, serializationContext);

            // Invoke the callback for additional deserialization
            callback?.Invoke(obj);
        }
    }
}
