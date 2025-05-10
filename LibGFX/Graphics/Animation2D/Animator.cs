using LibGFX.Graphics.Materials;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Animation2D
{
    /// <summary>
    /// Represents an animator for 2D animations.
    /// </summary>
    public class Animator
    {
        /// <summary>
        /// The list of animations associated with this animator.
        /// </summary>
        public List<Animation> Animations { get; set; }

        /// <summary>
        /// The current animation being played.
        /// </summary>
        public Animation CurrentAnimation { get; set; }

        /// <summary>
        /// The list of animation callbacks associated with this animator.
        /// </summary>
        public List<IAnimationCallback> AnimationCallbacks { get; set; }

        public bool Loop { get; set; } = true;

        // Indicates if the animation is currently playing
        private bool _isPlaying = false;

        /// <summary>
        /// Creates a new animator.
        /// </summary>
        public Animator()
        {
            Animations = new List<Animation>();
            AnimationCallbacks = new List<IAnimationCallback>();
        }

        /// <summary>
        /// Updates the animator and the current animation.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            if (!_isPlaying)
                return;

            if (CurrentAnimation != null)
            {
                // Update the current animation and check if it has finished
                bool finished; 
                CurrentAnimation.Update(deltaTime, out finished);

                // Trigger callbacks for the current animation within the current frame
                var currentFrame = CurrentAnimation.CurrentFrame;
                foreach (var callback in AnimationCallbacks) 
                {
                    if(callback.Active && callback.Animation == CurrentAnimation)
                    {
                        if (callback.TriggerFrames.Contains(currentFrame))
                        {
                            callback.OnTriggered(currentFrame);
                        }
                    }
                }

                // Check if the animation has finished
                if (finished)
                {
                    // Trigger end callbacks for the current animation
                    foreach (var callback in AnimationCallbacks)
                    {
                        if (callback.Active && callback.Animation == CurrentAnimation)
                        {
                            callback.OnAnimationEnd(CurrentAnimation.CurrentFrame);
                        }
                    }

                    // Stop the animation if it is not set to loop
                    if (!Loop)
                    {
                        _isPlaying = false;
                    }
                }
            }
        }

        /// <summary>
        /// Plays the specified animation.
        /// </summary>
        /// <param name="animationName"></param>
        public void PlayAnimation(string animationName)
        {
            var animation = Animations.FirstOrDefault(a => a.Name == animationName);
            if (animation != null)
            {
                CurrentAnimation = animation;
                CurrentAnimation.CurrentFrame = 0;
                _isPlaying = true;
            }
        }

        /// <summary>
        /// Stops the current animation.
        /// </summary>
        public void Stop()
        {
            _isPlaying = false;
        }

        /// <summary>
        /// Gets the current frame of the animation. It returns the material and the UV transform of the current frame.
        /// </summary>
        /// <returns></returns>
        public (SpriteMaterial, Vector4) GetCurrentFrame()
        {
            if (CurrentAnimation != null)
            {
                return (CurrentAnimation.Material, CurrentAnimation.GetUVTransform());
            }
            return (null, Vector4.Zero);
        }
    }
}
