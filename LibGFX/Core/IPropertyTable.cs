using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    /// <summary>
    /// Represents a collection of named properties and their associated values.
    /// </summary>
    /// <remarks>Implementations typically use this interface to expose a flexible set of key-value pairs for
    /// extensibility or metadata purposes. The property names are case-sensitive. Modifying the returned dictionary may
    /// affect the underlying state of the implementing object, depending on the implementation.</remarks>
    public interface IPropertyTable
    {
        public Dictionary<String, Object> Properties { get; }
    }
}
