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
    public class StaticModel : GameElement
    {
        private Graphics.StaticModel _model;

        public StaticModel(String name, Graphics.StaticModel model)
        {
            this.Name = name;
            _model = model;
            this.ComputeAABB();
        }

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
