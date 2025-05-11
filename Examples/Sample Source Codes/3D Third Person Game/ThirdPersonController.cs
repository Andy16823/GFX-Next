using LibGFX;
using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Graphics;
using LibGFX.Pyhsics;
using LibGFX.Pyhsics.Behaviors;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace NewGFXTest
{
    /// <summary>
    /// The AnimationState enum represents the different animation states of the character.
    /// </summary>
    public enum AnimationState
    {
        Idle,
        Walk,
        Run,
        Jump,
        StrafeLeft,
        StrafeRight
    }

    /// <summary>
    /// The ThirdPersonController class implements the IGameBehavior interface and provides functionality for a third-person character controller.
    /// </summary>
    public class ThirdPersonController : IGameBehavior
    {
        /// <summary>
        /// The name of the animation to play when the character is walking.
        /// </summary>
        public String WalkAnimation { get; set; } = "Walk";

        /// <summary>
        /// The name of the animation to play when the character is idle.
        /// </summary>
        public String IdleAnimation { get; set; } = "Idle";

        /// <summary>
        /// The name of the animation to play when the character is running.
        /// </summary>
        public String RunAnimation { get; set; } = "Run";

        /// <summary>
        /// The name of the animation to play when the character is jumping.
        /// </summary>
        public String JumpAnimation { get; set; } = "Jump";

        /// <summary>
        /// The name of the animation to play when the character is strafing left.
        /// </summary>
        public String StrafeLeft { get; set; } = "StrafeLeft";

        /// <summary>
        /// The name of the animation to play when the character is strafing right.
        /// </summary>
        public String StrafeRight { get; set; } = "StrafeRight";

        /// <summary>
        /// The height of the camera above the character.
        /// </summary>
        public float CameraHeight { get; set; } = 3.0f;

        /// <summary>
        /// The sensitivity of the mouse movement.
        /// </summary>
        public float MouseSensitivity { get; set; } = 0.2f;

        /// <summary>
        /// The distance of the camera from the character.
        /// </summary>
        public float CameraDistance { get; set; } = 5.0f;

        /// <summary>
        /// The height of the character's eyes above the ground.
        /// </summary>
        public float EyeHeight { get; set; } = 1.5f;

        private GameElement _element;
        private AnimationState _animationState = AnimationState.Idle;
        private bool _isColliding = false;

        public GameElement GetElement()
        {
            return _element;
        }

        public void OnCollide(Collision collision)
        {
            _isColliding = true;
        }

        public void OnDispose(BaseScene scene, IRenderDevice renderer)
        {

        }

        public void OnInit(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            // Initial the rigidbody for the character controller
            var rigidbodyBehavior = _element.GetBehavior<RigidBodyBehavior>();
            if (rigidbodyBehavior == null)
            {
                throw new Exception("No RigidBodyBehavior found. Did you add a RigidBodyBehavior to the element?");
            }
            rigidbodyBehavior.SetAngularFactor(new Vector3(0.0f, 1.0f, 0.0f));


            // Set the mouse position to the center of the window
            var window = GFX.Instance.GetWindow();
            if (window == null)
            {
                throw new Exception("No window found. Did you initialize the GFX instance?");
            }
            if (window.IsFocused())
            {
                window.SetMousePosition(viewport.Width / 2, viewport.Height / 2);
            }
        }

        public void OnRender(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {

        }

        public void OnUpdate(BaseScene scene)
        {
            var model = (Model)_element;

            var camera = (PerspectiveCamera)Camera.Current;
            if (camera == null)
            {
                throw new Exception("No active camera found. Did you use camera.SetAsCurrent()?");
            }

            var rigidbodyBehavior = _element.GetBehavior<RigidBodyBehavior>();
            if (rigidbodyBehavior == null)
            {
                throw new Exception("No RigidBodyBehavior found. Did you add a RigidBodyBehavior to the element?");
            }

            var window = GFX.Instance.GetWindow();
            if (window == null)
            {
                throw new Exception("No window found. Did you initialize the GFX instance?");
            }

            if (!window.IsFocused())
            {
                return;
            }

            // Set the linear velocity and the animation state based on the input
            var currentVelocity = rigidbodyBehavior.GetLinearVelocity();
            var velocity = new Vector3(0.0f, currentVelocity.Y, 0.0f);
            var speed = scene.RenderStats.DeltaTime * 0.5f;
            var jumpforce = scene.RenderStats.DeltaTime * 0.4f;
            _animationState = AnimationState.Idle;

            if (window.IsKeyDown(Keys.LeftShift))
            {
                speed *= 1.5f;
                _animationState = AnimationState.Run;
            }

            if (window.IsKeyDown(Keys.A))
            {
                _animationState = AnimationState.StrafeLeft;
                velocity += _element.Transform.GetRight() * speed;
            }
            else if (window.IsKeyDown(Keys.D))
            {
                _animationState = AnimationState.StrafeRight;
                velocity -= _element.Transform.GetRight() * speed;
            }
            else if (window.IsKeyDown(Keys.W))
            {
                if (_animationState != AnimationState.Run)
                {
                    _animationState = AnimationState.Walk;
                }
                velocity -= _element.Transform.GetFront() * speed;
            }
            else if (window.IsKeyDown(Keys.S))
            {
                if (_animationState != AnimationState.Run)
                {
                    _animationState = AnimationState.Walk;
                }
                velocity += _element.Transform.GetFront() * speed;
            }

            if (window.IsKeyPressed(Keys.Space) && _isColliding)
            {
                velocity += new Vector3(0.0f, jumpforce, 0.0f);
            }

            rigidbodyBehavior.SetLinearVelocity(velocity);

            // Set the angular velocity based on the mouse input
            var viewport = window.GetViewport();
            var angluarVelocity = Vector3.Zero;
            var mousePos = window.GetMousePosition();
            var deltaX = (viewport.Width / 2) - mousePos.X;
            var deltaY = (viewport.Height / 2) - mousePos.Y;
            angluarVelocity.Y = deltaX * MouseSensitivity;
            rigidbodyBehavior.SetAngularVelocity(angluarVelocity);

            if (window.IsMouseDown(MouseButton.Right))
            {
                CameraHeight += deltaY * MouseSensitivity;
            }

            var frontDirection = _element.Transform.GetFront();
            var cameraOffset = frontDirection * CameraDistance;
            var upwardOffset = new Vector3(0.0f, CameraHeight, 0.0f);
            var campos = _element.Transform.Position + cameraOffset + upwardOffset;
            camera.Transform.Position = campos;
            camera.LookAt(_element.Transform.Position + new Vector3(0, EyeHeight, 0));

            // Play the animation based on the animation state
            switch (_animationState)
            {
                case AnimationState.Idle:
                    model.PlayAnimation(IdleAnimation);
                    break;
                case AnimationState.Walk:
                    model.PlayAnimation(WalkAnimation);
                    break;
                case AnimationState.Run:
                    model.PlayAnimation(RunAnimation);
                    break;
                case AnimationState.Jump:
                    model.PlayAnimation(JumpAnimation);
                    break;
                case AnimationState.StrafeLeft:
                    model.PlayAnimation(StrafeLeft);
                    break;
                case AnimationState.StrafeRight:
                    model.PlayAnimation(StrafeRight);
                    break;
                default:
                    model.PlayAnimation(IdleAnimation);
                    break;
            }

            // Lock the mouse to the center of the window
            if (window.IsFocused())
            {
                window.SetMousePosition(viewport.Width / 2, viewport.Height / 2);
                window.HideCursor();
            }
            else
            {
                window.ShowCursor();
            }

            _isColliding = false;
        }

        public void SetElement(GameElement gameElement)
        {
            _element = gameElement;
        }
    }
}
