using LibGFX.Core;
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
    /// Defines the contract for a material used in rendering operations, including identification, configuration, and
    /// lifecycle management.
    /// </summary>
    /// <remarks>Implementations of this interface represent materials that can be initialized, used, and
    /// disposed of with a rendering device. The interface also provides a static method for loading a material from an
    /// external source. Implementers should ensure thread safety if materials are accessed concurrently.</remarks>
    public interface IMaterial : IGraphicsResource, IIdentifier, ISerialization
    {
        public bool IsTransparent { get; }

        /// <summary>
        /// Configures the current instance to use the specified render device for rendering operations.
        /// </summary>
        /// <param name="renderDevice">The render device to be used. Cannot be null.</param>
        public void Use(IRenderDevice renderDevice);

        /// <summary>
        /// Disables the Material after rendering operations are complete.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Disable(IRenderDevice renderDevice);

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
        public abstract void LoadMaterial(Assimp.Material asmat, String directory);
    }
}
