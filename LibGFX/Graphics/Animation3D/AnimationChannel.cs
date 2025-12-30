using LibGFX.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Animation3D
{
    /// <summary>
    /// Represents a keyframe position in an animation.
    /// </summary>
    public struct KeyPosition
    {
        public Vector3 position;
        public float timeStamp;
    };

    /// <summary>
    /// Represents a keyframe rotation in an animation.
    /// </summary>
    public struct KeyRotation
    {
        public Quaternion orientation;
        public float timeStamp;
    };

    /// <summary>
    /// Represents a keyframe scale in an animation.
    /// </summary>
    public struct KeyScale
    {
        public Vector3 scale;
        public float timeStamp;
    };

    /// <summary>
    /// Represents an animation channel for a specific bone, containing keyframes for position, rotation, and scale.
    /// </summary>
    public class AnimationChannel : ISerialization
    {
        /// <summary>
        /// Gets or sets the name of the bone associated with this instance.
        /// </summary>
        public String BoneName { get; set; }

        /// <summary>
        /// Gets or sets the collection of key positions associated with the object.
        /// </summary>
        public List<KeyPosition> Positions { get; set; } = new List<KeyPosition>();

        /// <summary>
        /// Gets or sets the collection of key rotation records associated with this entity.
        /// </summary>
        /// <remarks>Each entry in the collection represents a single key rotation event, including
        /// relevant metadata such as the rotation date and status. Modifying this collection affects the recorded
        /// history of key rotations for the entity.</remarks>
        public List<KeyRotation> Rotations { get; set; } = new List<KeyRotation>();

        /// <summary>
        /// Gets or sets the collection of key scales associated with this instance.
        /// </summary>
        public List<KeyScale> Scales { get; set; } = new List<KeyScale>();

        /// <summary>
        /// Gets the number of positions in the collection.
        /// </summary>
        public int NumPositions => Positions.Count;

        /// <summary>
        /// Gets the number of rotations in the collection.
        /// </summary>
        public int NumRotations => Rotations.Count;

        /// <summary>
        /// Gets the number of scaling operations currently defined.
        /// </summary>
        public int NumScalings => Scales.Count;

        /// <summary>
        /// Serializes the bone animation data to the specified JSON writer using the provided serialization context.
        /// Optionally invokes a callback to perform additional custom serialization.
        /// </summary>
        /// <remarks>The method writes the type information, bone name, and keyframe data for positions,
        /// rotations, and scales to the JSON output. The callback parameter allows callers to extend the serialized
        /// output with custom data if needed.</remarks>
        /// <param name="writer">The JSON writer to which the bone animation data will be written. Must not be null.</param>
        /// <param name="serializationContext">The context that provides information and services required for serialization. Must not be null.</param>
        /// <param name="callback">An optional callback that receives the JSON writer and can be used to write additional custom properties or
        /// data. If null, no additional serialization is performed.</param>
        public void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            // Serialize BoneName and keyframes
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(this.GetType().FullName);
            writer.WritePropertyName("BoneName");
            writer.WriteValue(this.BoneName);

            // Serialize Positions
            writer.WritePropertyName("Positions");
            writer.WriteStartArray();
            foreach (var pos in this.Positions)
            {
                Utils.SerializeKeyPosition(pos, writer);
            }
            writer.WriteEndArray();

            // Serialize Rotations
            writer.WritePropertyName("Rotations");
            writer.WriteStartArray();
            foreach (var rot in this.Rotations)
            {
                Utils.SerializeKeyRotation(rot, writer);
            }
            writer.WriteEndArray();

            // Serialize Scales
            writer.WritePropertyName("Scales");
            writer.WriteStartArray();
            foreach (var scale in this.Scales)
            {
                Utils.SerializeKeyScale(scale, writer);
            }
            writer.WriteEndArray();

            // Callback if provided
            callback?.Invoke(writer);

            // End of object
            writer.WriteEndObject();
        }

        /// <summary>
        /// Deserializes the object from the specified JSON reader using the provided serialization context.
        /// </summary>
        /// <remarks>The method expects the JSON to contain properties such as "BoneName", "Positions",
        /// "Rotations", and "Scales". Collections for positions, rotations, and scales are cleared and repopulated
        /// based on the JSON content. Unknown properties are skipped.</remarks>
        /// <param name="reader">The JSON reader positioned at the start of the object to deserialize. Must not be null and must be at a
        /// StartObject token.</param>
        /// <param name="serializationContext">The context containing serialization settings and state information used during deserialization.</param>
        /// <exception cref="JsonSerializationException">Thrown if the JSON structure is invalid or if expected tokens are missing during deserialization.</exception>
        public void Deserialize(JsonReader reader, SerializationContext serializationContext, Func<JsonReader, string, bool> callback = null)
        {
            if(reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException("Expected StartObject token");

            while(reader.Read())
            {
                if(reader.TokenType == JsonToken.EndObject)
                    break;

                if(reader.TokenType == JsonToken.PropertyName)
                {
                    var propertyName = (string) reader.Value;
                    reader.Read();

                    switch (propertyName)
                    {
                        case "Type":
                            reader.Skip();
                            break;
                        case "BoneName":
                            this.BoneName = (string) reader.Value;
                            break;
                        case "Positions":
                            this.Positions.Clear();
                            if(reader.TokenType != JsonToken.StartArray)
                                throw new JsonSerializationException("Expected StartArray token for Positions");

                            while(reader.Read())
                            {
                                if (reader.TokenType == JsonToken.EndArray)
                                    break;

                                if(reader.TokenType == JsonToken.StartObject) 
                                {
                                    KeyPosition position = Utils.DeserializeKeyPosition(reader);
                                    this.Positions.Add(position);
                                }
                            }
                            break;
                        case "Rotations":
                            this.Rotations.Clear();

                            if(reader.TokenType != JsonToken.StartArray)
                                throw new JsonSerializationException("Expected StartArray token for Rotations");

                            while(reader.Read())
                            {
                                if (reader.TokenType == JsonToken.EndArray)
                                    break;

                                if(reader.TokenType == JsonToken.StartObject) 
                                {
                                    KeyRotation rotation = Utils.DeserializeKeyRotation(reader);
                                    this.Rotations.Add(rotation);
                                }
                            }
                            break;
                        case "Scales":
                            this.Scales.Clear();

                            if(reader.TokenType != JsonToken.StartArray)
                                throw new JsonSerializationException("Expected StartArray token for Scales");

                            while(reader.Read())
                            {
                                if (reader.TokenType == JsonToken.EndArray)
                                    break;

                                if(reader.TokenType == JsonToken.StartObject) 
                                {
                                    KeyScale scale = Utils.DeserializeKeyScale(reader);
                                    this.Scales.Add(scale);
                                }
                            }
                            break;
                        default:
                            if(callback != null && callback(reader, propertyName))
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
