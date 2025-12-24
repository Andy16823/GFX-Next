using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Materials
{

    /// <summary>
    /// Class for importing materials using registered importers.
    /// </summary>
    public class MaterialImporter
    {
        private Dictionary<Type, IMaterialImporter> Importers { get; } = new Dictionary<Type, IMaterialImporter>();

        public void RegisterImporter<T>(IMaterialImporter importer) where T : IMaterial
        {
            var type = typeof(T);
            if (!Importers.ContainsKey(type))
            {
                Importers[type] = importer;
            }
            else
            {
                throw new InvalidOperationException($"An importer for material type '{type}' is already registered.");
            }
        }

        public IMaterial ImportAssimpMaterial<T>(Assimp.Material asmat, String directory) where T : IMaterial
        {
            var type = typeof(T);
            if (Importers.ContainsKey(type))
            {
                return Importers[type].ImportAssimpMaterial(asmat, directory);
            }
            throw new NotSupportedException($"Material type '{type}' is not supported.");
        }
    }
}
