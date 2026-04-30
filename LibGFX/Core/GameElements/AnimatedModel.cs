using LibGFX.Graphics;
using LibGFX.Graphics.Animation3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGFX.Math;
using OpenTK.Mathematics;
using System.Diagnostics;

namespace LibGFX.Core.GameElements
{
    /// <summary>
    /// An animated 3D model game element
    /// </summary>
    public class AnimatedModel : GameElement
    {
        /// <summary>
        /// The skinned mesh model
        /// </summary>
        private Graphics.SkinnedMeshModel _model;

        /// <summary>
        /// The animator for this model
        /// </summary>
        public Animator Animator { get; }

        /// <summary>
        /// The animation speed multiplier
        /// </summary>
        public float AnimationSpeed { get; set; } = 1.0f;

        public override bool HasTransparency => _model.HasTransparency;

        /// <summary>
        /// Creates a new instance of the AnimatedModel class.
        /// </summary>
        public AnimatedModel()
        {
            this.Animator = new Animator();
        }

        /// <summary>
        /// Creates a new animated model game element
        /// Can share the same model instance with other AnimatedModel elements
        /// </summary>
        /// <param name="name"></param>
        /// <param name="model"></param>
        public AnimatedModel(String name, Graphics.SkinnedMeshModel model)
        {
            this.Name = name;
            this.Animator = new Animator();
            _model = model;
        }

        /// <summary>
        /// Creates a new animated model game element
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="model"></param>
        public AnimatedModel(String name, Vector3 position, Graphics.SkinnedMeshModel model)
        {
            this.Name = name;
            this.Transform.Position = position;
            this.Animator = new Animator();
            _model = model;
        }

        /// <summary>
        /// Creates a new animated model game element
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        /// <param name="model"></param>
        public AnimatedModel(String name, Vector3 position, Vector3 scale, Graphics.SkinnedMeshModel model)
        {
            this.Name = name;
            this.Transform.Position = position;
            this.Transform.Scale = scale;
            this.Animator = new Animator();
            _model = model;
        }

        /// <summary>
        /// Updates the animated model
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="dt"></param>
        public override void Update(BaseScene scene, float dt)
        {
            base.Update(scene, dt);
            if(this.Animator.CurrentAnimation != null)
            {
                float deltaTimeInSeconds = scene.RenderStats.DeltaTime / 1000f;
                float animationSpeed = deltaTimeInSeconds * this.AnimationSpeed;
                Animator.UpdateAnimation(animationSpeed);
            }
        }

        /// <summary>
        /// Renders the animated model
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            if(this.Visible)
            {
                // Call base render (for effects, etc.)
                base.Render(scene, viewport, renderer, camera);

                // Get the world transform of the element
                var transform = this.GetWorldTransform();

                // Draw each mesh of the model
                foreach (var (mesh, material) in _model.Meshes)
                {
                    // Bind material and set shader uniforms
                    material.Use(renderer);
                    scene.LightManager?.BindLights(viewport, renderer, camera);
                    renderer.PrepareShader("finalBonesMatrices", true, Animator.FinalBoneMatrices.ToArray());
                    renderer.PrepareShader("viewPos", camera.Transform.Position);

                    // Draw the mesh and update stats and unbind material
                    renderer.DrawMesh(transform, mesh);
                    scene.RenderStats.IncrementDrawCalls();
                    material.Disable(renderer);
                }
            }
        }

        /// <summary>
        /// Renders the shadow of the animated model
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void RenderShadow(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            if(this.Visible)
            {
                base.RenderShadow(scene, viewport, renderer);
                var transform = this.GetWorldTransform();
                var shader = renderer.GetRenderShader("AnimatedDepthMeshShader");
                renderer.BindShaderProgram(shader);
                renderer.PrepareShader("finalBonesMatrices", true, Animator.FinalBoneMatrices.ToArray());
                foreach (var (mesh, material) in _model.Meshes)
                {
                    renderer.DrawMesh(transform, mesh);
                    scene.RenderStats.IncrementDrawCalls();
                }
                renderer.UnbindShaderProgram();
            }
        }

        /// <summary>
        /// Computes the axis-aligned bounding box of the animated model
        /// </summary>
        public override void ComputeAABB()
        {
            if (_model.Meshes.Count == 0)
            {
                this.AABB = new AABB(Vector3.Zero, Vector3.Zero);
                this.AABB = new AABB(Vector3.Zero, Vector3.Zero);
                return;
            }

            AABB aabb = _model.Meshes[0].Item1.Bounds;
            for (int i = 1; i < _model.Meshes.Count; i++)
            {
                aabb = AABB.Combine(aabb, _model.Meshes[i].Item1.Bounds);
            }
            this.AABB = aabb;
        }

        /// <summary>
        /// Plays an animation by name
        /// </summary>
        /// <param name="name"></param>
        public void PlayAnimation(String name)
        {
            var animation = _model.Animations.FirstOrDefault(a => a.Name == name);
            if(animation != null)
            {
                this.Animator.CurrentAnimation = animation;
            }
        }

        /// <summary>
        /// Plays an animation
        /// </summary>
        /// <param name="animation"></param>
        public void PlayAnimation(Animation3D animation)
        {
            this.Animator.CurrentAnimation = animation;
        }

        /// <summary>
        /// Plays an animation by index
        /// </summary>
        /// <param name="index"></param>
        public void PlayAnimation(int index)
        {
            if(index >= 0 && index < _model.Animations.Count)
            {
                this.Animator.CurrentAnimation = _model.Animations[index];
            }
        }

        /// <summary>
        /// Gets the underlying skinned mesh model associated with this instance.
        /// </summary>
        /// <returns>The <see cref="Graphics.SkinnedMeshModel"/> representing the current skinned mesh model.</returns>
        public Graphics.SkinnedMeshModel GetModel()
        {
            return _model;
        }

        /// <summary>
        /// Clones the current AnimatedModel instance, creating a new instance with the same properties and state.
        /// </summary>
        /// <returns></returns>
        override public GameElement Clone()
        {
            var clone = new AnimatedModel(this.Name, this.Transform.Position, this.Transform.Scale, this._model);
            clone.Transform = this.Transform.Clone();
            clone.AnimationSpeed = this.AnimationSpeed;
            clone.Animator.CurrentAnimation = this.Animator.CurrentAnimation;

            foreach (var behavior in this.Behaviors)
            {
                var clonedBehavior = behavior.Clone();
                clone.AddBehavior(clonedBehavior);
            }

            return clone;
        }
    }
}
