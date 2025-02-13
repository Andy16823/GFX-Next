using LibGFX.Core;
using LibGFX.Graphics;
using OpenTK.Windowing.Common;
using System.Reflection.Metadata;

namespace LibGFX
{
    public class GFX
    {
        private static readonly object _lock = new object();
        private static GFX _instance;
        private Window? _window;
        private Dictionary<String, Object> _assets;

        private GFX()
        {
            _assets = new Dictionary<string, object>();
        }

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

        public Window CreateWindow(String title, Viewport viewport, WindowState windowState)
        {
            if (_window == null)
            {
                _window = new Window(title, viewport, windowState);
            }
            return _window;
        }

        public Window GetWindow()
        {
            return _window ?? throw new InvalidOperationException("The window has not been created yet.");
        }

        public void AddAsset<T>(string name, T asset)
        {
            if (!_assets.TryAdd(name, asset))
            {
                throw new ArgumentException($"An asset with the name '{name}' already exists.");
            }
        }

        public T GetAsset<T>(string name)
        {
            if (_assets.TryGetValue(name, out var asset))
            {
                return (T)asset;
            }
            throw new KeyNotFoundException($"Asset with the name '{name}' was not found.");
        }

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

        public void Dispose()
        {
            _assets.Clear();
        }
    }
}
