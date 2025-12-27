using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    /// <summary>
    /// Defines methods to serialize an object to a JSON representation and to deserialize an object from a JSON
    /// representation.
    /// </summary>
    /// <remarks>Implement this interface to enable custom serialization and deserialization of objects using
    /// JSON. The interface uses the JObject type from the Newtonsoft.Json.Linq namespace to represent JSON
    /// data.</remarks>
    public interface ISerialization
    {
        /// <summary>
        /// Serializes the current object to a new JSON object representation.
        /// </summary>
        /// <returns>A <see cref="JObject"/> containing the serialized data of the current object.</returns>
        JObject Serialize(SerializationContext serializationContext);

        /// <summary>
        /// Populates the current object with values from the specified JSON object.
        /// </summary>
        /// <param name="jObject">A <see cref="JObject"/> containing the JSON data to deserialize into the current object. Cannot be null.</param>
        void Deserialize(JObject jObject, SerializationContext serializationContext);
    }
}
