using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    /// <summary>
    /// Class to hold render statistics
    /// </summary>
    public class RenderStats
    {
        /// <summary>
        /// Current frame time in milliseconds
        /// </summary>
        public long CurrentFrameTime { get; set; }

        /// <summary>
        /// Last frame time in milliseconds
        /// </summary>
        public long LastFrameTime { get; set; }

        /// <summary>
        /// Delta time in milliseconds
        /// </summary>
        public long DeltaTime { get; set; }

        /// <summary>
        /// TotalFrames per second
        /// </summary>
        public int FPS { get; set; }

        /// <summary>
        /// Total draw calls made during the current frame
        /// </summary>
        public int DrawCalls => _totalDrawCalls;


        private int _totalDrawCalls = 0;


        /// <summary>
        /// New frame method to be called at the beginning of each frame
        /// </summary>
        public void NewFrame()
        {
            LastFrameTime = CurrentFrameTime;
            CurrentFrameTime = Utils.GetCurrentTimeMillis();
            DeltaTime = CurrentFrameTime - LastFrameTime;
            if (DeltaTime > 0)
            {
                FPS = (int)(1000 / DeltaTime);
            }
            _totalDrawCalls = 0;
        }

        /// <summary>
        /// Start method to initialize the render stats
        /// </summary>
        public void Start()
        {
            CurrentFrameTime = Utils.GetCurrentTimeMillis();
            LastFrameTime = CurrentFrameTime;
            DeltaTime = 0;
            FPS = 0;
            _totalDrawCalls = 0;
        }

        /// <summary>
        /// Reset method to reset the render stats
        /// </summary>
        public void Reset()
        {
            CurrentFrameTime = 0;
            LastFrameTime = 0;
            DeltaTime = 0;
            FPS = 0;
            _totalDrawCalls = 0;
        }

        /// <summary>
        /// Increment the draw calls count by the specified amount
        /// </summary>
        /// <param name="count"></param>
        public void IncrementDrawCalls(int count = 1)
        {
            _totalDrawCalls += count;
        }

        /// <summary>
        /// Override ToString method to display the render stats
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"FPS: {FPS}, DeltaTime: {DeltaTime}ms";
        }
    }
}
