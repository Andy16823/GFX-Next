using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Primitives;
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
    /// Represents a primitive game element that can be rendered with a material and shader.
    /// </summary>
    public class Primitive : GameElement
    {
        public enum PrimitiveType
        {
            Quad,
            Cube,
            Sphere
        }

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

        public Primitive(String name, IMaterial material, Mesh mesh)
        {
            this.Name = name;
            this.Mesh = mesh;
            this.Material = material;
            this.ComputeAABB();
        }

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
            this.ComputeAABB();
        }

        /// <summary>
        /// Creates a new instance of the Primitive class with a specified type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="material"></param>
        /// <param name="type"></param>
        /// <exception cref="ArgumentException"></exception>
        public Primitive(String name, IMaterial material, PrimitiveType type = PrimitiveType.Cube)
        {
            this.Name = name;

            switch (type)
            {
                case PrimitiveType.Quad:
                    this.Mesh = new Quad().GetMesh();
                    break;
                case PrimitiveType.Cube:
                    this.Mesh = new Cube().GetMesh();
                    break;
                case PrimitiveType.Sphere:
                    this.Mesh = new Sphere().GetMesh();
                    break;
                default:
                    throw new ArgumentException("Unsupported primitive type: " + type);
            }
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

            // Load the mesh into the renderer
            if (this.Mesh != null)
            {
                renderer.LoadMesh(this.Mesh);
            }

            // Initialize the material
            if (this.Material == null)
            {
                this.Material = new SGMaterial("Default Material", Vector4.One);
            }
            this.Material.Init(renderer);

            // Get the default shader if none is assigned
            if (this.Shader == null)
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
            var transform = this.GetWorldTransform(); // Get the world transform of the primitive

            renderer.BindShaderProgram(this.Shader);
            renderer.PrepareShader("viewPos", camera.Transform.Position);
            if (scene.LightManager != null)
            {
                scene.LightManager.BindLights(viewport, renderer, camera);
            }
            renderer.DrawMesh(transform, Mesh, Material);
            scene.RenderStats.IncrementDrawCalls();
            renderer.UnbindShaderProgram();
        }

        /// <summary>
        /// Render the primitive for shadow mapping purposes.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void RenderShadow(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.RenderShadow(scene, viewport, renderer);

            var shader = renderer.GetShaderProgram("DepthMeshShader");
            renderer.BindShaderProgram(shader);
            renderer.DrawMesh(this.Transform, Mesh, Material);
            scene.RenderStats.IncrementDrawCalls();
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

        /// <summary>
        /// Computes the axis-aligned bounding box (AABB) for the primitive based on its mesh vertices.
        /// </summary>
        public override void ComputeAABB()
        {
            if (Mesh.Vertices == null || Mesh.Vertices.Count == 0)
            {
                this.AABB = new AABB(Vector3.Zero, Vector3.Zero);
                return;
            }

            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (var vertex in Mesh.Vertices)
            {
                min = Vector3.ComponentMin(min, vertex.Position);
                max = Vector3.ComponentMax(max, vertex.Position);
            }

            this.AABB = new AABB(min, max);
            Debug.WriteLine($"Primitive {this.Name} AABB computed: Min {this.AABB.Min}, Max {this.AABB.Max}");
        }
    }
}
