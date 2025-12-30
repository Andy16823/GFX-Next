using Assimp;
using LibGFX.Assets;
using LibGFX.Core;
using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Mesh = LibGFX.Graphics.Mesh;

namespace NewGFXEditor.Exporter
{
    public class GFXExporter : IExporter
    {
        public string Name => "GFX Exporter";
        public string FileExtension => ".gfxlevel";
        public bool SupportsImport => true;

        /// <summary>
        /// Reads assets from a JSON reader into the provided AssetManager.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="assets"></param>
        /// <param name="ctx"></param>
        /// <exception cref="JsonSerializationException"></exception>
        private void ReadAssets(JsonReader reader, AssetManager assets, SerializationContext ctx)
        {
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException("Expected start of Assets object");

            // move into object
            reader.Read();

            while (reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    throw new JsonSerializationException("Expected asset category property name");

                string assetType = (string)reader.Value;

                // move to value (array)
                reader.Read();

                if (reader.TokenType != JsonToken.StartArray)
                    throw new JsonSerializationException($"Expected start of {assetType} array");

                switch (assetType)
                {
                    case "Materials":
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                        {
                            var material = new SGMaterial();
                            material.Deserialize(reader, ctx);
                            ctx.SetValue(material.ID.ToString(), material);
                            assets.Add(material);
                        }
                        break;

                    case "Meshes":
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                        {
                            var mesh = new Mesh();
                            mesh.Deserialize(reader, ctx);
                            ctx.SetValue(mesh.ID.ToString(), mesh);
                            assets.Add(mesh);
                        }
                        break;

                    case "StaticMeshModels":
                        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                        {
                            var staticMeshModel = new StaticMeshModel();
                            staticMeshModel.Deserialize(reader, ctx);
                            ctx.SetValue(staticMeshModel.ID.ToString(), staticMeshModel);
                            assets.Add(staticMeshModel);
                        }
                        break;

                    default:
                        // unbekannte Asset-Kategorie überspringen
                        reader.Skip();
                        break;
                }

                // nach EndArray weiter
                reader.Read();
            }
        }

        /// <summary>
        /// Reads a scene file in JSON format.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="scene"></param>
        /// <param name="assets"></param>
        /// <exception cref="JsonSerializationException"></exception>
        private void ReadSceneFile(string filePath, LibGFX.Core.Scene3D scene, AssetManager assets)
        {
            var ctx = new SerializationContext();

            using var fs = File.OpenRead(filePath);
            using var sr = new StreamReader(fs);
            using var jr = new JsonTextReader(sr);

            if (!jr.Read() || jr.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException("Expected root object");

            while (jr.Read())
            {
                if (jr.TokenType != JsonToken.PropertyName)
                    continue;

                string prop = (string)jr.Value;
                jr.Read();

                switch (prop)
                {
                    case "Assets":
                        ReadAssets(jr, assets, ctx);
                        break;
                    case "Scene":
                        scene.Deserialize(jr, ctx);
                        break;
                    default:
                        jr.Skip();
                        break;
                }
            }

            ctx.ContextData.Clear();
        }

        /// <summary>
        /// Exports the specified 3D scene to a file at the given path.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="scene"></param>
        /// <param name="assets"></param>
        public void ExportScene(string filePath, LibGFX.Core.Scene3D scene, AssetManager assets)
        {
            using var fs = File.Create(filePath);
            using var sw = new StreamWriter(fs);
            using var jw = new Newtonsoft.Json.JsonTextWriter(sw)
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            };

            SerializationContext ctx = new SerializationContext();

            // Start root object
            jw.WriteStartObject();

            // Serialize Assets
            jw.WritePropertyName("Assets");
            jw.WriteStartObject();

            // Materials Objects first
            jw.WritePropertyName("Materials");
            jw.WriteStartArray();
            assets.ForeachAsset<IMaterial>(asset =>
            {
                asset.Serialize(jw, ctx);
            });
            jw.WriteEndArray();

            // Meshes
            jw.WritePropertyName("Meshes");
            jw.WriteStartArray();
            assets.ForeachAsset<Mesh>(asset =>
            {
                asset.Serialize(jw, ctx);
            });
            jw.WriteEndArray();

            // StaticMeshModels
            jw.WritePropertyName("StaticMeshModels");
            jw.WriteStartArray();
            assets.ForeachAsset<StaticMeshModel>(asset =>
            {
                asset.Serialize(jw, ctx);
            });
            jw.WriteEndArray();
            jw.WriteEndObject();

            // Export Scene
            jw.WritePropertyName("Scene");
            scene.Serialize(jw, ctx);

            jw.WriteEndObject();

            // Serialize Scene
            jw.Flush();
        }

        /// <summary>
        /// Imports 3D scene data from the specified file into the provided scene using the given asset manager.
        /// Assets are expected to be preloaded into the AssetManager.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="scene"></param>
        /// <param name="assets"></param>
        public void ImportScene(string filePath, Scene3D scene, AssetManager assets)
        {
            this.ReadSceneFile(filePath, scene, assets);
        }
    }
}
