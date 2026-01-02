using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Primitives
{
    public enum PrimitiveType
    {
        Cube,
        Sphere,
        Quad,
    }

    public interface IPrimitive
    {
        Mesh GetMesh();    
    }
}
