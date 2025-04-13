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
        public bool ShouldCache => true;

        public T Load<T>(string path) where T : class
        {
            if (typeof(T) == typeof(Cubemap))
            {
                var name = Path.GetFileNameWithoutExtension(path);
                return Cubemap.LoadCubemap(path) as T;
            }
            throw new NotSupportedException($"Asset type '{typeof(T)}' is not supported.");
        }
    }
}
