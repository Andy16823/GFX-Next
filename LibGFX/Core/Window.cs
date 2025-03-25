using LibGFX.Graphics;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    public class Window
    {
        private GameWindow _window;

        public Window(String title, Viewport viewport, WindowState windowState = WindowState.Normal)
        {
            var windowSettings = new NativeWindowSettings()
            {
                ClientSize = viewport.ToVector2I(),
                Title = title,
                NumberOfSamples = 8,
                WindowState = windowState
            };

            _window = new GameWindow(GameWindowSettings.Default, windowSettings);
        }

        public bool RequestClose()
        {
            return _window.IsExiting;
        }

        public void ProcessEvents()
        {
            _window.ProcessEvents(0);
        }

        public IGLFWGraphicsContext GetContext()
        {
            return _window.Context;
        }

        public Viewport GetViewport()
        {
            return new Viewport(_window.ClientSize.X, _window.ClientSize.Y);
        }

        public bool IsKeyDown(Keys key)
        {
            return _window.KeyboardState.IsKeyDown(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            return _window.KeyboardState.IsKeyPressed(key);
        }

        public bool IsFocused()
        {
            return _window.IsFocused;
        }

        public Vector2 GetMousePosition()
        {
            return new Vector2(_window.MouseState.X, _window.MouseState.Y);
        }
    }
}
