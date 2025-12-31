using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets.Loaders
{
    /// <summary>
    /// Loader for static mesh models
    /// </summary>
    public class StaticMeshModelLoader : IAssetLoader
    {
        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            if (typeof(T) == typeof(StaticMeshModel))
            {
                var model = new StaticMeshModel(path);
                return (T)(object)model;
            }
            throw new NotSupportedException($"StaticMeshModelLoader cannot load assets of type {typeof(T).FullName}");
        }
    }
}
