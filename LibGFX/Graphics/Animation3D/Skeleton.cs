using LibGFX.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Animation3D
{
    /// <summary>
    /// Represents a bone information for the rendering pipeline
    /// </summary>
    public class Skeleton : ISerialization
    {
        /// <summary>
        /// The Bones of the skeleton.
        /// </summary>
        public Dictionary<String, BoneInfo> BoneInfoMap { get; set; }

        /// <summary>
        /// The number of bones in the skeleton.
        /// </summary>
        public int BoneCounter;

        public Skeleton()
        {
            this.BoneInfoMap = new Dictionary<String, BoneInfo>();
        }

        /// <summary>
        /// Serializes the current bone data and related information into a JSON object.
        /// </summary>
        /// <param name="serializationContext">The context that provides serialization settings and state information used during the serialization
        /// process.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized bone counter and bone information map.</returns>
        public void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(this.GetType().FullName);
            writer.WritePropertyName("BoneCounter");
            writer.WriteValue(BoneCounter);
            writer.WritePropertyName("BoneInfoMap");
            writer.WriteStartArray();
            foreach (var bone in BoneInfoMap)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Key");
                writer.WriteValue(bone.Key);
                writer.WritePropertyName("BoneInfo");
                Utils.SerializeBoneInfo(bone.Value, writer);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            callback?.Invoke(writer);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Populates the current object with values from the specified JSON object using the provided serialization
        /// context.
        /// </summary>
        /// <param name="jObject">The JSON object containing the data to deserialize into the current instance. Cannot be null.</param>
        /// <param name="serializationContext">The context that provides information and services for the deserialization process. Cannot be null.</param>
        /// <exception cref="NotImplementedException">Thrown in all cases as this method is not yet implemented.</exception>
        public void Deserialize(JsonReader reader, SerializationContext serializationContext, Func<JsonReader, string, bool> callback = null)
        {
            if(reader.TokenType != JsonToken.StartObject)
                throw new JsonException("Expected StartObject token");

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                    break;

                if (reader.TokenType == JsonToken.PropertyName)
                {
                    string propertyName = (string)reader.Value!;
                    reader.Read();

                    switch (propertyName)
                    {
                        case "Type":
                            reader.Skip();
                            break;
                        case "BoneCounter":
                            this.BoneCounter = reader.Value != null ? Convert.ToInt32(reader.Value) : 0;
                            break;
                        case "BoneInfoMap":
                            if (reader.TokenType != JsonToken.StartArray)
                                throw new JsonException("Expected StartArray token for BoneInfoMap");

                            // Clear existing data
                            BoneInfoMap.Clear();
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.EndArray)
                                    break;

                                if (reader.TokenType == JsonToken.StartObject)
                                {
                                    string key = string.Empty;
                                    BoneInfo boneInfo = new BoneInfo();

                                    while (reader.Read())
                                    {
                                        if (reader.TokenType == JsonToken.EndObject)
                                            break;

                                        if (reader.TokenType == JsonToken.PropertyName)
                                        {
                                            string bonePropertyName = (string)reader.Value!;
                                            reader.Read();

                                            switch (bonePropertyName)
                                            {
                                                case "Key":
                                                    key = (string)reader.Value;
                                                    break;
                                                case "BoneInfo":
                                                    boneInfo = Utils.DeserializeBoneInfo(reader);
                                                    break;
                                                default:
                                                    reader.Skip();
                                                    break;
                                            }
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(key))
                                    {
                                        BoneInfoMap.Add(key, boneInfo);
                                    }
                                }
                            }
                            break;
                        default:
                            if (callback != null && callback(reader, propertyName))
                            {
                                break;
                            }
                            reader.Skip();
                            break;
                    }
                }
            }
        }
    }
}
