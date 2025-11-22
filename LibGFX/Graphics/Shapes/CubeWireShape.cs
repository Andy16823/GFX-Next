using System;

namespace LibGFX.Graphics.Shapes
{
    /// <summary>
    /// Wireframe representation of a cube shape.
    /// </summary>
    public class CubeWireShape : Shape
    {
        public override int GetIndexCount() => 24;
        public override bool HasTangents() => false;
        public override bool HasUvCoords() => false;
        public override bool HasNormals() => false;
        public override string GetShapeName() => "CubeWireShape";
        public override float[] GetVertices()
        {
            return new float[]
            {
                -1, -1, -1, // 0
                 1, -1, -1, // 1
                 1,  1, -1, // 2
                -1,  1, -1, // 3
                -1, -1,  1, // 4
                 1, -1,  1, // 5
                 1,  1,  1, // 6
                -1,  1,  1  // 7
            };
        }
        public override uint[] GetIndices()
        {
            return new uint[]
            {
                // Bottom face
                0, 1,
                1, 2,
                2, 3,
                3, 0,

                // Top face
                4, 5,
                5, 6,
                6, 7,
                7, 4,

                // Vertical edges
                0, 4,
                1, 5,
                2, 6,
                3, 7
            };
        }
        public override float[] GetUVCoords() { throw new NotImplementedException(); }
        public override float[] GetNormals() { throw new NotImplementedException(); }
        public override float[] GetTangents() { throw new NotImplementedException(); }
    }
}