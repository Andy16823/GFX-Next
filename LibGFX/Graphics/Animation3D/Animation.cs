using Assimp;
using LibGFX.Core.GameElements;
using LibGFX.Math;
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
    public struct AssimpNodeData
    {
        public Matrix4 transformation;
        public string name;
        public int childrenCount;
        public List<AssimpNodeData> children;
    };

    /// <summary>
    /// Represents an animation associated with a 3D model.
    /// </summary>
    public class Animation
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
        public AssimpNodeData RootNode { get; set; }

        /// <summary>
        /// Mapping of bone names to bone information.
        /// </summary>
        public Dictionary<String, BoneInfo> BoneInfoMap { get; set; }

        private List<AnimationChannel> _animationChannels; 

        /// <summary>
        /// Initializes a new instance of the Animation class.
        /// </summary>
        public Animation(Assimp.Scene scene, int index, Skeleton skeleton)
        {
            this.Bones = new List<Bone>();
            var animation = scene.Animations[index];
            this.Name = animation.Name;
            this.Duration = (float)animation.DurationInTicks;
            this.TicksPerSecond = (float)animation.TicksPerSecond;
            var rootNode = new AssimpNodeData();
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
        public Animation(Assimp.Scene scene, int index)
        {
            this.Bones = new List<Bone>();
            var animation = scene.Animations[index];
            this.Name = animation.Name;
            this.Duration = (float)animation.DurationInTicks;
            this.TicksPerSecond = (float)animation.TicksPerSecond;
            var rootNode = new AssimpNodeData();
            this.ReadHeirarchyData(ref rootNode, scene.RootNode);
            this.RootNode = rootNode;
            this.LoadAnimationChannel(animation);
        }

        /// <summary>
        /// Reads hierarchy data from the Assimp scene node.
        /// </summary>
        void ReadHeirarchyData(ref AssimpNodeData dest, Assimp.Node src)
        {
            Debug.Assert(src != null);
            dest.name = src.Name;
            dest.transformation = Math.Math.ToTKMatrix(src.Transform);
            dest.childrenCount = src.ChildCount;
            dest.children = new List<AssimpNodeData>();

            for (int i = 0; i < src.ChildCount; i++)
            {
                AssimpNodeData newData = new AssimpNodeData();
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
                    Assimp.Quaternion aiOrientation = nodeChannel.RotationKeys[rotationIndex].Value;
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
    }
}
