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
            this.ComputeAABB();
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
            base.Render(scene, viewport, renderer, camera);
            var transform = this.GetWorldTransform();

            // Bind and prepare shader uniforms
            var shader = renderer.GetShaderProgram("AnimatedMeshShader");
            renderer.BindShaderProgram(shader);
            renderer.PrepareShader("finalBonesMatrices", true, Animator.FinalBoneMatrices.ToArray());
            renderer.PrepareShader("viewPos", camera.Transform.Position);
            if (scene.LightManager != null)
            {
                scene.LightManager.BindLights(viewport, renderer, camera);
            }

            foreach (var mesh in _model.Meshes.Values)
            {
                renderer.DrawMesh(transform, mesh);
                scene.RenderStats.IncrementDrawCalls();
            }

            renderer.UnbindShaderProgram();
        }

        /// <summary>
        /// Renders the shadow of the animated model
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void RenderShadow(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.RenderShadow(scene, viewport, renderer);
            var transform = this.GetWorldTransform();
            var shader = renderer.GetShaderProgram("AnimatedDepthMeshShader");
            renderer.BindShaderProgram(shader);
            renderer.PrepareShader("finalBonesMatrices", true, Animator.FinalBoneMatrices.ToArray());
            foreach (var mesh in _model.Meshes.Values)
            {
                renderer.DrawMesh(transform, mesh);
                scene.RenderStats.IncrementDrawCalls();
            }
            renderer.UnbindShaderProgram();
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

            var min = new Vector3(float.MaxValue);
            var max = new Vector3(float.MinValue);

            foreach (var mesh in _model.Meshes.Values)
            {
                foreach (var vertex in mesh.Vertices)
                {
                    min = Vector3.ComponentMin(min, vertex.Position);
                    max = Vector3.ComponentMax(max, vertex.Position);
                }
            }

            this.AABB = new AABB(min, max);
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
        public void PlayAnimation(Animation animation)
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
    }
}
