using LibGFX.Assets;
using LibGFX.Core;
using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

        private Dictionary<String, Mesh> BuildMeshTable(Scene3D scene)
        {
            Dictionary<String, Mesh> table = new Dictionary<string, Mesh>();
            scene.ForEachElement(element =>
            {
                foreach (var mesh in element.GetMeshes())
                {
                    if(!table.ContainsKey(mesh.ID.ToString()))
                    {
                        table[mesh.ID.ToString()] = mesh;
                    }
                }
            });
            return table;
        }

        private JArray CreateMaterialsArray(List<Mesh> meshes)
        {
            // Create materials table from given meshes
            Dictionary<String, IMaterial> materials = new Dictionary<string, IMaterial>();
            foreach(var mesh in meshes)
            {
                if(!materials.ContainsKey(mesh.Material.ID.ToString()))
                {
                    if(mesh.Material.GetType() != typeof(SGMaterial))
                    {
                        throw new Exception("Material is not type of SGMaterial");
                    }
                    materials[mesh.Material.ID.ToString()] = mesh.Material;
                }
            }

            // Pass the materials to the array
            JArray result = new JArray();
            foreach(var material in materials.Values)
            {
                result.Add(material.Serialize(null));
            }
            return result;
        }

        public void Export(string filePath, LibGFX.Core.Scene3D scene)
        {
            var meshtable = BuildMeshTable(scene);

            // Root Object
            JObject result = new JObject();

            // Assets
            JObject assetsObject = new JObject();
            assetsObject["Materials"] = this.CreateMaterialsArray(meshtable.Values.ToList());
            JArray meshes = new JArray();
            foreach(var mesh in meshtable.Values)
            {
                meshes.Add(mesh.Serialize(null));
            }
            assetsObject["Meshes"] = meshes;

            result["Assets"] = assetsObject;

            // Scene
            JObject sceneObject = new JObject();
            sceneObject["Name"] = scene.Name;
            sceneObject["ID"] = scene.ID.ToString();
            sceneObject["DirectionalLight"] = scene.DirectionalLight.Serialize(null);
            sceneObject["Enviroment"] = scene.Enviroment.Serialize(null);
            var layerArray = new JArray();
            foreach (var layer in scene.Layers)
            {
                JObject layerObject = new JObject();
                layerObject["Name"] = layer.Name;
                layerObject["ID"] = layer.ID.ToString();
                layerObject["Enabled"] = layer.Enabled;

                JArray elementsArray = new JArray();
                foreach(var element in layer.Elements)
                {
                    JObject elementObject = new JObject();
                    elementObject["Name"] = element.Name;
                    elementObject["ID"] = element.ID;
                    elementObject["Enabled"] = element.Enabled;
                    elementObject["Visible"] = element.Visible;
                    elementObject["Transform"] = element.Transform.Serialize(null);
                    elementObject["Properties"] = JObject.FromObject(element.Properties);
                    JArray meshesArray = new JArray();
                    foreach(var mesh in element.GetMeshes())
                    {
                        meshesArray.Add(mesh.ID.ToString());
                    }
                    elementObject["Meshes"] = meshesArray;
                    elementsArray.Add(elementObject);
                }
                layerObject["elements"] = elementsArray;
                layerArray.Add(layerObject);
            }
            sceneObject["Layer"] = layerArray;
            result["Scene"] = sceneObject;
            
            string json = result.ToString();
            File.WriteAllText(filePath, json);
        }

        public void Import(string filePath, Scene3D scene, AssetManager assets)
        {
            var context = new SerializationContext();
            var json = File.ReadAllText(filePath);
            var root = JObject.Parse(json);

            // Load Assets
            var assetsObject = root["Assets"] as JObject;
            var materialsArray = assetsObject["Materials"] as JArray;

            foreach(var materialToken in materialsArray)
            {
                var materialObject = materialToken as JObject;
                var material = new SGMaterial();
                material.Deserialize(materialObject, context);
                context.SetValue(material.ID.ToString(), material);

            }

            var meshesArray = assetsObject["Meshes"] as JArray;
            foreach(var meshToken in meshesArray)
            {
                var meshObject = meshToken as JObject;
                var mesh = new Mesh();
                mesh.Deserialize(meshObject, context);
                context.SetValue(mesh.ID.ToString(), mesh);
            }

            // Load Materials
            var sceneObject = root["Scene"] as JObject;


        }
    }
}
