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
        public bool ShouldCache => true;

        public bool CanCreate => false;

        public T Create<T>(string id, Action<T>? initializer = null, object? creationArgs = null) where T : class
        {
            throw new NotImplementedException();
        }

        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            if(typeof(T) == typeof(SpriteMaterial))
            {
                var spriteMaterial = new SpriteMaterial();
                spriteMaterial.Texture = Texture.LoadTexture(path);
                return spriteMaterial as T;
            }
            throw new NotSupportedException($"Type {typeof(T)} is not supported by {nameof(SpriteMaterialLoader)}.");
        }
    }
}
