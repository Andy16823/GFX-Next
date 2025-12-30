using Assimp;
using LibGFX.Core;
using LibGFX.Graphics.Materials;
using LibGFX.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents a vertex for the rendering pipeline
    /// </summary>
    public struct Vertex
    {
        public Vector3 Position;
        public Vector2 TexCoord;
        public Vector3 Normal;
        public Vector4 Tangent;
        public Vector4i BoneIDs;
        public Vector4 BoneWeights;
    }

    /// <summary>
    /// Represents a bone information for the rendering pipeline
    /// </summary>
    public struct BoneInfo
    {
        public int id;
        public Matrix4 offset;
    }

    /// <summary>
    /// Represents a mesh for the rendering pipeline
    /// </summary>
    public class Mesh : IGraphicsResource, IIdentifier, ISerialization
    {
        /// <summary>
        /// The name of the mesh.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The unique identifier of the mesh.
        /// </summary>
        public Guid ID { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// The vertices of the mesh.
        /// </summary>
        public List<Vertex> Vertices { get; set; }

        /// <summary>
        /// The indices of the mesh.
        /// </summary>
        public List<int> Indices { get; set; }

        /// <summary>
        /// The local translation of the mesh.
        /// </summary>
        public Vector3 LocalTranslation { get; set; }

        /// <summary>
        /// the local rotation of the mesh.
        /// </summary>
        public Quaternion LocalRotation { get; set; }

        /// <summary>
        /// The local scale of the mesh.
        /// </summary>
        public Vector3 LocalScale { get; set; }

        /// <summary>
        /// The render data associated with the mesh.
        /// </summary>
        public RenderData RenderData { get; set; }

        /// <summary>
        /// Material used by the mesh.
        /// </summary>
        public IMaterial Material { get; set; }

        /// <summary>
        /// Gets a value indicating whether the object has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Initializes a new instance of the Mesh class with default values for vertices, indices, transformation, and
        /// render data.
        /// </summary>
        /// <remarks>The new Mesh instance starts with empty vertex and index collections, identity
        /// transformation (no translation or rotation, unit scale), and a default RenderData object. Use the properties
        /// to configure the mesh as needed before rendering or processing.</remarks>
        public Mesh()
        {
            Vertices = new List<Vertex>();
            Indices = new List<int>();
            LocalTranslation = Vector3.Zero;
            LocalRotation = Quaternion.Identity;
            LocalScale = Vector3.One;
            RenderData = new RenderData();
        }

        /// <summary>
        /// Calculates the local transformation matrix by combining the current scale, rotation, and translation values.
        /// </summary>
        /// <remarks>The resulting matrix applies scaling first, then rotation, and finally translation.
        /// This order affects how objects are transformed in local space. The method does not account for any parent
        /// transformations; it only reflects the local transform.</remarks>
        /// <returns>A <see cref="Matrix4"/> representing the local transformation composed of scale, rotation, and translation,
        /// applied in that order.</returns>
        public Matrix4 GetTransform()
        {
            Matrix4 translation = Matrix4.CreateTranslation(LocalTranslation);
            Matrix4 rotation = Matrix4.CreateFromQuaternion(LocalRotation);
            Matrix4 scale = Matrix4.CreateScale(LocalScale);
            return scale * rotation * translation;
        }

        /// <summary>
        /// Initializes the mesh by loading it into the specified render device.
        /// </summary>
        /// <param name="renderer">The render device used to load and initialize the mesh. Cannot be null.</param>
        public void Init(IRenderDevice renderer)
        {
            renderer.LoadMesh(this);
            IsInitialized = true;
        }

        /// <summary>
        /// Releases the resources associated with this mesh using the specified render device.
        /// </summary>
        /// <remarks>After calling this method, the mesh is no longer initialized and should not be used
        /// in rendering operations.</remarks>
        /// <param name="renderer">The render device used to dispose of the mesh resources. Cannot be null.</param>
        public void Dispose(IRenderDevice renderer)
        {
            renderer.DisposeMesh(this);
            IsInitialized = false;
        }

        public void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(this.GetType().FullName);
            writer.WritePropertyName("Name");
            writer.WriteValue(Name);
            writer.WritePropertyName("ID");
            writer.WriteValue(ID);
            writer.WritePropertyName("Vertices");
            writer.WriteStartArray();
            foreach (var vertex in Vertices)
            {
                Utils.SerializeVertex(vertex, writer);
            }
            writer.WriteEndArray();
            writer.WritePropertyName("Indices");
            writer.WriteStartArray();
            foreach (var index in Indices)
            {
                writer.WriteValue(index);
            }
            writer.WriteEndArray();
            writer.WritePropertyName("LocalTranslation");
            Utils.SerializeVec3(this.LocalTranslation, writer);
            writer.WritePropertyName("LocalRotation");
            Utils.SerializeQuat(this.LocalRotation, writer);
            writer.WritePropertyName("LocalScale");
            Utils.SerializeVec3(this.LocalScale, writer);
            writer.WritePropertyName("Material");
            writer.WriteValue(this.Material != null ? this.Material.ID.ToString() : null);
            callback?.Invoke(writer);
            writer.WriteEndObject();
        }

        public void Deserialize(JObject jObject, SerializationContext serializationContext)
        {
            // Ensure the mesh is not initialized before deserializing
            if (this.IsInitialized)
            {
                throw new InvalidOperationException("Cannot deserialize into an initialized Mesh. Dispose the mesh before deserializing.");
            }

            // Deserialize General Properties
            Name = jObject["Name"].Value<string>();
            ID = Guid.Parse(jObject["ID"].Value<string>());
            Vertices = new List<Vertex>();
            var vertArray = jObject["Vertices"] as JArray;
            foreach (var vertToken in vertArray)
            {
                Vertices.Add(Utils.DeserializeVertex(vertToken as JObject));
            }
            Indices = jObject["Indices"].ToObject<List<int>>();
            LocalTranslation = Utils.DeserializeVec3(jObject["LocalTranslation"] as JObject);
            LocalRotation = Utils.DeserializeQuat(jObject["LocalRotation"] as JObject);
            LocalScale = Utils.DeserializeVec3(jObject["LocalScale"] as JObject);

            // Deserialize Material
            var materialID = Guid.Parse(jObject["Material"].Value<string>());
            var material = serializationContext.GetValue<IMaterial>(materialID.ToString());
            if (material == null)
            {
                throw new InvalidOperationException($"Material with ID {materialID} not found in serialization context.");
            }
            this.Material = material;
        }
    }
}
