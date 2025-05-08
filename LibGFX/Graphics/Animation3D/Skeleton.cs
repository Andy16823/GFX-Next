using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Animation3D
{
    /// <summary>
    /// Represents a bone information for the rendering pipeline
    /// </summary>
    public class Skeleton
    {
        /// <summary>
        /// The Bones of the skeleton.
        /// </summary>
        public Dictionary<String, BoneInfo> BoneInfoMap { get; set; }

        /// <summary>
        /// The number of bones in the skeleton.
        /// </summary>
        public int BoneCounter;
    }
}
