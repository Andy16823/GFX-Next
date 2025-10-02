using LibGFX.Graphics.Materials;
using NAudio.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets.Loaders
{
    /// <summary>
    /// Loader for material assets.
    /// </summary>
    public class SGMaterialLoader : IAssetLoader
    {
        /// <summary>
        /// Indicates whether the asset should be cached.
        /// </summary>
        public bool ShouldCache => true;

        /// <summary>
        /// Indicates whether the asset loader can create new assets.
        /// </summary>
        public bool CanCreate => true;

        /// <summary>
        /// Creates an new material asset with the specified ID and optional initializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="initializer"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public T Create<T>(string id, Action<T>? initializer = null, object? creationArgs = null) where T : class
        {
            if(typeof(T) == typeof(SGMaterial))
            {
                var material = new SGMaterial();
                initializer?.Invoke(material as T);
                return material as T;
            }
            throw new NotSupportedException($"Asset type '{typeof(T)}' is not supported.");
        }

        /// <summary>
        /// Loads a material asset from the specified path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            if (typeof(T) == typeof(SGMaterial))
            {
                var name = Path.GetFileNameWithoutExtension(path);
                return SGMaterial.LoadFromFile(path) as T;
            }
            throw new NotSupportedException($"Asset type '{typeof(T)}' is not supported.");
        }
    }
}
