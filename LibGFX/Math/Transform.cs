using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Math
{
    public class Transform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public Transform()
        {
            
        }

        public Transform(Vector2 position, float rotationZ, Vector2 scale)
        {
            this.Position = new Vector3(position);
            this.Rotation = Quaternion.FromAxisAngle(Vector3.UnitZ, rotationZ);
            this.Scale = new Vector3(scale);
        }

        public Transform(Vector2 position, Vector2 scale) : this(position, 0, scale)
        {

        }

        public Transform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            this.Position = position;
            this.Rotation = Quaternion.FromEulerAngles(rotation);
            this.Scale = scale;
        }

        public Transform(Vector3 position, Vector3 scale) : this(position, Vector3.Zero, scale)
        {
            
        }

        public Matrix4 GetMatrix()
        {
            var mt_mat = Matrix4.CreateTranslation(Position);
            var mr_mat = Matrix4.CreateFromQuaternion(Rotation);
            var ms_mat = Matrix4.CreateScale(Scale);
            var m_mat = mt_mat * mr_mat * ms_mat;

            return m_mat;
        }
    }
}
