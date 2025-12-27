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
            sceneObject["DirectionalLight"] = new JObject()
            {
                new JProperty("Direction", Utils.SerializeVec3(scene.DirectionalLight.Direction)),
                new JProperty("Color", Utils.SerializeVec4(scene.DirectionalLight.Color)),
                new JProperty("Intensity", scene.DirectionalLight.Intensity)
            };
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
    }
}
