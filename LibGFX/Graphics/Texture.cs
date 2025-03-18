using LibGFX.Math;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

        public static Texture LoadTexture(Bitmap source)
        {
            Texture texture = new Texture()
            {
                Width = source.Width,
                Height = source.Height,
                TextureData = ConvertBitmapToByteArray(source),
                Flags = TextureFlags.Loaded
            };
            return texture;
        }

        private static byte[] ConvertBitmapToByteArray(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            byte[] pixelData = new byte[width * height * 4]; // RGBA -> 4 Bytes pro Pixel

            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    pixelData[index++] = pixel.R;
                    pixelData[index++] = pixel.G;
                    pixelData[index++] = pixel.B;
                    pixelData[index++] = pixel.A;
                }
            }

            return pixelData;
        }

        public float[] GetSubImageUVCords(Rect area)
        {
            float span_x = 1.0f / (float) Width;
            float span_y = 1.0f / (float) Height;

            float bottom_left_x = span_x * area.X;
            float bottom_left_y = span_y * area.Y;
            float top_left_x = bottom_left_x;
            float top_left_y = bottom_left_y + (span_y * area.Height);
            float top_right_x = top_left_x + (span_x * area.Width);
            float top_right_y = top_left_y;
            float bottom_right_x = top_right_x;
            float bottom_right_y = bottom_left_y;

            return [
                bottom_left_x, bottom_left_y,
                top_left_x, top_left_y,
                top_right_x, top_right_y,
                bottom_right_x, bottom_right_y
            ];
        }
    }
}
