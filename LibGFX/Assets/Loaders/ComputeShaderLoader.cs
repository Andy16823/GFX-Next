using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets.Loaders
{
    public class ComputeShaderLoader : IAssetLoader
    {
        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            if (typeof(T) == typeof(Compute.ComputeShader))
            {
                string shaderSource = System.IO.File.ReadAllText(path);
                Compute.ComputeShader computeShader = new Compute.ComputeShader
                {
                    Name = System.IO.Path.GetFileNameWithoutExtension(path),
                    ShaderSource = shaderSource,
                    Invocations = 1
                };
                return (T)(object)computeShader;
            }
            throw new NotSupportedException($"ComputeShaderLoader does not support loading assets of type {typeof(T).FullName}");
        }
    }
}
