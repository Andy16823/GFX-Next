using LibGFX.Core.GameElements;
using LibGFX.Graphics;
using LibGFX.Graphics.Enviroment;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.PostProcessing;
using LibGFX.Graphics.Renderer.OpenGL;
using LibGFX.Graphics.Shader;
using LibGFX.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    /// <summary>
    /// Represents an entry for enqueuing a 3D scene element, including the element, an optional action to perform, and
    /// any additional data required for processing.
    /// </summary>
    /// <remarks>Use this class to encapsulate all information needed to enqueue a 3D scene element for
    /// processing or rendering. The associated action and extra data allow for flexible handling of scene-specific
    /// logic.</remarks>
    public class EnqueScene3DEntry : IEnqueEntry
    {
        public GameElement Element { get; set; }
        public Action<BaseScene, GameElement, Dictionary<string, object>>? EnqueAction { get; set; }
        public Dictionary<string, object>? ExtraData { get; set; }
    }

    /// <summary>
    /// Represents a 3D scene for rendering 3D objects and lights.
    /// </summary>
    public class Scene3D : BaseScene
    {
        /// <summary>
        /// Gets the render target associated with this scene instance.
        /// </summary>
        public override IRenderTarget RenderTarget { get => _renderTarget; }

        /// <summary>
        /// Gets or sets the collection of game elements contained in this instance.
        /// </summary>
        public List<GameElement> Elements { get; set; } = new List<GameElement>();

        /// <summary>
        /// Determines if the enviroment texture should be rendered
        /// </summary>
        public bool RenderEnviromentTexture { get; set; } = true;

        /// <summary>
        /// The enviroment texture of the scene
        /// </summary>
        public IEnviroment Enviroment { get; set; }

        /// <summary>
        /// The render target of the scene  
        /// </summary>
        private MSAARenderTarget2D _renderTarget;

        /// <summary>
        /// The light manager for the 3D scene
        /// </summary>
        public override ILightManager LightManager { get => _lightManager; }

        /// <summary>
        /// Sets the directional light for the scene
        /// </summary>
        public DirectionalLight3D DirectionalLight { get => _lightManager.DirectionalLight; set => _lightManager.DirectionalLight = value; }

        /// <summary>
        /// The number of samples for the scene rendering
        /// </summary>
        public uint Samples { get; set; } = 4;

        /// <summary>
        /// Determines if the scene should perform a shadow pass
        /// </summary>
        public bool PerformShadowPass { get; set; } = true;

        // Init events
        public override event Action<BaseScene, Viewport, IRenderDevice> OnInitStart;
        public override event Action<BaseScene, Viewport, IRenderDevice> AfterRenderTargetCreation;
        public override event Action<BaseScene, Viewport, IRenderDevice> OnInitEnd;

        // Render events
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderStart;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> AfterLightCulling;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderPassBegin;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderPassEnd;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderEnd;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnShadowPassStart;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnShadowPassEnd;

        // Update events
        public override event Action<BaseScene, float> OnUpdateStart;
        public override event Action<BaseScene, float> OnUpdateEnd;

        // Physics update events
        public override event Action<BaseScene, float> OnPhysicsUpdateStart;
        public override event Action<BaseScene, float> OnPhysicsUpdateEnd;

        // Dispose events
        public override event Action<BaseScene, IRenderDevice> OnDisposeStart;
        public override event Action<BaseScene, IRenderDevice> OnDispose;
        public override event Action<BaseScene, IRenderDevice> OnDisposeEnd;

        // Private members
        private Light3DManager _lightManager;
        private float _physicsAccumulator = 0.0f;

        /// <summary>
        /// Creates a new 3D scene
        /// </summary>
        public Scene3D() : base()
        {
            _lightManager = new Light3DManager();
        }

        /// <summary>
        /// Disposes the scene and all its layers
        /// </summary>
        /// <param name="renderer"></param>
        public override void DisposeScene(IRenderDevice renderer)
        {
            // On dispose start event
            OnDisposeStart?.Invoke(this, renderer);

            // Dispose the enviroment texture if available
            if (this.Enviroment != null)
            {
                this.Enviroment.Dispose(renderer);
            }

            // Dispose all layers and their elements
            this.Elements.ForEach(e =>
            {
                e.Dispose(this, renderer);
            });

            // On dispose event
            OnDispose?.Invoke(this, renderer);

            // Dispose the render target
            _renderTarget.Dispose();

            // Dispose the light manager
            _lightManager.Dispose(renderer);

            // On dispose end event
            OnDisposeEnd?.Invoke(this, renderer);
        }

        /// <summary>
        /// Initializes the scene and all its layers
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void Init(Viewport viewport, IRenderDevice renderer)
        {
            // On init start event
            OnInitStart?.Invoke(this, viewport, renderer);

            // Create the render target for the scene
            _renderTarget = new MSAARenderTarget2D(viewport.Width, viewport.Height, (int)this.Samples);
            _renderTarget.Create();

            // After render target creation event
            AfterRenderTargetCreation?.Invoke(this, viewport, renderer);

            // Load the enviroment texture if available
            if (this.Enviroment != null)
            {
                this.Enviroment.Init(renderer);
            }

            // Init all layers and there elements
            this.Elements.ForEach(e =>
            {
                e.Init(this, viewport, renderer);
            });

            // Initialize the light manager
            var lightManager = this.LightManager as Light3DManager;
            if(lightManager != null)
            {
                lightManager.Init(renderer);
            }

            // Initialize the scene behaviors
            OnInitEnd?.Invoke(this, viewport, renderer);

            // OnStart the render stats
            this.RenderStats.Start();
        }

        /// <summary>
        /// Renders the scene and all its layers
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void Render(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            // OnStart new frame for the render stats
            this.RenderStats.NewFrame();

            // On render start event
            OnRenderStart?.Invoke(this, viewport, renderer, camera);

            // Cull the lights in the scene
            if (this.LightManager != null)
            {
                this.LightManager.CullLights(viewport, renderer, camera);
            }

            // After light culling event
            AfterLightCulling?.Invoke(this, viewport, renderer, camera);

            // Get the current depth test state
            var dephTest = renderer.IsDepthTestEnabled();

            // Enable depth test and set the viewport, projection and view matrix
            renderer.EnableDepthTest();
            renderer.SetViewport(viewport);
            renderer.SetProjectionMatrix(camera.GetProjectionMatrix(viewport));
            renderer.SetViewMatrix(camera.GetViewMatrix());

            // Render the scene to the render target
            _renderTarget.Resize(viewport.Width, viewport.Height);
            renderer.BindRenderTarget(_renderTarget);
            renderer.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);

            // On render start event
            OnRenderPassBegin?.Invoke(this, viewport, renderer, camera);

            // Render the enviroment texture if available
            if (this.Enviroment != null && this.RenderEnviromentTexture)
            {
                this.Enviroment.Render(renderer, camera, viewport);
            }

            // Order the scene elements based on transparency and distance to camera
            OrderScene(camera);

            // Render all layers in the scene
            this.Elements.ForEach(e =>
            {
                e.Render(this, viewport, renderer, camera);
            });

            // Debug draw the physics if enabled
            if (this.PhysicsHandler != null && this.PhysicsHandler.DebugPhysics)
            {
                if(this.PhysicsHandler.HasDebugDrawer())
                {
                    renderer.DisableDepthTest();
                    this.PhysicsHandler.DebugDraw(renderer);    
                    renderer.EnableDepthTest();
                }
                else {
                    Debug.Assert(this.PhysicsHandler.DebugPhysics, "DebugPhysics is enabled but no debug drawer is set");
                }
            }

            // Process the scene behaviors after rendering
            OnRenderPassEnd?.Invoke(this, viewport, renderer, camera);

            // Unbind the render target and set the depth test state back to the original state
            renderer.UnbindRenderTarget();
            _renderTarget.ResolveMultisample();
            renderer.SetDepthTest(dephTest);

            // On render end.
            OnRenderEnd?.Invoke(this, viewport, renderer, camera);
        }

        /// <summary>
        /// Renders the shadow maps for the scene
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void RenderShadowMaps(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            // Get the directional light and its shadow map. 
            var light = this.DirectionalLight as DirectionalLight3D;
            if(light == null || light.ShadowMap == null || !light.CastsShadows || !this.PerformShadowPass)
            {
                return;
            }

            // Ensure the shadow map is a cascaded shadow map since only that type is supported for directional lights in this implementation
            var csm = light.ShadowMap as CascadedShadowMap;
            if(csm == null)
            {
                return;
            }

            // On shadow pass start event
            OnShadowPassStart?.Invoke(this, viewport, renderer, camera);

            // Compute the light space matrix for the directional light
            this.LightManager.ComputeLightSpaceMatrix(camera, viewport);

            // Bind the depth shader and set the light space matrix uniform
            var shader = renderer.GetRenderShader<DepthMeshShader>();
            renderer.BindShaderProgram(shader);
            this.LightManager.BindLightSpaceMatrix(renderer, 3);

            // Bind the shadow map render target and clear the depth buffer
            renderer.BindRenderTarget(light.ShadowMap);
            renderer.SetViewport(new Viewport(csm.Width, csm.Height));
            renderer.Clear(RenderFlags.ClearFlags.Depth);
            renderer.EnableDepthTest();
            //GL.Enable(EnableCap.CullFace);
            renderer.SetCullMode(CullMode.Front);
            this.Elements.ForEach(e =>
            {
                if(!e.Visible) return;
                e.RenderShadow(this, viewport, renderer);
            });
            renderer.SetCullMode(CullMode.Back);
            //GL.Disable(EnableCap.CullFace);
            renderer.DisableDepthTest();
            renderer.UnbindRenderTarget();

            // On shadow pass end event
            OnShadowPassEnd?.Invoke(this, viewport, renderer, camera);
        }

        /// <summary>
        /// Updates the scene and all its layers
        /// </summary>
        public override void Update(float dt)
        {
            OnUpdateStart?.Invoke(this, dt);

            this.Elements.ForEach(e =>
            {
                e.Update(this, dt);
            });

            OnUpdateEnd?.Invoke(this, dt);
        }

        /// <summary>
        /// Updates the physics of the scene
        /// </summary>
        public override void UpdatePhysics(float dt)
        {
            if(this.PhysicsHandler == null) return;
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
        /// Adds a light to the scene. Supports point and directional lights.
        /// </summary>
        /// <remarks>Only PointLight3D and DirectionalLight3D types are supported. Attempting to add other
        /// light types will result in a NotSupportedException.</remarks>
        /// <typeparam name="T">The type of the light to add. Must be either PointLight3D or DirectionalLight3D.</typeparam>
        /// <param name="light">The light instance to add to the scene. Must be a supported light type.</param>
        /// <exception cref="NotSupportedException">Thrown if the specified light type is not supported by the scene.</exception>
        public override void AddLight<T>(T light)
        {
            if (light is PointLight3D pointLight)
            {
                _lightManager.AddPointLight(pointLight);
            }
            else if (light is DirectionalLight3D directionalLight)
            {
                _lightManager.DirectionalLight = directionalLight;
            }
            else
            {
                throw new NotSupportedException($"Light type {typeof(T).Name} is not supported in Scene3D.");
            }
        }

        /// <summary>
        /// Retrieves the scene's directional light if the specified type is supported.
        /// </summary>
        /// <remarks>This method only supports retrieval of the directional light. Attempting to request
        /// other light types will result in an exception.</remarks>
        /// <typeparam name="T">The type of light to retrieve. Only <see cref="DirectionalLight3D"/> is supported.</typeparam>
        /// <returns>The directional light instance cast to the specified type <typeparamref name="T"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown if <typeparamref name="T"/> is not <see cref="DirectionalLight3D"/>.</exception>
        public override T GetLight<T>()
        {
            if(typeof(T) == typeof(DirectionalLight3D))
            {
                return (T)(object)_lightManager.DirectionalLight;
            }
            else
            {
                throw new NotSupportedException($"Light type {typeof(T).Name} is not supported in Scene3D.");
            }
        }

        /// <summary>
        /// Removes a directional light from the scene if it is currently present.
        /// </summary>
        /// <remarks>Only directional lights are supported for removal. Attempting to remove other light
        /// types will result in a NotSupportedException.</remarks>
        /// <typeparam name="T">The type of light to remove. Must be a DirectionalLight3D.</typeparam>
        /// <param name="light">The light instance to remove from the scene. Must be a DirectionalLight3D that is part of the scene.</param>
        /// <exception cref="InvalidOperationException">Thrown if the specified directional light is not part of the scene.</exception>
        /// <exception cref="NotSupportedException">Thrown if the specified light type is not supported by Scene3D.</exception>
        public override void RemoveLight<T>(T light)
        {
            LightManager.RemoveLight(light);
        }

        /// <summary>
        /// Adds a game element to the scene
        /// </summary>
        /// <param name="element"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public override void AddGameElement(GameElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            this.Elements.Add(element);
        }

        /// <summary>
        /// Gets all game elements in the scene
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<GameElement> GetAllElements()
        {
            return this.Elements;
        }

        /// <summary>
        /// For each game element in the scene, perform the specified action
        /// </summary>
        /// <param name="action"></param>
        public override void ForEachElement(Action<GameElement> action)
        {
            this.Elements.ForEach(action);
        }

        /// <summary>
        /// Gets a game element by its unique identifier (UUID)
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public override GameElement? GetElementByID(string uuid)
        {
            return this.Elements.FirstOrDefault(e => e.ID.ToString() == uuid);
        }

        /// <summary>
        /// Finds a game element by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override T? FindElement<T>(string name) where T : class
        {
            return this.Elements.OfType<T>().FirstOrDefault(e => e.Name == name);
        }

        /// <summary>
        /// Gets all game elements of the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override ICollection<GameElement> GetElements<T>()
        {
            return this.Elements.OfType<T>().Cast<GameElement>().ToList();
        }

        /// <summary>
        /// Finds all game elements with the specified behavior
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override ICollection<GameElement> FindElementsWithBehavior<T>()
        {
            return this.Elements.Where(e => e.GetBehavior<T>() != null).ToList();
        }

        /// <summary>
        /// Finds all game elements with the specified tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public override ICollection<GameElement> FindElementsWithTag(string tag)
        {
            return this.Elements.Where(e => e.Tags.Contains(tag)).ToList();
        }

        /// <summary>
        /// Removes a game element from the scene
        /// </summary>
        /// <param name="element"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public override void RemoveElement(GameElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            this.Elements.Remove(element);
        }

        /// <summary>
        /// Clrears all elements from the scene
        /// </summary>
        public override void ClearElements()
        {
            this.Elements.Clear();
        }

        /// <summary>
        /// Processes the enque entries and adds the elements to the scene. Clears the enque entries after processing.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public override void EnqueElements()
        {
            foreach (EnqueScene3DEntry entry in EnqueEntries)
            {
                // Skip null elements
                if (entry.Element == null)
                {
                    continue;
                }

                // Trigger the event before adding the element to the scene
                entry.EnqueAction?.Invoke(this, entry.Element, entry.ExtraData ?? new Dictionary<string, object>());

                // Add the element to the scene
                if (entry.Element != null)
                {
                    AddGameElement(entry.Element);
                }
                else
                {
                    throw new InvalidOperationException("EnqueScene3DEntry contains a null Element after EnqueAction execution.");
                }
            }
            EnqueEntries.Clear();
        }

        /// <summary>
        /// Adds an enque entry to the scene which gets processed during the enque phase
        /// </summary>
        /// <param name="entry"></param>
        /// <exception cref="ArgumentException"></exception>
        public override void AddEnqueEntry(IEnqueEntry entry)
        {
            if (entry is EnqueScene3DEntry sceneEntry)
            {
                EnqueEntries.Add(sceneEntry);
            }
            else
            {
                throw new ArgumentException("Entry must be of type EnqueScene3DEntry", nameof(entry));
            }
        }

        /// <summary>
        /// Frees the scene from the game elements
        /// </summary>
        /// <param name="renderer"></param>
        public override void FreeScene(IRenderDevice renderer)
        {
            this.Elements.ForEach(e =>
            {
                e.Dispose(this, renderer);
            });
            this.Elements.Clear();
        }

        /// <summary>
        /// Initializes the scene elements
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void InitializeElements(Viewport viewport, IRenderDevice renderer)
        {
            this.Elements.ForEach(e =>
            {
                e.Init(this, viewport, renderer);
            });
        }

        /// <summary>
        /// Orders the scene elements based on transparency and distance to the camera.
        /// </summary>
        /// <param name="camera"></param>
        private void OrderScene(Camera camera)
        {
            this.Elements.Sort((a, b) =>
            {
                bool aTransparent = a.HasTransparency;
                bool bTransparent = b.HasTransparency;

                // Opaque first
                if (aTransparent != bTransparent)
                    return aTransparent ? 1 : -1;

                // Both transparent → back-to-front
                if (aTransparent)
                {
                    float da = (camera.Transform.Position - a.Transform.Position).LengthSquared;
                    float db = (camera.Transform.Position - b.Transform.Position).LengthSquared;

                    return db.CompareTo(da);
                }

                // Both opaque → order doesn't matter
                return 0;
            });
        }

        /// <summary>
        /// Creates a new 3D scene with a default configuration, including a directional light source.
        /// </summary>
        /// <returns>A new instance of <see cref="Scene3D"/> preconfigured with default lighting and settings.</returns>
        public static Scene3D CreateDefaultScene()
        {
            var scene = new Scene3D();
            scene.DirectionalLight = new DirectionalLight3D(new Vector3(-0.2f, 1.0f, -0.3f), ColorPresets.Gray, 1.0f);
            return scene;
        }

        /// <summary>
        /// Serializes the scene to JSON
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="serializationContext"></param>
        /// <param name="callback"></param>
        public override void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            base.Serialize(writer, serializationContext, (w) =>
            {
                w.WritePropertyName("Elements");
                w.WriteStartArray();
                this.Elements.ForEach(e =>
                {
                    e.Serialize(w, serializationContext);
                });
                w.WriteEndArray();
            });
        }

        /// <summary>
        /// Deserializes the scene from JSON
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializationContext"></param>
        /// <param name="callback"></param>
        /// <exception cref="Exception"></exception>
        public override void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            base.Deserialize(obj, serializationContext, (refObj) =>
            {
                // Deserialize the light manager
                var lightManagerToken = refObj.Value<JObject>("LightManager");
                if (lightManagerToken != null)
                {
                    this.LightManager = new Light3DManager();
                    this.LightManager.Deserialize(lightManagerToken, serializationContext);
                }

                // Deserialize the scene elements
                var elementsToken = refObj.Value<JArray>("Elements");
                if (elementsToken != null)
                {
                    this.Elements.Clear();
                    foreach (var elementToken in elementsToken)
                    {
                        if (elementToken is JObject elementObj)
                        {
                            var element = Utils.DeserializeGameElement(elementObj, serializationContext);
                            if (element == null)
                            {
                                throw new Exception("Failed to deserialize child GameElement.");
                            }
                            this.Elements.Add(element);
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
