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
    public class TriggerBehavior : PhysicsBehavior
    {
        public GhostObject Trigger { get; set; }

        public Vector3 Offset { get; set; } = Vector3.Zero;

        public GameElement Parent { get; set; }

        public TriggerBehavior(PhysicsHandler physicsHandler) : base(physicsHandler)
        {

        }

        public void Translate(Vector3 value)
        {
            System.Numerics.Matrix4x4 translation = System.Numerics.Matrix4x4.CreateTranslation((System.Numerics.Vector3)value);
            System.Numerics.Quaternion rotation = this.Trigger.WorldTransform.GetRotation();
            System.Numerics.Matrix4x4 rotaionMatrx = System.Numerics.Matrix4x4.CreateFromQuaternion(rotation);
            this.Trigger.WorldTransform = rotaionMatrx * translation;
        }

        public void Rotate(Vector3 rotation)
        {
            var radRotation = Utils.ToRadians(rotation);
            this.Rotate(Quaternion.FromEulerAngles(radRotation));
        }

        public void Rotate(Quaternion rotation)
        {
            System.Numerics.Matrix4x4 transform = this.Trigger.WorldTransform;
            transform.SetRotation((System.Numerics.Quaternion)rotation, out transform);
            this.Trigger.WorldTransform = transform;
        }

        public Quaternion GetRotation()
        {
            var rotation = Trigger.WorldTransform.GetBasis();
            return (Quaternion)System.Numerics.Quaternion.CreateFromRotationMatrix(rotation);
        }
        public Vector3 GetLocation()
        {
            return (Vector3)Trigger.WorldTransform.Translation;
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
            return this.Trigger;
        }

        public virtual void RemoveCollider()
        {
            this.PhysicsHandler.RemoveElement(this);
            this.Trigger.CollisionShape.Dispose();
            this.Trigger.Dispose();
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
            System.Numerics.Vector3 position = Trigger.WorldTransform.Translation;
            System.Numerics.Quaternion rotation = System.Numerics.Quaternion.CreateFromRotationMatrix(Trigger.WorldTransform);

            Parent.Transform.Position = (Vector3)position - this.Offset;
            Parent.Transform.Rotation = (Quaternion)rotation;

            Trigger.Activate(true);
        }
    }
}
