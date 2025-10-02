using Assimp.Configs;
using LibGFX.Graphics.Animation3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets.Loaders
{
    public class Animation3DLoader : IAssetLoader
    {
        public bool ShouldCache => true;

        public bool CanCreate => false;

        public T Create<T>(string id, Action<T>? initializer = null, object? creationArgs = null) where T : class
        {
            throw new NotImplementedException();
        }

        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            if (typeof(T) != typeof(List<Animation>))
                throw new InvalidOperationException($"AnimationLoader can only load assets of type {typeof(List<Animation>)}, but got {typeof(T)}.");

            List<Animation> animations = new List<Animation>();

            var importer = new Assimp.AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            var assimpScene = importer.ImportFile(path, Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.CalculateTangentSpace | Assimp.PostProcessSteps.JoinIdenticalVertices);

            for (int i = 0; i < assimpScene.AnimationCount; i++)
            {
                var animation = new Animation(assimpScene, i);
                animations.Add(animation);
            }

            return animations as T;
        }
    }
}
