using FreeTypeSharp;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public enum FontAlignment
    {
        BottomLeft,
        BottomCenter,
        BottomRight,
        Left,
        Center,
        Right,
        TopLeft,
        TopCenter,
        TopRight
    }

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
        public int GLBO;


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

        public Vector2 MeasureString(String text, float scale)
        {
            var width = 0.0f;
            var height = 0.0f;
            foreach (var c in text)
            {
                if (this.Characters.ContainsKey(c))
                {
                    var character = this.Characters[c];
                    width += ((int)(character.advance) >> 6) * scale;
                    height = System.Math.Max(height, character.size.Y * scale);
                }
            }
            return new Vector2(width, height);
        }

        public Vector2 GetAlignmentOffset(String text, FontAlignment alignment, float scale)
        {
            var size = this.MeasureString(text, scale);
            switch (alignment)
            {
                case FontAlignment.BottomLeft:
                    return new Vector2(0, 0);
                case FontAlignment.BottomCenter:
                    return new Vector2(-size.X / 2, 0);
                case FontAlignment.BottomRight:
                    return new Vector2(-size.X, 0);
                case FontAlignment.Left:
                    return new Vector2(0, -size.Y / 2);
                case FontAlignment.Center:
                    return new Vector2(-size.X / 2, -size.Y / 2);
                case FontAlignment.Right:
                    return new Vector2(-size.X, -size.Y / 2);
                case FontAlignment.TopLeft:
                    return new Vector2(0, -size.Y);
                case FontAlignment.TopCenter:
                    return new Vector2(-size.X / 2, -size.Y);
                case FontAlignment.TopRight:
                    return new Vector2(-size.X, -size.Y);
                default:
                    return new Vector2(0, 0);
            }
        }
    }
}
