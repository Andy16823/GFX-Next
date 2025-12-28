using Assimp;
using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Math;
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
        public JObject Serialize(SerializationContext serializationContext)
        {
            // Serialize Bones
            var boneArray = new JArray();
            foreach (var bone in Bones)
            {
                boneArray.Add(bone.Serialize(serializationContext));
            }

            // Serialize BoneInfoMap
            var boneInfoArray = new JArray();
            foreach(var boneinfo in this.BoneInfoMap)
            {
                var boneInfoObject = new JObject()
                {
                    ["Key"] = boneinfo.Key,
                    ["Value"] = Core.Utils.SerializeBoneInfo(boneinfo.Value)
                };
                boneInfoArray.Add(boneInfoObject);
            }

            // Serialize AnimationChannels
            var animChannelArray = new JArray();
            foreach(var channel in _animationChannels)
            {
                animChannelArray.Add(channel.Serialize(serializationContext));
            }

            // Return the complete serialized object
            return new JObject()
            {
                ["Type"] = this.GetType().FullName,
                ["Name"] = this.Name,
                ["Duration"] = this.Duration,
                ["TicksPerSecond"] = this.TicksPerSecond,
                ["Bones"] = boneArray,
                ["RootNode"] = Core.Utils.SerializeSceneNodeData(RootNode),
                ["BoneInfoMap"] = boneInfoArray,
                ["AnimationChannels"] = animChannelArray
            };
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
        public void Deserialize(JObject jObject, SerializationContext serializationContext)
        {
            this.Name = jObject["Name"].ToString();
            this.Duration = jObject["Duration"].ToObject<float>();
            this.TicksPerSecond = jObject["TicksPerSecond"].ToObject<float>();

            // Deserialize Bones
            this.Bones = new List<Bone>();
            var boneArray = jObject["Bones"] as JArray;
            foreach (var boneToken in boneArray)
            {
                var bone = new Bone();
                bone.Deserialize(boneToken as JObject, serializationContext);
                this.Bones.Add(bone);
            }

            // Deserialize BoneInfoMap
            this.BoneInfoMap = new Dictionary<string, BoneInfo>();
            var boneInfoArray = jObject["BoneInfoMap"] as JArray;
            foreach (var boneInfoToken in boneInfoArray)
            {
                var key = boneInfoToken["Key"].ToString();
                var value = Core.Utils.DeserializeBoneInfo(boneInfoToken["Value"] as JObject);
                this.BoneInfoMap.Add(key, value);
            }

            // Deserialize AnimationChannels
            _animationChannels = new List<AnimationChannel>();
            var animChannelArray = jObject["AnimationChannels"] as JArray;
            foreach (var channelToken in animChannelArray)
            {
                var channel = new AnimationChannel();
                channel.Deserialize(channelToken as JObject, serializationContext);
                _animationChannels.Add(channel);
            }

            // Deserialize RootNode
            this.RootNode = Core.Utils.DeserializeSceneNodeData(jObject["RootNode"] as JObject);
        }
    }
}
