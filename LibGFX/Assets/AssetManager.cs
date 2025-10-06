using LibGFX.Assets.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets
{
    /// <summary>
    /// AssetManager is responsible for loading and managing assets in the application.
    /// </summary>
    public class AssetManager
    {
        public String AssemblyPath { get => this.GetAssemblyPath(); }
        private readonly Dictionary<object, IAssetLoader> _loaders = new();
        private readonly Dictionary<(Type, string), object> _assets = new();

        /// <summary>
        /// Loads an asset from the specified path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            if (this.LooksLikeFilePath(path))
            {
                path = Path.IsPathRooted(path) ? path : Path.Combine(AssemblyPath, path);
            }

            var key = (typeof(T), path);
            if (_assets.TryGetValue(key, out var asset))
            {
                return (T)asset;
            }
            asset = this.LoadAssetFromDisk<T>(path, loadingArgs);

            return (T)asset;
        }

        /// <summary>
        /// Tries to load an asset from the specified path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        public bool TryLoad<T>(string path, out T? asset, object? loadingArgs = null) where T : class
        {
            asset = this.Load<T>(path, loadingArgs);
            if(asset == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Loads a new asset from the specified path, bypassing the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T LoadNew<T>(string path, object? loadingArgs = null) where T : class
        {
            return this.LoadAssetFromDisk<T>(path, loadingArgs);
        }

        /// <summary>
        /// Gets the asset count for a specific asset type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int GetAssetCount<T>() where T : class
        {
            return _assets.OfType<T>().Count();
        }

        /// <summary>
        /// Registers a loader for a specific asset type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loader"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void RegisterLoader<T>(IAssetLoader loader) where T : class
        {
            if (loader == null)
            {
                throw new ArgumentNullException(nameof(loader));
            }

            var assetType = typeof(T);
            if (_loaders.ContainsKey(assetType))
            {
                throw new InvalidOperationException($"Loader for asset type '{assetType}' is already registered.");
            }
            _loaders.Add(assetType, loader);
        }

        /// <summary>
        /// Adds an asset to the asset manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="asset"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public T Add<T>(string name, T asset) where T : class
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Asset name cannot be null or empty.", nameof(name));
            }
            if (asset == null)
            {
                throw new ArgumentNullException(nameof(asset));
            }

            var key = (typeof(T), name);
            if (!_assets.TryAdd(key, asset))
            {
                throw new InvalidOperationException($"Asset with name '{name}' already exists.");
            }
            return (T) asset;
        }

        /// <summary>
        /// Loads an asset from disk using the registered loader for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private T LoadAssetFromDisk<T>(string path, object? loadingArgs = null) where T : class
        {
            if (!_loaders.TryGetValue(typeof(T), out var loader))
            {
                throw new NotSupportedException($"No loader found for asset type '{typeof(T)}'.");
            }

            var asset = loader.Load<T>(path, loadingArgs);
            if (asset == null)
            {
                throw new InvalidOperationException($"Failed to load asset from path '{path}'.");
            }

            if(loader.ShouldCache)
            {
                var key = (typeof(T), path);
                _assets.Add(key, asset);
            }

            return (T)asset;
        }

        /// <summary>
        /// Creates a new asset of the specified type with the given ID and optional initializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="initializer"></param>
        /// <param name="creationArgs"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public T Create<T>(string id, Action<T>? initializer = null, object? creationArgs = null) where T : class
        {
            if (!_loaders.TryGetValue(typeof(T), out var loader))
            {
                throw new NotSupportedException($"No loader found for asset type '{typeof(T)}'.");
            }
            if (!loader.CanCreate)
            {
                throw new InvalidOperationException($"Loader for asset type '{typeof(T)}' does not support creation.");
            }
            var asset = loader.Create<T>(id, initializer, creationArgs);
            if(loader.ShouldCache)
            {
                var key = (typeof(T), id);
                _assets.Add(key, asset);
            }

            return (T)asset;
        }

        /// <summary>
        /// Tries to create a new asset of the specified type with the given ID and optional initializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="asset"></param>
        /// <param name="initializer"></param>
        /// <param name="creationArgs"></param>
        /// <returns></returns>
        public bool TryCreate<T>(string id, out T? asset, Action<T>? initializer = null, object? creationArgs = null) where T : class
        {
            // Check if an loader is registered for the asset type
            if (!_loaders.TryGetValue(typeof(T), out var loader))
            {
                asset = null;
                return false;
            }
            // Check if the loader can create assets
            if (!loader.CanCreate)
            {
                asset = null;
                return false;
            }
            // Check if an asset with the same ID already exists
            var key = (typeof(T), id);
            if (loader.ShouldCache && _assets.ContainsKey(key))
            {
                asset = null;
                return false;
            }
            // Create the asset
            asset = loader.Create<T>(id, initializer, creationArgs);
            // Check if the asset was created successfully
            if (asset == null)
            {
                return false;
            }
            // Cache the asset if the loader supports caching
            if (loader.ShouldCache)
            {
                _assets.Add(key, asset);
            }
            // Return true to indicate success
            return true;
        }

        /// <summary>
        /// Get all assets of a specific type from the asset manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAllAssets<T>(bool assignables = true) where T : class
        {
            Type targetType = typeof(T);

            foreach (var asset in _assets.Values)
            {
                if (assignables)
                {
                    if (targetType.IsAssignableFrom(asset.GetType()))
                    {
                        yield return asset as T;
                    }
                }
                else if (asset is T)
                {
                    yield return asset as T;
                }
            }
        }

        /// <summary>
        /// Executes an action for each asset of a specific type in the asset manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public void ForeachAsset<T>(Action<T> action) where T : class
        {
            Type targetType = typeof(T);
            foreach (var asset in _assets.Values)
            {
                if (targetType.IsAssignableFrom(asset.GetType()))
                {
                    action(asset as T);
                }
            }
        }

        /// <summary>
        /// Unloads an asset from the asset manager.
        /// </summary>
        /// <param name="path"></param>
        public void UnloadAsset(string path)
        {
            var key = (typeof(object), path);
            if (_assets.ContainsKey(key))
            {
                _assets.Remove(key);
            }
        }

        /// <summary>
        /// Unloads all assets from the asset manager.
        /// </summary>
        public void UnloadAllAssets()
        {
            _assets.Clear();
        }

        /// <summary>
        /// Checks if the input string looks like a file path.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool LooksLikeFilePath(string input)
        {
            return input.Contains("/") || input.Contains("\\") || Path.HasExtension(input);
        }

        /// <summary>
        /// Gets the assembly path of the executing assembly.
        /// </summary>
        /// <returns></returns>
        private String GetAssemblyPath()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var location = assembly.Location;
            return System.IO.Path.GetDirectoryName(location) ?? string.Empty;
        }
    }
}
