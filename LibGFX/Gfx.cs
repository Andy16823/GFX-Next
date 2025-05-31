using LibGFX.Assets;
using LibGFX.Assets.Loaders;
using LibGFX.Audio;
using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
using System.Diagnostics;
using System.Reflection.Metadata;
using static LibGFX.Core.Window;

namespace LibGFX
{
    /// <summary>
    /// Singleton class for managing graphics resources and window creation.
    /// </summary>
    public class GFX
    {
        /// <summary>
        /// The service container for managing services.
        /// </summary>
        public ServiceContainer Services { get; } = new();

        /// <summary>
        /// The asset manager for managing assets.
        /// </summary>
        public AssetManager AssetManager { get; set; }

        private static readonly object _lock = new object();
        private static GFX _instance;
        private Window? _window;

        /// <summary>
        /// Private constructor to prevent instantiation from outside.
        /// </summary>
        private GFX()
        {
            Debug.WriteLine("GFX instance created.");
            AssetManager = new AssetManager();
            AssetManager.RegisterLoader<Texture>(new TextureLoader());
            AssetManager.RegisterLoader<AudioClip>(new AudioLoader());
            AssetManager.RegisterLoader<SGMaterial>(new SGMaterialLoader());
            AssetManager.RegisterLoader<Model>(new ModelLoader());
            AssetManager.RegisterLoader<Cubemap>(new CubemapLoader());
            AssetManager.RegisterLoader<MeshCollection>(new MeshLoader());
            AssetManager.RegisterLoader<SpriteMaterial>(new SpriteMaterialLoader());
        }

        /// <summary>
        /// Singleton instance of the GFX class.
        /// </summary>
        public static GFX Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new GFX();
                    }
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Creates a new window with the specified title, viewport, and window state.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="viewport"></param>
        /// <param name="windowState"></param>
        /// <returns></returns>
        public Window CreateWindow(String title, Viewport viewport, WindowState windowState)
        {
            if (_window == null)
            {
                _window = new Window(title, viewport, windowState);
            }
            return _window;
        }

        /// <summary>
        /// Gets the current window.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Window GetWindow()
        {
            return _window ?? throw new InvalidOperationException("The window has not been created yet.");
        }

        /// <summary>
        /// Disposes the GFX instance and clears all assets.
        /// </summary>
        public void Dispose()
        {
            AssetManager.UnloadAllAssets();
        }
    }
}
