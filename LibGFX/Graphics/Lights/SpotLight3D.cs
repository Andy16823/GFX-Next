using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Lights
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SpotLight3DData
    {
        public Vector4 Position;
        public Vector4 Direction;
        public float CutOff;
    }

    public class SpotLight3D : Light
    {
        public override bool CastsShadows { get; set; }
        public Vector3 Direction { get; set; }
        public float cutOff { get; set; }

        public SpotLight3D(Vector3 position, Vector3 direction, float cutOff = 12.5f)
        {
            Position = position;
            Direction = direction;
            this.cutOff = cutOff;
        }

        public SpotLight3DData ToStruct()
        {
            return new SpotLight3DData
            {
                Position = new Vector4(Position, 1.0f),
                Direction = new Vector4(Direction, 0.0f),
                CutOff = MathF.Cos(MathHelper.DegreesToRadians(cutOff))
            };
        }

        public override void Dispose(IRenderDevice renderer)
        {
            throw new NotImplementedException();
        }

        public override void Init(IRenderDevice renderer)
        {
            throw new NotImplementedException();
        }
    }
}
