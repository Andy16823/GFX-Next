using LibGFX.Graphics.Animation3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Specifies the state of a model within its lifecycle.
    /// </summary>
    /// <remarks>Use this enumeration to determine whether a model has not been initialized, is currently
    /// initialized, or has been disposed. The state can be used to guard operations that require the model to be in a
    /// specific lifecycle phase.</remarks>
    public enum ModelState
    {
        None,
        Initialized,
        Disposed
    }

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
        public SceneNodeData NodeStructure { get; set; }

        /// <summary>
        /// Gets the current state of the model.
        /// </summary>
        public ModelState State { get; }

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
