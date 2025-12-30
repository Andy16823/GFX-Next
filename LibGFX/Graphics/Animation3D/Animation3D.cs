using Assimp;
using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Animation3D
{
    /// <summary>
    /// Represents data associated with a node in the Assimp scene hierarchy.
    /// </summary>
    public struct SceneNodeData
    {
        public Matrix4 transformation;
        public string name;
        public int childrenCount;
        public List<SceneNodeData> children;
    };

    /// <summary>
    /// Represents an animation associated with a 3D model.
    /// </summary>
    public class Animation3D : ISerialization
    {
        /// <summary>
        /// Name of the animation.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Duration of the animation in ticks.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Number of ticks per second for the animation.
        /// </summary>
        public float TicksPerSecond { get; set; }

        /// <summary>
        /// List of bones affected by the animation.
        /// </summary>
        public List<Bone> Bones { get; set; }

        /// <summary>
        /// Root node of the animation's scene hierarchy.
        /// </summary>
        public SceneNodeData RootNode { get; set; }

        /// <summary>
        /// Mapping of bone names to bone information.
        /// </summary>
        public Dictionary<String, BoneInfo> BoneInfoMap { get; set; }

        /// <summary>
        /// The animation channels associated with the animation.
        /// </summary>
        private List<AnimationChannel> _animationChannels;

        /// <summary>
        /// Initializes a new instance of the Animation3D class.
        /// Used for deserialization purposes.
        /// </summary>
        public Animation3D()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the Animation class.
        /// </summary>
        public Animation3D(Assimp.Scene scene, int index, Skeleton skeleton)
        {
            this.Bones = new List<Bone>();
            var animation = scene.Animations[index];
            this.Name = animation.Name;
            this.Duration = (float)animation.DurationInTicks;
            this.TicksPerSecond = (float)animation.TicksPerSecond;
            var rootNode = new SceneNodeData();
            this.ReadHeirarchyData(ref rootNode, scene.RootNode);
            this.RootNode = rootNode;
            LoadAnimationChannel(animation);
            ReadBones(skeleton);
        }

        /// <summary>
        /// Initializes a new instance of the Animation class without a model reference.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="index"></param>
        public Animation3D(Assimp.Scene scene, int index)
        {
            this.Bones = new List<Bone>();
            var animation = scene.Animations[index];
            this.Name = animation.Name;
            this.Duration = (float)animation.DurationInTicks;
            this.TicksPerSecond = (float)animation.TicksPerSecond;
            var rootNode = new SceneNodeData();
            this.ReadHeirarchyData(ref rootNode, scene.RootNode);
            this.RootNode = rootNode;
            this.LoadAnimationChannel(animation);
        }

        /// <summary>
        /// Reads hierarchy data from the Assimp scene node.
        /// </summary>
        void ReadHeirarchyData(ref SceneNodeData dest, Assimp.Node src)
        {
            Debug.Assert(src != null);
            dest.name = src.Name;
            dest.transformation = (Matrix4) src.Transform;
            dest.childrenCount = src.ChildCount;
            dest.children = new List<SceneNodeData>();

            for (int i = 0; i < src.ChildCount; i++)
            {
                SceneNodeData newData = new SceneNodeData();
                ReadHeirarchyData(ref newData, src.Children[i]);
                dest.children.Add(newData);
            }
        }

        public void LoadAnimationChannel(Assimp.Animation animation)
        {
            _animationChannels = new List<AnimationChannel>();

            foreach (var nodeChannel in animation.NodeAnimationChannels)
            {
                var channel = new AnimationChannel();
                channel.BoneName = nodeChannel.NodeName;

                int positionKeyCount = nodeChannel.PositionKeyCount;
                for (int positionIndex = 0; positionIndex < positionKeyCount; positionIndex++)
                {
                    var assimpPosition = nodeChannel.PositionKeys[positionIndex];
                    var position = new KeyPosition();
                    position.position = new Vector3(assimpPosition.Value.X, assimpPosition.Value.Y, assimpPosition.Value.Z);
                    position.timeStamp = (float)assimpPosition.Time;
                    channel.Positions.Add(position);
                }

                int rotationKeyCount = nodeChannel.RotationKeyCount;
                for (int rotationIndex = 0; rotationIndex < rotationKeyCount; rotationIndex++)
                {
                    System.Numerics.Quaternion aiOrientation = nodeChannel.RotationKeys[rotationIndex].Value;
                    var assimpRotation = nodeChannel.RotationKeys[rotationIndex];
                    var rotation = new KeyRotation();
                    rotation.orientation = new OpenTK.Mathematics.Quaternion(aiOrientation.X, aiOrientation.Y, aiOrientation.Z, aiOrientation.W);
                    rotation.timeStamp = (float)assimpRotation.Time;
                    channel.Rotations.Add(rotation);
                }

                int scalingKeyCount = nodeChannel.ScalingKeyCount;
                for (int scalingIndex = 0; scalingIndex < scalingKeyCount; scalingIndex++)
                {
                    var assimpScale = nodeChannel.ScalingKeys[scalingIndex];
                    var scale = new KeyScale();
                    scale.scale = new Vector3(assimpScale.Value.X, assimpScale.Value.Y, assimpScale.Value.Z);
                    scale.timeStamp = (float)assimpScale.Time;
                    channel.Scales.Add(scale);
                }

                _animationChannels.Add(channel);
            }
        }

        /// <summary>
        /// Reads bones from the provided skeleton and associates them with the animation.
        /// </summary>
        /// <param name="skeleton"></param>
        public void ReadBones(Skeleton skeleton)
        {
            foreach(var channel in _animationChannels)
            {
                if (!skeleton.BoneInfoMap.ContainsKey(channel.BoneName))
                {
                    BoneInfo boneinfo = new BoneInfo();
                    boneinfo.id = skeleton.BoneCounter;
                    // why no offset????
                    skeleton.BoneInfoMap.Add(channel.BoneName, boneinfo);
                    skeleton.BoneCounter++;
                }
                Bones.Add(new Bone(channel.BoneName, skeleton.BoneInfoMap[channel.BoneName].id, channel));
            }

            BoneInfoMap = skeleton.BoneInfoMap;
        }

        /// <summary>
        /// Finds a bone with the specified name.
        /// </summary>
        public Bone FindBone(string name)
        {
            var bone = Bones.FirstOrDefault(b => b.Name == name);
            if (bone != null)
            {
                return bone;
            }
            return null;
        }

        /// <summary>
        /// Calculates the keyframe length of the animation based on the maximum number of position, rotation, and scaling keyframes among all bones.
        /// </summary>
        /// <returns>
        /// The maximum number of keyframes (positions, rotations, or scalings) among all bones; if no bones are present, returns -1.
        /// </returns>
        /// <remarks>
        /// This method iterates through all bones to determine the maximum keyframe length.
        /// </remarks>
        public int AnimationLength()
        {
            var length = -1;

            foreach (var bone in Bones)
            {
                var boneKeyframes = System.Math.Max(bone.AnimationChannel.NumPositions, System.Math.Max(bone.AnimationChannel.NumRotations, bone.AnimationChannel.NumScalings));
                if (boneKeyframes > length)
                {
                    length = boneKeyframes;
                }
            }

            return length;
        }

        /// <summary>
        /// Calculates the keyframe index based on the specified animation time.
        /// </summary>
        /// <param name="animationTime"></param>
        /// <returns></returns>
        public int GetKeyFrameIndex(float animationTime)
        {
            int totalFrames = this.AnimationLength();
            int frame = (int)((animationTime / Duration) * totalFrames);

            return frame;
        }

        /// <summary>
        /// Serializes the animation and its related data into a JSON object.
        /// </summary>
        /// <remarks>The returned JSON object includes all key components of the animation, making it
        /// suitable for storage, transmission, or further processing. The structure of the output matches the expected
        /// schema for deserialization or interoperability with other systems.</remarks>
        /// <param name="serializationContext">The context that provides settings and state information required for serialization.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized representation of the animation, including bones, bone
        /// information, root node, and animation channels.</returns>
        public void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(this.GetType().FullName);
            writer.WritePropertyName("Name");
            writer.WriteValue(this.Name);
            writer.WritePropertyName("Duration");
            writer.WriteValue(this.Duration);
            writer.WritePropertyName("TicksPerSecond");
            writer.WriteValue(this.TicksPerSecond);

            writer.WritePropertyName("Bones");
            writer.WriteStartArray();
            foreach (var bone in Bones)
            {
                bone.Serialize(writer, serializationContext);
            }
            writer.WriteEndArray();

            writer.WritePropertyName("RootNode");
            Core.Utils.SerializeSceneNodeData(RootNode, writer);

            writer.WritePropertyName("BoneInfoMap");
            writer.WriteStartArray();
            foreach (var boneinfo in this.BoneInfoMap)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Key");
                writer.WriteValue(boneinfo.Key);
                writer.WritePropertyName("Value");
                Core.Utils.SerializeBoneInfo(boneinfo.Value, writer);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            
            writer.WritePropertyName("AnimationChannels");
            writer.WriteStartArray();
            foreach (var channel in _animationChannels)
            {
                channel.Serialize(writer, serializationContext);
            }
            writer.WriteEndArray();

            // Callback for additional serialization
            callback?.Invoke(writer);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Deserializes the list of bones from the JSON reader.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="serializationContext"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        private List<Bone> DeserializeBones(JsonReader reader, SerializationContext serializationContext)
        {
            if (reader.TokenType != JsonToken.StartArray)
                throw new JsonException("Expected StartArray token for Bones");

            var bones = new List<Bone>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndArray)
                    break;

                if (reader.TokenType == JsonToken.StartObject)
                {
                    var bone = new Bone();
                    bone.Deserialize(reader, serializationContext);
                    bones.Add(bone);
                }
            }
            return bones;
        }

        /// <summary>
        /// Deserializes the bone info map from the JSON reader.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="serializationContext"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        private Dictionary<string, BoneInfo> DeserializeBoneInfoMap(JsonReader reader, SerializationContext serializationContext)
        {
            if(reader.TokenType != JsonToken.StartArray)
                throw new JsonException("Expected StartArray token for BoneInfoMap");

            var boneInfoMap = new Dictionary<string, BoneInfo>();

            while (reader.Read())
            {
                if( reader.TokenType == JsonToken.EndArray)
                    break;

                if (reader.TokenType == JsonToken.StartObject)
                {
                    string key = null;
                    BoneInfo value = new BoneInfo();

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.EndObject)
                            break;

                        if (reader.TokenType == JsonToken.PropertyName)
                        {
                            string propertyName = (string)reader.Value;
                            reader.Read();
                            switch (propertyName)
                            {
                                case "Key":
                                    key = reader.Value as string;
                                    break;
                                case "Value":
                                    value = Core.Utils.DeserializeBoneInfo(reader);
                                    break;
                            }
                        }
                    }
                    if (key != null)
                    {
                        boneInfoMap.Add(key, value);
                    }
                }
            }
            return boneInfoMap;
        }

        /// <summary>
        /// Deserializes the list of animation channels from the JSON reader.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="serializationContext"></param>
        /// <returns></returns>
        private List<AnimationChannel> DeserializeAnimationChannels(JsonReader reader, SerializationContext serializationContext)
        {
            var channels = new List<AnimationChannel>();

            if (reader.TokenType == JsonToken.StartArray)
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndArray)
                        break;

                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        var channel = new AnimationChannel();
                        channel.Deserialize(reader, serializationContext);
                        channels.Add(channel);
                    }
                }
            }
            return channels;
        }

        /// <summary>
        /// Populates the object's properties by deserializing data from the specified JSON object.
        /// </summary>
        /// <remarks>This method expects the JSON object to include all necessary fields such as 'Name',
        /// 'Duration', 'TicksPerSecond', 'Bones', 'BoneInfoMap', 'AnimationChannels', and 'RootNode'. Existing property
        /// values will be overwritten. The method does not perform deep validation of the JSON structure; missing or
        /// malformed fields may result in exceptions.</remarks>
        /// <param name="jObject">A <see cref="JObject"/> containing the serialized data to deserialize. Must not be null and is expected to
        /// contain all required fields for the object.</param>
        /// <param name="serializationContext">A <see cref="SerializationContext"/> providing context or settings used during deserialization. This may
        /// influence how certain fields are interpreted or constructed.</param>
        public void Deserialize(JsonReader reader, SerializationContext serializationContext, Func<JsonReader, string, bool> callback = null)
        {
            if(reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException("Expected StartObject token");

            while (reader.Read())
            {
                if(reader.TokenType == JsonToken.EndObject)
                    break;

                if(reader.TokenType == JsonToken.PropertyName)
                {
                    string propertyName = (string)reader.Value;
                    reader.Read(); // Move to the value token

                    switch (propertyName)
                    {
                        case "Type":
                            reader.Skip();
                            break;
                        case "Name":
                            this.Name = reader.Value as string ?? string.Empty;
                            break;
                        case "Duration":
                            this.Duration = Convert.ToSingle(reader.Value);
                            break;
                        case "TicksPerSecond":
                            this.TicksPerSecond = Convert.ToSingle(reader.Value);
                            break;
                        case "Bones":
                            this.Bones = DeserializeBones(reader, serializationContext);
                            break;
                        case "BoneInfoMap":
                            this.BoneInfoMap = DeserializeBoneInfoMap(reader, serializationContext);
                            break;
                        case "AnimationChannels":
                            _animationChannels = DeserializeAnimationChannels(reader, serializationContext);
                            break;
                        case "RootNode":
                            this.RootNode = Core.Utils.DeserializeSceneNodeData(reader);
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
