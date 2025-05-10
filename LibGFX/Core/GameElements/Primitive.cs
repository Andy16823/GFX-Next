using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Primitives;
using LibGFX.Graphics.Shader;
using LibGFX.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core.GameElements
{
    /// <summary>
    /// Represents a primitive game element that can be rendered with a material and shader.
    /// </summary>
    public class Primitive : GameElement
    {
        /// <summary>
        /// The name of the primitive.
        /// </summary>
        public Mesh Mesh { get; set; }

        /// <summary>
        /// The material used for rendering the primitive.
        /// </summary>
        public IMaterial Material { get; set; }

        /// <summary>
        /// the shader program used for rendering the primitive.
        /// </summary>
        public ShaderProgram Shader { get; set; }

        /// <summary>
        /// Creates a new instance of the Primitive class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="material"></param>
        /// <param name="primitive"></param>
        public Primitive(String name, IMaterial material, IPrimitive primitive) 
        {
            this.Name = name;
            this.Mesh = primitive.GetMesh();
            this.Material = material;
        }

        /// <summary>
        /// Initializes the primitive with the specified scene, viewport, and renderer.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.Init(scene, viewport, renderer);
            renderer.LoadMesh(this.Mesh);

            if(this.Shader == null)
            {
                this.Shader = renderer.GetShaderProgram("MeshShader");
            }
        }

        /// <summary>
        /// Renders the primitive with the specified scene, viewport, renderer, and camera.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public override void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            base.Render(scene, viewport, renderer, camera);
            var light = renderer.GetLightSource<DirectionalLight>();

            renderer.BindShaderProgram(this.Shader);
            if (scene.LightManager != null)
            {
                scene.LightManager.BindLights(viewport, renderer, camera);
            }
            renderer.DrawMesh(this.Transform, Mesh, Material);
            renderer.UnbindShaderProgram();
        }

        /// <summary>
        /// Disposes the primitive and the mesh resources.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="renderer"></param>
        public override void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            base.Dispose(scene, renderer);
            renderer.DisposeMesh(this.Mesh);
        }

        /// <summary>
        /// Gets the meshes and materials associated with the primitive.
        /// </summary>
        /// <returns></returns>
        public override (Mesh, IMaterial)[]? GetMeshes()
        {
            return new (Mesh, IMaterial)[] { (this.Mesh, this.Material) };
        }
    }
}
