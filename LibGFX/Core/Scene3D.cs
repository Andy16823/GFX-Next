using LibGFX.Graphics;
using LibGFX.Graphics.Enviroment;
using LibGFX.Graphics.Lights;
using LibGFX.Math;
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
        private RenderTarget2D _renderTarget;

        /// <summary>
        /// The light manager for the 3D scene
        /// </summary>
        public override ILightManager LightManager { get => _lightManager; }

        /// <summary>
        /// Sets the directional light for the scene
        /// </summary>
        public DirectionalLight3D DirectionalLight { get => this.GetDirectionalLight(); set => this.SetDirectionalLight(value); }

        /// <summary>
        /// The number of samples for the scene rendering
        /// </summary>
        public uint Samples { get; set; } = 4;

        /// <summary>
        /// Determines if the scene should perform a shadow pass
        /// </summary>
        public bool PerformShadowPass { get; set; } = true;

        // The light manager for the 3D scene
        private Light3DManager _lightManager;

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
            _renderTarget = new RenderTarget2D(RenderTargetDescriptor.Default(viewport.Width, viewport.Height, (int)this.Samples));
            _renderTarget.Create(renderer);

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
            _renderTarget.Resize(renderer, viewport.Width, viewport.Height);
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
            renderer.SetDepthTest(dephTest);

            // Render the render target to the screen
            renderer.BindShaderProgram(renderer.GetShaderProgram("ScreenShader"));
            renderer.DrawRenderTarget(_renderTarget);
            renderer.UnbindShaderProgram();
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

            renderer.CullFrontFace();
            this.Layers.ForEach(layer =>
            {
                layer.RenderShadows(this, viewport, renderer);
            });
            renderer.CullBackFace();

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
            this.SceneBehaviors.ForEach(behavior =>
            {
                behavior.BeforePhysicsUpdate(this, this.PhysicsHandler);
            });

            this.PhysicsHandler.Process(this, dt);

            this.SceneBehaviors.ForEach(behavior =>
            {
                behavior.AfterPhysicsUpdate(this, this.PhysicsHandler);
            });
        }

        /// <summary>
        /// Sets the directional light of the scene
        /// </summary>
        /// <param name="light"></param>
        public void SetDirectionalLight(DirectionalLight3D light)
        {
            _lightManager.DirectionalLight = light;
        }

        /// <summary>
        /// Gets the directional light of the scene
        /// </summary>
        /// <returns></returns>
        public DirectionalLight3D GetDirectionalLight()
        {
            return _lightManager.DirectionalLight;
        }

        /// <summary>
        /// Adds a point light to the scene
        /// </summary>
        /// <param name="light"></param>
        public void AddPointLight(PointLight3D light)
        {
            _lightManager.AddPointLight(light);
        }
    }
}
