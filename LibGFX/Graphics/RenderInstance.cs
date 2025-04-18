using LibGFX.Core;
using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public class RenderInstance
    {
        public Transform Transform { get; set; }
        public bool Visible { get; set; }


        public Matrix4 GetMatrix()
        {
            return Transform.GetMatrix();
        }

        public Vector4 GetExtras()
        {
            var result = new Vector4();
            result.X = Visible ? 1 : 0;

            return result;
        }
    }
}
