using LibGFX.Graphics.Shader;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Materials
{
    /// <summary>
    /// Represents a material used in rendering.
    /// </summary>
    public class SpriteMaterial : IMaterial
    {
        /// <summary>
        /// The name of the material.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unique identifier of the material.
        /// </summary>
        public Guid ID { get; } = Guid.NewGuid();

        /// <summary>
        /// The Material flags.
        /// </summary>
        public MaterialFlags Flags { get; set; }

        /// <summary>
        /// The texture of the material.
        /// </summary>
        public Texture Texture { get; set; }

        /// <summary>
        /// Default constructor for the SpriteMaterial class.
        /// </summary>
        public SpriteMaterial()
        {
            
        }

        /// <summary>
        /// Creates a new SpriteMaterial with the specified texture and shader.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="shader"></param>
        public SpriteMaterial(Texture texture)
        {
            this.Texture = texture;
        }

        /// <summary>
        /// Disposes the material and releases any resources used by it.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Dispose(IRenderDevice renderDevice)
        {
            Debug.WriteLine($"Disposing material {Name}");
            renderDevice.DisposeTexture(Texture);
            Flags = MaterialFlags.Disposed;
            Debug.WriteLine($"Disposed material {Name}");
        }

        /// <summary>
        /// Initializes the material and loads its resources.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Init(IRenderDevice renderDevice)
        {
            Debug.WriteLine($"Loading material {Name}");
            if (this.Flags != MaterialFlags.None)
            {
                Debug.WriteLine($"Material {Name} is already loaded.");
                return;
            }

            renderDevice.LoadTexture(Texture, TextureOptions.PixelPerfect);
            Flags = MaterialFlags.Loaded;
            Debug.WriteLine($"Loaded material {Name}");
        }

        /// <summary>
        /// Sets the material as the current material for rendering. No-op for SpriteMaterial.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Use(IRenderDevice renderDevice)
        {
            renderDevice.PrepareShader("textureSampler", OpenTK.Graphics.OpenGL4.TextureUnit.Texture0, Texture);
        }
    }
}
