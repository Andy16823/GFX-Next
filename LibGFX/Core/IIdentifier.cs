using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    /// <summary>
    /// Defines a contract for objects that have a unique identifier and a name.
    /// </summary>
    /// <remarks>Implementations of this interface can be used to represent entities that require both a
    /// human-readable name and a globally unique identifier. This is commonly used for objects that need to be
    /// referenced or tracked across different systems or contexts.</remarks>
    public interface IIdentifier
    {
        /// <summary>
        /// Gets or sets the name associated with the object.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        Guid ID { get; }
    }
}
