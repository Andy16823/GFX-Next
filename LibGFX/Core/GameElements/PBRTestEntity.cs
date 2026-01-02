using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Primitives;
using LibGFX.Graphics.Shader;
using LibGFX.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using LibGFX.Graphics.Lights;

namespace LibGFX.Core.GameElements
{
    public class PBRTestEntity : GameElement
    {
        public Mesh Mesh { get; set; }
        public RenderShader Shader { get; set; }
        public override bool HasTransparency => Mesh.Material.IsTransparent;

        public PBRTestEntity(String name, PBRMaterial material) 
        {
            this.Name = name;
            this.Mesh = new Cube().GetMesh();
            this.Mesh.Material = material;
            this.Transform = new Transform();
        }

        public override void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.Init(scene, viewport, renderer);
            this.Mesh.Material.Init(renderer);
            this.Mesh.Init(renderer);

            if(this.Shader == null)
            {
                this.Shader = renderer.GetRenderShader("PBRMeshShader");
            }
        }

        public override void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            base.Render(scene, viewport, renderer, camera);
            renderer.BindShaderProgram(this.Shader);
            if (scene.LightManager != null)
            {
                scene.LightManager.BindLights(viewport, renderer, camera);
            }
            renderer.PrepareShader("camPos", camera.Transform.Position);
            renderer.DrawMesh(this.Transform, Mesh);
            scene.RenderStats.IncrementDrawCalls();
            renderer.UnbindShaderProgram();
        }

        public override void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            this.Mesh.Dispose(renderer);
            base.Dispose(scene, renderer);
        }

        public override void ComputeAABB()
        {
            if (Mesh.Positions == null || Mesh.Positions.Count == 0)
            {
                this.AABB = new AABB(Vector3.Zero, Vector3.Zero);
                return;
            }

            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < Mesh.Positions.Count; i++)
            {
                var position = Mesh.Positions[i];
                min = Vector3.ComponentMin(min, position);
                max = Vector3.ComponentMax(max, position);
            }

            this.AABB = new AABB(min, max);
        }
    }
}
