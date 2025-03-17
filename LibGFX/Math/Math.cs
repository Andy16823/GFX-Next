using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Math
{
    public class Math
    {
        public static float ToRadians(float degrees)
        {
            return (float)(System.Math.PI * degrees / 180.0);
        }

        public static Matrix4 ToTKMatrix(Assimp.Matrix4x4 input)
        {
            return new Matrix4(input.A1, input.B1, input.C1, input.D1,
                               input.A2, input.B2, input.C2, input.D2,
                               input.A3, input.B3, input.C3, input.D3,
                               input.A4, input.B4, input.C4, input.D4);
        }

        public static Matrix4 AssimpMatrixToOpenTKMatrix4(Assimp.Matrix4x4 matrix)
        {
            var mat = new Matrix4();
            mat.M11 = matrix.A1; // col 0, row 0
            mat.M21 = matrix.B1; // col 0, row 1
            mat.M31 = matrix.C1; // col 0, row 2
            mat.M41 = matrix.D1; // col 0, row 3

            mat.M12 = matrix.A2; // col 1, row 0
            mat.M22 = matrix.B2; // col 1, row 1
            mat.M32 = matrix.C2; // col 1, row 2
            mat.M42 = matrix.D2; // col 1, row 3

            mat.M13 = matrix.A3; // col 2, row 0
            mat.M23 = matrix.B3; // col 2, row 1
            mat.M33 = matrix.C3; // col 2, row 2
            mat.M43 = matrix.D3; // col 2, row 3

            mat.M14 = matrix.A4; // col 3, row 0
            mat.M24 = matrix.B4; // col 3, row 1
            mat.M34 = matrix.C4; // col 3, row 2
            mat.M44 = matrix.D4; // col 3, row 3

            return mat;
        }

    }
}
