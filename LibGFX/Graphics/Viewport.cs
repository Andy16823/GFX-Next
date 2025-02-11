using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    }
}
