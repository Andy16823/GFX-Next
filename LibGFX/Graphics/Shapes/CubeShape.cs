using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shapes
{
    public class CubeShape : Shape
    {
        public override int GetIndexCount()
        {
            return 36;
        }

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
            return new float[]
            {
                // 0
                -1.0f, -1.0f, -1.0f,
                // 1
                 1.0f, -1.0f, -1.0f,
                // 2
                 1.0f,  1.0f, -1.0f,
                // 3
                -1.0f,  1.0f, -1.0f,

                // 4
                -1.0f, -1.0f,  1.0f,
                // 5
                 1.0f, -1.0f,  1.0f,
                // 6
                 1.0f,  1.0f,  1.0f,
                // 7
                -1.0f,  1.0f,  1.0f
            };
        }

        public override uint[] GetIndices()
        {
            return new uint[]
            {
                // Front (-Z)
                0, 1, 2,
                2, 3, 0,

                // Back (+Z)
                4, 6, 5,
                6, 4, 7,

                // Left (-X)
                4, 7, 3,
                3, 0, 4,

                // Right (+X)
                1, 2, 6,
                6, 5, 1,

                // Top (+Y)
                3, 2, 6,
                6, 7, 3,

                // Bottom (-Y)
                4, 0, 1,
                1, 5, 4
            };
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
