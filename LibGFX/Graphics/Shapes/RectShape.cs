using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shapes
{
    public class RectShape : Shape
    {
        public override bool HasTangents()
        {
            return false;
        }

        public override bool HasUvCoords()
        {
            return false;
        }

        public override bool HasNormals()
        {
            return false;
        }

        public override string GetShapeName()
        {
            return "RectShape";
        }

        public override float[] GetVertices()
        {
            return [
                -0.5f, -0.5f, 0.0f,
                -0.5f, 0.5f, 0.0f,
                0.5f, 0.5f, 0.0f,
                0.5f, -0.5f, 0.0f
            ];
        }

        public override uint[] GetIndices()
        {
            return [
                0, 1, 3,
                3, 1, 2
            ];
        }

        public override float[] GetNormals()
        {
            throw new NotImplementedException();
        }

        public override float[] GetTangents()
        {
            throw new NotImplementedException();
        }

        public override float[] GetUVCoords()
        {
            throw new NotImplementedException();
        }

        
    }
}
