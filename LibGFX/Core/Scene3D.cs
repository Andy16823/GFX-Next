using LibGFX.Core.GameElements;
using LibGFX.Graphics;
using LibGFX.Graphics.Enviroment;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.PostProcessing;
using LibGFX.Graphics.Renderer.OpenGL;
using LibGFX.Graphics.RenderTarget;
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
    /// Represents a 3D scene for rendering 3D objects and lights.
    /// </summary>
    public class Scene3D : BaseScene
    {
#region Actions
        /// <summary>
        /// OnInitStart is triggered at the beginning of the scene initialization process, allowing for custom setup before any rendering or physics updates occur.
        /// </summary>
        public override event Action<BaseScene, Viewport, IRenderDevice> OnInitStart;

        /// <summary>
        /// AfterRenderTargetCreation is triggered after the render target has been created, allowing for custom actions to be performed with the render target.
        /// </summary>
        public override event Action<BaseScene, Viewport, IRenderDevice> AfterRenderTargetCreation;

        /// <summary>
        /// OnInitEnd is triggered at the end of the scene initialization process, allowing for custom setup after all rendering and physics updates have been initialized.
        /// </summary>
        public override event Action<BaseScene, Viewport, IRenderDevice> OnInitEnd;

        /// <summary>
        /// OnRenderStart is triggered at the beginning of the scene rendering process, allowing for custom actions to be performed before any rendering occurs.
        /// </summary>
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderStart;

        /// <summary>
        /// AfterLightCulling is triggered after the light culling process has completed, allowing for custom actions to be performed with the culled lights.
        /// </summary>
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> AfterLightCulling;

        /// <summary>
        /// OnRenderPassBegin is triggered at the beginning of the scene render pass, allowing for custom actions to be performed before any rendering occurs.
        /// </summary>
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderPassBegin;

        /// <summary>
        /// OnRenderPassEnd is triggered at the end of the scene render pass, allowing for custom actions to be performed after all rendering has completed. 
        /// </summary>
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderPassEnd;

        /// <summary>
        /// OnRenderEnd is triggered at the end of the scene rendering process, allowing for custom actions to be performed after all rendering has completed.
        /// </summary>
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderEnd;

        /// <summary>
        /// OnShadowPassStart is triggered at the beginning of the shadow pass rendering process, allowing for custom actions to be performed before any shadow rendering occurs.
        /// </summary>
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnShadowPassStart;

        /// <summary>
        /// OnShadowPassEnd is triggered at the end of the shadow pass rendering process, allowing for custom actions to be performed after all shadow rendering has completed.
        /// </summary>
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnShadowPassEnd;

        /// <summary>
        /// OnUpdateStart is triggered at the beginning of the scene update process, allowing for custom actions to be performed before any updates occur.
        /// </summary>
        public override event Action<BaseScene, float> OnUpdateStart;

        /// <summary>
        /// OnUpdateEnd is triggered at the end of the scene update process, allowing for custom actions to be performed after all updates have completed.
        /// </summary>
        public override event Action<BaseScene, float> OnUpdateEnd;

        /// <summary>
        /// OnPhysicsUpdateStart is triggered at the beginning of the physics update process, allowing for custom actions to be performed before any physics updates occur.
        /// </summary>
        public override event Action<BaseScene, float> OnPhysicsUpdateStart;

        /// <summary>
        /// OnPhysicsUpdateEnd is triggered at the end of the physics update process, allowing for custom actions to be performed after all physics updates have completed.
        /// </summary>
        public override event Action<BaseScene, float> OnPhysicsUpdateEnd;

        /// <summary>
        /// OnDisposeStart is triggered at the beginning of the scene disposal process, allowing for custom actions to be performed before any resources are released.
        /// </summary>
        public override event Action<BaseScene, IRenderDevice> OnDisposeStart;

        /// <summary>
        /// OnDispose is triggered during the scene disposal process, allowing for custom actions to be performed while resources are being released.
        /// </summary>
        public override event Action<BaseScene, IRenderDevice> OnDispose;

        /// <summary>
        /// OnDisposeEnd is triggered at the end of the scene disposal process, allowing for custom actions to be performed after all resources have been released.
        /// </summary>
        public override event Action<BaseScene, IRenderDevice> OnDisposeEnd;
#endregion

#region Properties
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
        /// Sets the directional light for the scene
        /// </summary>
        public DirectionalLight3D DirectionalLight { get; set; }

        /// <summary>
        /// The point lights of the scene. The scene supports multiple point lights.
        /// </summary>
        public List<PointLight3D> PointLights { get; set; } = new List<PointLight3D>();

        /// <summary>
        /// The number of samples for the scene rendering
        /// </summary>
        public uint Samples { get; set; } = 4;

        /// <summary>
        /// Determines if the scene should perform a shadow pass
        /// </summary>
        public bool PerformShadowPass { get; set; } = true;
#endregion

#region Private Fields
        private float _physicsAccumulator = 0.0f;
        private int _pointLightBuffer = -1;
        #endregion

        /// <summary>
        /// Initializes a new instance of the Scene3D class with the specified number of samples for rendering.
        /// </summary>
        /// <param name="samples">The number of samples for rendering.</param>
        public Scene3D(uint samples = 4) : base()
        {
            this.Samples = samples;
        }

        /// <summary>
        /// Initializes the scene and all its layers
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void Init(Viewport viewport, IRenderDevice renderer)
        {
            OnInitStart?.Invoke(this, viewport, renderer);

            _renderTarget = new MSAARenderTarget2D(viewport.Width, viewport.Height, (int)this.Samples);
            renderer.CreateRenderTarget(_renderTarget);
            AfterRenderTargetCreation?.Invoke(this, viewport, renderer);

            this.Enviroment?.Init(renderer);
            this.Elements.ForEach(e =>
            {
                e.Init(this, viewport, renderer);
            });
            this.InitLights(renderer);

            OnInitEnd?.Invoke(this, viewport, renderer);
            this.RenderStats.Start();
        }

        /// <summary>
        /// Disposes the scene and all its layers
        /// </summary>
        /// <param name="renderer"></param>
        public override void DisposeScene(IRenderDevice renderer)
        {
            OnDisposeStart?.Invoke(this, renderer);

            this.Enviroment?.Dispose(renderer);
            this.Elements.ForEach(e =>
            {
                e.Dispose(this, renderer);
            });

            OnDispose?.Invoke(this, renderer);

            renderer.DisposeRenderTarget(_renderTarget);
            this.DisposeLights(renderer);

            OnDisposeEnd?.Invoke(this, renderer);
        }

        /// <summary>
        /// Initializes the lights in the scene and prepares their associated resources using the specified render device.
        /// </summary>
        /// <param name="renderer">The render device used to initialize the lights.</param>
        public void InitLights(IRenderDevice renderer)
        {
            this.DirectionalLight?.Init(renderer);

            if (_pointLightBuffer != -1)
            {
                renderer.DisposeBuffer(_pointLightBuffer);
            }

            _pointLightBuffer = renderer.CreateBuffer();
            var lightData = new List<PointLight3DData>();
            foreach (var pointLight in this.PointLights)
            {
                pointLight.Init(renderer);
                lightData.Add(pointLight.ToStruct());
            }
            renderer.SetVertexBufferData<PointLight3DData>(_pointLightBuffer, lightData.ToArray(), true);
        }

        /// <summary>
        /// Disposes the lights in the scene and releases their associated resources using the specified render device.
        /// </summary>
        /// <param name="renderer"></param>
        private void DisposeLights(IRenderDevice renderer)
        {
            this.DirectionalLight?.Dispose(renderer);
            foreach (var pointLight in this.PointLights)
            {
                pointLight.Dispose(renderer);
            }
            renderer.DisposeBuffer(_pointLightBuffer);
        }

        /// <summary>
        /// Culls the lights in the scene based on the camera's view frustum and updates the point light buffer for rendering.
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void CullLights(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            var frustum = camera.GetFrustum(viewport);
            List<PointLight3DData> lightData = new List<PointLight3DData>();
            foreach (var pointLight in this.PointLights)
            {
                var lightAABB = pointLight.GetAABB();
                if (Frustum.ContainsAABB(frustum, lightAABB.min, lightAABB.max))
                {
                    lightData.Add(pointLight.ToStruct());
                }
            }
            renderer.SetVertexBufferData<PointLight3DData>(_pointLightBuffer, lightData.ToArray(), true);
        }

        /// <summary>
        /// Bind the lights to the shader for rendering. This method prepares the shader with the directional light and point lights data.
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void BindLights(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            this.DirectionalLight?.Bind(renderer, viewport, camera);
            renderer.BindBufferBase(GFXBufferTarget.ShaderStorageBuffer, 4, _pointLightBuffer);
        }

        /// <summary>
        /// Renders the scene and all its layers
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void Render(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            this.RenderStats.NewFrame();

            OnRenderStart?.Invoke(this, viewport, renderer, camera);

            this.CullLights(viewport, renderer, camera);
            AfterLightCulling?.Invoke(this, viewport, renderer, camera);

            var dephTest = renderer.IsDepthTestEnabled();

            renderer.EnableDepthTest();
            renderer.SetViewport(viewport);
            renderer.SetProjectionMatrix(camera.GetProjectionMatrix(viewport));
            renderer.SetViewMatrix(camera.GetViewMatrix());

            renderer.ResizeRenderTarget(_renderTarget, viewport.Width, viewport.Height);

            using(_renderTarget.Begin(renderer))
            {
                renderer.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
                renderer.Clear(GFXClearFlags.Color | GFXClearFlags.Depth);

                OnRenderPassBegin?.Invoke(this, viewport, renderer, camera);

                if (this.Enviroment != null && this.RenderEnviromentTexture)
                {
                    this.Enviroment.Render(renderer, camera, viewport);
                }

                OrderScene(camera);

                this.Elements.ForEach(e =>
                {
                    e.Render(this, viewport, renderer, camera);
                });

                if (this.PhysicsHandler != null && this.PhysicsHandler.DebugPhysics)
                {
                    if (this.PhysicsHandler.HasDebugDrawer())
                    {
                        renderer.DisableDepthTest();
                        this.PhysicsHandler.DebugDraw(renderer);
                        renderer.EnableDepthTest();
                    }
                    else
                    {
                        Debug.Assert(this.PhysicsHandler.DebugPhysics, "DebugPhysics is enabled but no debug drawer is set");
                    }
                }

                OnRenderPassEnd?.Invoke(this, viewport, renderer, camera);
            }
            renderer.BlitRenderTarget(_renderTarget);
            renderer.SetDepthTest(dephTest);

            OnRenderEnd?.Invoke(this, viewport, renderer, camera);
        }

        /// <summary>
        /// Renders the shadow maps for the scene
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void BuildShadowMaps(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            var light = this.DirectionalLight as DirectionalLight3D;
            if(light == null || light.ShadowMap == null || !light.CastsShadows || !this.PerformShadowPass)
            {
                return;
            }

            var csm = light.ShadowMap as CascadedShadowMap;
            if(csm == null)
            {
                return;
            }

            OnShadowPassStart?.Invoke(this, viewport, renderer, camera);

            light.ComputeLightSpaceMatrix(camera, viewport);

            var shader = renderer.GetRenderShader<DepthMeshShader>();
            using(shader.Begin(renderer))
            {
                light.BindLightSpaceMatrix(renderer, 3);

                using (light.ShadowMap.Begin(renderer))
                {
                    renderer.SetViewport(new Viewport(csm.Width, csm.Height));
                    renderer.Clear(GFXClearFlags.Depth);
                    renderer.EnableDepthTest();
                    renderer.SetCullMode(GFXCullMode.Front);
                    this.Elements.ForEach(e =>
                    {
                        if (!e.Visible) return;
                        e.RenderShadow(this, viewport, renderer);
                    });
                    renderer.SetCullMode(GFXCullMode.Back);
                    renderer.DisableDepthTest();
                }

                OnShadowPassEnd?.Invoke(this, viewport, renderer, camera);
            }
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
                // Only add the point light if it is not already present in the scene
                if (this.PointLights.Find(l => l.ID == pointLight.ID) == null)
                {
                    this.PointLights.Add(pointLight);
                }
            }
            else if (light is DirectionalLight3D directionalLight)
            {
                this.DirectionalLight = directionalLight;
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
                return (T)(object)this.DirectionalLight;
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
            if (light is PointLight3D pointLight)
            {
                this.PointLights.Remove(pointLight);
            }
            else
            {
                throw new NotSupportedException($"Light type {typeof(T).Name} is not supported in Scene3D.");
            }
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
        public override ICollection<GameElement> GetElementsWithBehavior<T>()
        {
            return this.Elements.Where(e => e.GetBehavior<T>() != null).ToList();
        }

        /// <summary>
        /// Finds all game elements with the specified tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public override ICollection<GameElement> GetElementsWithProperty(string key)
        {
            return this.Elements.Where(e => e.HasProperty(key)).ToList();
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
            // Dispose the game elements and clear the list
            this.Elements.ForEach(e =>
            {
                e.Dispose(this, renderer);
            });
            this.Elements.Clear();

            // Dispose the lights and clear the point lights list
            this.DirectionalLight?.Dispose(renderer);
            this.DirectionalLight = null;
            foreach (var pointLight in this.PointLights)
            {
                pointLight.Dispose(renderer);
            }
            this.PointLights.Clear();
            renderer.DisposeBuffer(_pointLightBuffer);
            _pointLightBuffer = -1;
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
                if(this.DirectionalLight != null)
                {
                    w.WritePropertyName("DirectionalLight");
                    this.DirectionalLight.Serialize(w, serializationContext);
                }
                if(this.PointLights.Count > 0)
                {
                    w.WritePropertyName("PointLights");
                    w.WriteStartArray();
                    this.PointLights.ForEach(l =>
                    {
                        l.Serialize(w, serializationContext);
                    });
                    w.WriteEndArray();
                }
                w.WritePropertyName("Elements");
                w.WriteStartArray();
                this.Elements.ForEach(e =>
                {
                    e.Serialize(w, serializationContext);
                });
                w.WriteEndArray();
                callback?.Invoke(w);
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
                // Deserialize the directional light
                if (refObj.TryGetValue("DirectionalLight", out JToken directionalLightToken) && directionalLightToken is JObject directionalLightObj)
                {
                    this.DirectionalLight = new DirectionalLight3D();
                    this.DirectionalLight.Deserialize(directionalLightObj, serializationContext);
                }

                // Deserialize the point lights
                if (refObj.TryGetValue("PointLights", out JToken pointLightsToken) && pointLightsToken is JArray pointLightsArray)
                {
                    this.PointLights.Clear();
                    foreach (var pointLightToken in pointLightsArray)
                    {
                        if (pointLightToken is JObject pointLightObj)
                        {
                            var pointLight = new PointLight3D();
                            pointLight.Deserialize(pointLightObj, serializationContext);
                            this.PointLights.Add(pointLight);
                        }
                    }
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
