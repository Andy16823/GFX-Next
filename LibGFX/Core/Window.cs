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
        public enum WindowState
        {
            Normal = OpenTK.Windowing.Common.WindowState.Normal,
            Maximized = OpenTK.Windowing.Common.WindowState.Maximized,
            Minimized = OpenTK.Windowing.Common.WindowState.Minimized,
            Fullscreen = OpenTK.Windowing.Common.WindowState.Fullscreen
        }

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
                WindowState = (OpenTK.Windowing.Common.WindowState)windowState
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
        /// Gets the window _context.
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
        [Obsolete("Use GrabCursor() instead.")]
        public void SetMousePosition(Vector2 position)
        {
            _window.MousePosition = position;
        }

        /// <summary>
        /// Sets the mouse position to the specified coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [Obsolete("Use GrabCursor() instead.")]
        public void SetMousePosition(float x, float y)
        {
            this.SetMousePosition(new Vector2(x, y));
        }

        /// <summary>
        /// Hides the mouse cursor.
        /// </summary>
        [Obsolete("Use GrabCursor() instead.")]
        public void HideCursor()
        {
            _window.CursorState = CursorState.Hidden;
        }

        /// <summary>
        /// Shows the mouse cursor.
        /// </summary>
        [Obsolete("Use ReleaseCursor() instead.")]
        public void ShowCursor()
        {
            _window.CursorState = CursorState.Normal;
        }

        /// <summary>
        /// Grabs the mouse cursor, confining it to the window.
        /// </summary>
        /// <remarks>
        /// This is useful for first-person camera controls or when you want to lock the cursor to the window.
        /// </remarks>
        public void GrabCursor()
        {
            _window.CursorState = CursorState.Grabbed;
        }

        /// <summary>
        /// Releases the mouse cursor, allowing it to move freely.
        /// </summary>
        /// <remarks>
        /// This is useful when you want to allow the user to move the cursor outside of the window after it has been grabbed.
        /// </remarks>
        public void ReleaseCursor()
        {
            _window.CursorState = CursorState.Normal;
        }

        /// <summary>
        /// Gets the mouse delta (change in position) since the last frame.
        /// </summary>
        /// <returns>The change in mouse position since the last frame.</returns>
        public Vector2 GetMouseDelta()
        {
            return new Vector2(_window.MouseState.Delta.X, _window.MouseState.Delta.Y);
        }

        /// <summary>
        /// Sets the window title.
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title)
        {
            _window.Title = title;
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        public void Close()
        {
            _window.Close();
        }   
    }
}
