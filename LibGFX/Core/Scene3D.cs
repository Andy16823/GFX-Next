using LibGFX.Graphics;
using LibGFX.Graphics.Enviroment;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.PostProcessing;
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

        private Light3DManager _lightManager;
        private float _physicsAccumulator = 0.0f;

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
            _renderTarget.Dispose(renderer);

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
            _renderTarget = renderer.CreateMSAARenderTarget2D(viewport.Width, viewport.Height, (int)this.Samples);

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
            renderer.ResizeRenderTarget(_renderTarget, viewport.Width, viewport.Height);
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
            renderer.ResolveRenderTarget(_renderTarget);
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
            // On shadow pass start event
            OnShadowPassStart?.Invoke(this, viewport, renderer, camera);

            // Get the directional light for the scene
            var light = this.LightManager.GetLight<DirectionalLight3D>();
            if (light == null)
            {
                Debug.WriteLine("No directional light found for shadow pass.");
                return;
            }

            // Render the shadow map for the directional light
            var shadowMap = light.ShadowMap;
            var depthTest = renderer.IsDepthTestEnabled();
            renderer.EnableDepthTest();

            var lightDir = light.Direction.Normalized();
            var cameraXZ = camera.Transform.Position;
            var lightOffset = new Vector3(0f, 10.0f, 0f);
            var lightPos = cameraXZ + lightOffset;
            var lightTarget = lightPos - (light.Direction.Normalized() * 20.0f);

            float near_plane = 0.1f, far_plane = 20.0f;
            var lightView = Matrix4.LookAt(lightPos, lightTarget, new Vector3(0, 1, 0));
            var lightProjection = Matrix4.CreateOrthographic(60, 60, near_plane, far_plane);
            var lightSpaceMatrix = lightView * lightProjection;

            renderer.SetViewport((Viewport) light.ShadowMapSize);
            renderer.SetProjectionMatrix(lightProjection);
            renderer.SetViewMatrix(lightView);

            renderer.BindRenderTarget(shadowMap);
            renderer.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            renderer.Clear(RenderFlags.ClearFlags.Depth);

            renderer.SetCullMode(CullMode.Front);
            this.Elements.ForEach(e =>
            {
                e.RenderShadow(this, viewport, renderer);
            });
            renderer.SetCullMode(CullMode.Back);

            renderer.UnbindRenderTarget();
            renderer.SetDepthTest(depthTest);
            LightManager.SetLightSpaceMatrix(lightSpaceMatrix);

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
            // TODO: Support removal of point lights
            if (light is DirectionalLight3D directionalLight)
            {
                if (_lightManager.DirectionalLight == directionalLight)
                {
                    _lightManager.DirectionalLight = null;
                }
                else
                {
                    throw new InvalidOperationException("The specified directional light is not part of the scene.");
                }
            }
            else
            {
                throw new NotSupportedException($"Light type {typeof(T).Name} is not supported in Scene3D.");
            }
        }

        public override void AddGameElement(GameElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            this.Elements.Add(element);
        }

        public override IEnumerable<GameElement> GetAllElements()
        {
            return this.Elements;
        }

        public override void ForEachElement(Action<GameElement> action)
        {
            this.Elements.ForEach(action);
        }

        public override GameElement? GetElementByID(string uuid)
        {
            return this.Elements.FirstOrDefault(e => e.ID.ToString() == uuid);
        }

        public override GameElement? FindElementByName(string name)
        {
            return this.Elements.FirstOrDefault(e => e.Name == name);
        }

        public override ICollection<GameElement> GetElements<T>()
        {
            return this.Elements.OfType<T>().Cast<GameElement>().ToList();
        }

        public override ICollection<GameElement> FindElementsWithBehavior<T>()
        {
            return this.Elements.Where(e => e.GetBehavior<T>() != null).ToList();
        }

        public override ICollection<GameElement> FindElementsWithTag(string tag)
        {
            return this.Elements.Where(e => e.Tags.Contains(tag)).ToList();
        }

        public override void RemoveElement(GameElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            this.Elements.Remove(element);
        }

        public override void ClearElements()
        {
            this.Elements.Clear();
        }

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

        public override void FreeScene(IRenderDevice renderer)
        {
            this.Elements.ForEach(e =>
            {
                e.Dispose(this, renderer);
            });
            this.Elements.Clear();
        }

        public override void InitializeElements(Viewport viewport, IRenderDevice renderer)
        {
            this.Elements.ForEach(e =>
            {
                e.Init(this, viewport, renderer);
            });
        }

        private void OrderScene(Camera camera)
        {
            this.Elements.Sort((a, b) =>
            {
                bool aTransparent = a.HasTransparency;
                bool bTransparent = b.HasTransparency;

                // 1️⃣ Opaque vor Transparent
                if (aTransparent != bTransparent)
                    return aTransparent ? 1 : -1;

                // 2️⃣ Beide transparent → back-to-front
                if (aTransparent)
                {
                    float da = (camera.Transform.Position - a.Transform.Position).LengthSquared;
                    float db = (camera.Transform.Position - b.Transform.Position).LengthSquared;

                    return db.CompareTo(da); // weiter weg zuerst
                }

                // 3️⃣ Beide opaque → Reihenfolge egal
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
        /// <param name="reader"></param>
        /// <param name="serializationContext"></param>
        /// <param name="callback"></param>
        public override void Deserialize(JsonReader reader, SerializationContext serializationContext, Func<JsonReader, string, bool> callback = null)
        {
            base.Deserialize(reader, serializationContext, (r, param) =>
            {
                switch (param)
                {
                    case "LightManager":
                        if(r.TokenType == JsonToken.StartObject)
                        {
                            this.LightManager = new Light3DManager();
                            this.LightManager.Deserialize(r, serializationContext);
                            return true;
                        }
                        break;
                    case "Elements":
                        if(r.TokenType != JsonToken.StartArray)
                            throw new JsonSerializationException("Expected StartArray token for Elements property.");
                        
                        while (r.Read())
                        {
                            if (r.TokenType == JsonToken.EndArray)
                                break;

                            if(r.TokenType == JsonToken.StartObject)
                            {
                                var element = Utils.DeserializeGameElement(r, serializationContext);
                                if (element == null)
                                {
                                    throw new Exception("Failed to deserialize child GameElement.");
                                }
                                this.Elements.Add(element);
                            }
                        }
                        return true;
                    default:
                        break;
                }
                return false;
            });
        }
    }
}
