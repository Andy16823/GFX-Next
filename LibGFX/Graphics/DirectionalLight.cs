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
        public DirectionalLight()
        {
            this.Position = new Vector3(5.0f, 15.0f, 5.0f);
            this.Color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            this.Intensity = 0.3f;
        }

        public DirectionalLight(Vector3 position, Vector4 color, float intensity)
        {
            this.Position = position;
            this.Color = color;
            this.Intensity = intensity;
        }
    }
}
