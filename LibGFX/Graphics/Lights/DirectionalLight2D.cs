using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Lights
{
    public class DirectionalLight2D : Light
    {
        public Vector3 Direction { get; set; }

        public DirectionalLight2D(Vector4 color, float intensity)
        {
            Color = color;
            Intensity = intensity;
        }
    }
}
