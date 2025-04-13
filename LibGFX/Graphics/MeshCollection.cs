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
        /// <param name="index"></param>
        /// <returns></returns>
        public Mesh this[int index] => this.GetMesh(index);

        /// <summary>
        /// Gets or sets the mesh by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Mesh this[string name]
        {
            get => this.GetMesh(name);
            set => this.Set(name, value);
        }

        /// <summary>
        /// Adds a mesh to the collection.
        /// </summary>
        /// <param name="mesh"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Add(Mesh mesh)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException(nameof(mesh));
            }
            
            if(!this._meshes.TryAdd(mesh.Name, mesh))
            {
                throw new ArgumentException($"Mesh with name {mesh.Name} already exists.");
            }
        }

        /// <summary>
        /// Sets a mesh in the collection by name. If the mesh does not exist, it will be added.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mesh"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Set(string name, Mesh mesh)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException(nameof(mesh));
            }
            if (this._meshes.ContainsKey(name))
            {
                this._meshes[name] = mesh;
            }
            else
            {
                this.Add(mesh);
            }
        }

        /// <summary>
        /// Adds a range of meshes to the collection.
        /// </summary>
        /// <param name="meshes"></param>
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
        /// Gets a mesh by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public Mesh GetMesh(String name)
        {
            if (this._meshes.TryGetValue(name, out var mesh))
            {
                return mesh;
            }
            throw new KeyNotFoundException($"Mesh with name {name} not found.");
        }

        public Mesh GetMesh(int index)
        {
            if (index < 0 || index >= this._meshes.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
            return this._meshes.ElementAt(index).Value;
        }

        /// <summary>
        /// Clears the collection of meshes and materials.
        /// </summary>
        public void Clear()
        {
            this._meshes.Clear();
        }

        /// <summary>
        /// Removes a mesh by name.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void Remove(String name)
        {
            if (!this._meshes.Remove(name))
            {
                throw new KeyNotFoundException($"Mesh with name {name} not found.");
            }
        }

        /// <summary>
        /// Checks if a mesh exists in the collection.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(String name)
        {
            return this._meshes.ContainsKey(name);
        }

        /// <summary>
        /// Gets all meshes in the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Mesh> GetAll()
        {
            return this._meshes.Values;
        }

        /// <summary>
        /// Iterates over all meshes in the collection and applies the action to each mesh.
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<Mesh> action)
        {
            foreach (var mesh in this._meshes.Values)
            {
                action(mesh);
            }
        }

        public void SingleMeshAction(int index, Action<Mesh> action)
        {
            if (index < 0 || index >= this._meshes.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
            var mesh = this._meshes.ElementAt(index).Value;
            if (mesh != null)
            {
                action(mesh);
            }
            else
            {
                throw new KeyNotFoundException($"Mesh with index {index} not found.");
            }
        }

        public void SingleMeshAction(String name, Action<Mesh> action)
        {
            if (this._meshes.TryGetValue(name, out var mesh))
            {
                action(mesh);
            }
            else
            {
                throw new KeyNotFoundException($"Mesh with name {name} not found.");
            }
        }

        public IEnumerator<Mesh> GetEnumerator()
        {
            return this._meshes.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
