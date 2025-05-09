using LibGFX.Graphics.Materials;
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
    public class MaterialCollection : IEnumerable<IMaterial>
    {
        /// <summary>
        /// The collection of materials.
        /// </summary>
        private readonly Dictionary<string, IMaterial> _materials = new();

        /// <summary>
        /// The number of materials in the collection.
        /// </summary>
        public int Count => this._materials.Count;

        /// <summary>
        /// Gets a material by index.
        /// </summary>
        /// <param materialKey="index"></param>
        /// <returns></returns>
        public IMaterial this[int index] => this.GetMaterial(index);

        /// <summary>
        /// Gets or sets a material by materialKey.
        /// </summary>
        /// <param materialKey="materialKey"></param>
        /// <returns></returns>
        public IMaterial this[string materialKey]
        {
            get => this.GetMaterial(materialKey);
            set => this.Set(materialKey, value);
        }

        /// <summary>
        /// Initializes a new instance of the MaterialCollection class.
        /// </summary>
        /// <param materialKey="material"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Add(IMaterial material)
        {
            if (material == null)
            {
                throw new ArgumentNullException(nameof(material));
            }

            if (!this._materials.TryAdd(material.ID.ToString(), material))
            {
                throw new ArgumentException($"Material with name {material.Name} already exists.");
            }
        }

        /// <summary>
        /// Sets a material in the collection by materialKey. If the material does not exist, it will be added.
        /// </summary>
        /// <param materialKey="materialKey"></param>
        /// <param materialKey="material"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Set(string materialKey, IMaterial material)
        {
            if (material == null)
            {
                throw new ArgumentNullException(nameof(material));
            }
            if (this._materials.ContainsKey(materialKey))
            {
                this._materials[materialKey] = material;
            }
            else
            {
                this.Add(material);
            }
        }

        /// <summary>
        /// Adds a range of materials to the collection.
        /// </summary>
        /// <param materialKey="materials"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddRange(IEnumerable<IMaterial> materials)
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
        /// Gets a material by materialKey.
        /// </summary>
        /// <param materialKey="materialKey"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public IMaterial GetMaterial(String materialKey)
        {
            if (_materials.TryGetValue(materialKey, out var material))
            {
                return material;
            }
            throw new KeyNotFoundException($"Material with key {materialKey} not found.");
        }

        /// <summary>
        /// Gets a material by index.
        /// </summary>
        /// <param materialKey="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IMaterial GetMaterial(int index)
        {
            if (index < 0 || index >= this._materials.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
            return _materials.ElementAt(index).Value;
        }

        /// <summary>
        /// Clears the collection of meshes and materials.
        /// </summary>
        public void Clear()
        {
            _materials.Clear();
        }

        /// <summary>
        /// Removes a material by materialKey.
        /// </summary>
        /// <param materialKey="materialKey"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void Remove(String materialKey)
        {
            if (!_materials.Remove(materialKey))
            {
                throw new KeyNotFoundException($"Material with name {materialKey} not found.");
            }
        }

        /// <summary>
        /// Checks if a material exists in the collection.
        /// </summary>
        /// <param materialKey="materialKey"></param>
        /// <returns></returns>
        public bool Contains(String materialKey)
        {
            return _materials.ContainsKey(materialKey);
        }

        /// <summary>
        /// Gets all materials in the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMaterial> GetAll()
        {
            return _materials.Values;
        }

        /// <summary>
        /// Iterates over all materials in the collection and applies the action to each material.
        /// </summary>
        /// <param materialKey="action"></param>
        public void ForEach(Action<IMaterial> action)
        {
            foreach (var material in _materials.Values)
            {
                action(material);
            }
        }

        /// <summary>
        /// Executes an action for a single material by index.
        /// </summary>
        /// <param materialKey="index"></param>
        /// <param materialKey="action"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public void SingleMaterialAction(int index, Action<IMaterial> action)
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
        /// Executes an action for a single material by materialKey.
        /// </summary>
        /// <param materialKey="materialKey"></param>
        /// <param materialKey="action"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        public void SingleMaterialAction(String materialKey, Action<IMaterial> action)
        {
            if (_materials.TryGetValue(materialKey, out var material))
            {
                action(material);
            }
            else
            {
                throw new KeyNotFoundException($"Material with key {materialKey} not found.");
            }
        }

        /// <summary>
        /// Gets an enumerator that iterates through the collection of materials.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IMaterial> GetEnumerator()
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
