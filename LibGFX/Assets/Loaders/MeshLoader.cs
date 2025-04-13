using Assimp.Configs;
using Assimp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets.Loaders
{
    public class MeshLoader : IAssetLoader
    {
        public bool ShouldCache => true;

        public T Load<T>(string path) where T : class
        {
            var directory = Path.GetDirectoryName(path);

            // Load the model using Assimp
            var importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            var assimpScene = importer.ImportFile(path, Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.CalculateTangentSpace | Assimp.PostProcessSteps.JoinIdenticalVertices);




            throw new NotImplementedException();
        }
    }
}
