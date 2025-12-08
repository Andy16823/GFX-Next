using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets.Loaders
{
    /// <summary>
    /// Loader for skinned mesh models.
    /// </summary>
    public class SkinnedMeshModelLoader : IAssetLoader
    {
        public bool ShouldCache => true;

        public bool CanCreate => false;

        public T Create<T>(string id, Action<T>? initializer = null, object? creationArgs = null) where T : class
        {
            throw new Exception("SkinnedMeshModelLoader does not support Create.");
        }

        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            if(typeof(T) == typeof(SkinnedMeshModel))
            {
                var model = new SkinnedMeshModel(path);
                return model as T;
            }
            throw new NotSupportedException($"SkinnedMeshModelLoader cannot load assets of type {typeof(T).FullName}.");
        }
    }
}
