using LibGFX.Core;
using LibGFX.Graphics;
using OpenTK.Windowing.Common;

namespace LibGFX
{
    public class GFX
    {
        private static GFX _instance;
        private Window _window;

        public static GFX Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GFX();
                }
                return _instance;
            }
        }

        public Window CreateWindow(String title, Viewport viewport, WindowState windowState)
        {
            if (_window == null)
            {
                _window = new Window(title, viewport, windowState);
            }
            return _window;
        }

        public Window GetWindow()
        {
            return _window;
        }
    }
}
