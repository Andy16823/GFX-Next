using LibGFX.Core;
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
    public interface IModel : IGraphicsResource
    {
        /// <summary>
        /// The meshes contained in this model, indexed by their names
        /// </summary>
        public Dictionary<string, Mesh> Meshes { get; set; }

        /// <summary>
        /// The node structure of this model
        /// </summary>
        public SceneNodeData NodeStructure { get; set; }
    }
}
