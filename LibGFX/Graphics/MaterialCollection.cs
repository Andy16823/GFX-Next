using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents a collection of materials.
    /// </summary>
    public class MaterialCollection : IEnumerable<Material>
    {
        /// <summary>
        /// The collection of materials.
        /// </summary>
        private readonly Dictionary<string, Material> _materials = new();

        /// <summary>
        /// The number of materials in the collection.
        /// </summary>
        public int Count => this._materials.Count;

        /// <summary>
        /// Gets a material by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Material this[int index] => this.GetMaterial(index);

        /// <summary>
        /// Gets or sets a material by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Material this[string name]
        {
            get => this.GetMaterial(name);
            set => this.Set(name, value);
        }

        /// <summary>
        /// Initializes a new instance of the MaterialCollection class.
        /// </summary>
        /// <param name="material"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Add(Material material)
        {
            if (material == null)
            {
                throw new ArgumentNullException(nameof(material));
            }

            if (!this._materials.TryAdd(material.Name, material))
            {
                throw new ArgumentException($"Material with name {material.Name} already exists.");
            }
        }

        /// <summary>
        /// Sets a material in the collection by name. If the material does not exist, it will be added.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="material"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Set(string name, Material material)
        {
            if (material == null)
            {
                throw new ArgumentNullException(nameof(material));
            }
            if (this._materials.ContainsKey(name))
            {
                this._materials[name] = material;
            }
            else
            {
                this.Add(material);
            }
        }

        /// <summary>
        /// Adds a range of materials to the collection.
        /// </summary>
        /// <param name="materials"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddRange(IEnumerable<Material> materials)
        {
            if (materials == null)
            {
                throw new ArgumentNullException(nameof(materials));
            }
            foreach (var material in materials)
            {
                this.Add(material);
            }
        }

        /// <summary>
        /// Gets a material by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public Material GetMaterial(String name)
        {
            if (this._materials.TryGetValue(name, out var material))
            {
                return material;
            }
            throw new KeyNotFoundException($"Material with name {name} not found.");
        }

        /// <summary>
        /// Gets a material by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Material GetMaterial(int index)
        {
            if (index < 0 || index >= this._materials.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
            return this._materials.ElementAt(index).Value;
        }

        /// <summary>
        /// Clears the collection of meshes and materials.
        /// </summary>
        public void Clear()
        {
            this._materials.Clear();
        }

        /// <summary>
        /// Removes a material by name.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void Remove(String name)
        {
            if (!this._materials.Remove(name))
            {
                throw new KeyNotFoundException($"Material with name {name} not found.");
            }
        }

        /// <summary>
        /// Checks if a material exists in the collection.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(String name)
        {
            return this._materials.ContainsKey(name);
        }

        /// <summary>
        /// Gets all materials in the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Material> GetAll()
        {
            return this._materials.Values;
        }

        /// <summary>
        /// Iterates over all materials in the collection and applies the action to each material.
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(Action<Material> action)
        {
            foreach (var material in this._materials.Values)
            {
                action(material);
            }
        }

        /// <summary>
        /// Executes an action for a single material by index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="action"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public void SingleMaterialAction(int index, Action<Material> action)
        {
            if (index < 0 || index >= this._materials.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
            var material = this._materials.ElementAt(index).Value;
            if (material != null)
            {
                action(material);
            }
            else
            {
                throw new KeyNotFoundException($"Material with index {index} not found.");
            }
        }

        /// <summary>
        /// Executes an action for a single material by name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void SingleMaterialAction(String name, Action<Material> action)
        {
            if (this._materials.TryGetValue(name, out var material))
            {
                action(material);
            }
            else
            {
                throw new KeyNotFoundException($"Material with name {name} not found.");
            }
        }

        /// <summary>
        /// Gets an enumerator that iterates through the collection of materials.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Material> GetEnumerator()
        {
            return _materials.Values.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that iterates through the collection of materials.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
