using LibGFX.Assets;
using LibGFX.Assets.Loaders;
using LibGFX.Audio;
using LibGFX.Core.GameElements;
using LibGFX.Graphics.Animation3D;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Shader;
using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGFX.Graphics.Enviroment;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Renderer.OpenGL;
using LibGFX.Physics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace LibGFX.Core
{
    /// <summary>
    /// Base class for creating a game application.
    /// </summary>
    public abstract class Game
    {
        /// <summary>
        /// The main application window.
        /// </summary>
        public Window Window { get; set; }

        /// <summary>
        /// The service container for managing services.
        /// </summary>
        public ServiceContainer Services { get; set; }

        /// <summary>
        /// The asset manager for managing assets.
        /// </summary>
        public AssetManager AssetManager { get; set; }

        /// <summary>
        /// The render device for rendering graphics.
        /// </summary>
        public IRenderDevice RenderDevice { get; set; }

        /// <summary>
        /// The viewport defining the rendering area.
        /// </summary>
        public Viewport Viewport { get; set; }

        /// <summary>
        /// Initializes a new instance of the Game class.
        /// </summary>
        protected Game()
        {
            this.AssetManager = GFX.Instance.AssetManager;
            this.Services = GFX.Instance.Services;
        }

        /// <summary>
        /// Runs the game application with the specified parameters.
        /// </summary>
        /// <param name="renderDevice"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="title"></param>
        /// <param name="windowState"></param>
        public void Run(IRenderDevice renderDevice, int width = 800, int height = 600, string title = "LibGFX Application", bool useVsync = true, Window.WindowState windowState = Window.WindowState.Normal)
        {
            // Initialize the render device and window
            this.RenderDevice = renderDevice;
            this.Viewport = new Viewport(width, height);
            this.Window = GFX.Instance.CreateWindow(title, this.Viewport, windowState);
            this.RenderDevice.Init(this.Window);
            this.RenderDevice.UseVsync(useVsync);

            // Initialize game elements
            this.Initialize();

            // Load content and assets
            this.LoadContent();

            // Call the OnStart method
            this.OnStart();

            while (!this.Window.RequestClose())
            {
                var deltaTime = (float)GLFW.GetTime();
                GLFW.SetTime(0);

                // Process window events and update game state
                this.Window.ProcessEvents();
                this.Update(deltaTime);

                // Render the current frame
                this.RenderDevice.MakeCurrent();
                this.RenderDevice.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
                this.RenderDevice.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);

                // Invoke the render method
                this.Render();

                // Swap buffers to display the rendered frame
                this.RenderDevice.Flush();
                this.RenderDevice.SwapBuffers();
            }

            // Dispose resources and clean up
            this.Dispose();
            this.RenderDevice.Dispose();
            Environment.Exit(0);
        }

        /// <summary>
        /// Initializes game elements and state.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Loads game content and assets.
        /// </summary>
        public abstract void LoadContent();

        /// <summary>
        /// Called when the game starts.
        /// </summary>
        public abstract void OnStart();
        /// <summary>
        /// Updates the game state based on the elapsed time since the last frame.
        /// </summary>
        /// <param name="deltaTime"></param>
        public abstract void Update(float deltaTime);

        /// <summary>
        /// Renders the current frame.
        /// </summary>
        public abstract void Render();

        /// <summary>
        /// Disposes of game resources and performs cleanup.
        /// </summary>
        public abstract void Dispose();
    }
}
