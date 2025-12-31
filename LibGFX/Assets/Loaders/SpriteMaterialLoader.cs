using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets.Loaders
{
    public class SpriteMaterialLoader : IAssetLoader
    {
        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            if(typeof(T) == typeof(SpriteMaterial))
            {
                var spriteMaterial = new SpriteMaterial();
                spriteMaterial.Texture = new Texture(path);
                return (T)(object)spriteMaterial;
            }
            throw new NotSupportedException($"Type {typeof(T)} is not supported by {nameof(SpriteMaterialLoader)}.");
        }
    }
}
