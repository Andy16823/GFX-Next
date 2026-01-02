using LibGFX.Core;
using LibGFX.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents the mirror modes for a texture.
    /// </summary>
    [Flags]
    public enum TextureMirrorMode
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2
    }

    /// <summary>
    /// Represents a texture that can be used in rendering.
    /// </summary>
    public class Texture : IGraphicsResource, ISerialization, IIdentifier
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = "UnnamedTexture";

        /// <summary>
        /// Gets the unique identifier for this instance.
        /// </summary>
        public Guid ID { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// The unique identifier for the texture.
        /// </summary>
        public int TextureId { get; set; }

        /// <summary>
        /// The raw texture data in RGBA format.
        /// </summary>
        public byte[]? TextureData { get; set; }

        /// <summary>
        /// The width of the texture in pixels.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the texture in pixels.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets a value indicating whether the object has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the image contains an alpha (transparency) channel.
        /// </summary>
        public bool HasAlpha { get; set; } = false;

        /// <summary>
        /// The default UV transform for a texture.
        /// Scale: (1.0, 1.0), Offset: (0.0, 0.0)
        /// </summary>
        public static readonly Vector4 DefaultUVTransform = new Vector4(1.0f, 1.0f, 0.0f, 0.0f);

        /// <summary>
        /// The default UV scale for a texture.
        /// Value: (1.0, 1.0)
        /// </summary>
        public static readonly Vector2 DefaultUVScale = new Vector2(1.0f, 1.0f);

        /// <summary>
        /// Gets or sets the parameters used to configure texture sampling and filtering.
        /// Must be set before initializing the texture with a render device.
        /// </summary>
        public TextureParameters TextureParameters { get => _parameters; set => SetTextureParameters(value); }

        // Texture parameters
        private TextureParameters _parameters = TextureParameters.Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class with default values.
        /// </summary>
        public Texture()
        {
            TextureId = 0;
            Width = 0;
            Height = 0;
            TextureData = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class with specified width, height, and solid color.
        /// </summary>
        /// <param name="widt"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        public Texture(int widt, int height, Vector4i color)
        {
            TextureId = 0;
            Width = widt;
            Height = height;
            TextureData = Utils.CreateImageData(widt, height, color);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class with specified width, height, and raw pixel data in RGBA format.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="pixeldata"></param>
        /// <exception cref="ArgumentException"></exception>
        public Texture(int width, int height, byte[] pixeldata)
        {
            if (pixeldata.Length != width * height * 4)
                throw new ArgumentException("Pixel data length does not match width and height.");

            TextureId = 0;
            Width = width;
            Height = height;
            TextureData = pixeldata;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class by loading texture data from a file path.
        /// </summary>
        /// <param name="path"></param>
        public Texture(string path)
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
            using(var stream = File.OpenRead(path))
            {
                var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                TextureId = 0;
                TextureData = image.Data;
                Width = image.Width;
                Height = image.Height;
                Name = Path.GetFileNameWithoutExtension(path);
                HasAlpha = DetectAlphaUsage();
            }
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
            float offsetX = (float)x / Width;
            float offsetY = (float)y / Height;

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
        [SupportedOSPlatform("windows")]
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

        /// <summary>
        /// Creates a copy of the texture.
        /// </summary>
        /// <returns></returns>
        public Texture Copy()
        {
            Texture copy = new Texture
            {
                TextureId = 0,
                Width = this.Width,
                Height = this.Height,
            };

            if (this.TextureData != null)
            {
                copy.TextureData = new byte[this.TextureData.Length];
                Array.Copy(this.TextureData, copy.TextureData, this.TextureData.Length);
            }

            return copy;
        }

        /// <summary>
        /// Initializes the texture with the specified render device.
        /// </summary>
        /// <param name="renderer"></param>
        public void Init(IRenderDevice renderer)
        {
            // TODO: Think about if the texture data should be kept in memory after initialization or not.
            renderer.LoadTexture(this, _parameters);
            //this.TextureData = null;
            this.IsInitialized = true;
        }

        /// <summary>
        /// Frees resources used by the texture within the gpu.
        /// </summary>
        /// <param name="renderer"></param>
        public void Dispose(IRenderDevice renderer)
        {
            renderer.DisposeTexture(this);
            this.IsInitialized = false;
        }

        /// <summary>
        /// Sets the texture parameters.
        /// </summary>
        /// <param name="parameters"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private void SetTextureParameters(TextureParameters parameters)
        {
            if (this.IsInitialized)
            {
                throw new InvalidOperationException("Cannot set texture parameters after the texture has been initialized.");
            }
            _parameters = parameters;
        }

        /// <summary>
        /// Checks if the texture uses alpha transparency.
        /// </summary>
        /// <returns></returns>
        private bool DetectAlphaUsage()
        {
            if (this.TextureData == null)
                return false;
            for (int i = 3; i < this.TextureData.Length; i += 4)
            {
                byte alpha = this.TextureData[i];
                if (alpha < 255)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Serializes the texture to JSON.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="context"></param>
        /// <param name="callback"></param>
        public void Serialize(JsonWriter writer, SerializationContext context, Action<JsonWriter> callback = null)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(this.GetType().FullName);
            writer.WritePropertyName("ID");
            writer.WriteValue(this.ID.ToString());
            writer.WritePropertyName("Name");
            writer.WriteValue(this.Name);
            writer.WritePropertyName("Width");
            writer.WriteValue(this.Width);
            writer.WritePropertyName("Height");
            writer.WriteValue(this.Height);
            writer.WritePropertyName("HasAlpha");
            writer.WriteValue(this.HasAlpha);
            writer.WritePropertyName("TextureData");
            writer.WriteValue(Convert.ToBase64String(this.TextureData));
            writer.WritePropertyName("TextureParameters");

            writer.WriteStartObject();
            writer.WritePropertyName("MinFilter");
            writer.WriteValue((int)this.TextureParameters.MinFilter);
            writer.WritePropertyName("MagFilter");
            writer.WriteValue((int)this.TextureParameters.MagFilter);
            writer.WritePropertyName("WrapS");
            writer.WriteValue((int)this.TextureParameters.WrapS);
            writer.WritePropertyName("WrapT");
            writer.WriteValue((int)this.TextureParameters.WrapT);
            writer.WritePropertyName("GenerateMipmaps");
            writer.WriteValue(this.TextureParameters.GenerateMipmaps);
            writer.WriteEndObject();

            callback?.Invoke(writer);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Constructs the texture from JSON data.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <param name="callback"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="JsonException"></exception>
        public void Deserialize(JObject obj, SerializationContext context, Func<JObject, bool> callback = null)
        {
            if(this.IsInitialized)
            {
                throw new InvalidOperationException("Cannot deserialize into an initialized texture.");
            }

            // Texture properties
            this.Name = obj.Value<string>("Name");
            this.ID = Guid.Parse(obj.Value<string>("ID"));
            this.Width = obj.Value<int>("Width");
            this.Height = obj.Value<int>("Height");
            this.HasAlpha = obj.Value<bool>("HasAlpha");

            // Texture data
            string base64Data = obj.Value<string>("TextureData");
            this.TextureData = Convert.FromBase64String(base64Data);

            // Texture parameters
            JObject texParamsObj = obj.Value<JObject>("TextureParameters");
            TextureParameters texParams = new TextureParameters
            {
                MinFilter = (RenderFlags.TextureFilterMode) texParamsObj.Value<int>("MinFilter"),
                MagFilter = (RenderFlags.TextureFilterMode) texParamsObj.Value<int>("MagFilter"),
                WrapS = (RenderFlags.TextureWrapMode) texParamsObj.Value<int>("WrapS"),
                WrapT = (RenderFlags.TextureWrapMode) texParamsObj.Value<int>("WrapT"),
                GenerateMipmaps = texParamsObj.Value<bool>("GenerateMipmaps")
            };
            this.TextureParameters = texParams;

            // Callback
            callback?.Invoke(obj);
        }
    }
}
