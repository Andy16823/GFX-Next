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
        public bool ShouldCache => true;

        public bool CanCreate => false;

        public T Create<T>(string id, Action<T>? initializer = null, object? creationArgs = null) where T : class
        {
            throw new Exception("StaticMeshModelLoader cannot create assets.");
        }

        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            if (typeof(T) == typeof(StaticMeshModel))
            {
                var model = new StaticMeshModel(path);
                return model as T;
            }
            throw new NotSupportedException($"StaticMeshModelLoader cannot load assets of type {typeof(T).FullName}");
        }
    }
}
