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

namespace NewGFXEditor.Exporter
{
    public class GFXExporter : IExporter
    {
        public string Name => "GFX Exporter";
        public string FileExtension => ".gfxlevel";
        public bool SupportsImport => true;
  

        private void ReadAssets(JsonReader reader, AssetManager assets, SerializationContext ctx)
        {
            var settings = new JsonLoadSettings
            {
                LineInfoHandling = LineInfoHandling.Ignore,
                CommentHandling = CommentHandling.Ignore
            };

            reader.Read(); // StartArray
            while(reader.Read())
            {
                if(reader.TokenType == JsonToken.EndArray)
                {
                    break;
                }

                if(reader.TokenType == JsonToken.StartObject)
                {
                    JObject assetObj = JObject.Load(reader, settings);
                    string typeName = assetObj["Type"].Value<string>();

                    if(typeName == null)
                    {
                        Debug.WriteLine("[GFXExporter] Asset type is null during import.");
                        continue;
                    }

                    if(typeName == typeof(SGMaterial).FullName)
                    {
                        var material = new SGMaterial();
                        material.Deserialize(assetObj, ctx);
                        assets.Add(material);
                        ctx.SetValue(material.ID.ToString(), material); // Register in context
                        continue;
                    }

                    if(typeName == typeof(Mesh).FullName)
                    {
                        var mesh = new Mesh();
                        mesh.Deserialize(assetObj, ctx);
                        assets.Add(mesh);
                        ctx.SetValue(mesh.ID.ToString(), mesh); // Register in context
                        continue;
                    }

                    if(typeName == typeof(StaticMeshModel).FullName)
                    {
                        var model = new StaticMeshModel();
                        model.Deserialize(assetObj, ctx);
                        assets.Add(model);
                        ctx.SetValue(model.ID.ToString(), model); // Register in context
                        continue;
                    }

                    Debug.WriteLine($"[GFXExporter] Unknown asset type during import: {typeName}");
                }
            }
        }

        private void ReadScene(JsonReader reader, Scene3D scene, SerializationContext ctx)
        {
            var settings = new JsonLoadSettings
            {
                LineInfoHandling = LineInfoHandling.Ignore,
                CommentHandling = CommentHandling.Ignore
            };

            reader.Read();
            JObject sceneObject = JObject.Load(reader, settings);
            scene.Deserialize(sceneObject, ctx);
        }

        public void Export(string filePath, LibGFX.Core.Scene3D scene, AssetManager assets)
        {
            using var fs = File.Create(filePath);
            using var sw = new StreamWriter(fs);
            using var jw = new Newtonsoft.Json.JsonTextWriter(sw)
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            };

            jw.WriteStartObject();

            // Serialize Assets
            jw.WritePropertyName("Assets");
            jw.WriteStartArray();
            // Serialize Materials first
            assets.ForeachAsset<IMaterial>(asset =>
            {
                asset.Serialize(jw, null);
            });
            // Serialize Meshes next
            assets.ForeachAsset<Mesh>(asset =>
            {
                asset.Serialize(jw, null);
            });
            // Serialize Models last
            assets.ForeachAsset<StaticMeshModel>(asset =>
            {
                asset.Serialize(jw, null);
            });
            jw.WriteEndArray();

            // Serialize Scene
            jw.WritePropertyName("Scene");
            scene.Serialize(jw, null);
            jw.WriteEndObject();
            jw.Flush();
        }

        public void Import(string filePath, Scene3D scene, AssetManager assets)
        {
            var ctx = new SerializationContext();

            using var fs = File.OpenRead(filePath);
            using var sr = new StreamReader(fs);
            using var jr = new Newtonsoft.Json.JsonTextReader(sr);

            jr.Read();

            while (jr.Read())
            {
                if(jr.TokenType != Newtonsoft.Json.JsonToken.PropertyName)
                {
                    continue;
                }

                switch ((string)jr.Value)
                {
                    case "Assets":
                        ReadAssets(jr, assets, ctx);
                        break;
                    case "Scene":
                        ReadScene(jr, scene, ctx);
                        break;
                }
            }
            ctx.ContextData.Clear();
        }
    }
}
