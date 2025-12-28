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
            assets.ForeachAsset<ISerialization>(asset =>
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
        }
    }
}
