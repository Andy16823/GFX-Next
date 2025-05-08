using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Animation3D
{
    /// <summary>
    /// Interface for animation callbacks.
    /// With this interface, you can create custom callbacks that are triggered at specific frames during the animation.
    /// You can also control the activation state of the callback.
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
        public void OnTriggered(float time);

        /// <summary>
        /// Called when the animation ends.
        /// </summary>
        /// <param name="time"></param>
        public void OnAnimationEnd(float time);
    }
}
