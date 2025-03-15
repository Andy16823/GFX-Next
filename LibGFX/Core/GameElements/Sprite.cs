using LibGFX.Graphics;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core.GameElements
{
    public class Sprite : GameElement
    {
        public Vector4 Color { get; set; }
        public Texture Texture { get; set; }

        public Sprite(String name, Vector2 position, Vector2 scale, Texture texture)
        {
            this.Name = name;   
            this.Color = new Vector4(1, 1, 1, 1);
            this.Transform = new Math.Transform(position, scale);
            this.Texture = texture;
        }

        public Sprite(String name, Vector3 position, Vector3 scale, Texture texture)
        {
            this.Name = name;
            this.Color = new Vector4(1, 1, 1, 1);
            this.Transform = new Math.Transform(position, scale);
            this.Texture = texture;
        }

        public override void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            base.Render(scene, viewport, renderer, camera);
            if(this.Visible)
            {
                renderer.BindShaderProgram(renderer.GetShaderProgram("SpriteShader"));
                renderer.DrawTexture(this.Transform, Texture.TextureId, Color);
                renderer.UnbindRenderTarget();
            }
        }
    }
}
