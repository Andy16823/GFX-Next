using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets.Loaders
{
    /// <summary>
    /// Interface for loading assets.
    /// </summary>
    public interface IAssetLoader
    {
        /// <summary>
        /// Indicates whether the asset should be cached.
        /// </summary>
        bool ShouldCache { get; }

        /// <summary>
        /// Indicates whether the asset loader can create new assets.
        /// </summary>
        bool CanCreate { get; }

        /// <summary>
        /// Loads an asset from the specified path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        T Load<T>(string path) where T : class;

        /// <summary>
        /// Creates an asset with the specified ID and optional initializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="initializer"></param>
        /// <returns></returns>
        T Create<T>(string id, Action<T>? initializer = null, object? creationArgs = null) where T : class;
    }
}
