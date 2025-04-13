using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets.Loaders
{
    /// <summary>
    /// Loader for texture assets.
    /// </summary>
    public class TextureLoader : IAssetLoader
    {
        public bool ShouldCache => true;

        public T Load<T>(string path) where T : class
        {
            if (typeof(T) == typeof(Texture))
            {
                return Texture.LoadTexture(path) as T;
            }
            throw new NotSupportedException($"Asset type '{typeof(T)}' is not supported.");
        }
    }
}
