using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    /// <summary>
    /// Defines a contract for objects that require explicit initialization before use.
    /// </summary>
    /// <remarks>Implement this interface to indicate that an object must be initialized with a rendering
    /// device before it can be used. The initialization state can be queried via the IsInitialized property.</remarks>
    public interface IRenderResource
    {
        /// <summary>
        /// Gets a value indicating whether the object has been initialized and is ready for use.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Initializes the current instance using the specified render device.
        /// </summary>
        /// <param name="renderer">The render device to use for initialization. Cannot be null.</param>
        void Init(IRenderDevice renderer);

        /// <summary>
        /// Releases all resources used by the current instance and performs cleanup using the specified render device.
        /// </summary>
        /// <param name="renderer">The render device to use for releasing resources. Cannot be null.</param>
        void Dispose(IRenderDevice renderer);
    }
}
