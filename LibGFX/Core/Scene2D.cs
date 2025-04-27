using LibGFX.Graphics;
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
        private RenderTarget _renderTarget;

        /// <summary>
        /// Creates a new 2D scene
        /// </summary>
        public Scene2D() : base()
        {
            
        }

        /// <summary>
        /// Creates a new 2D scene with the given layers
        /// </summary>
        /// <param name="layers"></param>
        public Scene2D(params String[] layers) : base()
        {
            foreach (var item in layers)
            {
                this.Layers.Add(new Layer(item));
            }
        }

        /// <summary>
        /// Initializes the scene
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void Init(Viewport viewport, IRenderDevice renderer)
        {
            var renderTargetDescriptor = new RenderTargetDescriptor()
            {
                Width = viewport.Width,
                Height = viewport.Height,
                Border = 0
            };
            _renderTarget = renderer.CreateRenderTarget(renderTargetDescriptor);

            this.Layers.ForEach(l =>
            {
                l.Init(this, viewport, renderer);
            });
        }

        /// <summary>
        /// Renders the scene
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void Render(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            var depthTest = renderer.IsDepthTestEnabled();

            // Disable depth test and set the viewport, projection and view matrix
            renderer.DisableDepthTest();
            renderer.SetViewport(viewport);
            renderer.SetProjectionMatrix(camera.GetProjectionMatrix(viewport));
            renderer.SetViewMatrix(camera.GetViewMatrix());

            // Render the scene to the render target
            renderer.ResizeRenderTarget(_renderTarget, viewport.Width, viewport.Height);
            renderer.BindRenderTarget(_renderTarget);
            renderer.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);


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

            // Unbind the render target and set the depth test state back to the original state
            renderer.UnbindRenderTarget();

            renderer.BindShaderProgram(renderer.GetShaderProgram("ScreenShader"));
            renderer.DrawRenderTarget(_renderTarget);  
            renderer.UnbindShaderProgram();

            renderer.SetDepthTest(depthTest);
        }

        /// <summary>
        /// Updates the scene
        /// </summary>
        public override void Update()
        {
            this.Layers.ForEach(l => { 
                l.Update(this); 
            });
        }

        /// <summary>
        /// Disposes the scene
        /// </summary>
        /// <param name="renderer"></param>
        public override void DisposeScene(IRenderDevice renderer)
        {
            this.Layers.ForEach(l =>
            {
                l.Dispose(this, renderer);
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
