using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents a viewport in 2D or 3D space
    /// </summary>
    public struct Viewport
    {
        /// <summary>
        /// The width of the viewport
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the viewport
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Creates a new viewport with the given width and height
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Viewport(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Converts the viewport to a Vector2i
        /// </summary>
        /// <returns></returns>
        public Vector2i ToVector2I()
        {
            return new Vector2i(Width, Height);
        }

        /// <summary>
        /// Converts the viewport to a Vector2
        /// </summary>
        /// <returns></returns>
        public Vector2 ToVector2()
        {
            return new Vector2(Width, Height);
        }

        public static implicit operator Vector2i(Viewport vp)
        {
            return new Vector2i(vp.Width, vp.Height);
        }

        public static implicit operator Vector2(Viewport vp)
        {
            return new Vector2(vp.Width, vp.Height);
        }

        public static explicit operator Viewport(Vector2i v)
        {
            return new Viewport(v.X, v.Y);
        }

        public static explicit operator Viewport(Vector2 v)
        {
            return new Viewport((int)v.X, (int)v.Y);
        }
    }
}
