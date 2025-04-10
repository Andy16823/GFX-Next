﻿using Assimp;
using LibGFX.Math;
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
    /// Controls the animation playback of a 3D model.
    /// </summary>
    public class Animator
    {
        /// <summary>
        /// List of final bone transformation matrices.
        /// </summary>
        public List<Matrix4> FinalBoneMatrices { get; set; }

        /// <summary>
        /// Currently active animation.
        /// </summary>
        public Animation CurrentAnimation { get; set; }

        /// <summary>
        /// Current time in the animation.
        /// </summary>
        public float CurrentTime { get; set; }

        /// <summary>
        /// Time elapsed since last frame.
        /// </summary>
        public float DeltaTime { get; set; }

        /// <summary>
        /// Flag indicating whether the animation is playing.
        /// </summary>
        public bool Play { get; set; } = true;

        /// <summary>
        /// Flag indicating whether to interpolate frames during animation playback.
        /// </summary>
        public bool InterpolateFrames { get; set; } = true;

        /// <summary>
        /// Flag indicating whether to loop the animation.
        /// </summary>
        public bool Loop { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the Animator class with the specified animation.
        /// </summary>
        public Animator(Animation animation)
        {
            this.CurrentTime = 0;
            this.CurrentAnimation = animation;

            FinalBoneMatrices = new List<Matrix4>();
            for (int i = 0; i < 100; i++)
            {
                FinalBoneMatrices.Add(Matrix4.Identity);
            }
        }

        /// <summary>
        /// Initializes a new instance of the Animator class.
        /// </summary>
        public Animator()
        {
            this.CurrentTime = 0;

            FinalBoneMatrices = new List<Matrix4>();
            for (int i = 0; i < 100; i++)
            {
                FinalBoneMatrices.Add(Matrix4.Identity);
            }
        }

        /// <summary>
        /// Updates the animation based on the elapsed time since the last frame.
        /// </summary>
        public void UpdateAnimation(float dt)
        {
            if (this.Play)
            {
                this.DeltaTime = dt;
                if (CurrentAnimation != null)
                {
                    this.CurrentTime += CurrentAnimation.TicksPerSecond * dt;
                    if (CurrentTime >= CurrentAnimation.Duration && !this.Loop)
                    {
                        return;
                    }
                    CurrentTime = CurrentTime % CurrentAnimation.Duration;
                    CalculateBoneTransform(CurrentAnimation.RootNode, Matrix4.Identity);
                }
            }
        }

        /// <summary>
        /// Loads a new animation.
        /// </summary>
        public void LoadAnimation(Animation3D.Animation animation)
        {
            this.Play = false;
            this.CurrentAnimation = animation;
            this.CurrentTime = 0;
            this.Play = true;
        }

        /// <summary>
        /// Calculates bone transformations recursively based on the animation hierarchy.
        /// </summary>
        public void CalculateBoneTransform(AssimpNodeData node, Matrix4 parentTransform)
        {
            string nodeName = node.name;
            Matrix4 nodeTransform = node.transformation;

            Bone Bone = CurrentAnimation.FindBone(nodeName);

            if (Bone != null)
            {
                Bone.Update(CurrentTime, this.InterpolateFrames);
                nodeTransform = Bone.LocalTransform;
            }

            Matrix4 globalTransformation = nodeTransform * parentTransform;

            var boneInfoMap = CurrentAnimation.BoneInfoMap;
            if (boneInfoMap.ContainsKey(nodeName))
            {
                int index = boneInfoMap[nodeName].id;
                Matrix4 offset = boneInfoMap[nodeName].offset;
                //Debug.Assert(!glm.IsNaN(offset.m00));
                FinalBoneMatrices[index] = offset * globalTransformation;
            }

            for (int i = 0; i < node.childrenCount; i++)
            {
                CalculateBoneTransform(node.children[i], globalTransformation);
            }
        }
    }
}
