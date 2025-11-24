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

        public static System.Numerics.Matrix4x4 ToColumnMajorMatrix(System.Numerics.Matrix4x4 rowMajor)
        {
            return System.Numerics.Matrix4x4.Transpose(rowMajor);
        }
    }
}
