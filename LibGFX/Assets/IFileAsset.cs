using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets
{
    /// <summary>
    /// File-based asset interface. 
    /// This interface extends the IAsset interface and adds functionality for loading assets from files. 
    /// It includes a property for the file path and a method to load the asset data from the specified file. 
    /// This allows for a standardized way to handle assets that are stored as files, such as textures, models, or audio files.
    /// </summary>
    public interface IFileAsset : IAsset
    {
        /// <summary>
        /// Gets or sets the full file system path associated with this instance.
        /// </summary>
        public String FilePath { get; set; }

        /// <summary>
        /// Loads the asset data from the file specified by the FilePath property. 
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadFromFile(String filePath);
    }
}
