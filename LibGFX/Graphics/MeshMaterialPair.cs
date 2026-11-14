using LibGFX.Graphics.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public class MeshMaterialPair
    {
        public IMaterial Material { get; set; }
        public Mesh Mesh { get; set; }

        public MeshMaterialPair(Mesh mesh, IMaterial material)
        {
            this.Mesh = mesh;
            this.Material = material;
        }
    }
}
