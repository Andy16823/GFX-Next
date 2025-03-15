using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public class Material
    {
        public String Namme { get; set; }
        public Texture BaseColor { get; set; }
        public Texture Normal { get; set; }
        public Texture Metallic { get; set; }
        public Texture Roughness { get; set; }
        public Texture AmbientOcclusion { get; set; }
        public Texture Emissive { get; set; }
        public Texture Height { get; set; }
        public Texture Displacement { get; set; }
        public Vector4 VertexColor { get; set; }

        public Material()
        {
            BaseColor = null;
            Normal = null;
            Metallic = null;
            Roughness = null;
            AmbientOcclusion = null;
            Emissive = null;
            Height = null;
            Displacement = null;
            VertexColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        }
    }
}
