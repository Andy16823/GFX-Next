using LibGFX.Assets;
using LibGFX.Core;
using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
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
  

        public void Export(string filePath, LibGFX.Core.Scene3D scene, AssetManager assets)
        {
            // Root Object
            JObject result = new JObject();

            // Serialize all assets wich implement ISerialization
            JArray assetsArray = new JArray();

            // Serialize Materials first
            assets.ForeachAsset<IMaterial>(asset =>
            {
                assetsArray.Add(asset.Serialize(null));
            });

            // Serialize Meshes next
            assets.ForeachAsset<Mesh>(asset =>
            {
                assetsArray.Add(asset.Serialize(null));
            });

            // Serialize Models last
            assets.ForeachAsset<StaticMeshModel>(asset =>
            {
                assetsArray.Add(asset.Serialize(null));
            });
            result.Add("Assets", assetsArray);

            // Serialize Scene
            var sceneObj = scene.Serialize(null);
            result.Add("Scene", sceneObj);

            // Save to file
            var json = result.ToString();
            File.WriteAllText(filePath, json);
        }

        public void Import(string filePath, Scene3D scene, AssetManager assets)
        {
            var context = new SerializationContext();
            var json = File.ReadAllText(filePath);
            var root = JObject.Parse(json);

            var ctx = new SerializationContext();
            // Load Assets
            var assetsArray = root["Assets"] as JArray;
            foreach(var assetToken in assetsArray)
            {
                var typeName = assetToken["Type"].Value<string>();
                var Name = assetToken["Name"].Value<string>();

                // Check for SGMaterial
                if (typeName == typeof(SGMaterial).FullName)
                {
                    var material = new SGMaterial();
                    material.Deserialize(assetToken as JObject, ctx);
                    assets.Add(material);
                    ctx.SetValue(material.ID.ToString(), material); // Register in context
                    continue;
                }

                // Check for Mesh
                if (typeName == typeof(Mesh).FullName)
                {
                    var mesh = new Mesh();
                    mesh.Deserialize(assetToken as JObject, ctx);
                    assets.Add(mesh);
                    ctx.SetValue(mesh.ID.ToString(), mesh); // Register in context
                    continue;
                }

                // Check for StaticMeshModel
                if (typeName == typeof(StaticMeshModel).FullName)
                {
                    var model = new StaticMeshModel();
                    model.Deserialize(assetToken as JObject, ctx);
                    assets.Add(model);
                    ctx.SetValue(model.ID.ToString(), model); // Register in context
                    continue;
                }
                Debug.WriteLine($"[GFXExporter] Unknown asset type during import: {typeName}");
            }

            // Load Scene Layers
            var sceneObj = root["Scene"] as JObject;
            scene.Deserialize(sceneObj, ctx);
        }
    }
}
