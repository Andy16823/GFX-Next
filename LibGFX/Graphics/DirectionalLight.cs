using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public class DirectionalLight : Light
    {
        public Vector3 Direction { get; set; }
        public Vector3 Ambient { get; set; }
        public Vector3 Specular { get; set; }

        public DirectionalLight(Vector3 direction, Vector4 color, float intensity)
        {
            this.Position = Vector3.PositiveInfinity;
            this.Direction = direction;
            this.Color = color;
            this.Intensity = intensity;
            this.Ambient = new Vector3(0.2f, 0.2f, 0.2f);
            this.Specular = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }
}
