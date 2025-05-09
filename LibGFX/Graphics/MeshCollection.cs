using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents a collection of meshes and materials.
    /// </summary>
    public class MeshCollection : IEnumerable<Mesh>
    {
        /// <summary>
        /// The collection of meshes.
        /// </summary>
        private readonly Dictionary<String, Mesh> _meshes = new();

        /// <summary>
        /// The collection of materials.
        /// </summary>
        //public readonly Dictionary<String, Material> _materials;

        /// <summary>
        /// The number of meshes and materials in the collection.
        /// </summary>
        public int Count => this._meshes.Count;

        /// <summary>
        /// Gets the mesh at the specified index.
        /// </summary>
        /// <param index="index"></param>
        /// <returns></returns>
        public Mesh this[int index] => this.GetMesh(index);

        /// <summary>
        /// Gets or sets the mesh by meshKey.
        /// </summary>
        /// <param meshKey="meshKey"></param>
        /// <returns></returns>
        public Mesh this[string meshKey]
        {
            get => this.GetMesh(meshKey);
            set => this.Set(meshKey, value);
        }

        /// <summary>
        /// Adds a mesh to the collection.
        /// </summary>
        /// <param meshKey="mesh"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Add(Mesh mesh)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException(nameof(mesh));
            }
            
            if(!this._meshes.TryAdd(mesh.ID.ToString(), mesh))
            {
                throw new ArgumentException($"Mesh with name {mesh.Name} already exists.");
            }
        }

        /// <summary>
        /// Checks if a mesh exists in the collection by meshKey.
        /// </summary>
        /// <param meshKey="meshKey"></param>
        /// <returns></returns>
        public bool Exists(string meshKey)
        {
            return _meshes.ContainsKey(meshKey);
        }

        /// <summary>
        /// Sets a mesh in the collection by meshKey. If the mesh does not exist, it will be added.
        /// </summary>
        /// <param meshKey="meshKey"></param>
        /// <param meshKey="mesh"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Set(string meshKey, Mesh mesh)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException(nameof(mesh));
            }
            if (_meshes.ContainsKey(meshKey))
            {
                _meshes[meshKey] = mesh;
            }
            else
            {
                this.Add(mesh);
            }
        }

        /// <summary>
        /// Adds a range of meshes to the collection.
        /// </summary>
        /// <param meshKey="meshes"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddRange(IEnumerable<Mesh> meshes)
        {
            if (meshes == null)
            {
                throw new ArgumentNullException(nameof(meshes));
            }
            foreach (var mesh in meshes)
            {
                this.Add(mesh);
            }
        }

        /// <summary>
        /// Gets a mesh by meshKey.
        /// </summary>
        /// <param meshKey="meshKey"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public Mesh GetMesh(String meshKey)
        {
            if (_meshes.TryGetValue(meshKey, out var mesh))
            {
                return mesh;
            }
            throw new KeyNotFoundException($"Mesh with key {meshKey} not found.");
        }

        /// <summary>
        /// Gets a mesh by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Mesh GetMesh(int index)
        {
            if (index < 0 || index >= _meshes.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
            return _meshes.ElementAt(index).Value;
        }

        /// <summary>
        /// Finds all meshes by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<Mesh> FindMeshesByName(string name)
        {
            foreach (var mesh in _meshes.Values)
            {
                if (mesh.Name == name)
                {
                    yield return mesh;
                }
            }
        }

        /// <summary>
        /// Clears the collection of meshes and materials.
        /// </summary>
        public void Clear()
        {
            _meshes.Clear();
        }

        /// <summary>
        /// Removes a mesh by meshKey.
        /// </summary>
        /// <param meshKey="meshKey"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void Remove(String meshKey)
        {
            if (!_meshes.Remove(meshKey))
            {
                throw new KeyNotFoundException($"Mesh with key {meshKey} not found.");
            }
        }

        /// <summary>
        /// Checks if a mesh exists in the collection.
        /// </summary>
        /// <param meshKey="meshKey"></param>
        /// <returns></returns>
        public bool Contains(String meshKey)
        {
            return _meshes.ContainsKey(meshKey);
        }

        /// <summary>
        /// Gets all meshes in the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Mesh> GetAll()
        {
            return _meshes.Values;
        }

        /// <summary>
        /// Iterates over all meshes in the collection and applies the action to each mesh.
        /// </summary>
        /// <param meshKey="action"></param>
        public void ForEach(Action<Mesh> action)
        {
            foreach (var mesh in _meshes.Values)
            {
                action(mesh);
            }
        }

        /// <summary>
        /// Applies an action to a single mesh in the collection by index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="action"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public void SingleMeshAction(int index, Action<Mesh> action)
        {
            if (index < 0 || index >= _meshes.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
            var mesh = _meshes.ElementAt(index).Value;
            if (mesh != null)
            {
                action(mesh);
            }
            else
            {
                throw new KeyNotFoundException($"Mesh with index {index} not found.");
            }
        }

        /// <summary>
        /// Applies an action to a single mesh in the collection by meshKey.
        /// </summary>
        /// <param name="meshKey"></param>
        /// <param name="action"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void SingleMeshAction(String meshKey, Action<Mesh> action)
        {
            if (_meshes.TryGetValue(meshKey, out var mesh))
            {
                action(mesh);
            }
            else
            {
                throw new KeyNotFoundException($"Mesh with key {meshKey} not found.");
            }
        }

        /// <summary>
        /// Applies an action to a single mesh in the collection by name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public void MeshActionByName(String name, Action<Mesh> action)
        {
            foreach (var mesh in _meshes.Values)
            {
                if (mesh.Name == name)
                {
                    action(mesh);
                }
            }
        }

        /// <summary>
        /// Gets the enumerator for the collection of meshes.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Mesh> GetEnumerator()
        {
            return _meshes.Values.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator for the collection of meshes as a non-generic enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
