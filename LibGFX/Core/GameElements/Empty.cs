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

        public override void ComputeAABB()
        {
            this.AABB = new Math.AABB(Vector3.Zero, Vector3.Zero);
        }
    }
}
