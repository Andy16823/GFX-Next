using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Animation2D
{
    /// <summary>
    /// Represents a callback interface for 2D animations.
    /// </summary>
    public interface IAnimationCallback
    {
        /// <summary>
        /// Gets the list of trigger frames for the animation callback.
        /// </summary>
        public IEnumerable<int> TriggerFrames { get; }

        /// <summary>
        /// Determines if the animation callback is active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// The animation associated with the callback.
        /// </summary>
        public Animation Animation { get; set; }

        /// <summary>
        /// Called when the animation is triggered.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="frame"></param>
        /// <param name="totalFrames"></param>
        public void OnTriggered(int frame);

        /// <summary>
        /// Called when the animation ends.
        /// </summary>
        /// <param name="time"></param>
        public void OnAnimationEnd(int frame);
    }
}
