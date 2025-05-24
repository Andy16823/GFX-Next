using BulletSharp;
using LibGFX.Core;
using LibGFX.Graphics;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Physics.Behaviors2D
{
    /// <summary>
    /// Represents a 2D rigid body behavior
    /// </summary>
    public abstract class RigidBodyBehavior : PhysicsBehavior
    {
        /// <summary>
        /// The rigid body of the behavior
        /// </summary>
        public RigidBody RigidBody { get; set; }

        /// <summary>
        /// The offset of the rigid body
        /// </summary>
        public Vector3 Offset { get; set; }

        /// <summary>
        /// The parent game element of the rigid body
        /// </summary>
        public GameElement Parent { get; set; }

        /// <summary>
        /// Creates a new 2D rigid body behavior
        /// </summary>
        /// <param name="physicsHandler"></param>
        public RigidBodyBehavior(PhysicsHandler physicsHandler) : base(physicsHandler)
        {

        }

        /// <summary>
        /// Rotates the rigid body by the given angle in degrees
        /// </summary>
        /// <param name="angle"></param>
        public virtual void Rotate(float angle)
        {
            this.Rotate(new Vector3(0, 0, angle));
        }

        /// <summary>
        /// Rotates the rigid body by the given angle in degrees
        /// </summary>
        /// <param name="rotation"></param>
        public virtual void Rotate(Vector3 rotation)
        {
            var radRotation = Utils.ToRadians(rotation);
            Quaternion quaternion = Quaternion.FromEulerAngles(radRotation);
            this.Rotate(quaternion);
        }

        /// <summary>
        /// Rotates the rigid body by the given quaternion
        /// </summary>
        /// <param name="rotation"></param>
        public virtual void Rotate(Quaternion rotation)
        {
            System.Numerics.Matrix4x4 transform = this.RigidBody.WorldTransform;
            transform.SetRotation((System.Numerics.Quaternion) rotation, out transform);
            this.RigidBody.WorldTransform = transform;
        }

        /// <summary>
        /// Gets the rotation of the rigid body
        /// </summary>
        /// <returns></returns>
        public Quaternion GetRotation()
        {
            var rotation = RigidBody.WorldTransform.GetBasis();
            return (Quaternion) System.Numerics.Quaternion.CreateFromRotationMatrix(rotation);
        }

        /// <summary>
        /// Gets the location of the rigid body
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLocation()
        {
            return (Vector3) RigidBody.WorldTransform.Translation;
        }

        /// <summary>
        /// Translates the rigid body by the given value
        /// </summary>
        /// <param name="translation"></param>
        public void Translate(Vector2 translation)
        {
            RigidBody.Translate(new System.Numerics.Vector3(translation.X, translation.Y, 0));
        }

        /// <summary>
        /// Translates the rigid body by the given value
        /// </summary>
        /// <param name="translation"></param>
        public void Translate(Vector3 translation)
        {
            RigidBody.Translate((System.Numerics.Vector3) translation);
        }

        /// <summary>
        /// Gets the linear velocity of the rigid body
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLinearVelocity()
        {
            return (Vector3) RigidBody.LinearVelocity;
        }

        /// <summary>
        /// Sets the linear velocity of the rigid body
        /// </summary>
        /// <param name="velocity"></param>
        public void SetLinearVelocity(Vector2 velocity)
        {
            this.SetLinearVelocity(new Vector3(velocity.X, velocity.Y, 0));
        }

        /// <summary>
        /// Sets the linear velocity of the rigid body
        /// </summary>
        /// <param name="velocity"></param>
        public void SetLinearVelocity(Vector3 velocity)
        {
            RigidBody.LinearVelocity = (System.Numerics.Vector3) velocity;
        }

        /// <summary>
        /// Gets the angular velocity of the rigid body
        /// </summary>
        /// <returns></returns>
        public Vector3 GetAngularVelocity()
        {
            return (Vector3) RigidBody.AngularVelocity;
        }

        /// <summary>
        /// Sets the angular velocity of the rigid body
        /// </summary>
        /// <param name="velocity"></param>
        public void SetAngularVelocity(float velocity)
        {
            this.SetAngularVelocity(new Vector3(0, 0, velocity));
        }

        /// <summary>
        /// Sets the angular velocity of the rigid body
        /// </summary>
        /// <param name="velocity"></param>
        public void SetAngularVelocity(Vector3 velocity)
        {
            RigidBody.AngularVelocity = (System.Numerics.Vector3) velocity;
        }

        /// <summary>
        /// Gets the angular factor of the rigid body
        /// </summary>
        /// <returns></returns>
        public Vector3 GetAngularFactor()
        {
            return (Vector3) RigidBody.AngularFactor;
        }

        /// <summary>
        /// Sets the angular factor of the rigid body
        /// </summary>
        /// <param name="value"></param>
        public void SetAngularFactor(Vector3 value)
        {
            RigidBody.AngularFactor = (System.Numerics.Vector3) value;
        }

        /// <summary>
        /// Gets the linear factor of the rigid body
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLinearFactor()
        {
            return (Vector3) RigidBody.LinearFactor;
        }

        /// <summary>
        /// Sets the linear factor of the rigid body
        /// </summary>
        /// <param name="value"></param>
        public void SetLinearFactor(Vector2 value)
        {
            this.SetLinearFactor(new Vector3(value.X, value.Y, 0));
        }

        /// <summary>
        /// Sets the linear factor of the rigid body
        /// </summary>
        /// <param name="value"></param>
        public void SetLinearFactor(Vector3 value)
        {
            RigidBody.LinearFactor = (System.Numerics.Vector3) value;
        }

        /// <summary>
        /// Applies a central impulse to the rigid body
        /// </summary>
        /// <param name="impulse"></param>
        public void ApplyCentralImpulse(Vector2 impulse)
        {
            this.ApplyCentralImpulse(new Vector3(impulse.X, impulse.Y, 0));
        }

        /// <summary>
        /// Applies a central impulse to the rigid body
        /// </summary>
        /// <param name="impulse"></param>
        public void ApplyCentralImpulse(float impulse)
        {
            this.ApplyCentralImpulse(new Vector3(impulse, 0, 0));
        }

        /// <summary>
        /// Applies a central impulse to the rigid body
        /// </summary>
        /// <param name="impulse"></param>
        public void ApplyCentralImpulse(Vector3 impulse)
        {
            RigidBody.ApplyCentralImpulse((System.Numerics.Vector3) impulse);
        }

        /// <summary>
        /// Applies a central force to the rigid body
        /// </summary>
        /// <param name="force"></param>
        public void ApplyCentralForce(Vector2 force)
        {
            this.ApplyCentralForce(force);
        }

        /// <summary>
        /// Applies a impulse to the rigid body
        /// </summary>
        /// <param name="impulse"></param>
        public void ApplyImpulse(Vector2 impulse)
        {
            this.ApplyImpulse(new Vector3(impulse.X, impulse.Y, 0));
        }

        /// <summary>
        /// Applies a impulse to the rigid body
        /// </summary>
        /// <param name="impulse"></param>
        public void ApplyImpulse(Vector3 impulse)
        {
            RigidBody.ApplyCentralImpulse((System.Numerics.Vector3) impulse);
        }

        /// <summary>
        /// Calculates the forward vector of the rigid body
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public Vector3 CalculateForwardVector(float distance)
        {
            var rotation = GetRotation();
            var forward = new Vector3(0, 0, -1);

            forward = Vector3.Transform(forward, rotation);

            forward *= distance;
            return forward;
        }

        /// <summary>
        /// Calculates the right vector of the rigid body
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public Vector3 CalculateRightVector(float distance)
        {
            var rotation = GetRotation();
            var right = new Vector3(1, 0, 0);

            right = Vector3.Transform(right, rotation);

            right *= distance;
            return right;
        }

        /// <summary>
        /// Calculates the up vector of the rigid body
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public Vector3 CalculateUpVector(float distance)
        {
            var rotation = GetRotation();
            var up = new Vector3(0, 1, 0);

            up = Vector3.Transform(up, rotation);

            up *= distance;
            return up;
        }

        /// <summary>
        /// Gets the physics object of the rigid body
        /// </summary>
        /// <returns></returns>
        public override object GetPhysicsObject()
        {
            return this.RigidBody;
        }

        /// <summary>
        /// Removes the rigid body from the physics handler
        /// </summary>
        public virtual void RemoveRigidBody()
        {
            this.PhysicsHandler.RemoveElement(this);
            this.RigidBody.CollisionShape.Dispose();
            this.RigidBody.MotionState.Dispose();
            this.RigidBody.Dispose();
        }

        /// <summary>
        /// Gets the parent game element of the rigid body
        /// </summary>
        /// <returns></returns>
        public override GameElement GetElement()
        {
            return this.Parent;
        }

        /// <summary>
        /// Sets the parent game element of the rigid body
        /// </summary>
        /// <param name="gameElement"></param>
        public override void SetElement(GameElement gameElement)
        {
            this.Parent = gameElement;
        }

        /// <summary>
        /// Initializes the rigid body behavior
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void OnInit(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            
        }

        /// <summary>
        /// Renders the rigid body behavior
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void OnRender(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            
        }

        /// <summary>
        /// Handles the collision of the rigid body
        /// </summary>
        /// <param name="collision"></param>
        public override void OnCollide(Collision collision)
        {

        }

        /// <summary>
        /// Disposes the rigid body behavior
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="renderer"></param>
        public override void OnDispose(BaseScene scene, IRenderDevice renderer)
        {
            this.RemoveRigidBody();
        }

        /// <summary>
        /// Updates the rigid body behavior
        /// </summary>
        /// <param name="scene"></param>
        public override void OnUpdate(BaseScene scene)
        {
            System.Numerics.Vector3 position = RigidBody.WorldTransform.Translation;
            System.Numerics.Quaternion rotation = System.Numerics.Quaternion.CreateFromRotationMatrix(RigidBody.WorldTransform);

            Parent.Transform.Position = (Vector3) position - this.Offset;
            Parent.Transform.Rotation = (Quaternion) rotation;

            RigidBody.Activate(true);
        }

    }
}
