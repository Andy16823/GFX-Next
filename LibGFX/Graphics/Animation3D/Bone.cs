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
    /// Represents a bone in a skeletal animation system.
    /// </summary>
    public class Bone
    {
        /// <summary>
        /// List of position keyframes for the bone.
        /// </summary>
        public AnimationChannel AnimationChannel { get; set; }

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

        public Bone(String name, int id, AnimationChannel animationChannel)
        {
            this.Name = name;
            this.ID = id;
            this.LocalTransform = Matrix4.Identity;
            this.AnimationChannel = animationChannel;
        }

        /// <summary>
        /// Gets the index of the position keyframe at the specified animation time.
        /// </summary>
        public int GetPositionIndex(float animationTime)
        {
            for (int index = 0; index < AnimationChannel.NumPositions - 1; ++index)
            {
                if (animationTime < AnimationChannel.Positions[index + 1].timeStamp)
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
            for (int index = 0; index < AnimationChannel.NumRotations - 1; ++index)
            {
                if (animationTime < AnimationChannel.Rotations[index + 1].timeStamp)
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
            for (int index = 0; index < AnimationChannel.NumScalings - 1; ++index)
            {
                if (animationTime < AnimationChannel.Scales[index + 1].timeStamp)
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
            if (AnimationChannel.NumPositions == 1)
                return Matrix4.CreateTranslation(AnimationChannel.Positions[0].position);

            int p0Index = GetPositionIndex(animationTime);
            int p1Index = p0Index + 1;
            float scaleFactor = GetScaleFactor(AnimationChannel.Positions[p0Index].timeStamp, AnimationChannel.Positions[p1Index].timeStamp, animationTime);
            Vector3 finalPosition = Vector3.Lerp(AnimationChannel.Positions[p0Index].position, AnimationChannel.Positions[p1Index].position, scaleFactor);

            return Matrix4.CreateTranslation(finalPosition);
        }

        public Matrix4 InterpolateRotation(float animationTime)
        {
            if (AnimationChannel.NumRotations == 1)
            {
                var rotation = AnimationChannel.Rotations[0].orientation.Normalized();
                return Matrix4.CreateFromQuaternion(rotation);
            }

            int p0Index = GetRotationIndex(animationTime);
            int p1Index = p0Index + 1;
            float scaleFactor = GetScaleFactor(AnimationChannel.Rotations[p0Index].timeStamp, AnimationChannel.Rotations[p1Index].timeStamp, animationTime);

            OpenTK.Mathematics.Quaternion finalRotation = OpenTK.Mathematics.Quaternion.Slerp(AnimationChannel.Rotations[p0Index].orientation, AnimationChannel.Rotations[p1Index].orientation, scaleFactor);
            finalRotation.Normalize();

            if (float.IsNaN(finalRotation.Length))
            {
                var rotation = AnimationChannel.Rotations[p0Index].orientation.Normalized();
                return Matrix4.CreateFromQuaternion(rotation);
            }

            return Matrix4.CreateFromQuaternion(finalRotation);
        }

        public Matrix4 InterpolateScaling(float animationTime)
        {
            if (AnimationChannel.NumScalings == 1)
                return Matrix4.CreateScale(AnimationChannel.Scales[0].scale);

            int p0Index = GetScaleIndex(animationTime);
            int p1Index = p0Index + 1;
            float scaleFactor = GetScaleFactor(AnimationChannel.Scales[p0Index].timeStamp, AnimationChannel.Scales[p1Index].timeStamp, animationTime);
            Vector3 finalScale = Vector3.Lerp(AnimationChannel.Scales[p0Index].scale, AnimationChannel.Scales[p1Index].scale, scaleFactor);

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
                Matrix4 translation = Matrix4.CreateTranslation(AnimationChannel.Positions[GetPositionIndex(animationTime)].position);
                Matrix4 rotation = Matrix4.CreateFromQuaternion(AnimationChannel.Rotations[GetRotationIndex(animationTime)].orientation);
                Matrix4 scale = Matrix4.CreateScale(AnimationChannel.Scales[GetScaleIndex(animationTime)].scale);
                LocalTransform = scale * rotation * translation; // translation * rotation * scale;
            }
        }
    }
}
