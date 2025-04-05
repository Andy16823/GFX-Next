using LibGFX.Core.GameElements;
using LibGFX.Core;
using LibGFX.Graphics.Primitives;
using LibGFX.Graphics;
using LibGFX.Pyhsics.Behaviors3D;
using LibGFX.Pyhsics;
using LibGFX.UI;
using LibGFX;
using System.Diagnostics;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Button = LibGFX.UI.Button;
using Label = LibGFX.UI.Label;

namespace GfxLib_3D_Template
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Define the resources path
            var ressources = Path.Combine(Application.StartupPath,"Resources");

            // Create the viewport, window and renderer
            var viewport = new Viewport(800, 600);
            var window = GFX.Instance.CreateWindow("GFX", viewport, OpenTK.Windowing.Common.WindowState.Normal);
            var renderer = new GLRenderer();
            renderer.Init(window);
            renderer.UseVsync(true);

            // Create an scene and an layer for the sprite
            var scene = new Scene3D();
            scene.Sun.Position = new Vector3(0, 500, 0);
            scene.Sun.Intensity = 0.5f;
            var layer = new Layer("BaseLayer");
            scene.Layers.Add(layer);

            // Create an physics handler for the scene
            var physicsHander = new PhysicsHandler3D(new Vector3(0, -9.8f, 0));
            scene.PhysicsHandler = physicsHander;

            // Create an camera for the rendering
            var camera = new PerspectiveCamera(Vector3.Zero, new Vector3(800, 600, 0));
            camera.SetAsCurrent();

            // Load an model from the resources
            var model = new Model("Test", Path.Combine(ressources, "Models/Girly/girly.fbx"));
            model.AnimationSpeed = 2.5f;
            model.Transform.Position = new Vector3(0, 0, 0);
            model.PlayAnimation("Run");
            layer.Elements.Add(model);

            // Add an capsule rigid body to the model
            var rigidbody = model.AddBehavior<CapsuleRigidBody>(new CapsuleRigidBody(scene.PhysicsHandler));
            rigidbody.Offset = new Vector3(0, 1, 0);
            rigidbody.CreateRigidBody(10f);


            // Create an cube for the ground
            var material = Material.LoadMaterial(Path.Combine(ressources, "Materials/Checker/material.json"));
            var cube = new Primitive("Cube", material, new Cube());
            cube.Transform.Position = new Vector3(0, -2, 0);
            cube.Transform.Scale = new Vector3(2.5f, 0.5f, 2.5f);
            cube.Transform.Rotate(new Vector3(0.0f, 45.0f, 0.0f));

            // Add an box collider to the cube
            var collider = cube.AddBehavior<BoxCollider>(new BoxCollider(scene.PhysicsHandler));
            collider.CreateCollider(0f);
            cube.AddBehavior<FlyCam>(new FlyCam());
            layer.Elements.Add(cube);

            // Create an canvas for the UI
            var font = renderer.LoadFont(Path.Combine(ressources, "Fonts/ARIAL.ttf"), 64);
            var canvas = new Canvas2D(0, 0, 800, 600);

            // Add an label, progress bar and buttons to the canvas
            var label = new Label("TestLabel", "Hello World", new Vector2(0, -170), new Vector2(300, 32), font, 0.2f);
            label.Name = "TestLabel";
            canvas.AddControl(label);

            var pbar = new Progressbar("TestProgressBar", new Vector2(0, -50), new Vector2(300, 32));
            pbar.Progress = 25.0f;
            canvas.AddControl(pbar);

            var button = new Button("RunButton", "Play The Run Animation", new Vector2(0, -100), new Vector2(300, 32), font, 0.2f);
            button.OnMouseDown += (sender, args) =>
            {
                model.PlayAnimation("Run");
            };
            canvas.AddControl(button);

            var button2 = new Button("IdleButton", "Play The Idle Animation", new Vector2(0, -140), new Vector2(300, 32), font, 0.2f);
            button2.OnMouseDown += (sender, args) =>
            {
                model.PlayAnimation("Idle");
            };
            canvas.AddControl(button2);

            // Add the canvas as an service to the GFX instance
            GFX.Instance.Services.AddService<Canvas2D>("hud", canvas);


            // Initialize the scene and canvas
            scene.Init(viewport, renderer);
            canvas.Init(renderer);

            // Enable alpha blending for the renderer
            renderer.EnableAlphaBlend();
            while (!window.RequestClose())
            {
                // Increment the progress bar
                pbar.Process(0.25f);

                // Get the current viewport and update the camera
                viewport = window.GetViewport();
                camera.Transform.Scale = new Vector3(viewport.Width, viewport.Height, 0);

                // Process the events for the window
                window.ProcessEvents();

                // Update the scene and canvas
                scene.UpdatePhysics();
                scene.Update();
                canvas.Update(window);

                // Clear the screen and render the scene and canvas
                renderer.MakeCurrent();
                renderer.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
                renderer.Clear((int)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
                scene.Render(viewport, renderer, Camera.Current);
                canvas.Render(viewport, renderer);
                renderer.Flush();
                renderer.SwapBuffers();

                //Debug.WriteLine($"Error {renderer.GetError()}");
            }
            canvas.Dispose(renderer);
            scene.DisposeScene(renderer);
            renderer.DisposeFont(font);
            renderer.Dispose();
        }
    }
}