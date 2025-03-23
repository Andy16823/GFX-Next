﻿using LibGFX.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    public class Scene3D : BaseScene
    {
        public DirectionalLight Sun { get; set; }

        private RenderTarget _renderTarget;

        public Scene3D()
        {
            this.Sun = new DirectionalLight();
        }

        public override void DisposeScene(IRenderDevice renderer)
        {
            this.Layers.ForEach(l =>
            {
                l.Dispose(this, renderer);
            });
        }

        public override void Init(Viewport viewport, IRenderDevice renderer)
        {
            // Enable Depth Test for 3D
            renderer.EnableDepthTest();

            // Create Render Target for the scene
            var renderTargetDescriptor = new RenderTargetDescriptor()
            {
                Width = viewport.Width,
                Height = viewport.Height,
                Border = 0
            };
            _renderTarget = renderer.CreateRenderTarget(renderTargetDescriptor);

            // Init all layers and there elements
            this.Layers.ForEach(l =>
            {
                l.Init(this, viewport, renderer);
            });

            renderer.AddLightSource("Sun", this.Sun);
        }

        public override void Render(Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            renderer.SetViewport(viewport);
            renderer.SetProjectionMatrix(camera.GetProjectionMatrix(viewport));
            renderer.SetViewMatrix(camera.GetViewMatrix());

            // Render the scene to the render target
            renderer.ResizeRenderTarget(_renderTarget, viewport.Width, viewport.Height);
            renderer.BindRenderTarget(_renderTarget);
            renderer.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            renderer.Clear((int)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

            this.Layers.ForEach(layer => {
                layer.RenderLayer(this, viewport, renderer, camera);
            });

            if(this.PhysicsHandler.DebugPhysics)
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

            renderer.UnbindRenderTarget();

            // Clear Screen
            //renderer.GetFramebufferIndex();
            renderer.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            renderer.Clear((int)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
            renderer.BindShaderProgram(renderer.GetShaderProgram("ScreenShader"));
            renderer.DrawRenderTarget(_renderTarget);
            renderer.UnbindShaderProgram();

            //Debug.WriteLine($"error {renderer.GetError()}");
        }

        public override void Update()
        {
            this.Layers.ForEach(l => {
                l.Update(this);
            });
        }

        public override void UpdatePhysics()
        {
            this.PhysicsHandler.Process(this);
        }
    }
}
