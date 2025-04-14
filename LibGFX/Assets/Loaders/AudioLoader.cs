using LibGFX.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets.Loaders
{
    /// <summary>
    /// Loader for audio assets.
    /// </summary>
    public class AudioLoader : IAssetLoader
    {
        public bool ShouldCache => true;

        public bool CanCreate => false;

        public T Create<T>(string id, Action<T>? initializer = null, object? creationArgs = null) where T : class
        {
            throw new NotImplementedException();
        }

        public T Load<T>(string path) where T : class
        {
           if(typeof(T) == typeof(AudioClip))
            {
                var name = Path.GetFileNameWithoutExtension(path);

                return new AudioClip(name, path) as T;
            }
            throw new NotSupportedException($"Asset type '{typeof(T)}' is not supported.");
        }
    }
}
