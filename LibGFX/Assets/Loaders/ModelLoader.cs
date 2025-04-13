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
        public bool ShouldCache => false;

        public T Load<T>(string path) where T : class
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
