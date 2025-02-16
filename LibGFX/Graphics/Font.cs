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
        public Vector2 padding;
        public int advance;
    }
    public class Font
    {
        public Dictionary<char, Character> Characters { get; set; }
        public int TextureWidth { get; set; }
        public int TextureHeight { get; set; }
        public int TextureId { get; set; }
        public int VAO;
        public int VBO;

        public Font()
        {
            this.Characters = new Dictionary<char, Character>();
        }

        public static (float u0, float v0, float u1, float v1) GetGlyphUV(Character character, int textureWidth, int textureHeight)
        {
            float u0 = 0.0f;
            float v0 = 0.0f;

            float u1 = (float) character.size.X / textureWidth;
            float v1 = (float) character.size.Y / textureHeight;

            return (u0, v0, u1, v1);
        }
    }
}
