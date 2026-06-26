using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Render target for cascaded shadow mapping. It uses a texture array to store depth maps for multiple cascades in a single texture.
    /// </summary>
    public class CascadedShadowMap : IRenderTarget
    {
        /// <summary>
        /// Frambuffer ID for rendering to the cascaded shadow map.
        /// </summary>
        public int FramebufferId { get; set; }

        /// <summary>
        /// Texture ID for the depth texture array that stores the shadow maps for each cascade.
        /// </summary>
        public int TextureId { get; set; }
        
        /// <summary>
        /// Width of each shadow map in the cascaded shadow map.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of each shadow map in the cascaded shadow map.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Cascade count
        /// </summary>
        public int CascadeCount { get; set; }

        /// <summary>
        /// Creates a new cascaded shadow map with the specified width, height, and cascade count. 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="cascadeCount"></param>
        public CascadedShadowMap(int width, int height, int cascadeCount)
        {
            this.Width = width;
            this.Height = height;
            this.CascadeCount = cascadeCount;
        }

        /// <summary>
        /// Creates the framebuffer and texture array for the cascaded shadow map.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Create()
        {
            var framebuffer = GL.GenFramebuffer();
            var lightDepthMaps = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, lightDepthMaps);
            GL.TexImage3D(TextureTarget.Texture2DArray, 0, PixelInternalFormat.DepthComponent, this.Width, this.Height, this.CascadeCount, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            float[] borderColor = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureBorderColor, borderColor);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, lightDepthMaps, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception($"Failed to create framebuffer for cascaded shadow map. Status: {status}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Successfully created framebuffer for cascaded shadow map.");
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            this.FramebufferId = framebuffer;
            this.TextureId = lightDepthMaps;
        }

        /// <summary>
        /// Disposes of the framebuffer and texture resources used by the cascaded shadow map.
        /// </summary>
        public void Dispose()
        {
            GL.DeleteFramebuffer(this.FramebufferId);
            GL.DeleteTexture(this.TextureId);
        }

        /// <summary>
        /// Gets the pixel data from the depth texture array of the cascaded shadow map. This can be used for debugging or analysis purposes.
        /// </summary>
        /// <returns>A byte array containing the pixel data of the depth texture array.</returns>
        public byte[] GetPixelData()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, this.FramebufferId);
            byte[] pixelData = new byte[this.Width * this.Height * this.CascadeCount * sizeof(float)];
            GL.ReadPixels(0, 0, this.Width, this.Height, PixelFormat.DepthComponent, PixelType.Float, pixelData);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            return pixelData;
        }

        /// <summary>
        /// Resizes the cascaded shadow map by updating the dimensions of the depth texture array. This is useful when the shadow map resolution needs to be changed dynamically.
        /// </summary>
        /// <param name="width">The new width of the shadow map.</param>
        /// <param name="height">The new height of the shadow map.</param>
        public void Resize(int width, int height)
        {
            if (width == this.Width && height == this.Height)
            {
                return;
            }

            GL.BindTexture(TextureTarget.Texture2DArray, this.TextureId);
            GL.TexImage3D(TextureTarget.Texture2DArray, 0, PixelInternalFormat.DepthComponent, width, height, this.CascadeCount, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2DArray, 0);
        }

        /// <summary>
        /// Returns the cascade levels for the cascaded shadow map based on the camera's far plane.
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        public float[] GetCascadeLevels(Camera camera)
        {
            return new float[]
            {
                10.0f,
                30.0f,
                100.0f,
                camera.Far
            };
        }
    }
}
