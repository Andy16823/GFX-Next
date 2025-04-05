using LibGFX;
using LibGFX.Core;
using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
using System.Diagnostics;
using OpenTK.Mathematics;
using LibGFX.Pyhsics;
using Assimp;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using LibGFX.UI;

namespace GfxLib_3D_Template
{
    public class FlyCam : IGameBehavior
    {
        private GameElement _element;
        private Vector2 _mousePos;

        public GameElement GetElement()
        {
            return _element;
        }

        public void SetElement(GameElement gameElement)
        {
            _element = gameElement;
        }

        public void OnCollide()
        {

        }

        public void OnDispose(BaseScene scene, IRenderDevice renderer)
        {

        }

        public void OnInit(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {

        }

        public void OnRender(BaseScene scene, Viewport viewport, IRenderDevice renderer, LibGFX.Graphics.Camera camera)
        {

        }

        public void OnUpdate(BaseScene scene)
        {
            var window = GFX.Instance.GetWindow();
            var camera = (PerspectiveCamera)PerspectiveCamera.Current;
            var mousePos = window.GetMousePosition();

            var deltaX = _mousePos.X - mousePos.X;
            var deltaY = _mousePos.Y - mousePos.Y;

            var cameraFront = camera.Transform.GetFront();
            var cameraRight = camera.Transform.GetRight();
            var cameraUp = camera.Transform.GetUp();

            var speed = 0.5f;
            if (window.IsKeyDown(Keys.A))
            {
                camera.Transform.Position -= cameraRight * speed;
            }
            else if (window.IsKeyDown(Keys.D))
            {
                camera.Transform.Position += cameraRight * speed;
            }
            else if (window.IsKeyDown(Keys.W))
            {
                camera.Transform.Position += cameraFront * speed;
            }
            else if (window.IsKeyDown(Keys.S))
            {
                camera.Transform.Position -= cameraFront * speed;
            }
            else if (window.IsKeyDown(Keys.PageUp))
            {
                camera.Transform.Position += cameraUp * speed;
            }
            else if (window.IsKeyDown(Keys.PageDown))
            {
                camera.Transform.Position -= cameraUp * speed;
            }

            if (window.IsMouseDown(MouseButton.Right))
            {
                camera.Transform.Rotate(new Vector3(deltaY * 0.1f, deltaX * 0.1f, 0.0f));
            }

            if (window.IsMouseDown(MouseButton.Left))
            {
                var result = Raycast.PerformRaycastFromScreen(camera, window.GetViewport(), (PhysicsHandler3D)scene.PhysicsHandler, (int)mousePos.X, (int)mousePos.Y);
                if (result.hitElement != null)
                {
                    Debug.WriteLine($"Hit Element {result.hitElement.Name}");
                    var canvas = GFX.Instance.Services.GetService<Canvas2D>("hud");
                    var label = (LibGFX.UI.Label)canvas.GetControl("TestLabel");
                    label.Text = $"Hit Element {result.hitElement.Name}";
                }
            }


            _mousePos = mousePos;
        }

        public void OnCollide(Collision collision)
        {

        }
    }
}
