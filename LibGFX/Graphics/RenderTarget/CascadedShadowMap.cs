
namespace LibGFX.Graphics.RenderTarget
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
        public int Width { get; internal set; }

        /// <summary>
        /// Height of each shadow map in the cascaded shadow map.
        /// </summary>
        public int Height { get; internal set; }

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
