using LibGFX.Graphics;
using LibGFX.Graphics.Enviroment;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.PostProcessing;
using LibGFX.Math;
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
        /// <summary>
        /// Gets the render target associated with this scene instance.
        /// </summary>
        public override IRenderTarget RenderTarget { get => _renderTarget; }

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

        /// <summary>
        /// Creates a new 3D scene
        /// </summary>
        public Scene3D() : base()
        {
            _lightManager = new Light3DManager();
        }

        /// <summary>
        /// Creates a new 3D scene with the given layers
        /// </summary>
        /// <param name="layers"></param>
        public Scene3D(params String[] layers) : base()
        {
            _lightManager = new Light3DManager();
            foreach (var item in layers)
            {
                this.Layers.Add(new Layer(item));
            }
        }

        /// <summary>
        /// Disposes the scene and all its layers
        /// </summary>
        /// <param name="renderer"></param>
        public override void DisposeScene(IRenderDevice renderer)
        {
            // Dispose the enviroment texture if available
            if (this.Enviroment != null)
            {
                this.Enviroment.Dispose(renderer);
            }

            // Dispose all layers and their elements
            this.Layers.ForEach(l =>
            {
                l.Dispose(this, renderer);
            });

            // Dispose the scene behaviors
            this.SceneBehaviors.ForEach(behavior =>
            {
                behavior.OnDispose(this, renderer);
            });

            // Dispose the render target
            _renderTarget.Dispose(renderer);

            // Dispose the light manager
            _lightManager.Dispose(renderer);
        }

        /// <summary>
        /// Initializes the scene and all its layers
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void Init(Viewport viewport, IRenderDevice renderer)
        {
            _renderTarget = renderer.CreateMSAARenderTarget2D(viewport.Width, viewport.Height, (int)this.Samples);

            // Load the enviroment texture if available
            if (this.Enviroment != null)
            {
                this.Enviroment.Init(renderer);
            }

            // Init all layers and there elements
            this.Layers.ForEach(l =>
            {
                l.Init(this, viewport, renderer);
            });

            // Initialize the light manager
            var lightManager = this.LightManager as Light3DManager;
            if(lightManager != null)
            {
                lightManager.Init(renderer);
            }

            // Initialize the scene behaviors
            this.SceneBehaviors.ForEach(behavior =>
            {
                behavior.OnInit(this, viewport, renderer);
            });

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

            // Process the scene behaviors
            this.SceneBehaviors.ForEach(behavior =>
            {
                behavior.BeforeRender(this, viewport, renderer, camera);
            });

            // Cull the lights in the scene
            if (this.LightManager != null)
            {
                this.LightManager.CullLights(viewport, renderer, camera);
            }

            // Get the current depth test state
            var dephTest = renderer.IsDepthTestEnabled();

            // Shadow pass rendering if enabled
            if (this.PerformShadowPass && this.LightManager != null)
            {
                //this.CreateShadowMap(renderer, viewport, camera);
            }

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

            // Render the enviroment texture if available
            if (this.Enviroment != null && this.RenderEnviromentTexture)
            {
                this.Enviroment.Render(renderer, camera, viewport);
            }

            // Render all layers in the scene
            this.Layers.ForEach(layer => {
                layer.RenderLayer(this, viewport, renderer, camera);
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
            this.SceneBehaviors.ForEach(behavior =>
            {
                behavior.AfterRender(this, viewport, renderer, camera);
            });

            // Proccess the render action
            this.ProcessRenderActions(viewport, renderer, camera);

            // Unbind the render target and set the depth test state back to the original state
            renderer.UnbindRenderTarget();
            renderer.ResolveRenderTarget(_renderTarget);
            renderer.SetDepthTest(dephTest);
        }

        /// <summary>
        /// Renders the shadow maps for the scene
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void RenderShadowMaps(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            var light = this.LightManager.GetLight<DirectionalLight3D>();
            if (light == null)
            {
                Debug.WriteLine("No directional light found for shadow pass.");
                return;
            }

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
            this.Layers.ForEach(layer =>
            {
                layer.RenderShadows(this, viewport, renderer);
            });
            renderer.SetCullMode(CullMode.Back);

            renderer.UnbindRenderTarget();
            renderer.SetDepthTest(depthTest);
            LightManager.SetLightSpaceMatrix(lightSpaceMatrix);
        }

        /// <summary>
        /// Updates the scene and all its layers
        /// </summary>
        public override void Update(float dt)
        {
            this.SceneBehaviors.ForEach(behavior =>
            {
                behavior.BeforeUpdate(this);
            });

            this.Layers.ForEach(l => {
                l.Update(this, dt);
            });

            this.SceneBehaviors.ForEach(behavior =>
            {
                behavior.AfterUpdate(this);
            });
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
                this.SceneBehaviors.ForEach(b => b.BeforePhysicsUpdate(this, PhysicsHandler));

                // Process the physics handler
                this.PhysicsHandler.Process(this);
                _physicsAccumulator -= this.PhysicsHandler.FixedTimeStep;

                // After physics update behaviors
                this.SceneBehaviors.ForEach(b => b.AfterPhysicsUpdate(this, PhysicsHandler));
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

        /// <summary>
        /// Creates a new 3D scene with a default configuration, including a directional light source.
        /// </summary>
        /// <returns>A new instance of <see cref="Scene3D"/> preconfigured with default lighting and settings.</returns>
        public static Scene3D CreateDefaultScene()
        {
            var scene = new Scene3D("Default");
            scene.DirectionalLight = new DirectionalLight3D(new Vector3(-0.2f, 1.0f, -0.3f), ColorPresets.Gray, 1.0f);
            return scene;
        }

        /// <summary>
        /// Deserializes the object's state from the specified JSON object, restoring the LightManager and Layers
        /// collections.
        /// </summary>
        /// <remarks>This method replaces the current LightManager and clears and repopulates the Layers
        /// collection based on the provided JSON data. Ensure that the object is in a valid state for deserialization
        /// before calling this method.</remarks>
        /// <param name="jObject">A <see cref="JObject"/> containing the serialized data to deserialize from. Must include 'LightManager' and
        /// 'Layers' properties.</param>
        /// <param name="serializationContext">A <see cref="SerializationContext"/> that provides context and settings for the deserialization process.</param>
        /// <exception cref="InvalidOperationException">Thrown if the LightManager is already initialized when deserialization is attempted.</exception>
        public override void Deserialize(JObject jObject, SerializationContext serializationContext)
        {
            base.Deserialize(jObject, serializationContext);

            // Deserialize the light manager Make sure its disposed first
            this.LightManager = new Light3DManager();
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
