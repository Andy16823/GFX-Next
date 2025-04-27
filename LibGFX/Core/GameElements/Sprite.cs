using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Shader;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core.GameElements
{
    /// <summary>
    /// Represents a sprite
    /// </summary>
    public class Sprite : GameElement
    {
        /// <summary>
        /// The color of the sprite
        /// </summary>
        public Vector4 Color { get; set; }

        /// <summary>
        /// The UV transform of the sprite
        /// </summary>
        public Vector4 UVTransform { get; set; } = Vector4.One;

        /// <summary>
        /// The material of the sprite
        /// </summary>
        public SpriteMaterial Material { get; set; }

        /// <summary>
        /// The shader program used for rendering the sprite
        /// </summary>
        public ShaderProgram Shader { get; set; }

        /// <summary>
        /// Creates a new sprite
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        /// <param name="material"></param>
        public Sprite(String name, Vector2 position, Vector2 scale, SpriteMaterial material)
        {
            this.Name = name;   
            this.Color = new Vector4(1, 1, 1, 1);
            this.Transform = new Math.Transform(position, scale);
            this.Material = material;
        }

        /// <summary>
        /// Creates a new sprite
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        /// <param name="material"></param>
        public Sprite(String name, Vector3 position, Vector3 scale, SpriteMaterial material)
        {
            this.Name = name;
            this.Color = new Vector4(1, 1, 1, 1);
            this.Transform = new Math.Transform(position, scale);
            this.Material = material;
        }

        /// <summary>
        /// Initializes the sprite
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.Init(scene, viewport, renderer);

            if(this.Shader == null)
            {
                this.Shader = renderer.GetShaderProgram("SpriteShader");
            }
        }

        /// <summary>
        /// Renders the sprite
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            base.Render(scene, viewport, renderer, camera);
            if(this.Visible)
            {
                renderer.BindShaderProgram(this.Shader);
                renderer.DrawTexture(this.Transform, this.Material.Texture.TextureId, Color, UVTransform);
                renderer.UnbindShaderProgram();
            }
        }
    }
}
