using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shapes
{
    public class LineShape : Shape
    {
        public override string GetShapeName()
        {
            return "LineShape";
        }

        public override bool DynamicVertices()
        {
            return true;
        }

        public override bool HasNormals()
        {
            return false;
        }

        public override bool HasUvCoords()
        {
            return false;
        }

        public override bool HasTangents()
        {
            return false;
        }

        public override uint[] GetIndices()
        {
            return new uint[] { 0, 1 };
        }

        public override float[] GetNormals()
        {
            return new float[] { 0, 0, 0 };
        }

        public override float[] GetTangents()
        {
            return new float[] { 0, 0, 0 };
        }

        public override float[] GetUVCoords()
        {
            return new float[] { 0, 0, 0, 0 };
        }

        public override float[] GetVertices()
        {
            return new float[] { 0, 0, 0, 0, 0, 0 };
        }
    }
}
