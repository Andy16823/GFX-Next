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
        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            if(typeof(T) == typeof(SkinnedMeshModel))
            {
                var model = new SkinnedMeshModel(path);
                return (T)(object)model;
            }
            throw new NotSupportedException($"SkinnedMeshModelLoader cannot load assets of type {typeof(T).FullName}.");
        }
    }
}
