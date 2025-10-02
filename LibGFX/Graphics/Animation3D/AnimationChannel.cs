using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
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
    /// Represents an animation channel for a specific bone, containing keyframes for position, rotation, and scale.
    /// </summary>
    public class AnimationChannel
    {
        public String BoneName { get; set; }
        public List<KeyPosition> Positions { get; set; } = new List<KeyPosition>();
        public List<KeyRotation> Rotations { get; set; } = new List<KeyRotation>();
        public List<KeyScale> Scales { get; set; } = new List<KeyScale>();

        public int NumPositions => Positions.Count;
        public int NumRotations => Rotations.Count;
        public int NumScalings => Scales.Count;
    }
}
