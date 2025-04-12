using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core.GameElements
{
    public class Empty : GameElement
    {
        public Empty(String name, Vector3 position)
        {
            this.Name = name;
            this.Transform = new Math.Transform(position, Vector3.One);
        }
    }
}
