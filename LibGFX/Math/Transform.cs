using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Math
{
    /// <summary>
    /// Represents a 3D transformation.
    /// </summary>
    public class Transform
    {
        /// <summary>
        /// The position of the transformation.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The rotation of the transformation.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// The scale of the transformation.
        /// </summary>
        public Vector3 Scale { get; set; }

        /// <summary>
        /// Creates a new instance of the Transform class.
        /// </summary>
        public Transform()
        {
            this.Position = Vector3.Zero;
            this.Rotation = Quaternion.Identity;
            this.Scale = Vector3.One;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform"/> class with a 2D position, rotation around the Z-axis in degrees, and scale.
        /// </summary>
        /// <param name="position">The 2D position.</param>
        /// <param name="rotationZ">The rotation around the Z-axis in degrees.</param>
        /// <param name="scale">The 2D scale.</param>
        public Transform(Vector2 position, float rotationZ, Vector2 scale)
        {
            this.Position = new Vector3(position);
            this.Rotation = Quaternion.FromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(rotationZ));
            this.Scale = new Vector3(scale);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform"/> class with a 2D position and scale.
        /// </summary>
        /// <param name="position">The 2D position.</param>
        /// <param name="scale">The 2D scale.</param>
        public Transform(Vector2 position, Vector2 scale) : this(position, 0, scale)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform"/> class with position, rotation in degrees, and scale.
        /// </summary>
        /// <param name="position">The 3D position.</param>
        /// <param name="rotation">The rotation in degrees.</param>
        /// <param name="scale">The 3D scale.</param>
        public Transform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            this.Position = position;
            this.Rotation = Quaternion.FromEulerAngles(Transform.ToRadians(rotation));
            this.Scale = scale;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform"/> class with position and scale.
        /// </summary>
        /// <param name="position">The 3D position.</param>
        /// <param name="scale">The 3D scale.</param>
        public Transform(Vector3 position, Vector3 scale) : this(position, Vector3.Zero, scale)
        {
            
        }

        /// <summary>
        /// Sets the rotation in degrees using a 2D vector.
        /// </summary>
        /// <param name="rotation">The rotation vector in degrees.</param>
        public void SetRotation(Vector2 rotation)
        {
            this.SetRotation(new Vector3(rotation));
        }

        /// <summary>
        /// Sets the rotation in degrees using a single float value representing rotation around the Z-axis.
        /// </summary>
        /// <param name="rotation">The rotation in degrees.</param>
        public void SetRotation(float rotation)
        {
            this.SetRotation(new Vector3(0, 0, rotation));
        }

        /// <summary>
        /// Sets the rotation in degrees using a 3D vector.
        /// </summary>
        /// <param name="rotation">The rotation vector in degrees.</param>
        public void SetRotation(Vector3 rotation)
        {
            this.Rotation = Quaternion.FromEulerAngles(Transform.ToRadians(rotation));
        }

        /// <summary>
        /// Rotates by a specified 2D vector in degrees.
        /// </summary>
        /// <param name="rotation">The rotation vector in degrees.</param>
        public void Rotate(Vector2 rotation)
        {
            this.Rotate(new Vector3(rotation));
        }

        /// <summary>
        /// Rotates around all axis by the specified degrees.
        /// </summary>
        /// <param name="rotation">The rotation in degrees.</param>
        public void Rotate(float rotation)
        {
            this.Rotate(new Vector3(rotation));
        }

        /// <summary>
        /// Rotates by a specified 3D vector in degrees.
        /// </summary>
        /// <param name="rotation">The rotation vector in degrees.</param>
        public void Rotate(Vector3 rotation)
        {
            this.Rotation *= Quaternion.FromEulerAngles(Transform.ToRadians(rotation));
        }

        /// <summary>
        /// Rotates by specified pitch, yaw, and roll values in degrees.
        /// </summary>
        /// <param name="pitch">Rotation around the X-axis in degrees.</param>
        /// <param name="yaw">Rotation around the Y-axis in degrees.</param>
        /// <param name="roll">Rotation around the Z-axis in degrees.</param>
        public void Rotate(float pitch, float yaw, float roll)
        {
            this.Rotation *= Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(pitch), MathHelper.DegreesToRadians(yaw), MathHelper.DegreesToRadians(roll));
        }

        /// <summary>
        /// Translates by a 2D vector.
        /// </summary>
        /// <param name="translation">The translation vector.</param>
        public void Translate(Vector2 translation)
        {
            this.Translate(new Vector3(translation));
        }

        /// <summary>
        /// Translates by specified X, Y, and Z values.
        /// </summary>
        public void Translate(float x, float y, float z)
        {
            this.Translate(new Vector3(x, y, z));
        }

        /// <summary>
        /// Translates by a 3D vector.
        /// </summary>
        public void Translate(Vector3 translation)
        {
            this.Position += translation;
        }

        /// <summary>
        /// Scales by a 2D vector.
        /// </summary>
        public void ScaleBy(Vector2 scale)
        {
            this.ScaleBy(new Vector3(scale));
        }

        /// <summary>
        /// Scales by specified X, Y, and Z values.
        /// </summary>
        public void ScaleBy(float x, float y, float z)
        {
            this.ScaleBy(new Vector3(x, y, z));
        }

        /// <summary>
        /// Scales by a 3D vector.
        /// </summary>
        public void ScaleBy(Vector3 scale)
        {
            this.Scale *= scale;
        }

        /// <summary>
        /// Gets the rotation in Euler angles (degrees).
        /// </summary>
        public Vector3 GetEulerAngles()
        {
            return Transform.ToDegrees(this.Rotation.ToEulerAngles());
        }

        /// <summary>
        /// Gets the up direction based on the current rotation.
        /// </summary>
        public Vector3 GetUp()
        {
            return Vector3.Transform(Vector3.UnitY, Rotation);
        }

        /// <summary>
        /// Gets the forward direction based on the current rotation.
        /// </summary>
        public Vector3 GetFront()
        {
            return -Vector3.Transform(Vector3.UnitZ, Rotation);
        }

        /// <summary>
        /// Gets the right direction based on the current rotation.
        /// </summary>
        public Vector3 GetRight()
        {
            return Vector3.Transform(Vector3.UnitX, Rotation);
        }

        /// <summary>
        /// Gets the transformation matrix.
        /// </summary>
        public Matrix4 GetMatrix()
        {
            var mt_mat = Matrix4.CreateTranslation(Position);
            var mr_mat = Matrix4.CreateFromQuaternion(Rotation);
            var ms_mat = Matrix4.CreateScale(Scale);
            var m_mat = ms_mat * mr_mat * mt_mat;//  mt_mat * mr_mat * ms_mat;

            return m_mat;
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        public static Vector3 ToRadians(Vector3 input)
        {
            return new Vector3(MathHelper.DegreesToRadians(input.X), MathHelper.DegreesToRadians(input.Y), MathHelper.DegreesToRadians(input.Z));
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        public static Vector3 ToDegrees(Vector3 input)
        {
            return new Vector3(MathHelper.RadiansToDegrees(input.X), MathHelper.RadiansToDegrees(input.Y), MathHelper.RadiansToDegrees(input.Z));
        }
    }
}
