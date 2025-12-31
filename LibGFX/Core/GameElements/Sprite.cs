using LibGFX.Graphics;
using LibGFX.Graphics.Animation2D;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Shader;
using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public Vector4 UVTransform { get; set; } = Texture.DefaultUVTransform;

        /// <summary>
        /// The UV scale of the sprite
        /// </summary>
        public Vector2 UVScale { get; set; } = Vector2.One;

        /// <summary>
        /// The material of the sprite
        /// </summary>
        public SpriteMaterial Material { get; set; }

        /// <summary>
        /// The shader program used for rendering the sprite
        /// </summary>
        public ShaderProgram Shader { get; set; }

        /// <summary>
        /// The animator of the sprite
        /// </summary>
        public Animator Animator { get; set; }

        /// <summary>
        /// The bounds of the sprite in 2D space, computed from its transform and scale.
        /// </summary>
        public Rect Bounds2D => GetBounds();

        /// <summary>
        /// The mirror mode of the sprite's texture
        /// </summary>
        public TextureMirrorMode MirrorMode { get; set; } = TextureMirrorMode.None;

        /// <summary>
        /// Gets a value indicating whether the object uses a material with transparency.
        /// </summary>
        public override bool HasTransparency => this.Material.IsTransparent;


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
            this.Animator = new Animator();
            this.ComputeAABB(); 
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
            this.Animator = new Animator();
            this.ComputeAABB(); 
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

            if (this.Shader == null)
            {
                this.Shader = renderer.GetShaderProgram("LitSpriteShader");
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
            if (this.Visible)
            {
                var transform = this.GetWorldTransform(); // Get the world transform of the sprite
                renderer.BindShaderProgram(this.Shader);


                if (scene.LightManager != null)
                {
                    scene.LightManager.BindLights(viewport, renderer, camera);
                }

                var uvTransform = this.UVTransform;
                if (this.MirrorMode != TextureMirrorMode.None)
                {
                    uvTransform = Texture.MirrorUVTransform(uvTransform, this.MirrorMode);
                }
                renderer.DrawTexture(transform, this.Material.Texture.TextureId, Color, uvTransform, UVScale);
                scene.RenderStats.IncrementDrawCalls();
                renderer.UnbindShaderProgram();
            }
        }

        /// <summary>
        /// Updates the sprite
        /// </summary>
        /// <param name="scene"></param>
        public override void Update(BaseScene scene, float dt)
        {
            base.Update(scene, dt);
            float deltaTime = scene.RenderStats.DeltaTime;
            if (this.Animator != null)
            {
                this.Animator.Update(deltaTime);
                var (material, uvTransform) = this.Animator.GetCurrentFrame();
                if (material != null)
                {
                    this.Material = material;
                    this.UVTransform = uvTransform;
                }
            }
        }

        /// <summary>
        /// Gets an image sub-region from the sprite's texture and sets the UV transform accordingly.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SubImage(float x, float y, float width, float height)
        {
            if (this.Material != null)
            {
                this.UVTransform = this.Material.Texture.GetSubImage(x, y, width, height);

            }
        }

        /// <summary>
        /// Sets the mirror mode for the sprite's texture.
        /// </summary>
        /// <param name="mode"></param>
        public void SetMirrorMode(TextureMirrorMode mode)
        {
            this.MirrorMode = mode;
        }

        /// <summary>
        /// Adds an animation to the sprite's animator.
        /// </summary>
        /// <param name="animation"></param>
        public void AddAnimation(Animation2D animation)
        {
            if (this.Animator != null)
            {
                this.Animator.Animations.Add(animation);
            }
        }

        /// <summary>
        /// Plays an animation by name using the sprite's animator.
        /// </summary>
        /// <param name="name"></param>
        public void PlayAnimation(String name)
        {
            if (this.Animator != null)
            {
                this.Animator.PlayAnimation(name);
            }
        }

        /// <summary>
        /// Plays the specified animation if it is not already the current animation.
        /// </summary>
        /// <remarks>If the animator is not set or the specified animation is already playing, this method
        /// does nothing.</remarks>
        /// <param name="name">The name of the animation to play. Comparison is case-insensitive.</param>
        public void PlayAnimationIfNotCurrent(String name)
        {
            if(this.Animator != null)
            {
                if(this.Animator.CurrentAnimation == null || !this.Animator.CurrentAnimation.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    this.Animator.PlayAnimation(name);
                }
            }
        }

        /// <summary>
        /// Stops the currently playing animation on the sprite's animator.
        /// </summary>
        public void StopAnimation()
        {
            if (this.Animator != null)
            {
                this.Animator.Stop();
            }
        }

        /// <summary>
        /// Adds an animation callback to the sprite's animator.
        /// </summary>
        /// <param name="callback"></param>
        public void AddAnimationCallback(IAnimationCallback callback)
        {
            if (this.Animator != null)
            {
                this.Animator.AnimationCallbacks.Add(callback);
            }
        }

        /// <summary>
        /// Sets whether the animation should loop or not in the sprite's animator.
        /// </summary>
        /// <param name="loop"></param>
        public void SetAnimationLoop(bool loop)
        {
            if (this.Animator != null)
            {
                this.Animator.Loop = loop;
            }
        }

        /// <summary>
        /// Gets the current animation from the sprite's animator.
        /// </summary>
        /// <returns></returns>
        public Animation2D GetCurrentAnimation()
        {
            if (this.Animator != null)
            {
                return this.Animator.CurrentAnimation;
            }
            return null;
        }

        /// <summary>
        /// Finds an animation by name in the sprite's animator.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Animation2D FindAnimation(String name)
        {
            if (this.Animator != null)
            {
                return this.Animator.Animations.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
            return null;
        }

        /// <summary>
        /// Computes the axis-aligned bounding box (AABB) of the sprite based on its transform.
        /// </summary>
        public override void ComputeAABB()
        {
            var min = new Vector3(
                this.Transform.Position.X - this.Transform.Scale.X / 2,
                this.Transform.Position.Y - this.Transform.Scale.Y / 2,
                0);

            var max = new Vector3(
                this.Transform.Position.X + this.Transform.Scale.X / 2,
                this.Transform.Position.Y + this.Transform.Scale.Y / 2,
                0);

            this.AABB = new AABB(min, max);
        }

        /// <summary>
        /// Gets the bounds of the sprite in 2D space based on its transform and scale.
        /// </summary>
        /// <returns></returns>
        private Rect GetBounds()
        {
            return new Rect(
                this.Transform.Position.X - this.Transform.Scale.X / 2,
                this.Transform.Position.Y - this.Transform.Scale.Y / 2,
                this.Transform.Scale.X,
                this.Transform.Scale.Y);
        }
    }
}
