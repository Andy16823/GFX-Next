using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
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
        /// The render target of the scene
        /// </summary>
        private RenderTarget2D _renderTarget;

        /// <summary>
        /// Sets the main light manager for the scene
        /// </summary>
        public DirectionalLight2D SceneLight { get => this.GetDirectionalLight(); set => this.SetDirectionalLight(value); }

        /// <summary>
        /// The light manager of the scene
        /// </summary>
        public override ILightManager LightManager { get => _lightManager; }

        /// <summary>
        /// The light manager for 2D lights
        /// </summary>
        private Light2DManager _lightManager;

        private float _physicsAccumulator = 0.0f;



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
        /// Sets the directional light for the scene
        /// </summary>
        /// <param name="light"></param>
        public void SetDirectionalLight(DirectionalLight2D light)
        {
            _lightManager.DirectionalLight = light;
        }

        /// <summary>
        /// Gets the directional light for the scene
        /// </summary>
        /// <returns></returns>
        public DirectionalLight2D GetDirectionalLight()
        {
            return _lightManager.DirectionalLight;
        }

        /// <summary>
        /// Adds a point light to the scene
        /// </summary>
        /// <param name="light"></param>
        public void AddPointLight(PointLight2D light)
        {
            _lightManager.AddPointLight(light);
        }

        /// <summary>
        /// Removes a point light from the scene
        /// </summary>
        /// <param name="light"></param>
        public void RemovePointLight(PointLight2D light)
        {
            _lightManager.RemovePointLight(light);
        }

        /// <summary>
        /// Initializes the scene
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void Init(Viewport viewport, IRenderDevice renderer)
        {
            _renderTarget = renderer.CreateRenderTarget2D(viewport.Width, viewport.Height);

            // Iinitialize the layers of the scene
            this.Layers.ForEach(l =>
            {
                l.Init(this, viewport, renderer);
            });

            // Initialize the light manager
            this.LightManager.Init(renderer);

            // Call the init behaviors for the scene
            this.SceneBehaviors.ForEach(behavior =>
            {
                behavior.OnInit(this, viewport, renderer);
            });

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
            // OnStart a new frame for the render stats
            this.RenderStats.NewFrame();

            // Call before render behaviors
            this.SceneBehaviors.ForEach(behavior =>
            {
                behavior.BeforeRender(this, viewport, renderer, camera);
            });

            // Cull lights in the scene
            if (this.LightManager != null)
            {
                this.LightManager.CullLights(viewport, renderer, camera);
            }

            // Get the current depth test state
            var depthTest = renderer.IsDepthTestEnabled();

            // Disable depth test and set the viewport, projection and view matrix
            renderer.DisableDepthTest();
            renderer.SetViewport(viewport);
            renderer.SetProjectionMatrix(camera.GetProjectionMatrix(viewport));
            renderer.SetViewMatrix(camera.GetViewMatrix());

            // Render the scene to the render target
            renderer.ResizeRenderTarget2D(_renderTarget, viewport.Width, viewport.Height);
            renderer.BindRenderTarget(_renderTarget);
            renderer.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);

            // Render the layers of the scene
            this.Layers.ForEach(layer => { 
                layer.RenderLayer(this, viewport, renderer, camera); 
            });

            // Debug draw the physics if enabled
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

            // Call after render behaviors
            this.SceneBehaviors.ForEach(behavior =>
            {
                behavior.AfterRender(this, viewport, renderer, camera);
            });

            // Proccess the render action
            this.ProcessRenderActions(viewport, renderer, camera);

            // Unbind the render target and set the depth test state back to the original state
            renderer.UnbindRenderTarget();

            // Render the render target to the screen
            renderer.BindShaderProgram(renderer.GetShaderProgram("ScreenShader"));
            renderer.DrawRenderTarget(_renderTarget);  
            renderer.UnbindShaderProgram();

            // Restore the depth test state
            renderer.SetDepthTest(depthTest);
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
            this.SceneBehaviors.ForEach(behavior =>
            {
                behavior.BeforeUpdate(this);
            });

            // Update the scene
            this.Layers.ForEach(l => { 
                l.Update(this, dt); 
            });

            // Call the after update behaviors for the scene
            this.SceneBehaviors.ForEach(behavior =>
            {
                behavior.AfterUpdate(this);
            });
        }

        /// <summary>
        /// Disposes the scene
        /// </summary>
        /// <param name="renderer"></param>
        public override void DisposeScene(IRenderDevice renderer)
        {
            // Dispose the layers of the scene
            this.Layers.ForEach(l =>
            {
                l.Dispose(this, renderer);
            });

            // Call the dispose behaviors for the scene
            this.SceneBehaviors.ForEach(behavior => {
                behavior.OnDispose(this, renderer);
            });

            // Dispose the render target of the scene
            _renderTarget.Dispose(renderer);

            // Dispose the light manager
            this.LightManager.Dispose(renderer);
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
                this.SceneBehaviors.ForEach(b => b.BeforePhysicsUpdate(this, this.PhysicsHandler));

                // Process the physics handler
                this.PhysicsHandler.Process(this);
                _physicsAccumulator -= this.PhysicsHandler.FixedTimeStep;

                // After physics update behaviors
                this.SceneBehaviors.ForEach(b => b.AfterPhysicsUpdate(this, this.PhysicsHandler));
            }
        }

    }
}
