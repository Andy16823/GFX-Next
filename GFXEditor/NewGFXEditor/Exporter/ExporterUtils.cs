using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewGFXEditor.Exporter
{
    public class ExporterUtils
    {
        public static JObject Vector2Obj(Vector2 vec)
        {
            JObject obj = new JObject();
            obj["X"] = vec.X;
            obj["Y"] = vec.Y;
            return obj;
        }

        public static JObject Vector3Obj(Vector3 vec)
        {
            JObject obj = new JObject();
            obj["X"] = vec.X;
            obj["Y"] = vec.Y;
            obj["Z"] = vec.Z;
            return obj;
        }

        public static JObject Vector4Obj(Vector4 vec)
        {
            JObject obj = new JObject();
            obj["X"] = vec.X;
            obj["Y"] = vec.Y;
            obj["Z"] = vec.Z;
            obj["W"] = vec.W;
            return obj;
        }

        public static JObject QuatObj(Quaternion quat)
        {
            JObject obj = new JObject();
            obj["X"] = quat.X;
            obj["Y"] = quat.Y;
            obj["Z"] = quat.Z;
            obj["W"] = quat.W;
            return obj;
        }

        public static JObject TransformObj(LibGFX.Math.Transform transform)
        {
            JObject obj = new JObject();
            obj["Position"] = Vector3Obj(transform.Position);
            obj["Rotation"] = QuatObj(transform.Rotation);
            obj["Scale"] = Vector3Obj(transform.Scale);
            return obj;
        }

        public static JObject VertexObj(LibGFX.Graphics.Vertex vertex)
        {
            JObject obj = new JObject();
            obj["Position"] = Vector3Obj(vertex.Position);
            obj["Normal"] = Vector3Obj(vertex.Normal);
            obj["TexCoord"] = Vector2Obj(vertex.TexCoord);
            obj["Tangent"] = Vector4Obj(vertex.Tangent);
            obj["BoneWeights"] = Vector4Obj(vertex.BoneWeights);
            obj["BoneIDs"] = Vector4Obj(vertex.BoneIDs);
            return obj;
        }

        public static JObject MeshObj(LibGFX.Graphics.Mesh mesh)
        {
            JObject obj = new JObject();
            obj["Name"] = mesh.Name;
            obj["ID"] = mesh.ID;

            var vertArray = new JArray();
            foreach(var vertex in mesh.Vertices)
            {
                vertArray.Add(VertexObj(vertex));
            }
            obj["Vertices"] = vertArray;
            obj["Indices"] = new JArray(mesh.Indices);
            obj["LocalTranslation"] = Vector3Obj(mesh.LocalTranslation);
            obj["LocalRotation"] = QuatObj(mesh.LocalRotation);
            obj["LocalScale"] = Vector3Obj(mesh.LocalScale);
            obj["Material"] = mesh.Material.ID.ToString();
            return obj;
        }

        public static JObject TextureParamsObj(LibGFX.Graphics.TextureParameters parameter)
        {
            JObject result = new JObject();
            result["MinFilter"] = (int)parameter.MinFilter;
            result["MagFilter"] = (int)parameter.MagFilter;
            result["WrapS"] = (int)parameter.WrapS;
            result["WrapT"] = (int) parameter.WrapT;
            result["GenerateMipmaps"] = parameter.GenerateMipmaps;
            return result;
        }

        public static JObject TextureObj(LibGFX.Graphics.Texture texture)
        {
            JObject result = new JObject();
            result["Width"] = texture.Width;
            result["Height"] = texture.Height;
            result["TextureData"] = Convert.ToBase64String(texture.TextureData);
            result["TextureParameters"] = TextureParamsObj(texture.TextureParameters);
            return result;
        }

        public static JObject MaterialObj(LibGFX.Graphics.Materials.SGMaterial material)
        {
            // Create base material
            JObject result = new JObject();
            result["Name"] = material.Name;
            result["ID"] = material.ID;
            result["Color"] = Vector4Obj(material.Color);
            result["UVScale"] = Vector2Obj(material.UVScale);
            result["FlipNormal"] = material.FlipNormal;
            result["Opacity"] = material.Opacity;
            result["Shininess"] = material.Shininess;

            // Create material textures
            JObject textures = new JObject();
            textures["DiffuseTexture"] = TextureObj(material.DiffuseTexture);
            textures["NormalTexture"] = TextureObj(material.NormalTexture);
            textures["SpecularTexture"] = TextureObj(material.SpecularTexture);
            result["textures"] = textures;

            return result;
        }
    }
}
