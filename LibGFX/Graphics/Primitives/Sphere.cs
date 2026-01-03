using LibGFX.Graphics.Materials;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Primitives
{
    public class Sphere : IPrimitive<Sphere>
    {
        public static Mesh GetMesh(IMaterial material = null)
        {
            return Generator.SphereGenerator.CreateSphere(20, 20, 0.5f, material);
        }
    }
}
