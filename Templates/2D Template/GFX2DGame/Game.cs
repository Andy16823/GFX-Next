using LibGFX.Graphics;
using LibGFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGFX.Core;
using LibGFX.Graphics.Lights;
using OpenTK.Mathematics;
using LibGFX.Physics;
using System.Diagnostics;
using LibGFX.Graphics.Materials;
using LibGFX.Core.GameElements;
using LibGFX.Graphics.Renderer.OpenGL;

namespace GFX2DGame
{
    public class Game
    {
        private Window _window;
        private GLRenderer _renderer;
        private Scene2D _scene;
        private PhysicsHandler2D _physicsHandler;
        private OrthographicCamera _camera;

        public void Initialize()
        {
            // Create an new game window
            var viewport = new Viewport(800, 600);
            _window = GFX.Instance.CreateWindow("GFX", viewport, Window.WindowState.Normal);

            // Load the assets
            var spriteMaterial = GFX.Instance.AssetManager.Load<SpriteMaterial>("Ressources/Logo.png");

            // Create an OpenGL renderer
            _renderer = new GLRenderer();
            _renderer.Init(_window);
            _renderer.UseVsync(true);

            // Create the camera for the rendering
            _camera = new OrthographicCamera(Vector2.Zero, new Vector2(viewport.Width, viewport.Height));
            _camera.SetAsCurrent();

            // Create the scene with layers
            _scene = new Scene2D("BASE_LAYER", "AI_LAYER", "PLAYER_LAYER", "ITEM_LAYER");
            _scene.SetDirectionalLight(new DirectionalLight2D(new Vector4(0.3f, 0.3f, 0.3f, 1.0f), 1.0f));
            _scene.AddPointLight(new PointLight2D(new Vector2(0.0f, 0.0f), new Vector3(0.85f, 0.85f, 0.85f), 128.0f, 1.0f));

            // Add a sprite to the scene
            var sprite = new Sprite("Logo", new Vector2(0.0f, 0.0f), new Vector2(256.0f, 256.0f), spriteMaterial);
            sprite.Shader = _renderer.GetShaderProgram("LitSpriteShader");
            sprite.UVTransform = new Vector4(1.0f, 1.0f, 0f, 0f);
            _scene.AddGameElement("BASE_LAYER", sprite);

            // Create an 2D physics handler with zero gravity
            _physicsHandler = new PhysicsHandler2D(Vector2.Zero);
            _scene.PhysicsHandler = _physicsHandler;

            // Initialize game objects and resources here
            GFX.Instance.AssetManager.ForeachAsset<IMaterial>(material =>
            {
                material.Init(_renderer);
            });
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
