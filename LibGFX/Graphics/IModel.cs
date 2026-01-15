using LibGFX.Assets;
using LibGFX.Core;
using LibGFX.Graphics.Animation3D;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Shader;
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
    public interface IModel : IFileAsset, IGraphicsResource, IIdentifier, ISerialization
    {
        /// <summary>
        /// The meshes contained in this model, indexed by their names
        /// </summary>
        public List<(Mesh, IMaterial)> Meshes { get; set; }

        /// <summary>
        /// Gets a value indicating whether the content includes transparent regions.
        /// </summary>
        public bool HasTransparency { get; }

        /// <summary>
        /// The node structure of this model
        /// </summary>
        public SceneNodeData NodeStructure { get; set; }

        /// <summary>
        /// Assigns the given shader to all meshes in this model
        /// </summary>
        /// <param name="shader"></param>
        public void AssignShaderToMeshes(RenderShader shader);
    }
}
