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
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    /// <summary>
    /// Represents a 3D scene
    /// </summary>
    public class Scene3D : BaseScene
    {
        /// <summary>
        /// The sun light of the scene
        /// </summary>
        public DirectionalLight Sun { get; set; }

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
        private RenderTarget _renderTarget;

        /// <summary>
        /// Creates a new 3D scene
        /// </summary>
        public Scene3D() : base()
        {

        }

        public Scene3D(params String[] layers) : base()
        {
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

            // Dispose the render target
            renderer.DisposeRenderTarget(_renderTarget);
        }

        /// <summary>
        /// Initializes the scene and all its layers
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void Init(Viewport viewport, IRenderDevice renderer)
        {
            // Create Render Target for the scene
            var renderTargetDescriptor = new RenderTargetDescriptor()
            {
                Width = viewport.Width,
                Height = viewport.Height,
                Border = 0
            };
            _renderTarget = renderer.CreateRenderTarget(renderTargetDescriptor);

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

            renderer.AddLightSource("Sun", this.Sun);
        }

        /// <summary>
        /// Renders the scene and all its layers
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void Render(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
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
            //renderer.Clear((int)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);


            // Render the enviroment texture if available
            if (this.Enviroment != null && this.RenderEnviromentTexture)
            {
                this.Enviroment.Render(renderer, camera, viewport);
            }

            this.Layers.ForEach(layer => {
                layer.RenderLayer(this, viewport, renderer, camera);
            });

            if(this.PhysicsHandler != null && this.PhysicsHandler.DebugPhysics)
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

            // Unbind the render target and set the depth test state back to the original state
            renderer.UnbindRenderTarget();
            renderer.SetDepthTest(dephTest);

            // Render the render target to the screen
            renderer.BindShaderProgram(renderer.GetShaderProgram("ScreenShader"));
            renderer.DrawRenderTarget(_renderTarget);
            renderer.UnbindShaderProgram();
        }

        /// <summary>
        /// Updates the scene and all its layers
        /// </summary>
        public override void Update()
        {
            this.Layers.ForEach(l => {
                l.Update(this);
            });
        }

        /// <summary>
        /// Updates the physics of the scene
        /// </summary>
        public override void UpdatePhysics()
        {
            this.PhysicsHandler.Process(this);
        }
    }
}
