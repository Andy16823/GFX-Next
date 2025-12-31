using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets.Loaders
{
    /// <summary>
    /// Loader for cubemap assets.
    /// </summary>
    public class CubemapLoader : IAssetLoader
    {
        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            if (typeof(T) == typeof(Cubemap))
            {
                return (T)(object)Cubemap.LoadCubemap(path);
            }
            throw new NotSupportedException($"Asset type '{typeof(T)}' is not supported.");
        }
    }
}
