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
    /// <summary>
    /// Represents a window for rendering graphics.
    /// </summary>
    public class Window
    {
        private GameWindow _window;

        /// <summary>
        /// Creates a new window with the specified title, viewport, and window state.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="viewport"></param>
        /// <param name="windowState"></param>
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

        /// <summary>
        /// Requests to close the window.
        /// </summary>
        /// <returns></returns>
        public bool RequestClose()
        {
            return _window.IsExiting;
        }

        /// <summary>
        /// Processes the window events.
        /// </summary>
        public void ProcessEvents()
        {
            _window.ProcessEvents(0);
        }

        /// <summary>
        /// Gets the window context.
        /// </summary>
        /// <returns></returns>
        public IGLFWGraphicsContext GetContext()
        {
            return _window.Context;
        }

        /// <summary>
        /// Gets the current window size.
        /// </summary>
        /// <returns></returns>
        public Viewport GetViewport()
        {
            return new Viewport(_window.ClientSize.X, _window.ClientSize.Y);
        }

        /// <summary>
        /// Checks if the specified key is down.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyDown(Keys key)
        {
            return _window.KeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Checks if the specified key is pressed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyPressed(Keys key)
        {
            return _window.KeyboardState.IsKeyPressed(key);
        }

        /// <summary>
        /// Checks if the window is focused.
        /// </summary>
        /// <returns></returns>
        public bool IsFocused()
        {
            return _window.IsFocused;
        }

        /// <summary>
        /// Gets the current mouse position in the window.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMousePosition()
        {
            return new Vector2(_window.MouseState.X, _window.MouseState.Y);
        }

        /// <summary>
        /// Checks if the specified mouse button is down.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool IsMouseDown(MouseButton button)
        {
            return _window.MouseState.IsButtonDown(button);
        }

        /// <summary>
        /// Checks if the specified mouse button is pressed.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool IsMousePressed(MouseButton button)
        {
            return _window.MouseState.IsButtonPressed(button);
        }

        /// <summary>
        /// Checks if the specified mouse button is released.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool IsMouseReleased(MouseButton button) 
        {
            return _window.MouseState.IsButtonReleased(button);
        }

        /// <summary>
        /// Sets the mouse position to the specified coordinates.
        /// </summary>
        /// <param name="position"></param>
        public void SetMousePosition(Vector2 position)
        {
            _window.MousePosition = position;
        }
    }
}
