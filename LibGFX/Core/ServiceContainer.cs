using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    /// <summary>
    /// Represents a service container that can hold and manage services.
    /// </summary>
    public class ServiceContainer
    {
        /// <summary>
        /// The dictionary that holds the services.
        /// </summary>
        private readonly Dictionary<(Type, string), object> _services = new();

        /// <summary>
        /// Adds a service to the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="service"></param>
        public void AddService<T>(string name, T service) where T : class
        {
            _services[(typeof(T), name)] = service;
        }

        /// <summary>
        /// Removes a service from the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetService<T>(string name) where T : class
        {
            _services.TryGetValue((typeof(T), name), out var service);
            return service as T;
        }

        /// <summary>
        /// Gets all services of a specific type from the container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAllServices<T>() where T : class
        {
            return _services
                .Where(kvp => kvp.Key.Item1 == typeof(T))
                .Select(kvp => kvp.Value as T);
        }
    }
}
