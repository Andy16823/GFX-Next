using StbImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    [Flags]
    public enum TextureFlags
    {
        None,
        Loaded,
        Initialized,
        Disposed,
        Failed
    }

    public class Texture
    {
        public int TextureId { get; set; }
        public byte[]? TextureData { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public TextureFlags Flags { get; set; }

        public static Texture LoadTexture(String path)
        {
            StbImage.stbi_set_flip_vertically_on_load(1);

            Texture texture = new Texture();
            ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
            texture.TextureData = image.Data;
            texture.Width = image.Width;
            texture.Height = image.Height;
            texture.Flags = TextureFlags.Loaded;

            return texture;
        }
    }
}
