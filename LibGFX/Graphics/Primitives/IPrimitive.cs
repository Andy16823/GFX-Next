using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Primitives
{
    public interface IPrimitive
    {
        Mesh GetMesh(Material material);    
    }
}
