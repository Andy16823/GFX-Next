using Assimp;
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
        public OpenTK.Mathematics.Quaternion orientation;
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
    /// Represents a bone in a skeletal animation system.
    /// </summary>
    public class Bone
    {
        /// <summary>
        /// List of position keyframes for the bone.
        /// </summary>
        public List<KeyPosition> Positions { get; set; }

        /// <summary>
        /// Number of position keyframes.
        /// </summary>
        public int NumPositions { get; set; }

        /// <summary>
        /// List of rotation keyframes for the bone.
        /// </summary>
        public List<KeyRotation> Rotations { get; set; }

        /// <summary>
        /// Number of rotation keyframes.
        /// </summary>
        public int NumRotations { get; set; }

        /// <summary>
        /// List of scale keyframes for the bone.
        /// </summary>
        public List<KeyScale> Scales { get; set; }

        /// <summary>
        /// Number of scale keyframes.
        /// </summary>
        public int NumScalings { get; set; }

        /// <summary>
        /// Local transformation matrix of the bone.
        /// </summary>
        public Matrix4 LocalTransform { get; set; }

        /// <summary>
        /// Name of the bone.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// ID of the bone.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Initializes a new instance of the Bone class.
        /// </summary>
        public Bone(String name, int id, Assimp.NodeAnimationChannel channel)
        {
            this.Name = name;
            this.ID = id;
            this.LocalTransform = Matrix4.Identity;

            this.Positions = new List<KeyPosition>();
            NumPositions = channel.PositionKeyCount;
            for (int positionIndex = 0; positionIndex < NumPositions; ++positionIndex)
            {
                Vector3D aiPosition = channel.PositionKeys[positionIndex].Value;
                float timeStamp = (float)channel.PositionKeys[positionIndex].Time;
                KeyPosition data;
                data.position = new Vector3(aiPosition.X, aiPosition.Y, aiPosition.Z);
                data.timeStamp = timeStamp;
                Positions.Add(data);
            }

            this.Rotations = new List<KeyRotation>();
            NumRotations = channel.RotationKeyCount;
            for (int rotationIndex = 0; rotationIndex < NumRotations; ++rotationIndex)
            {
                Assimp.Quaternion aiOrientation = channel.RotationKeys[rotationIndex].Value;
                float timeStamp = (float)channel.RotationKeys[rotationIndex].Time;
                KeyRotation data;
                data.orientation = new OpenTK.Mathematics.Quaternion(aiOrientation.X, aiOrientation.Y, aiOrientation.Z, aiOrientation.W);
                data.timeStamp = timeStamp;
                Rotations.Add(data);
            }

            this.Scales = new List<KeyScale>();
            NumScalings = channel.ScalingKeyCount;
            for (int keyIndex = 0; keyIndex < NumScalings; ++keyIndex)
            {
                Vector3D scale = channel.ScalingKeys[keyIndex].Value;
                float timeStamp = (float)channel.ScalingKeys[keyIndex].Time;
                KeyScale data;
                data.scale = new Vector3(scale.X, scale.Y, scale.Z);
                data.timeStamp = timeStamp;
                Scales.Add(data);
            }
        }

        /// <summary>
        /// Gets the index of the position keyframe at the specified animation time.
        /// </summary>
        public int GetPositionIndex(float animationTime)
        {
            for (int index = 0; index < NumPositions - 1; ++index)
            {
                if (animationTime < Positions[index + 1].timeStamp)
                    return index;
            }
            Debug.Assert(false);
            return -1;
        }

        /// <summary>
        /// Gets the index of the rotation keyframe at the specified animation time.
        /// </summary>
        public int GetRotationIndex(float animationTime)
        {
            for (int index = 0; index < NumRotations - 1; ++index)
            {
                if (animationTime < Rotations[index + 1].timeStamp)
                    return index;
            }
            Debug.Assert(false);
            return -1;
        }

        /// <summary>
        /// Gets the index of the scale keyframe at the specified animation time.
        /// </summary>
        public int GetScaleIndex(float animationTime)
        {
            for (int index = 0; index < NumScalings - 1; ++index)
            {
                if (animationTime < Scales[index + 1].timeStamp)
                    return index;
            }
            Debug.Assert(false);
            return -1;
        }

        /// <summary>
        /// Calculates the interpolation factor between two keyframes.
        /// </summary>
        public float GetScaleFactor(float lastTimeStamp, float nextTimeStamp, float animationTime)
        {
            float scaleFactor = 0.0f;
            float midWayLength = animationTime - lastTimeStamp;
            float framesDiff = nextTimeStamp - lastTimeStamp;
            scaleFactor = midWayLength / framesDiff;
            return scaleFactor;
        }

        public Matrix4 InterpolatePosition(float animationTime)
        {
            if (NumPositions == 1)
                return Matrix4.CreateTranslation(Positions[0].position);

            int p0Index = GetPositionIndex(animationTime);
            int p1Index = p0Index + 1;
            float scaleFactor = GetScaleFactor(Positions[p0Index].timeStamp, Positions[p1Index].timeStamp, animationTime);
            Vector3 finalPosition = Vector3.Lerp(Positions[p0Index].position, Positions[p1Index].position, scaleFactor);

            return Matrix4.CreateTranslation(finalPosition);
        }

        public Matrix4 InterpolateRotation(float animationTime)
        {
            if (NumRotations == 1)
            {
                var rotation = Rotations[0].orientation.Normalized();
                return Matrix4.CreateFromQuaternion(rotation);
            }

            int p0Index = GetRotationIndex(animationTime);
            int p1Index = p0Index + 1;
            float scaleFactor = GetScaleFactor(Rotations[p0Index].timeStamp, Rotations[p1Index].timeStamp, animationTime);

            OpenTK.Mathematics.Quaternion finalRotation = OpenTK.Mathematics.Quaternion.Slerp(Rotations[p0Index].orientation, Rotations[p1Index].orientation, scaleFactor);
            finalRotation.Normalize();

            if (float.IsNaN(finalRotation.Length))
            {
                var rotation = Rotations[p0Index].orientation.Normalized();
                return Matrix4.CreateFromQuaternion(rotation);
            }

            return Matrix4.CreateFromQuaternion(finalRotation);
        }

        public Matrix4 InterpolateScaling(float animationTime)
        {
            if (NumScalings == 1)
                return Matrix4.CreateScale(Scales[0].scale);

            int p0Index = GetScaleIndex(animationTime);
            int p1Index = p0Index + 1;
            float scaleFactor = GetScaleFactor(Scales[p0Index].timeStamp, Scales[p1Index].timeStamp, animationTime);
            Vector3 finalScale = Vector3.Lerp(Scales[p0Index].scale, Scales[p1Index].scale, scaleFactor);

            return Matrix4.CreateScale(finalScale);
        }

        /// <summary>
        /// Updates the bone transformation based on the animation time.
        /// </summary>
        public void Update(float animationTime, bool interpolate)
        {
            if (interpolate)
            {
                Matrix4 translation = InterpolatePosition(animationTime);
                Matrix4 rotation = InterpolateRotation(animationTime);
                Matrix4 scale = InterpolateScaling(animationTime);
                LocalTransform = scale * rotation * translation; // translation * rotation * scale;
            }
            else
            {
                Matrix4 translation = Matrix4.CreateTranslation(Positions[GetPositionIndex(animationTime)].position);
                Matrix4 rotation = Matrix4.CreateFromQuaternion(Rotations[GetRotationIndex(animationTime)].orientation);
                Matrix4 scale = Matrix4.CreateScale(Scales[GetScaleIndex(animationTime)].scale);
                LocalTransform = scale * rotation * translation; // translation * rotation * scale;
            }
        }
    }
}
