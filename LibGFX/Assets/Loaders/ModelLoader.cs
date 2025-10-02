using LibGFX.Core.GameElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets.Loaders
{
    /// <summary>
    /// Loader for model assets.
    /// </summary>
    public class ModelLoader : IAssetLoader
    {
        /// <summary>
        /// Indicates whether the asset should be cached.
        /// </summary>
        public bool ShouldCache => false;

        /// <summary>
        /// Indicates whether the asset loader can create new assets.
        /// </summary>
        public bool CanCreate => false;

        /// <summary>
        /// Creates a new model asset with the specified ID and optional initializer.
        /// Note: This functionality is not supported in this loader.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="initializer"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public T Create<T>(string id, Action<T>? initializer = null, object? creationArgs = null) where T : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads a model asset from the specified path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            if (typeof(T) == typeof(Model))
            {
                var name = Path.GetFileNameWithoutExtension(path);
                return new Model(name, path) as T;
            }
            throw new NotSupportedException($"Asset type '{typeof(T)}' is not supported.");
        }
    }
}
