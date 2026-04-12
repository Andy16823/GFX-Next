using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets
{
    /// <summary>
    /// Interface for file assets.
    /// </summary>
    public interface IFileAsset : IAsset
    {
        /// <summary>
        /// Gets or sets the full file system path associated with this instance.
        /// </summary>
        public String FilePath { get; set; }

        public void LoadFromFile(String filePath);
    }
}
