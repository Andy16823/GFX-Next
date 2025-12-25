using LibGFX.Graphics.Shader;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Materials
{
    /// <summary>
    /// Specifies the status flags that describe the state of a material resource.
    /// </summary>
    /// <remarks>Use this enumeration to indicate whether a material has been loaded, disposed, failed to
    /// load, or has no status. Multiple flags may be combined to represent composite states, depending on the
    /// implementation.</remarks>
    public enum MaterialFlags
    {
        None,
        Loaded,
        Disposed,
        Failed
    }

    /// <summary>
    /// Defines the contract for a material used in rendering operations, including identification, configuration, and
    /// lifecycle management.
    /// </summary>
    /// <remarks>Implementations of this interface represent materials that can be initialized, used, and
    /// disposed of with a rendering device. The interface also provides a static method for loading a material from an
    /// external source. Implementers should ensure thread safety if materials are accessed concurrently.</remarks>
    public interface IMaterial
    {
        /// <summary>
        /// Gets or sets the name associated with the object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the unique identifier for this instance.
        /// </summary>
        public Guid ID { get; }

        /// <summary>
        /// Gets or sets the set of flags that define the material's rendering properties.
        /// </summary>
        public MaterialFlags Flags { get; set; }

        /// <summary>
        /// Initializes the current instance with the specified render device.
        /// </summary>
        /// <param name="renderDevice">The render device to use for initialization. Cannot be null.</param>
        public void Init(IRenderDevice renderDevice);

        /// <summary>
        /// Configures the current instance to use the specified render device for rendering operations.
        /// </summary>
        /// <param name="renderDevice">The render device to be used. Cannot be null.</param>
        public void Use(IRenderDevice renderDevice);

        /// <summary>
        /// Releases all resources used by the specified render device.
        /// </summary>
        /// <param name="renderDevice">The render device to dispose. Cannot be null.</param>
        public void Dispose(IRenderDevice renderDevice);

        /// <summary>
        /// Creates an IMaterial instance from the specified Assimp material and associated resource directory.
        /// </summary>
        /// <remarks>Use this method to convert materials imported via Assimp into the application's
        /// material representation. The directory parameter should point to the location of any textures or external
        /// resources referenced by the material.</remarks>
        /// <param name="asmat">The Assimp.Material object containing material properties to be loaded.</param>
        /// <param name="directory">The directory path used to resolve any external resources referenced by the material. Cannot be null or
        /// empty.</param>
        /// <returns>An IMaterial instance representing the loaded material, initialized with properties from the specified
        /// Assimp material.</returns>
        public abstract static IMaterial LoadMaterial(Assimp.Material asmat, String directory);
    }
}
