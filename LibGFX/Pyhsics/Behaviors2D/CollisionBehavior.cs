using BulletSharp;
using LibGFX.Core;
using LibGFX.Graphics;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics.Behaviors2D
{
    public class CollisionBehavior : PhysicsBehavior
    {
        public CollisionObject Collider { get; set; }

        public Vector3 Offset { get; set; } = Vector3.Zero;

        public GameElement Parent { get; set; }

        public CollisionBehavior(PhysicsHandler physicsHandler) : base(physicsHandler)
        {

        }

        public void Translate(Vector3 value)
        {
            System.Numerics.Matrix4x4 translation = System.Numerics.Matrix4x4.CreateTranslation((System.Numerics.Vector3)value);
            System.Numerics.Quaternion rotation = this.Collider.WorldTransform.GetRotation();
            System.Numerics.Matrix4x4 rotaionMatrx = System.Numerics.Matrix4x4.CreateFromQuaternion(rotation);
            this.Collider.WorldTransform = rotaionMatrx * translation;
        }

        public void Rotate(float value)
        {
            this.Rotate(new Vector3(0, 0, value));
        }

        public void Rotate(Vector3 rotation)
        {
            var radRotation = Utils.ToRadians(rotation);
            this.Rotate(Quaternion.FromEulerAngles(radRotation));
        }

        public void Rotate(Quaternion rotation)
        {
            System.Numerics.Matrix4x4 transform = this.Collider.WorldTransform;
            transform.SetRotation((System.Numerics.Quaternion)rotation, out transform);
            this.Collider.WorldTransform = transform;
        }

        public Quaternion GetRotation()
        {
            var rotation = Collider.WorldTransform.GetBasis();
            return (Quaternion)System.Numerics.Quaternion.CreateFromRotationMatrix(rotation);
        }

        public Vector3 GetLocation()
        {
            return (Vector3)Collider.WorldTransform.Translation;
        }

        public override GameElement GetElement()
        {
            return Parent;
        }

        public override void SetElement(GameElement gameElement)
        {
            this.Parent = gameElement;
        }

        public override object GetPhysicsObject()
        {
            return this.Collider;
        }

        public virtual void RemoveCollider()
        {
            this.PhysicsHandler.RemoveElement(this);
            this.Collider.CollisionShape.Dispose();
            this.Collider.Dispose();
        }

        public override void OnCollide(Collision collision)
        {

        }

        public override void OnDispose(BaseScene scene, IRenderDevice renderer)
        {
            this.RemoveCollider();
        }

        public override void OnInit(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {

        }

        public override void OnRender(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {

        }

        public override void OnUpdate(BaseScene scene)
        {
            System.Numerics.Vector3 position = Collider.WorldTransform.Translation;
            System.Numerics.Quaternion rotation = System.Numerics.Quaternion.CreateFromRotationMatrix(Collider.WorldTransform);

            Parent.Transform.Position = (Vector3)position - this.Offset;
            Parent.Transform.Rotation = (Quaternion)rotation;

            Collider.Activate(true);
        }
    }
}
