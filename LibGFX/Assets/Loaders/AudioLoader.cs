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
        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            if (typeof(T) == typeof(AudioClip))
            {
                var name = Path.GetFileNameWithoutExtension(path);
                return (T)(object)new AudioClip(name, path);
            }
            throw new NotSupportedException($"Asset type '{typeof(T)}' is not supported.");
        }
    }
}
