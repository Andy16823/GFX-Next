using Assimp;
using LibGFX.Core;
using Newtonsoft.Json.Linq;
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
    public class Transform : ISerialization
    {
        private Vector3 _position;
        /// <summary>
        /// The position of the transformation.
        /// </summary>
        public Vector3 Position { get => _position; set { _position = value; Changed?.Invoke(this); } }

        private Quaternion _rotation;
        /// <summary>
        /// The rotation of the transformation.
        /// </summary>
        public Quaternion Rotation { get => _rotation; set { _rotation = value; Changed?.Invoke(this); } }

        private Vector3 _scale;
        /// <summary>
        /// The scale of the transformation.
        /// </summary>
        public Vector3 Scale { get => _scale; set { _scale = value; Changed?.Invoke(this); } }

        /// <summary>
        /// Occurs when the transform has changed.
        /// </summary>
        /// <remarks>Subscribe to this event to be notified whenever the associated transform is modified.
        /// The event provides the updated transform as an argument to the event handler. Handlers are invoked in the
        /// order in which they were added.</remarks>
        public event Action<Transform> Changed;

        /// <summary>
        /// Gets the forward direction based on the current rotation.
        /// </summary>
        public Vector3 Forward { get => this.GetFront();}

        /// <summary>
        /// Gets the Right direction based on the current rotation.
        /// </summary>
        public Vector3 Right { get => this.GetRight(); }

        /// <summary>
        /// Gets the Up direction based on the current rotation.
        /// </summary>
        public Vector3 Up { get => this.GetUp(); }

        /// <summary>
        /// Gets the transformation matrix representing the position, rotation, and scale of the transform.
        /// </summary>
        public Matrix4 Matrix { get => this.GetMatrix(); }

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
        /// Initializes a new instance of the <see cref="Transform"/> class from a transformation matrix.
        /// </summary>
        /// <param name="matrix"></param>
        public Transform(Matrix4 matrix)
        {
            this.Position = matrix.ExtractTranslation();
            this.Scale = matrix.ExtractScale();
            this.Rotation = matrix.ExtractRotation();  
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
        /// Gets the pitch angle in radians based on the current rotation.
        /// </summary>
        /// <returns></returns>
        public float GetPitchAngle()
        {
            var forward = Rotation * -Vector3.UnitZ;
            return (float)MathHelper.Asin(MathHelper.Clamp(forward.Y, -1f, 1f));
        }

        /// <summary>
        /// Gets the pitch rotation as a quaternion.
        /// </summary>
        /// <returns></returns>
        public Quaternion GetPitchQuat()
        {
            float pitch = this.GetPitchAngle();
            return Quaternion.FromAxisAngle(Vector3.UnitX, pitch);
        }

        /// <summary>
        /// Gets the yaw angle in radians based on the current rotation.
        /// </summary>
        /// <returns></returns>
        public float GetYawAngle()
        {
            var forward = Rotation * -Vector3.UnitZ;
            return (float)MathHelper.Asin(MathHelper.Clamp(forward.X, -1f, 1f));
        }

        /// <summary>
        /// Gets the yaw rotation as a quaternion.
        /// </summary>
        /// <returns></returns>
        public Quaternion GetYawQuat()
        {
            float yaw = this.GetYawAngle();
            return Quaternion.FromAxisAngle(Vector3.UnitY, yaw);
        }

        /// <summary>
        /// Gets the roll angle in radians based on the current rotation.
        /// </summary>
        /// <returns></returns>
        public float GetRollAngle()
        {
            var right = Rotation * Vector3.UnitX;
            return (float)MathHelper.Asin(MathHelper.Clamp(right.Y, -1f, 1f));
        }

        /// <summary>
        /// Gets the roll rotation as a quaternion.
        /// </summary>
        /// <returns></returns>
        public Quaternion GetRollQuat()
        {
            float roll = this.GetRollAngle();
            return Quaternion.FromAxisAngle(Vector3.UnitZ, roll);
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

        public void SetPosition(Matrix4 matrix)
        {
            this.Position = matrix.ExtractTranslation();
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
        /// Inverted is used to get the opposite direction. Used for bullet physics.
        /// </summary>
        public Vector3 GetFront(bool inverted = false)
        {
            if(inverted) {
                return Vector3.Transform(Vector3.UnitZ, Rotation);
            }
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
        /// Gets the right direction based on the current rotation, but flattens it to the XZ plane.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetRightFlat()
        {
            var right = this.GetRight();
            right.Y = 0;
            return Vector3.Normalize(right);
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
        /// Rotates the transform to look at a specific target point.
        /// </summary>
        /// <param name="target"></param>
        public void Towards(Vector3 target)
        {
            Vector3 direction = Vector3.Normalize(this.Position - target); // Richtung ZUM Ziel
            Vector3 up = Vector3.UnitY;

            // Prüfen auf Gimbal Lock
            if (MathF.Abs(Vector3.Dot(direction, up)) > 0.999f)
                up = Vector3.UnitZ;

            this.Rotation = LookRotation(direction, up);
        }

        /// <summary>
        /// Creates a rotation that looks in the specified forward direction with the specified up direction.
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        private static Quaternion LookRotation(Vector3 forward, Vector3 up)
        {
            forward = Vector3.Normalize(forward);
            Vector3 right = Vector3.Normalize(Vector3.Cross(up, forward));
            Vector3 correctedUp = Vector3.Cross(forward, right);

            Matrix3 rotationMatrix = new Matrix3(
                right.X, correctedUp.X, forward.X,
                right.Y, correctedUp.Y, forward.Y,
                right.Z, correctedUp.Z, forward.Z
            );

            return Quaternion.FromMatrix(rotationMatrix);
        }

        /// <summary>
        /// Transforms a direction vector using the transform's rotation and scale.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="direction"></param>
        /// <param name="rotationSpaceOnly"></param>
        /// <returns></returns>
        public static Vector3 TransformDirection(Transform transform, Vector3 direction, bool rotationSpaceOnly = true)
        {
            var matrix = transform.GetMatrix();
            if (rotationSpaceOnly)
            {
                matrix.ClearTranslation();
                matrix.ClearScale();
            }
            return Vector3.TransformNormal(direction, matrix);
        }

        /// <summary>
        /// Converts a local position to a world position using the transform's position, rotation, and scale.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="localPosition"></param>
        /// <returns></returns>
        public static Vector3 LocalToWorldPositon(Transform transform, Vector3 localPosition)
        {
            return Utils.LocalToWorldPositon(transform, localPosition);
        }

        /// <summary>
        /// Converts a world position to a local position using the transform's position, rotation, and scale.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public static Vector3 WorldToLocalPosition(Transform transform, Vector3 worldPosition)
        {
            return Utils.WorldToLocalPosition(transform, worldPosition);
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

        /// <summary>
        /// Attaches a child transform to a parent transform, returning the child's local transform relative to the parent.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static Transform Attach(Transform parent, Transform child)
        {
            Matrix4 parentMatrix = parent.GetMatrix();
            Matrix4 localMatrix = child.GetMatrix() * parentMatrix;

            return new Transform(localMatrix);
        }

        /// <summary>
        /// Serializes the current object's position, rotation, and scale into a JSON representation.
        /// </summary>
        /// <param name="serializationContext">The context that provides information and settings required for the serialization process.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized position, rotation, and scale of the object.</returns>
        public JObject Serialize(SerializationContext serializationContext)
        {
            JObject obj = new JObject();
            obj["Type"] = this.GetType().FullName;
            obj["Position"] = Utils.SerializeVec3(this.Position);
            obj["Rotation"] = Utils.SerializeQuat(this.Rotation);
            obj["Scale"] = Utils.SerializeVec3(this.Scale);
            return obj;
        }

        /// <summary>
        /// Deserializes the object's position, rotation, and scale from the specified JSON object.
        /// </summary>
        /// <param name="jObject">A JSON object containing the serialized position, rotation, and scale data. Must not be null.</param>
        /// <param name="serializationContext">The context to use during deserialization. Provides additional information or services required for the
        /// deserialization process.</param>
        public void Deserialize(JObject jObject, SerializationContext serializationContext)
        {
            this.Position = Utils.DeserializeVec3(jObject["Position"] as JObject);
            this.Rotation = Utils.DeserializeQuat(jObject["Rotation"] as JObject);
            this.Scale = Utils.DeserializeVec3(jObject["Scale"] as JObject);
        }
    }
}
