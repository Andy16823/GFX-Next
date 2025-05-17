using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public struct Viewport
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Viewport(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public Vector2i ToVector2I()
        {
            return new Vector2i(Width, Height);
        }

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
