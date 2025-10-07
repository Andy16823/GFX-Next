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
    /// Represents a 2D collision behavior
    /// </summary>
    public class CollisionBehavior : PhysicsBehavior
    {
        /// <summary>
        /// The collision object
        /// </summary>
        public CollisionObject Collider { get; set; }

        /// <summary>
        /// The offset of the collider
        /// </summary>
        public Vector3 Offset { get; set; } = Vector3.Zero;

        /// <summary>
        /// The parent game element 
        /// </summary>
        public GameElement Parent { get; set; }

        /// <summary>
        /// Creates a new 2D collision behavior
        /// </summary>
        /// <param name="physicsHandler"></param>
        public CollisionBehavior(PhysicsHandler physicsHandler) : base(physicsHandler)
        {

        }

        /// <summary>
        /// Translates the collider by the given value
        /// </summary>
        /// <param name="value"></param>
        public void Translate(Vector3 value)
        {
            System.Numerics.Matrix4x4 translation = System.Numerics.Matrix4x4.CreateTranslation((System.Numerics.Vector3)value);
            System.Numerics.Quaternion rotation = this.Collider.WorldTransform.GetRotation();
            System.Numerics.Matrix4x4 rotaionMatrx = System.Numerics.Matrix4x4.CreateFromQuaternion(rotation);
            this.Collider.WorldTransform = rotaionMatrx * translation;
        }

        /// <summary>
        /// Rotates the collider by the given value around the Z axis in degrees
        /// </summary>
        /// <param name="value"></param>
        public void Rotate(float value)
        {
            this.Rotate(new Vector3(0, 0, value));
        }

        /// <summary>
        /// Rotates the collider by the given value in degrees
        /// </summary>
        /// <param name="rotation"></param>
        public void Rotate(Vector3 rotation)
        {
            var radRotation = Utils.ToRadians(rotation);
            this.Rotate(Quaternion.FromEulerAngles(radRotation));
        }

        /// <summary>
        /// Rotates the collider by the given quaternion
        /// </summary>
        /// <param name="rotation"></param>
        public void Rotate(Quaternion rotation)
        {
            System.Numerics.Matrix4x4 transform = this.Collider.WorldTransform;
            transform.SetRotation((System.Numerics.Quaternion)rotation, out transform);
            this.Collider.WorldTransform = transform;
        }

        /// <summary>
        /// Gets the rotation of the collider
        /// </summary>
        /// <returns></returns>
        public Quaternion GetRotation()
        {
            var rotation = Collider.WorldTransform.GetBasis();
            return (Quaternion)System.Numerics.Quaternion.CreateFromRotationMatrix(rotation);
        }

        /// <summary>
        /// Gets the location of the collider
        /// </summary>
        /// <returns></returns>
        public Vector3 GetLocation()
        {
            return (Vector3)Collider.WorldTransform.Translation;
        }

        /// <summary>
        /// Gets the game element of the collider
        /// </summary>
        /// <returns></returns>
        public override GameElement GetElement()
        {
            return Parent;
        }

        /// <summary>
        /// Sets the game element of the collider
        /// </summary>
        /// <param name="gameElement"></param>
        public override void SetElement(GameElement gameElement)
        {
            this.Parent = gameElement;
        }

        /// <summary>
        /// Gets the physics object of the collider
        /// </summary>
        /// <returns></returns>
        public override object GetPhysicsObject()
        {
            return this.Collider;
        }

        /// <summary>
        /// Removes the collider from the physics handler
        /// </summary>
        public virtual void RemoveCollider()
        {
            this.PhysicsHandler.RemoveElement(this);
            this.Collider.CollisionShape.Dispose();
            this.Collider.Dispose();
        }

        /// <summary>
        /// Handles the collision event
        /// </summary>
        /// <param name="collision"></param>
        public override void OnCollide(Collision collision)
        {

        }

        /// <summary>
        /// Disposes the collider
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="renderer"></param>
        public override void OnDispose(BaseScene scene, IRenderDevice renderer)
        {
            this.RemoveCollider();
        }

        /// <summary>
        /// Initializes the collider
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void OnInit(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {

        }

        /// <summary>
        /// Renders the collider
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void OnRender(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {

        }

        /// <summary>
        /// Updates the collider
        /// </summary>
        /// <param name="scene"></param>
        public override void OnUpdate(BaseScene scene, float dt)
        {
            System.Numerics.Vector3 position = Collider.WorldTransform.Translation;
            System.Numerics.Quaternion rotation = System.Numerics.Quaternion.CreateFromRotationMatrix(Collider.WorldTransform);

            Parent.Transform.Position = (Vector3)position - this.Offset;
            Parent.Transform.Rotation = (Quaternion)rotation;

            Collider.Activate(true);
        }
    }
}
