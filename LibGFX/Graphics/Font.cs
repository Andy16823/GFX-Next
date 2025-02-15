using FreeTypeSharp;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public struct Character
    {
        public int textureId;
        public Vector2 size;
        public Vector2 bearing;
        public int advance;
    }
    public class Font
    {
        public Dictionary<char, Character> Characters { get; set; }
        public int VAO;
        public int VBO;

        public Font()
        {
            this.Characters = new Dictionary<char, Character>();
        }
    }
}
