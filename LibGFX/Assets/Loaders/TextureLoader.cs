using LibGFX.Core;
using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets.Loaders
{
    public class TextureLoader : IAssetLoader
    {
        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            if (typeof(T) == typeof(Texture))
            {
                return (T)(object)new Texture(path);
            }
            throw new NotSupportedException($"Asset type '{typeof(T)}' is not supported.");
        }
    }
}
