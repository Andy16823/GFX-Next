using LibGFX.Core;
using LibGFX.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents a cubemap texture
    /// </summary>
    public class Cubemap : IGraphicsResource, ISerialization
    {
        /// <summary>
        /// The faces of the cubemap
        /// </summary>
        public List<byte[]> Faces { get; set; }

        /// <summary>
        /// The OpenGL texture ID of the cubemap
        /// </summary>
        public int TextureId { get; set; }

        /// <summary>
        /// The width of the cubemap
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the cubemap
        /// </summary>
        public int Height { get; set; }

        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Creates a new cubemap
        /// </summary>
        public Cubemap()
        {
            this.Faces = new List<byte[]>();
        }

        /// <summary>
        /// Loads a cubemap from the specified paths
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="swapYAxisFaces"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static Cubemap LoadCubemap(String[] paths, bool swapYAxisFaces = true)
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
            
            Cubemap cubemap = new Cubemap();
            foreach (var path in paths)
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Cubemap face not found: {path}");
                }

                ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
                cubemap.Faces.Add(image.Data);
                cubemap.Width = image.Width;
                cubemap.Height = image.Height;
            }

            if(swapYAxisFaces)
            {
                var temp = cubemap.Faces[2];
                cubemap.Faces[2] = cubemap.Faces[3];
                cubemap.Faces[3] = temp;
            }
            return cubemap;
        }

        /// <summary>
        /// Loads a cubemap from a JSON file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="swapYAxisFaces"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Cubemap LoadCubemap(string file, bool swapYAxisFaces = true)
        {
            // Check if the file exists
            if (!File.Exists(file))
            {
                throw new ArgumentException($"Cubemap file '{file}' does not exist.");
            }

            // Check if the file is a JSON file
            if (Path.GetExtension(file).ToLower() != ".json")
            {
                throw new ArgumentException($"Cubemap file '{file}' is not a JSON file.");
            }

            // Load the JSON file and parse it
            var basePath = Path.GetDirectoryName(file);
            var jsonString = File.ReadAllText(file);
            var jsonObject = JObject.Parse(jsonString);

            // Create the faces list and load the cubemap
            var faces = new List<string>();
            faces.Add(Path.Combine(basePath, jsonObject["cubemap"]["px"].Value<string>()));
            faces.Add(Path.Combine(basePath, jsonObject["cubemap"]["nx"].Value<string>()));
            faces.Add(Path.Combine(basePath, jsonObject["cubemap"]["py"].Value<string>()));
            faces.Add(Path.Combine(basePath, jsonObject["cubemap"]["ny"].Value<string>()));
            faces.Add(Path.Combine(basePath, jsonObject["cubemap"]["pz"].Value<string>()));
            faces.Add(Path.Combine(basePath, jsonObject["cubemap"]["nz"].Value<string>()));
            var cubemap = LoadCubemap(faces.ToArray(), swapYAxisFaces);
            return cubemap;
        }

        /// <summary>
        /// Initializes the cubemap by loading it into the specified render device.
        /// </summary>
        /// <param name="renderer">The render device used to load the cubemap. Cannot be null.</param>
        public void Init(IRenderDevice renderer)
        {
            renderer.LoadCubemap(this);
            this.IsInitialized = true;
        }

        /// <summary>
        /// Releases resources associated with this cubemap using the specified render device.
        /// </summary>
        /// <remarks>After calling this method, the cubemap is no longer initialized and should not be
        /// used in rendering operations.</remarks>
        /// <param name="renderer">The render device used to dispose of the cubemap resources. Cannot be null.</param>
        public void Dispose(IRenderDevice renderer)
        {
            renderer.DisposeCubemap(this);
            this.IsInitialized = false;
        }

        /// <summary>
        /// Serializes the current object to a JSON representation suitable for storage or transmission.
        /// </summary>
        /// <remarks>The returned JSON object includes the object's type, width, height, and an array of
        /// faces. Each face is represented as a Base64-encoded string. The structure of the output is intended for
        /// interoperability and persistence scenarios.</remarks>
        /// <param name="serializationContext">The context that provides information required for serialization. This parameter can be used to customize
        /// serialization behavior.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized data, including type information, dimensions, and face
        /// data encoded as Base64 strings.</returns>
        public void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(this.GetType().FullName);
            writer.WritePropertyName("Width");
            writer.WriteValue(this.Width);
            writer.WritePropertyName("Height");
            writer.WriteValue(this.Height);

            writer.WritePropertyName("Faces");
            writer.WriteStartArray();
            foreach (var face in Faces)
            {
                writer.WriteValue(Convert.ToBase64String(face));
            }
            writer.WriteEndArray();

            callback?.Invoke(writer);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Deserializes the object from a JSON representation.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializationContext"></param>
        /// <param name="callback"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            if (this.IsInitialized)
                throw new InvalidOperationException("Cannot deserialize an initialized Cubemap. Dispose it first.");

            this.Width = obj.Value<int>("Width");
            this.Height = obj.Value<int>("Height");

            this.Faces.Clear();
            var facesArray = obj.Value<JArray>("Faces");
            if (facesArray != null)
            {
                foreach (var face in facesArray)
                {
                    var faceData = Convert.FromBase64String(face.Value<string>());
                    this.Faces.Add(faceData);
                }
            }
            callback?.Invoke(obj);
        }

        public void FreeCPUResources()
        {
            this.Faces = null;
        }
    }
}
