using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    /// <summary>
    /// Represents an entry for enqueuing a 2D scene element, including the element, its target layer, an optional
    /// action to perform during enqueuing, and any additional data.
    /// </summary>
    public class EnqueScene2DEntry : IEnqueEntry
    {
        public GameElement Element { get; set; }
        public String LayerName { get; set; }
        public Action<BaseScene, GameElement, Dictionary<string, object>>? EnqueAction { get; set; }
        public Dictionary<string, object>? ExtraData { get; set; }
    }

    /// <summary>
    /// Represents a 2D scene
    /// </summary>
    public class Scene2D : BaseScene
    {
        /// <summary>
        /// Gets the render target associated with this instance.
        /// </summary>
        public override IRenderTarget RenderTarget { get => _renderTarget; }

        /// <summary>
        /// Gets or sets the collection of layers contained in the model.
        /// </summary>
        /// <remarks>Layers are typically processed in the order they appear in the collection. Modifying
        /// this list will affect the structure and behavior of the model.</remarks>
        public List<Layer> Layers { get; set; } = new List<Layer>();

        /// <summary>
        /// Sets the main light manager for the scene
        /// </summary>
        public DirectionalLight2D SceneLight { get => _lightManager.DirectionalLight; set => _lightManager.DirectionalLight = value; }

        /// <summary>
        /// The light manager of the scene
        /// </summary>
        public override ILightManager LightManager { get => _lightManager; }

        /// <summary>
        /// The light manager for 2D lights
        /// </summary>
        private Light2DManager _lightManager;

        private RenderTarget2D _renderTarget;
        private float _physicsAccumulator = 0.0f;

        // Init events
        public override event Action<BaseScene, Viewport, IRenderDevice> OnInitStart;
        public override event Action<BaseScene, Viewport, IRenderDevice> AfterRenderTargetCreation;
        public override event Action<BaseScene, Viewport, IRenderDevice> OnInitEnd;

        // Render events
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnShadowPassStart;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnShadowPassEnd;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderStart;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> AfterLightCulling;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderPassBegin;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderPassEnd;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderEnd;

        // Update events
        public override event Action<BaseScene, float> OnUpdateStart;
        public override event Action<BaseScene, float> OnUpdateEnd;

        // Physics events
        public override event Action<BaseScene, float> OnPhysicsUpdateStart;
        public override event Action<BaseScene, float> OnPhysicsUpdateEnd;

        // Dispose events
        public override event Action<BaseScene, IRenderDevice> OnDisposeStart;
        public override event Action<BaseScene, IRenderDevice> OnDispose;
        public override event Action<BaseScene, IRenderDevice> OnDisposeEnd;


        /// <summary>
        /// Creates a new 2D scene
        /// </summary>
        public Scene2D() : base()
        {
            _lightManager = new Light2DManager();
        }

        /// <summary>
        /// Creates a new 2D scene with the given layers
        /// </summary>
        /// <param name="layers"></param>
        public Scene2D(params String[] layers) : base()
        {
            _lightManager = new Light2DManager();
            foreach (var item in layers)
            {
                this.Layers.Add(new Layer(item));
            }
        }

        /// <summary>
        /// Searches for a layer with the specified name and returns the first matching layer, if found.
        /// </summary>
        /// <param name="name">The name of the layer to locate. The comparison is case-sensitive.</param>
        /// <returns>The first <see cref="Layer"/> whose <c>Name</c> property matches the specified name, or <see
        /// langword="null"/> if no such layer exists.</returns>
        public Layer? FindLayer(string name)
        {
            return this.Layers.FirstOrDefault(layer => layer.Name == name);
        }

        /// <summary>
        /// Adds a game element to the first layer of the collection, creating a default layer if none exist.
        /// </summary>
        /// <remarks>If no layers are present, a new layer named "Default" is created before adding the
        /// element. The element is always added to the first layer in the collection.</remarks>
        /// <param name="element">The game element to add to the collection. If null, the method performs no action.</param>
        public override void AddGameElement(GameElement element)
        {
            if (element == null) return;
            if (this.Layers.Count == 0)
            {
                this.Layers.Add(new Layer("Default"));
            }
            this.Layers[0].Elements.Add(element);
        }

        /// <summary>
        /// Attempts to add a game element to the specified layer.
        /// </summary>
        /// <param name="layerName">The name of the layer to which the game element will be added. Cannot be null or empty.</param>
        /// <param name="element">The game element to add to the layer. Cannot be null.</param>
        /// <returns>true if the element was successfully added to the specified layer; otherwise, false.</returns>
        public bool AddGameElement(String layerName, GameElement element)
        {
            if (element == null || String.IsNullOrEmpty(layerName))
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
        /// Returns a collection containing all game elements from all layers in the order they appear.
        /// </summary>
        /// <returns>An enumerable collection of all <see cref="GameElement"/> instances contained in every layer. The collection
        /// will be empty if there are no elements in any layer.</returns>
        public override IEnumerable<GameElement> GetAllElements()
        {
            List<GameElement> elements = new List<GameElement>();
            foreach (var layer in Layers)
            {
                elements.AddRange(layer.Elements);
            }
            return elements;
        }

        /// <summary>
        /// Retrieves the first game element with the specified unique identifier from all layers.
        /// </summary>
        /// <remarks>Searches each layer in order and returns the first matching element. If multiple
        /// elements share the same identifier across layers, only the first encountered is returned.</remarks>
        /// <param name="uuid">The unique identifier of the game element to locate. Cannot be null.</param>
        /// <returns>The first <see cref="GameElement"/> with the specified identifier, or <see langword="null"/> if no matching
        /// element is found.</returns>
        public override GameElement? GetElementByID(string uuid)
        {
            foreach (var layer in Layers)
            {
                var element = layer.FindElementByID(uuid);
                if (element != null)
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>
        /// Searches all layers for a game element with the specified name and returns the first match found.
        /// </summary>
        /// <remarks>The search is performed in the order of the layers as they appear in the collection.
        /// If multiple elements share the same name across different layers, only the first match is
        /// returned.</remarks>
        /// <param name="name">The name of the game element to locate. The search is case-sensitive and matches the element's name exactly.</param>
        /// <returns>The first <see cref="GameElement"/> with the specified name, or <see langword="null"/> if no matching
        /// element is found.</returns>
        public override T? FindElement<T>(string name) where T : class
        {
            foreach (var layer in Layers)
            {
                var element = layer.FindElement<T>(name);
                if (element != null)
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves all game elements of the specified type from all layers.
        /// </summary>
        /// <remarks>This method searches each layer in parallel and aggregates the results. The returned
        /// collection is not thread-safe.</remarks>
        /// <typeparam name="T">The type of game elements to retrieve.</typeparam>
        /// <returns>A collection containing all elements of type T found in all layers. The collection is empty if no matching
        /// elements are found.</returns>
        public override ICollection<GameElement> GetElements<T>()
        {
            List<GameElement> elements = new List<GameElement>();
            this.Layers.AsParallel().ForAll(layer =>
            {
                elements.AddRange(layer.GetElements<T>());
            });
            return elements;
        }

        /// <summary>
        /// Finds all game elements that implement the specified behavior type.
        /// </summary>
        /// <typeparam name="T">The type of behavior to search for. Must be implemented by the game elements to be included in the results.</typeparam>
        /// <returns>A collection of game elements that implement the specified behavior type. The collection is empty if no
        /// elements with the behavior are found.</returns>
        public override ICollection<GameElement> FindElementsWithBehavior<T>()
        {
            List<GameElement> elements = new List<GameElement>();

            this.Layers.AsParallel().ForAll(layer =>
            {
                elements.AddRange(layer.FindElementsWithBehavior<T>());
            });

            return elements;
        }

        /// <summary>
        /// Finds all game elements that have the specified tag across all layers.
        /// </summary>
        /// <remarks>This method searches all layers in parallel to improve performance when working with
        /// a large number of layers. The returned collection is not thread-safe.</remarks>
        /// <param name="tag">The tag to search for. Only elements with this tag will be included in the results. Cannot be null.</param>
        /// <returns>A collection of game elements that have the specified tag. The collection is empty if no elements with the
        /// tag are found.</returns>
        public override ICollection<GameElement> FindElementsWithTag(string tag)
        {
            return this.Layers.AsParallel().SelectMany(layer => layer.FindElementsWithTag(tag)).ToList();
        }

        /// <summary>
        /// Removes the specified game element from all layers in the collection.
        /// </summary>
        /// <remarks>This method attempts to remove the element from every layer in parallel. If the
        /// element does not exist in a layer, that layer is unaffected. This operation is thread-safe.</remarks>
        /// <param name="element">The game element to remove from each layer. Cannot be null.</param>
        public override void RemoveElement(GameElement element)
        {
            this.Layers.AsParallel().ForAll(layer =>
            {
                layer.TryRemoveElement(element);
            });
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public override void ClearElements()
        {
            foreach (var layer in this.Layers)
            {
                layer.Elements.Clear();
            }
        }

        /// <summary>
        /// Invokes the specified action for each game element contained in all layers.
        /// </summary>
        /// <remarks>The action is invoked once for every element in every layer, in the order they are
        /// stored. If the collection of layers or elements is modified during iteration, the behavior is
        /// undefined.</remarks>
        /// <param name="action">The action to perform on each <see cref="GameElement"/>. Cannot be null.</param>
        public override void ForEachElement(Action<GameElement> action)
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
        /// Adds all queued 2D scene elements to their designated layers and triggers any associated actions before
        /// insertion.
        /// </summary>
        /// <remarks>This method processes each entry in the queue, invoking any specified actions prior
        /// to adding the element to the scene. After all elements have been enqueued, the queue is cleared. Elements
        /// with a null reference are skipped and not added to the scene.</remarks>
        public override void EnqueElements()
        {
            foreach (EnqueScene2DEntry entry in EnqueEntries)
            {
                // Skip null elements
                if (entry.Element == null)
                {
                    continue;
                }

                // Trigger the event before adding the element to the scene
                entry.EnqueAction?.Invoke(this, entry.Element, entry.ExtraData ?? new Dictionary<string, object>());

                // Add the element to the scene
                if (!string.IsNullOrEmpty(entry.LayerName) && entry.Element != null)
                {
                    AddGameElement(entry.LayerName, entry.Element);
                }
            }
            EnqueEntries.Clear();
        }

        /// <summary>
        /// Adds an enque entry to the collection if it is of type EnqueScene2DEntry.
        /// </summary>
        /// <remarks>Only entries of type EnqueScene2DEntry are supported by this method. Attempting to
        /// add an entry of a different type will result in an exception.</remarks>
        /// <param name="entry">The enque entry to add. Must be an instance of EnqueScene2DEntry.</param>
        /// <exception cref="ArgumentException">Thrown if entry is not of type EnqueScene2DEntry.</exception>
        public override void AddEnqueEntry(IEnqueEntry entry)
        {
            if(entry is EnqueScene2DEntry sceneEntry)
            {
                EnqueEntries.Add(sceneEntry);
            }
            else
            {
                throw new ArgumentException("Invalid enque entry type for Scene2D. Only EnqueScene2DEntry is supported.");
            }
        }

        /// <summary>
        /// Releases all resources associated with the scene and its layers using the specified render device.
        /// </summary>
        /// <remarks>After calling this method, the scene's layers are cleared and should not be used
        /// further. This method should be called when the scene is no longer needed to ensure proper resource
        /// cleanup.</remarks>
        /// <param name="renderer">The render device to use when releasing resources for each layer. Cannot be null.</param>
        public override void FreeScene(IRenderDevice renderer)
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
        /// <param name="viewport">The viewport that defines the rendering context for the elements.</param>
        /// <param name="renderer">The render device used to initialize the elements for drawing operations.</param>
        public override void InitializeElements(Viewport viewport, IRenderDevice renderer)
        {
            foreach (var layer in Layers)
            {
                layer.Init(this, viewport, renderer);
            }
        }

        /// <summary>
        /// Adds a light to the 2D scene. Supports adding either a directional light or a point light.
        /// </summary>
        /// <remarks>If a DirectionalLight2D is provided, it replaces the current directional light in the
        /// scene. If a PointLight2D is provided, it is added to the collection of point lights managed by the
        /// scene.</remarks>
        /// <typeparam name="T">The type of the light to add. Must be either DirectionalLight2D or PointLight2D.</typeparam>
        /// <param name="light">The light instance to add to the scene. Must be of type DirectionalLight2D or PointLight2D.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="light"/> is not of type DirectionalLight2D or PointLight2D.</exception>
        public override void AddLight<T>(T light)
        {
            if(light is DirectionalLight2D dirLight)
            {
                _lightManager.DirectionalLight = dirLight;
            }
            else if (light is PointLight2D pointLight)
            {
                _lightManager.AddPointLight(pointLight);
            }
            else
            {
                throw new ArgumentException("Invalid light type for Scene2D. Only PointLight2D is supported.");
            }
        }

        /// <summary>
        /// Retrieves the directional light for the 2D scene if the specified type is supported.
        /// </summary>
        /// <remarks>Only <see cref="DirectionalLight2D"/> is supported for 2D scenes. Attempting to
        /// retrieve any other light type will result in an exception.</remarks>
        /// <typeparam name="T">The type of light to retrieve. Must be <see cref="DirectionalLight2D"/>.</typeparam>
        /// <returns>The directional light instance cast to the specified type <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if <typeparamref name="T"/> is not <see cref="DirectionalLight2D"/>.</exception>
        public override T GetLight<T>()
        {
            if(typeof(T) == typeof(DirectionalLight2D))
            {
                return (T)(object)_lightManager.DirectionalLight;
            }
            else
            {
                throw new ArgumentException("Invalid light type for Scene2D. Only DirectionalLight2D is supported.");
            }
        }

        /// <summary>
        /// Removes the specified light from the scene. Supports removal of directional and point lights.
        /// </summary>
        /// <remarks>If a DirectionalLight2D is specified, it will be unset from the scene. If a
        /// PointLight2D is specified, it will be removed from the collection of point lights. Other light types are not
        /// supported and will result in an exception.</remarks>
        /// <typeparam name="T">The type of the light to remove. Must be either DirectionalLight2D or PointLight2D.</typeparam>
        /// <param name="light">The light instance to remove from the scene. Must be a DirectionalLight2D or PointLight2D object.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="light"/> is not a DirectionalLight2D or PointLight2D.</exception>
        public override void RemoveLight<T>(T light)
        {
            if(light is DirectionalLight2D)
            {
                _lightManager.DirectionalLight = null;
            }
            else if (light is PointLight2D pointLight)
            {
                _lightManager.RemovePointLight(pointLight);
            }
            else
            {
                throw new ArgumentException("Invalid light type for Scene2D. Only PointLight2D is supported.");
            }
        }

        /// <summary>
        /// Initializes the scene
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void Init(Viewport viewport, IRenderDevice renderer)
        {
            // Call the on init start event
            OnInitStart?.Invoke(this, viewport, renderer);

            _renderTarget = new RenderTarget2D(viewport.Width, viewport.Height);
            _renderTarget.Create();

            // Call the after render target creation event
            AfterRenderTargetCreation?.Invoke(this, viewport, renderer);

            // Iinitialize the layers of the scene
            this.Layers.ForEach(l =>
            {
                l.Init(this, viewport, renderer);
            });

            // Initialize the light manager
            this.LightManager.Init(renderer);

            // Call the on init end event
            OnInitEnd?.Invoke(this, viewport, renderer);

            // OnStart the render stats for the scene
            this.RenderStats.Start();
        }

        /// <summary>
        /// Renders the scene
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void Render(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            // Start a new frame for the render stats
            this.RenderStats.NewFrame();

            // Call the before render behaviors for the scene
            OnRenderStart?.Invoke(this, viewport, renderer, camera);

            if (this.LightManager != null)
            {
                this.LightManager.CullLights(viewport, renderer, camera);
            }

            // Call the after light culling event
            AfterLightCulling?.Invoke(this, viewport, renderer, camera);

            var depthTest = renderer.IsDepthTestEnabled();

            renderer.DisableDepthTest();
            renderer.SetViewport(viewport);
            renderer.SetProjectionMatrix(camera.GetProjectionMatrix(viewport));
            renderer.SetViewMatrix(camera.GetViewMatrix());

            _renderTarget.Resize(viewport.Width, viewport.Height);
            renderer.BindRenderTarget(_renderTarget);
            renderer.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);

            // Call the on render pass begin event
            OnRenderPassBegin?.Invoke(this, viewport, renderer, camera);

            this.Layers.ForEach(layer => { 
                layer.RenderLayer(this, viewport, renderer, camera); 
            });

            if (this.PhysicsHandler.DebugPhysics)
            {
                if (this.PhysicsHandler.HasDebugDrawer())
                {
                    this.PhysicsHandler.DebugDraw(renderer);
                }
                else
                {
                    Debug.Assert(this.PhysicsHandler.DebugPhysics, "DebugPhysics is enabled but no debug drawer is set");
                }
            }

            // Call the on render pass end event
            OnRenderPassEnd?.Invoke(this, viewport, renderer, camera);

            renderer.UnbindRenderTarget();
            renderer.SetDepthTest(depthTest);

            // Call the on render end event
            OnRenderEnd?.Invoke(this, viewport, renderer, camera);
        }

        public override void BuildShadowMaps(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            throw new NotImplementedException("Shadow mapping is not implemented for 2D scenes.");
        }

        /// <summary>
        /// Updates the scene
        /// </summary>
        public override void Update(float dt)
        {
            // Call the before update behaviors for the scene
            OnUpdateStart?.Invoke(this, dt);

            // Update the scene
            this.Layers.ForEach(l => { 
                l.Update(this, dt); 
            });

            // Call the after update behaviors for the scene
            OnUpdateEnd?.Invoke(this, dt);
        }

        /// <summary>
        /// Disposes the scene
        /// </summary>
        /// <param name="renderer"></param>
        public override void DisposeScene(IRenderDevice renderer)
        {
            // Call the on dispose start event
            OnDisposeStart?.Invoke(this, renderer);

            // Dispose the layers of the scene
            this.Layers.ForEach(l =>
            {
                l.Dispose(this, renderer);
            });

            // Call the on dispose event
            OnDispose?.Invoke(this, renderer);

            // Dispose the render target of the scene
            _renderTarget.Dispose();

            // Dispose the light manager
            this.LightManager.Dispose(renderer);

            // Call the on dispose end event
            OnDisposeEnd?.Invoke(this, renderer);
        }

        /// <summary>
        /// Updates the physics of the scene
        /// </summary>
        public override void UpdatePhysics(float dt)
        {
            if (this.PhysicsHandler == null) return;
            _physicsAccumulator += dt;

            while (_physicsAccumulator >= this.PhysicsHandler.FixedTimeStep)
            {
                // Before physics update behaviors
                OnPhysicsUpdateStart?.Invoke(this, dt);

                // Process the physics handler
                this.PhysicsHandler.Process(this);
                _physicsAccumulator -= this.PhysicsHandler.FixedTimeStep;

                // After physics update behaviors
                OnPhysicsUpdateEnd?.Invoke(this, dt);
            }
        }

        /// <summary>
        /// Creates a new 2D scene with default settings.
        /// </summary>
        /// <returns>A <see cref="Scene2D"/> instance initialized with default parameters.</returns>
        public static Scene2D CreateDefaultScene()
        {
            var scene = new Scene2D("Default");
            return scene;
        }

        /// <summary>
        /// Serializes the scene to a JSON object
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="serializationContext"></param>
        /// <param name="callback"></param>
        public override void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            base.Serialize(writer, serializationContext, (w) =>
            {
                w.WritePropertyName("Layers");
                w.WriteStartArray();
                foreach (var layer in Layers)
                {
                    layer.Serialize(w, serializationContext);
                }
                w.WriteEndArray();
            });
        }

        /// <summary>
        /// Deserializes the scene from a JSON object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializationContext"></param>
        /// <param name="callback"></param>
        public override void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            base.Deserialize(obj, serializationContext, (refObj) =>
            {
                // Deserialize the light manager
                var lightManagerToken = refObj.Value<JObject>("LightManager");
                if (lightManagerToken != null)
                {
                    this.LightManager = new Light2DManager();
                    this.LightManager.Deserialize(lightManagerToken, serializationContext);
                }

                // Deserialize the layers
                var layersToken = refObj.Value<JArray>("Layers");
                if (layersToken != null)
                {
                    Layers.Clear();
                    foreach (var layerToken in layersToken)
                    {
                        var layerObj = layerToken as JObject;
                        if (layerObj != null)
                        {
                            var layer = new Layer();
                            layer.Deserialize(layerObj, serializationContext);
                            Layers.Add(layer);
                        }
                    }
                }

                // Invoke the callback if provided
                if (callback != null)
                {
                    return callback(refObj);
                }

                return true;
            });
        }

    }
}
