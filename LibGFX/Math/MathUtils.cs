using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Math
{
    /// <summary>
    /// Mathematical utility functions
    /// </summary>
    public class MathUtils
    {
        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static float ToRadians(float degrees)
        {
            return (float)(System.Math.PI * degrees / 180.0);
        }

        /// <summary>
        /// Converts a row-major matrix to a column-major matrix
        /// System.Numerics version
        /// </summary>
        /// <param name="rowMajor"></param>
        /// <returns></returns>
        public static System.Numerics.Matrix4x4 ToColumnMajorMatrix(System.Numerics.Matrix4x4 rowMajor)
        {
            return System.Numerics.Matrix4x4.Transpose(rowMajor);
        }

        /// <summary>
        /// Converts a row-major matrix to a column-major matrix
        /// OpenTK version
        /// </summary>
        /// <param name="rowMajor"></param>
        /// <returns></returns>
        public static Matrix4 ToColumnMajorMatrix(Matrix4 rowMajor)
        {
            return Matrix4.Transpose(rowMajor);
        }
    }
}
