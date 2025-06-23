using LibGFX.Graphics.Shader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets.Loaders
{
    /// <summary>
    /// Loader for shader assets.
    /// </summary>
    public class ShaderLoader : IAssetLoader
    {
        /// <summary>
        /// Indicates whether the asset should be cached.
        /// </summary>
        public bool ShouldCache => true;

        /// <summary>
        /// Indicates whether the asset loader can create new assets.
        /// </summary>
        public bool CanCreate => false;

        /// <summary>
        /// Creates a new shader asset with the specified ID and optional initializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="initializer"></param>
        /// <param name="creationArgs"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public T Create<T>(string id, Action<T>? initializer = null, object? creationArgs = null) where T : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads a shader asset from the specified path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public T Load<T>(string path) where T : class
        {
            if(typeof(T) == typeof(Shader))
            {
                var name = System.IO.Path.GetFileNameWithoutExtension(path);
                var shaderSource = System.IO.File.ReadAllText(path);
                return new Shader(shaderSource) as T;
            }
            throw new NotSupportedException($"Asset type '{typeof(T)}' is not supported.");
        }
    }
}
