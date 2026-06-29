using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    /// <summary>
    /// Provides a context for storing and retrieving key-value pairs during serialization operations.
    /// </summary>
    /// <remarks>Use this class to pass additional contextual information between serialization components.
    /// The context is typically used to share data that may influence how objects are serialized or deserialized. Keys
    /// are case-sensitive. This class is not thread-safe; synchronize access if used concurrently from multiple
    /// threads.</remarks>
    public class SerializationContext
    {
        /// <summary>
        /// Gets a collection of key-value pairs that provide additional contextual information associated with the
        /// current instance.
        /// </summary>
        /// <remarks>Use this property to store or retrieve custom data relevant to the context in which
        /// the instance is used. The dictionary is initialized and never null, but may be empty if no context data has
        /// been added.</remarks>
        public Dictionary<String, Object> ContextData { get; } = new Dictionary<String, Object>();
        
        /// <summary>
        /// Retrieves the value associated with the specified key, or returns a default value if the key is not found or
        /// cannot be cast to the specified type.
        /// </summary>
        /// <remarks>If the value associated with the key exists but is not of type T, the default value
        /// is returned instead of throwing an exception.</remarks>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="key">The key whose associated value is to be retrieved.</param>
        /// <param name="defaultValue">The value to return if the key does not exist or the value cannot be cast to type T.</param>
        /// <returns>The value associated with the specified key, cast to type T, if found and of the correct type; otherwise,
        /// the specified default value.</returns>
        public T GetValue<T>(String key, T defaultValue = default!)
        {
            if (ContextData.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets the first value in the context data that matches the specified type T, or returns a default value if no such value is found.
        /// Usefull when you accept any instance of a type in the context data without knowing the key. E.g. RenderShaders
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetFirstOfType<T>(T defaultValue = default!)
        {
            foreach (var value in ContextData.Values)
            {
                if (value is T typedValue)
                {
                    return typedValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets the first value in the context data that matches the specified type, or returns null if no such value is found.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object? GetFirstOfType(Type type)
        {
            foreach (var value in ContextData.Values)
            {
                if (type.IsInstanceOfType(value))
                {
                    return value;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the first value in the context data that matches the specified type full name, or returns null if no such value is found.
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <returns></returns>
        public object? GetFirstOfType(string typeFullName)
        {
            foreach (var value in ContextData.Values)
            {
                if (value.GetType().FullName == typeFullName)
                {
                    return value;
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the value associated with the specified key in the context data.
        /// </summary>
        /// <typeparam name="T">The type of the value to store.</typeparam>
        /// <param name="key">The key with which the value will be associated. Cannot be null.</param>
        /// <param name="value">The value to associate with the specified key.</param>
        public void SetValue<T>(String key, T value)
        {
            ContextData[key] = value!;
        }

        /// <summary>
        /// Sets multiple key-value pairs in the context data from the provided dictionary.
        /// </summary>
        /// <param name="values"></param>
        public void SetValues(Dictionary<String, Object> values)
        {
            foreach (var kvp in values)
            {
                ContextData[kvp.Key] = kvp.Value;
            }
        }
    }
}
