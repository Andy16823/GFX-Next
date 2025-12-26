using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    /// <summary>
    /// Defines a resource that can be released or cleaned up by a renderer device.
    /// </summary>
    /// <remarks>Implement this interface for objects that hold resources tied to a specific rendering device,
    /// such as GPU buffers or textures. The resource should be disposed of using the provided renderer to ensure proper
    /// cleanup within the rendering context.
    /// The owner of an IRenderResource is always the IRenderDevice that created it.
    /// </remarks>
    public interface IRendererResource
    {
        void Dispose(IRenderDevice renderer);
    }
}
