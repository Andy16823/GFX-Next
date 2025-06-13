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
        public IMaterial Material { get; set; }
        public ShaderProgram Shader { get; set; }

        private List<PointLight3D> lights = new List<PointLight3D>();
        private PointLight3DData[] lightDatas = new PointLight3DData[4];
        private int lightSsboBufferID = 0;

        public PBRTestEntity(String name, PBRMaterial material) 
        {
            this.Name = name;
            this.Mesh = new Cube().GetMesh();
            this.Material = material;
            this.Transform = new Transform();

            lights.Add(new PointLight3D(new Vector3(0, 0, -5), new Vector4(150.0f, 150.0f, 150.0f, 1.0f)));
            lights.Add(new PointLight3D(new Vector3(-5, 0, 0), new Vector4(150.0f, 150.0f, 150.0f, 1.0f)));
            lights.Add(new PointLight3D(new Vector3(0, 5, 0), new Vector4(150.0f, 150.0f, 150.0f, 1.0f)));
            lights.Add(new PointLight3D(new Vector3(0, -5, 0), new Vector4(150.0f, 150.0f, 150.0f, 1.0f)));

            for (int i = 0; i < lights.Count; i++)
            {
                lightDatas[i] = lights[i].ToStruct();
            }
            this.ComputeAABB();
        }

        public override void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.Init(scene, viewport, renderer);
            Material.Init(renderer);
            renderer.LoadMesh(this.Mesh);

            this.lightSsboBufferID = renderer.CreateBuffer<PointLight3DData>(lightDatas, true);

            if(this.Shader == null)
            {
                this.Shader = renderer.GetShaderProgram("PBRMeshShader");
            }
        }

        public override void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            base.Render(scene, viewport, renderer, camera);
            renderer.BindShaderProgram(this.Shader);
            renderer.BindShaderStorageBuffer(0, lightSsboBufferID);
            renderer.PrepareShader("camPos", camera.Transform.Position);
            renderer.PrepareShader("numLights", lightDatas.Length);
            renderer.DrawMesh(this.Transform, Mesh, Material);
            renderer.UnbindShaderProgram();
        }

        public override void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            renderer.DisposeMesh(Mesh);
            renderer.DisposeBuffer(lightSsboBufferID);
            base.Dispose(scene, renderer);
            renderer.DisposeMesh(this.Mesh);
        }

        public override void ComputeAABB()
        {
            if (Mesh.Vertices == null || Mesh.Vertices.Count == 0)
            {
                this.AABB = new AABB(Vector3.Zero, Vector3.Zero);
                return;
            }

            var min = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            var max = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            foreach (var vertex in Mesh.Vertices)
            {
                min = Vector3.ComponentMin(min, vertex.Position);
                max = Vector3.ComponentMax(max, vertex.Position);
            }

            this.AABB = new AABB(min, max);
        }
    }
}
