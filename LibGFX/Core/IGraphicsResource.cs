using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    /// <summary>
    /// Defines the contract for a graphics resource that can be initialized and disposed using a render device.
    /// </summary>
    /// <remarks>Implementations of this interface represent resources that require explicit initialization
    /// and cleanup with a specific render device. The lifecycle of the resource is managed through the Init and Dispose
    /// methods, which must be called with a valid render device before and after use, respectively.
    /// The initialization is strict. An IGraphicsResource must be initialized with a render device before it can be used. 
    /// Ownership for IGraphicsResource is always the user of the resource, not the IRenderDevice.
    /// </remarks>
    public interface IGraphicsResource
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

        /// <summary>
        /// Releases any CPU resources that are currently allocated by the component.
        /// </summary>
        /// <remarks>Call this method when the component no longer needs to perform CPU-intensive
        /// operations to allow the system to reclaim resources. After calling this method, the component may not be
        /// able to perform certain operations until resources are reallocated.</remarks>
        void FreeCPUResources();
    }
}
