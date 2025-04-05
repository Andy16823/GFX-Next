using BulletSharp;
using LibGFX.Core;
using LibGFX.Graphics;
using LibGFX.Graphics.Primitives;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics.Behaviors
{
    /// <summary>
    /// Represents a 3D rigid body
    /// </summary>
    public abstract class RigidBodyBehavior : PhysicsBehavior
    {
        /// <summary>
        /// The rigid body
        /// </summary>
        public RigidBody RigidBody { get; set; }

        /// <summary>
        /// The parent game element
        /// </summary>
        public GameElement Parent { get; set; }

        /// <summary>
        /// The offset of the rigid body
        /// </summary>
        public Vector3 Offset { get; set; }

        /// <summary>
        /// Creates a new 3D rigid body
        /// </summary>
        /// <param name="physicsHandler"></param>
        public RigidBodyBehavior(PhysicsHandler physicsHandler) : base(physicsHandler)
        {
        }

        /// <summary>
        /// Sets the linear velocity of the rigid body
        /// </summary>
        /// <param name="velocity"></param>
        public void SetLinearVelocity(Vector3 velocity)
        {
            RigidBody.LinearVelocity = (System.Numerics.Vector3)velocity;
        }

        /// <summary>
        /// Sets the angular velocity of the rigid body
        /// </summary>
        /// <param name="velocity"></param>
        public void SetAngularVelocity(Vector3 velocity)
        {
            RigidBody.AngularVelocity = (System.Numerics.Vector3)velocity;
        }

        /// <summary>
        /// Sets the angular factor of the rigid body
        /// </summary>
        /// <param name="factor"></param>
        public void SetAngularFactor(Vector3 factor)
        {
            RigidBody.AngularFactor = (System.Numerics.Vector3)factor;
        }

        /// <summary>
        /// Sets the linear factor of the rigid body
        /// </summary>
        /// <param name="factor"></param>
        public void SetLinearFactor(Vector3 factor)
        {
            RigidBody.LinearFactor = (System.Numerics.Vector3)factor;
        }

        /// <summary>
        /// Translates the rigid body
        /// </summary>
        /// <param name="translation"></param>
        public void Translate(Vector3 translation)
        {
            RigidBody.Translate((System.Numerics.Vector3)translation);
        }

        /// <summary>
        /// Rotates the rigid body
        /// </summary>
        /// <param name="rotation"></param>
        public void Rotate(Vector3 rotation)
        {
            var radRotation = Utils.ToRadians(rotation);
            this.Rotate(Quaternion.FromEulerAngles(radRotation));
        }

        /// <summary>
        /// Rotates the rigid body
        /// </summary>
        /// <param name="rotation"></param>
        public void Rotate(Quaternion rotation)
        {
            System.Numerics.Matrix4x4 transform = this.RigidBody.WorldTransform;
            transform.SetRotation((System.Numerics.Quaternion) rotation, out transform);
            this.RigidBody.WorldTransform = transform;
        }

        public Quaternion GetRotation()
        {
            var rotation = RigidBody.WorldTransform.GetBasis();
            return (Quaternion) System.Numerics.Quaternion.CreateFromRotationMatrix(rotation);
        }

        public Vector3 GetLocation()
        {
            return (Vector3) RigidBody.WorldTransform.Translation;
        }

        /// <summary>
        /// Calculates the forward vector of the rigid body based on its rotation and a specified distance.
        /// </summary>
        /// <param name="distance">The distance to scale the forward vector.</param>
        /// <returns>The forward vector scaled by the specified distance.</returns>
        public Vector3 CalculateForwardVector(float distance)
        {
            var rotation = GetRotation();
            var forward = new Vector3(0, 0, -1);
            forward = rotation * forward;

            forward *= distance;
            return forward;
        }

        /// <summary>
        /// Calculates the right vector of the rigid body based on its rotation and a specified distance.
        /// </summary>
        /// <param name="distance">The distance to scale the right vector.</param>
        /// <returns>The right vector scaled by the specified distance.</returns>
        public Vector3 CalculateRightVector(float distance)
        {
            var rotation = GetRotation();
            var right = new Vector3(1, 0, 0);
            right = rotation * right;

            right *= distance;
            return right;
        }

        /// <summary>
        /// Calculates the up vector of the rigid body based on its rotation and a specified distance.
        /// </summary>
        /// <param name="distance">The distance to scale the up vector.</param>
        /// <returns>The up vector scaled by the specified distance.</returns>
        public Vector3 CalculateUpVector(float distance)
        {
            var rotation = GetRotation();
            var up = new Vector3(0, 1, 0);
            up = rotation * up;

            up *= distance;
            return up;
        }

        /// <summary>
        /// Removes the rigid body
        /// </summary>
        public virtual void RemoveRigidBody()
        {
            this.PhysicsHandler.RemoveElement(this);
            this.RigidBody.CollisionShape.Dispose();
            this.RigidBody.MotionState.Dispose();
            this.RigidBody.Dispose();
        }

        /// <summary>
        /// Gets the game element
        /// </summary>
        /// <returns></returns>
        public override GameElement GetElement()
        {
            return this.Parent;
        }

        /// <summary>
        /// Sets the game element
        /// </summary>
        /// <param name="gameElement"></param>
        public override void SetElement(GameElement gameElement)
        {
            this.Parent = gameElement;
        }

        /// <summary>
        /// Renders the rigid body
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void OnRender(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {

        }

        /// <summary>
        /// Initializes the rigid body
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void OnInit(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {

        }

        /// <summary>
        /// On collide event
        /// </summary>
        /// <param name="collision"></param>
        public override void OnCollide(Collision collision)
        {

        }

        /// <summary>
        /// Disposes the rigid body
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="renderer"></param>
        public override void OnDispose(BaseScene scene, IRenderDevice renderer)
        {
            RemoveRigidBody();
        }

        /// <summary>
        /// Updates the rigid body
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
