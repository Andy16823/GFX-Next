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

namespace LibGFX.Core.GameElements
{
    public class PBRTestEntity : GameElement
    {
        public Mesh Mesh { get; set; }
        public IMaterial Material { get; set; }
        public ShaderProgram Shader { get; set; }

        private List<PointLight> lights = new List<PointLight>();
        private PointLightData[] lightDatas = new PointLightData[4];
        private int lightSsboBufferID = 0;

        public PBRTestEntity(String name) 
        {
            this.Name = name;
            this.Mesh = new Cube().GetMesh();
            this.Material = new PBRMaterial();
            this.Transform = new Transform();

            lights.Add(new PointLight(new Vector3(0, 0, 10), new Vector4(150.0f, 150.0f, 150.0f, 1.0f)));
            lights.Add(new PointLight(new Vector3(10, 0, 0), new Vector4(150.0f, 150.0f, 150.0f, 1.0f)));
            lights.Add(new PointLight(new Vector3(0, -10, 0), new Vector4(150.0f, 150.0f, 150.0f, 1.0f)));

            for (int i = 0; i < lights.Count; i++)
            {
                lightDatas[i] = lights[i].ToStruct();
            }
        }

        public override void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.Init(scene, viewport, renderer);
            renderer.LoadMesh(this.Mesh);

            this.lightSsboBufferID = renderer.CreateBuffer<PointLightData>(lightDatas, true);

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
    }
}
