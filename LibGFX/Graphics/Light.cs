using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public abstract class Light
    {
        public Vector4 Color { get; set; }
        public Vector3 Position { get; set; }
        public float Intensity { get; set; }
    }
}
