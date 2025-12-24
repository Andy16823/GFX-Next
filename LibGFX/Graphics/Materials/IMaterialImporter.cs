using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Materials
{
    /// <summary>
    /// Interface for material importers.
    /// </summary>
    public interface IMaterialImporter
    {
        /// <summary>
        /// Imports an Assimp material into the corresponding material type.
        /// </summary>
        /// <param name="asmat"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        IMaterial ImportAssimpMaterial(Assimp.Material asmat, String directory);
    }
}
