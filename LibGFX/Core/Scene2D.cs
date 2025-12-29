using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using Newtonsoft.Json.Linq;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
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

        public override event Action<BaseScene, Viewport, IRenderDevice> OnInitStart;
        public override event Action<BaseScene, Viewport, IRenderDevice> AfterRenderTargetCreation;
        public override event Action<BaseScene, Viewport, IRenderDevice> OnInitEnd;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnShadowPassStart;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnShadowPassEnd;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderStart;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> AfterLightCulling;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderPassBegin;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderPassEnd;
        public override event Action<BaseScene, Viewport, IRenderDevice, Camera> OnRenderEnd;
        public override event Action<BaseScene, float> OnUpdateStart;
        public override event Action<BaseScene, float> OnUpdateEnd;
        public override event Action<BaseScene, float> OnPhysicsUpdateStart;
        public override event Action<BaseScene, float> OnPhysicsUpdateEnd;
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

            _renderTarget = renderer.CreateRenderTarget2D(viewport.Width, viewport.Height);

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

            renderer.ResizeRenderTarget(_renderTarget, viewport.Width, viewport.Height);
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

        public override void RenderShadowMaps(Viewport viewport, IRenderDevice renderer, Camera camera)
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
            _renderTarget.Dispose(renderer);

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

        public override void Deserialize(JObject jObject, SerializationContext serializationContext)
        {
            base.Deserialize(jObject, serializationContext);

            // Deserialize the light manager Make sure its disposed first
            this.LightManager = new Light2DManager();
            this.LightManager.Deserialize(jObject["LightManager"] as JObject, serializationContext);

            // Deserialize the Layers
            var layerArray = jObject["Layers"] as JArray;
            if (layerArray != null)
            {
                Layers.Clear();
                foreach (var layerToken in layerArray)
                {
                    var layer = new Layer();
                    layer.Deserialize(layerToken as JObject, serializationContext);
                    Layers.Add(layer);
                }
            }
        }

    }
}
