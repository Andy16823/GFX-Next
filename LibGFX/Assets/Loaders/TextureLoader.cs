using LibGFX.Core;
using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets.Loaders
{
    /// <summary>
    /// Arguments for creating a texture with the asset loader.
    /// </summary>
    public struct TextureCreatingArgs
    {
        public int Width;
        public int Height;
    }

    /// <summary>
    /// Loader for texture assets.
    /// </summary>
    public class TextureLoader : IAssetLoader
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
        /// Creates a new texture asset with the specified ID and optional initializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="initializer"></param>
        /// <param name="creationArgs"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public T Create<T>(string id, Action<T>? initializer = null, object? creationArgs = null) where T : class
        {
            if (typeof(T) == typeof(Texture))
            {
                var width = 1;
                var height = 1;

                if(creationArgs != null && creationArgs is TextureCreatingArgs args)
                {
                    width = args.Width;
                    height = args.Height;
                }

                var texture = Texture.EmptyTexture(width, height);
                initializer?.Invoke(texture as T);
                return texture as T;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads a texture asset from the specified path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            if (typeof(T) == typeof(Texture))
            {
                return Texture.LoadTexture(path) as T;
            }
            throw new NotSupportedException($"Asset type '{typeof(T)}' is not supported.");
        }
    }
}
