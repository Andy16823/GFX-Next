using LibGFX.Graphics.Animation3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Base interface for 3D models
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// The meshes contained in this model, indexed by their names
        /// </summary>
        public Dictionary<string, Mesh> Meshes { get; set; }

        /// <summary>
        /// The node structure of this model
        /// </summary>
        public AssimpNodeData NodeStructure { get; set; }

        /// <summary>
        /// Initializes the model for the given render device
        /// </summary>
        /// <param name="renderer"></param>
        public void Init(IRenderDevice renderer);

        /// <summary>
        /// Disposes the model for the given render device
        /// </summary>
        /// <param name="renderer"></param>
        public void Dispose(IRenderDevice renderer);
    }
}
