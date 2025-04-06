using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shapes
{
    public class CubeShape : Shape
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
            return "CubeShape";
        }

        public override float[] GetVertices()
        {
            return [
                -1.0f,  1.0f, -1.0f,
                -1.0f, -1.0f, -1.0f,
                1.0f, -1.0f, -1.0f,
                1.0f, -1.0f, -1.0f,
                1.0f,  1.0f, -1.0f,
                -1.0f,  1.0f, -1.0f,

                -1.0f, -1.0f,  1.0f,
                -1.0f, -1.0f, -1.0f,
                -1.0f,  1.0f, -1.0f,
                -1.0f,  1.0f, -1.0f,
                -1.0f,  1.0f,  1.0f,
                -1.0f, -1.0f,  1.0f,

                1.0f, -1.0f, -1.0f,
                1.0f, -1.0f,  1.0f,
                1.0f,  1.0f,  1.0f,
                1.0f,  1.0f,  1.0f,
                1.0f,  1.0f, -1.0f,
                1.0f, -1.0f, -1.0f,

                -1.0f, -1.0f,  1.0f,
                -1.0f,  1.0f,  1.0f,
                1.0f,  1.0f,  1.0f,
                1.0f,  1.0f,  1.0f,
                1.0f, -1.0f,  1.0f,
                -1.0f, -1.0f,  1.0f,

                -1.0f,  1.0f, -1.0f,
                1.0f,  1.0f, -1.0f,
                1.0f,  1.0f,  1.0f,
                1.0f,  1.0f,  1.0f,
                -1.0f,  1.0f,  1.0f,
                -1.0f,  1.0f, -1.0f,

                -1.0f, -1.0f, -1.0f,
                -1.0f, -1.0f,  1.0f,
                1.0f, -1.0f, -1.0f,
                1.0f, -1.0f, -1.0f,
                -1.0f, -1.0f,  1.0f,
                1.0f, -1.0f,  1.0f
            ];
        }

        public override uint[] GetIndices()
        {
            return [
                0, 1, 2, 2, 3, 0,
                5, 4, 7, 7, 6, 5,
                8, 9, 10, 10, 11, 8,
                13, 12, 15, 15, 14, 13,
                16, 17, 18, 18, 19, 16,
                21, 20, 23, 23, 22, 21
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
