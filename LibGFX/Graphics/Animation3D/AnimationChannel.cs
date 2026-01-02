using LibGFX.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public Quaternion orientation;
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
        /// <summary>
        /// Gets or sets the name of the bone associated with this instance.
        /// </summary>
        public String BoneName { get; set; }

        /// <summary>
        /// Gets or sets the collection of key positions associated with the object.
        /// </summary>
        public List<KeyPosition> Positions { get; set; } = new List<KeyPosition>();

        /// <summary>
        /// Gets or sets the collection of key rotation records associated with this entity.
        /// </summary>
        /// <remarks>Each entry in the collection represents a single key rotation event, including
        /// relevant metadata such as the rotation date and status. Modifying this collection affects the recorded
        /// history of key rotations for the entity.</remarks>
        public List<KeyRotation> Rotations { get; set; } = new List<KeyRotation>();

        /// <summary>
        /// Gets or sets the collection of key scales associated with this instance.
        /// </summary>
        public List<KeyScale> Scales { get; set; } = new List<KeyScale>();

        /// <summary>
        /// Gets the number of positions in the collection.
        /// </summary>
        public int NumPositions => Positions.Count;

        /// <summary>
        /// Gets the number of rotations in the collection.
        /// </summary>
        public int NumRotations => Rotations.Count;

        /// <summary>
        /// Gets the number of scaling operations currently defined.
        /// </summary>
        public int NumScalings => Scales.Count;
    }
}
