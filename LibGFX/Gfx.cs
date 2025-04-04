using LibGFX.Core;
using LibGFX.Graphics;
using OpenTK.Windowing.Common;
using System.Reflection.Metadata;

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

        private static readonly object _lock = new object();
        private static GFX _instance;
        private Window? _window;
        private Dictionary<String, Object> _assets;

        /// <summary>
        /// Private constructor to prevent instantiation from outside.
        /// </summary>
        private GFX()
        {
            _assets = new Dictionary<string, object>();
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
        /// Adds an asset to the asset manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="asset"></param>
        /// <exception cref="ArgumentException"></exception>
        public void AddAsset<T>(string name, T asset)
        {
            if (!_assets.TryAdd(name, asset))
            {
                throw new ArgumentException($"An asset with the name '{name}' already exists.");
            }
        }

        /// <summary>
        /// Gets an asset from the asset manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public T GetAsset<T>(string name)
        {
            if (_assets.TryGetValue(name, out var asset))
            {
                return (T)asset;
            }
            throw new KeyNotFoundException($"Asset with the name '{name}' was not found.");
        }

        /// <summary>
        /// Loads a texture from the specified path and adds it to the asset manager.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public Texture LoadTexture(String path)
        {
            if (File.Exists(path))
            {
                var filename = Path.GetFileName(path);
                var texture = Texture.LoadTexture(path);
                this.AddAsset<Texture>(filename, texture);
                return texture;
            }
            throw new FileNotFoundException($"File {path} does not exist.");
        }

        /// <summary>
        /// Disposes the GFX instance and clears all assets.
        /// </summary>
        public void Dispose()
        {
            _assets.Clear();
        }
    }
}
