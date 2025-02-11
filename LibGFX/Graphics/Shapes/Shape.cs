using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shapes
{
    public abstract class Shape
    {
        public int VertexArray { get; set; }
        public int VertexBuffer { get; set; }
        public int TextureBuffer { get; set; }
        public int NormalBuffer { get; set; }
        public int IndexBuffer { get; set; }
        public int TangentBuffer { get; set; }

        public abstract float[] GetVertices();
        public abstract float[] GetUVCoords();
        public abstract float[] GetNormals();
        public abstract uint[] GetIndices();
        public abstract float[] GetTangents();
        public abstract String GetShapeName();

        public virtual bool HasNormals() => true;
        public virtual bool HasTangents() => true;
        public virtual bool HasUvCoords() => true;
    }
}
