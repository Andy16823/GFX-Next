using LibGFX.Core.GameElements;
using LibGFX.Core;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics;
using LibGFX.Pyhsics;
using LibGFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Assimp;
using System.Diagnostics;
using LibGFX.Graphics.Enviroment;
using LibGFX.Graphics.Primitives;

namespace GFX3DGame
{
    public class Game
    {
        private Window _window;
        private GLRenderer _renderer;
        private PerspectiveCamera _camera;
        private Scene3D _scene;
        private PhysicsHandler3D _physicsHandler;

        public Game()
        {
            // Initialize game components here
        }

        public void Initialize()
        {
            // Create an new game window
            var viewport = new Viewport(800, 600);
            _window = GFX.Instance.CreateWindow("GFX", viewport, OpenTK.Windowing.Common.WindowState.Normal);

            // Create an OpenGL renderer
            _renderer = new GLRenderer();
            _renderer.Init(_window);
            _renderer.UseVsync(true);

            // Create the camera for the rendering
            _camera = new PerspectiveCamera(new Vector3(0.0f, 5f, -7.0f), new Vector3(viewport.Width, viewport.Height, 0.0f));
            _camera.SetAsCurrent();

            // Create the scene with layers
            _scene = new Scene3D("BASE_LAYER", "AI_LAYER", "PLAYER_LAYER", "ITEM_LAYER");
            _scene.Enviroment = new ProceduralSky();
            _scene.SetDirectionalLight(new DirectionalLight(new Vector3(-0.2f, 1.0f, -0.3f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), 1.5f));
            _scene.AddPointLight(new PointLight3D(new Vector3(2f, 0f, 0f), new Vector4(0.8f, 0.0f, 0.0f, 1.0f), 4f, 30f));
            _scene.AddPointLight(new PointLight3D(new Vector3(-2f, 0f, 0f), new Vector4(0.0f, 0.8f, 0.0f, 1.0f), 4f, 30f));
            _scene.AddPointLight(new PointLight3D(new Vector3(0f, 2f, 0f), new Vector4(0.0f, 0.0f, 0.8f, 1.0f), 4f, 30f));

            // Load the assets
            var model = GFX.Instance.AssetManager.Load<Model>("Ressources/Lion/scene.gltf");
            model.Transform.Position = new Vector3(0.0f, 0.0f, 0.0f);
            model.Transform.Rotate(0.0f, -90f, 0.0f);
            model.Transform.Scale = new Vector3(1.5f, 1.5f, 1.5f);
            model.Shader = _renderer.GetShaderProgram("MeshShader");
            model.AddBehavior(new FlyCam());
            _scene.AddGameElement("BASE_LAYER", model);
            _camera.LookAt(model.Transform.Position);


            // Create an 2D physics handler with zero gravity
            _physicsHandler = new PhysicsHandler3D(Vector3.Zero);
            _scene.PhysicsHandler = _physicsHandler;

            // Initialize game objects and resources here
            GFX.Instance.AssetManager.ForeachAsset<IMaterial>(material =>
            {
                material.Init(_renderer);
            });

            // Initialize the scene
            _scene.Init(_window.GetViewport(), _renderer);
        }

        public void Run()
        {
            // Main game loop
            while (!_window.RequestClose())
            {
                var viewport = _window.GetViewport();
                _window.ProcessEvents();

                Update();
                Render(viewport);
            }

            Dispose();
        }

        public void Stop()
        {
            _window.Close();
        }

        private void Update()
        {
            // Update game logic here
            _scene.UpdatePhysics();
            _scene.Update();
        }

        private void Render(Viewport viewport)
        {
            // Clear the screen
            _renderer.MakeCurrent();
            _renderer.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            _renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);

            // Render game graphics here
            _scene.Render(viewport, _renderer, _camera);

            // Swap buffers
            _renderer.Flush();
            _renderer.SwapBuffers();

            // Check for errors
            Debug.WriteLine($"Render Error {_renderer.GetError()}");
        }

        private void Dispose()
        {
            _scene.DisposeScene(_renderer);
            _renderer.Dispose();
        }

    }
}
