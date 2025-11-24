using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGFX.Graphics;
using LibGFX.Math;
using OpenTK.Mathematics;

namespace LibGFX.Core.GameElements
{
    /// <summary>
    /// Static model game element
    /// </summary>
    public class StaticModel : GameElement
    {
        /// <summary>
        /// The static mesh model
        /// </summary>
        private Graphics.StaticMeshModel _model;

        /// <summary>
        /// Creates a new static model game element
        /// Shared models should be used when multiple instances of the same model are needed
        /// </summary>
        /// <param name="name"></param>
        /// <param name="model"></param>
        public StaticModel(String name, Graphics.StaticMeshModel model)
        {
            this.Name = name;
            _model = model;
            this.ComputeAABB();
        }

        /// <summary>
        /// Renders the static model
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            base.Render(scene, viewport, renderer, camera);
            var transform = this.GetWorldTransform();
            var shader = renderer.GetShaderProgram("MeshShader");

            renderer.BindShaderProgram(shader);
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
        /// Renders the static model for shadow mapping
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void RenderShadow(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.RenderShadow(scene, viewport, renderer);
            var transform = this.GetWorldTransform();
            var shader = renderer.GetShaderProgram("DepthMeshShader");
            renderer.BindShaderProgram(shader);
            foreach (var mesh in _model.Meshes.Values)
            {
                renderer.DrawMesh(transform, mesh);
                scene.RenderStats.IncrementDrawCalls();
            }
            renderer.UnbindShaderProgram();
        }

        /// <summary>
        /// Computes the axis-aligned bounding box for the static model
        /// </summary>
        public override void ComputeAABB()
        {
            if (_model.Meshes.Count == 0)
            {
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
    }
}
