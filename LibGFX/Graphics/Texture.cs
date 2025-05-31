using LibGFX.Core;
using LibGFX.Math;
using OpenTK.Mathematics;
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

    [Flags]
    public enum TextureMirrorMode
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2
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

        public static Texture EmptyTexture(int width = 1, int height = 1)
        {
            var bitmap = Utils.CreateEmptyTexture(width, height);
            return LoadTexture(bitmap);
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

        [Obsolete("Use GetUVTransform(Rect area) instead.")]
        public float[] GetSubImageUVCords(Rect area)
        {
            float span_x = 1.0f / (float)Width;
            float span_y = 1.0f / (float)Height;

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

        /// <summary>
        /// Get the UV transform for a given area of the texture.
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public Vector4 GetSubImage(Rect area)
        {
            return this.GetSubImage(area.X, area.Y, area.Width, area.Height);
        }

        /// <summary>
        /// Gets the sub-image UV transforms for a specified area of the texture.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Vector4 GetSubImage(float x, float y, float width, float height)
        {
            float scaleX = (float)width / this.Width;
            float scaleY = (float)height / this.Height;
            float offsetX = (float) x / Width;
            float offsetY = (float) y / Height;

            return new Vector4(scaleX, scaleY, offsetX, offsetY);
        }

        /// <summary>
        /// Get a safe UV transform for a given area of the texture, avoiding edge artifacts.
        /// Used for tilemaps and similar scenarios where you want to avoid sampling the edges of the texture.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Vector4 GetSafeUVTransform(float x, float y, float width, float height)
        {
            float texelW = 1.0f / this.Width;
            float texelH = 1.0f / this.Height;

            float safeWidth = (float)width / this.Width - texelW;
            float safeHeight = (float)height / this.Height - texelH;

            float offsetX = ((float)x / this.Width) + texelW * 0.5f;
            float offsetY = ((float)y / this.Height) + texelH * 0.5f;

            return new Vector4(safeWidth, safeHeight, offsetX, offsetY);
        }

        /// <summary>
        /// Mirror the UV transform based on the specified mirror mode.
        /// </summary>
        /// <param name="uvTransform"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static Vector4 MirrorUVTransform(Vector4 uvTransform, TextureMirrorMode mode)
        {
            float scaleX = uvTransform.X;
            float scaleY = uvTransform.Y;
            float offsetX = uvTransform.Z;
            float offsetY = uvTransform.W;

            if ((mode & TextureMirrorMode.Horizontal) != 0)
            {
                scaleX = -scaleX;
                offsetX = offsetX + uvTransform.X;
            }

            if ((mode & TextureMirrorMode.Vertical) != 0)
            {
                scaleY = -scaleY;
                offsetY = offsetY + uvTransform.Y;
            }

            return new Vector4(scaleX, scaleY, offsetX, offsetY);
        }

        /// <summary>
        /// Convert the texture data to a Bitmap.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Bitmap ToBitmap()
        {
            if (TextureData == null)
                throw new InvalidOperationException("Texture data is null.");

            Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, Width, Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb
            );

            IntPtr ptr = bmpData.Scan0;
            int bytes = Width * Height * 4;

            // RGBA -> ARGB (Windows expects BGRA, so a swap is needed)
            byte[] argbData = new byte[bytes];
            for (int i = 0; i < bytes; i += 4)
            {
                byte r = TextureData[i];
                byte g = TextureData[i + 1];
                byte b = TextureData[i + 2];
                byte a = TextureData[i + 3];

                argbData[i] = b; // Blue
                argbData[i + 1] = g; // Green
                argbData[i + 2] = r; // Red
                argbData[i + 3] = a; // Alpha
            }

            Marshal.Copy(argbData, 0, ptr, bytes);
            bitmap.UnlockBits(bmpData);

            return bitmap;
        }

        public Texture Copy()
        {
            Texture copy = new Texture
            {
                TextureId = 0,
                Width = this.Width,
                Height = this.Height,
                Flags = TextureFlags.Loaded
            };

            if (this.TextureData != null)
            {
                copy.TextureData = new byte[this.TextureData.Length];
                Array.Copy(this.TextureData, copy.TextureData, this.TextureData.Length);
            }

            return copy;
        }
    }
}
