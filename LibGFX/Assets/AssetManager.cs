using LibGFX.Assets.Loaders;
using LibGFX.Core;
using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <summary>
        /// Assembly path of the executing assembly.
        /// </summary>
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
        /// Adds the specified asset to the collection if an asset with the same name does not already exist.
        /// </summary>
        /// <typeparam name="T">The type of asset to add. Must implement <see cref="IIdentifier"/>.</typeparam>
        /// <param name="asset">The asset to add to the collection. Cannot be <see langword="null"/>.</param>
        /// <returns>The added asset of type <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="asset"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if an asset with the same name already exists in the collection.</exception>
        public T Add<T>(T asset) where T : class, IIdentifier
        {
            if (asset == null)
            {
                throw new ArgumentNullException(nameof(asset));
            }

            var key = (typeof(T), asset.Name);
            if (!_assets.TryAdd(key, asset))
            {
                throw new InvalidOperationException($"Asset with name '{asset.Name}' already exists.");
            }
            return (T)asset;
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

            var key = (typeof(T), path);
            _assets.Add(key, asset);

            return (T)asset;
        }

        /// <summary>
        /// Retrieves an asset of the specified type and name from the asset collection.
        /// </summary>
        /// <remarks>If multiple assets exist with the same name but different types, only the asset
        /// matching the specified type is returned. The method returns <see langword="null"/> if the asset does not
        /// exist or is not of the requested type.</remarks>
        /// <typeparam name="T">The type of asset to retrieve. Must be a reference type.</typeparam>
        /// <param name="name">The name of the asset to retrieve. The name is case-sensitive.</param>
        /// <returns>The asset of type <typeparamref name="T"/> with the specified name, or <see langword="null"/> if no matching
        /// asset is found.</returns>
        public T ?Get<T>(string name) where T : class
        {
            var key = (typeof(T), name);
            if (_assets.TryGetValue(key, out var asset))
            {
                return (T)(object)asset;
            }
            return null;
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

        /// <summary>
        /// Determines whether an asset of the specified type and name exists in the collection.
        /// </summary>
        /// <typeparam name="T">The type of the asset to check for. Must be a reference type.</typeparam>
        /// <param name="name">The name of the asset to locate. The comparison is case-sensitive.</param>
        /// <returns>true if an asset of type T with the specified name exists; otherwise, false.</returns>
        public bool Exists<T>(string name) where T : class
        {
            return _assets.ContainsKey((typeof(T), name));
        }

        /// <summary>
        /// Initializes all assets that implement the IRenderResource interface using the specified render device.
        /// </summary>
        /// <remarks>Only assets that implement IRenderResource are initialized. Other assets are
        /// ignored.</remarks>
        /// <param name="renderer">The render device to use for initializing render resources. Cannot be null.</param>
        public void InitializeAssets(IRenderDevice renderer)
        {
            foreach (var asset in _assets.Values)
            {
                if (asset is IGraphicsResource renderResource)
                {
                    renderResource.Init(renderer);
                }
                else
                {
                    Debug.WriteLine($"Asset of type '{asset.GetType()}' does not implement IRenderResource. Skipping initialization.");
                }
            }
        }

        /// <summary>
        /// Frees the CPU resources associated with the managed assets.
        /// </summary>
        public void FreeCPUResources()
        {
            foreach (var asset in _assets.Values)
            {
                if (asset is IGraphicsResource renderResource)
                {
                    renderResource.FreeCPUResources();
                }
                else
                {
                    Debug.WriteLine($"Asset of type '{asset.GetType()}' does not implement IRenderResource. Skipping FreeCPURessources.");
                }
            }
        }

        /// <summary>
        /// Releases CPU-side resources for all assets of the specified type that implement IGraphicsResource.
        /// </summary>
        /// <remarks>This method iterates through all managed assets and calls FreeCPUResources on those
        /// that are both of type T and implement IGraphicsResource. Assets that do not meet these criteria are
        /// skipped.</remarks>
        /// <typeparam name="T">The type of asset for which to free CPU resources. Must implement IRendererResource.</typeparam>
        public void FreeCPUResources<T>() where T : IRendererResource
        {
            foreach (var asset in _assets.Values)
            {
                if (asset is T && asset is IGraphicsResource renderResource)
                {
                    renderResource.FreeCPUResources();
                }
                else
                {
                    Debug.WriteLine($"Asset of type '{asset.GetType()}' does not implement IRenderResource or is not of type '{typeof(T)}'. Skipping FreeCPURessources.");
                }
            }
        }

        /// <summary>
        /// Releases all graphics resources associated with the managed assets using the specified rendering device.
        /// </summary>
        /// <remarks>Only assets that implement the IGraphicsResource interface are disposed. Assets that
        /// do not implement this interface are ignored.</remarks>
        /// <param name="renderer">The rendering device to use when disposing of graphics resources. Cannot be null.</param>
        public void DisposeAssets(IRenderDevice renderer)
        {
            foreach (var asset in _assets.Values)
            {
                if (asset is IGraphicsResource renderResource)
                {
                    renderResource.Dispose(renderer);
                }
                else
                {
                    Debug.WriteLine($"Asset of type '{asset.GetType()}' does not implement IRenderResource. Skipping disposal.");
                }
            }
        }

        /// <summary>
        /// Removes all assets from the collection.
        /// </summary>
        public void ClearAssets()
        {
            _assets.Clear();
        }
    }
}
