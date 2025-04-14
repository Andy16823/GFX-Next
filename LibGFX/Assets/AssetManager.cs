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
        private readonly Dictionary<object, IAssetLoader> _loaders = new();
        private readonly Dictionary<string, object> _assets = new();

        /// <summary>
        /// Loads an asset from the specified path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T Load<T>(string path) where T : class
        {
            if (_assets.TryGetValue(path, out var asset))
            {
                return (T)asset;
            }
            asset = this.LoadAssetFromDisk<T>(path);
            
            return (T)asset;
        }

        /// <summary>
        /// Tries to load an asset from the specified path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        public bool TryLoad<T>(string path, out T? asset) where T : class
        {
            asset = this.Load<T>(path);
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
        public T LoadNew<T>(string path) where T : class
        {
            return this.LoadAssetFromDisk<T>(path);
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
        public void AddAsset<T>(string name, T asset) where T : class
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Asset name cannot be null or empty.", nameof(name));
            }
            if (asset == null)
            {
                throw new ArgumentNullException(nameof(asset));
            }
            if (!_assets.TryAdd(name, asset))
            {
                throw new InvalidOperationException($"Asset with name '{name}' already exists.");
            }
        }

        /// <summary>
        /// Loads an asset from disk using the registered loader for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private T LoadAssetFromDisk<T>(string path) where T : class
        {
            if (!_loaders.TryGetValue(typeof(T), out var loader))
            {
                throw new NotSupportedException($"No loader found for asset type '{typeof(T)}'.");
            }

            var asset = loader.Load<T>(path);
            if (asset == null)
            {
                throw new InvalidOperationException($"Failed to load asset from path '{path}'.");
            }

            if(loader.ShouldCache)
            {
                _assets.Add(path, asset);
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
                _assets.Add(id, asset);
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
            if (loader.ShouldCache && _assets.ContainsKey(id))
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
                _assets.Add(id, asset);
            }
            // Return true to indicate success
            return true;
        }

        /// <summary>
        /// Get all assets of a specific type from the asset manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAllAssets<T>() where T : class
        {
            return _assets.Values.OfType<T>();
        }

        /// <summary>
        /// Executes an action for each asset of a specific type in the asset manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public void ForeachAsset<T>(Action<T> action) where T : class
        {
            foreach (var asset in _assets.Values.OfType<T>())
            {
                action(asset);
            }
        }

        /// <summary>
        /// Unloads an asset from the asset manager.
        /// </summary>
        /// <param name="path"></param>
        public void UnloadAsset(string path)
        {
            if (_assets.ContainsKey(path))
            {
                _assets.Remove(path);
            }
        }

        /// <summary>
        /// Unloads all assets from the asset manager.
        /// </summary>
        public void UnloadAllAssets()
        {
            _assets.Clear();
        }
    }
}
