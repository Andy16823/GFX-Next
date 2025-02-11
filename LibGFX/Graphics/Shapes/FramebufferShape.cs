using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shapes
{
    public class FramebufferShape : Shape
    {
        public override bool HasNormals() => false;
        public override bool HasTangents() => false;

        public override string GetShapeName()
        {
            return "FramebufferShape";
        }

        public override float[] GetVertices()
        {
            return [
                -1.0f, -1.0f, 0.0f,     // Bottom left
                -1.0f, 1.0f, 0.0f,      // Top left
                1.0f, 1.0f, 0.0f,       // Top right
                1.0f, -1.0f, 0.0f       // Bottom right
            ];
        }

        public override float[] GetUVCoords()
        {
            return [
                0.0f, 0.0f,
                0.0f, 1.0f,
                1.0f, 1.0f,
                1.0f, 0.0f
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
    }
}
