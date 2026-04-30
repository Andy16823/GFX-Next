using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core.GameElements
{
    /// <summary>
    /// Represents an empty game element that does not have any visual representation.
    /// </summary>
    public class Empty : GameElement
    {
        /// <summary>
        /// Creates a new instance of the Empty class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        public Empty(String name, Vector3 position)
        {
            this.Name = name;
            this.Transform = new Math.Transform(position, Vector3.One);
        }

        /// <summary>
        /// Gets a value indicating whether the image contains any transparent pixels.
        /// </summary>
        public override bool HasTransparency => false;

        public override void ComputeAABB()
        {
            this.AABB = new Math.AABB(Vector3.Zero, Vector3.Zero);
        }

        /// <summary>
        /// Returns a new instance of the Empty class that is a copy of the current instance.
        /// </summary>
        /// <returns></returns>
        public override GameElement Clone()
        {
            var clone = new Empty(this.Name, this.Transform.Position);
            clone.Transform = this.Transform.Clone();

            foreach (var behavior in this.Behaviors)
            {
                var cloneBehavior = behavior.Clone();
                clone.AddBehavior(cloneBehavior);
            }

            return clone;
        }
    }
}
