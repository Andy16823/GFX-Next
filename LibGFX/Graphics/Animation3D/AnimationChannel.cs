using LibGFX.Core;
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
    public class AnimationChannel : ISerialization
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

        public JObject Serialize(SerializationContext serializationContext)
        {
            // Serialize Positions
            JArray positionsArray = new JArray();
            foreach (var pos in this.Positions)
            {
                positionsArray.Add(Utils.SerializeKeyPosition(pos));
            }

            // Serialize Rotations
            JArray rotationsArray = new JArray();
            foreach (var rot in this.Rotations)
            {
                rotationsArray.Add(Utils.SerializeKeyRotation(rot));
            }

            // Serialize Scales
            JArray scalesArray = new JArray();
            foreach (var scale in this.Scales)
            {
                scalesArray.Add(Utils.SerializeKeyScale(scale));
            }

            // Return the final JObject
            return new JObject()
            {
                ["Type"] = this.GetType().FullName,
                ["BoneName"] = this.BoneName,
                ["Positions"] = positionsArray,
                ["Rotations"] = rotationsArray,
                ["Scales"] = scalesArray
            };
        }

        public void Deserialize(JObject jObject, SerializationContext serializationContext)
        {
            // Deserialize BoneName
            this.BoneName = jObject.Value<string>("BoneName");

            // Deserialize Positions
            this.Positions.Clear();
            JArray positionsArray = jObject.Value<JArray>("Positions");
            foreach (var posToken in positionsArray)
            {
                KeyPosition pos = Utils.DeserializeKeyPosition(posToken as JObject);
                this.Positions.Add(pos);
            }

            // Deserialize Rotations
            this.Rotations.Clear();
            JArray rotationsArray = jObject.Value<JArray>("Rotations");
            foreach (var rotToken in rotationsArray)
            {
                KeyRotation rot = Utils.DeserializeKeyRotation(rotToken as JObject);
                this.Rotations.Add(rot);
            }

            // Deserialize Scales
            this.Scales.Clear();
            JArray scalesArray = jObject.Value<JArray>("Scales");
            foreach (var scaleToken in scalesArray)
            {
                KeyScale scale = Utils.DeserializeKeyScale(scaleToken as JObject);
                this.Scales.Add(scale);
            }
        }
    }
}
