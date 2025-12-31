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
        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            if (typeof(T) == typeof(SGMaterial))
            {
                return (T)(object)SGMaterial.LoadFromFile(path);
            }
            throw new NotSupportedException($"Asset type '{typeof(T)}' is not supported.");
        }
    }
}
