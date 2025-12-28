using LibGFX.Assets;
using LibGFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewGFXEditor.Exporter
{
    /// <summary>
    /// Defines methods and properties for exporting and importing 3D scene data to and from files in a specific format.
    /// </summary>
    /// <remarks>Implementations of this interface provide support for reading and writing 3D scenes using a
    /// particular file format. The interface exposes metadata about the format, such as its name and file extension,
    /// and indicates whether import functionality is available. Use this interface to integrate custom exporters or
    /// importers into a 3D asset pipeline.</remarks>
    public interface IExporter
    {
        /// <summary>
        /// Gets the name associated with the current instance.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the file extension associated with the file, including the leading period.
        /// </summary>
        string FileExtension { get; }

        /// <summary>
        /// Gets a value indicating whether import operations are supported by this instance.
        /// </summary>
        public bool SupportsImport { get; }

        /// <summary>
        /// Exports the specified 3D scene to a file at the given path.
        /// </summary>
        /// <param name="filePath">The path to the file where the scene will be exported. Must be a valid file path and cannot be null or
        /// empty.</param>
        /// <param name="scene">The 3D scene to export. Cannot be null.</param>
        void Export(string filePath, Scene3D scene);

        /// <summary>
        /// Imports 3D scene data from the specified file into the provided scene using the given asset manager.
        /// </summary>
        /// <remarks>This method updates the provided scene with the contents of the specified file.
        /// Existing data in the scene may be replaced or modified depending on the import implementation. Ensure that
        /// the asset manager is properly configured to resolve any external resources required by the scene.</remarks>
        /// <param name="filePath">The path to the file containing the 3D scene data to import. Cannot be null or empty.</param>
        /// <param name="scene">The <see cref="Scene3D"/> instance to populate with the imported scene data. Cannot be null.</param>
        /// <param name="assets">The <see cref="AssetManager"/> used to manage assets referenced by the imported scene. Cannot be null.</param>
        void Import(string filePath, Scene3D scene, AssetManager assets);
    }
}
