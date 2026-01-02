using LibGFX.Assets;
using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Primitives;
using LibGFX.Graphics.Shader;
using LibGFX.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    /// TODO REWORK!
    /// Represents a primitive game element that can be rendered with a material and shader.
    /// </summary>
    public class Primitive : GameElement
    {
        /// <summary>
        /// The name of the primitive.
        /// </summary>
        public Mesh Mesh { get; set; }

        /// <summary>
        /// the shader program used for rendering the primitive.
        /// </summary>
        public RenderShader Shader { get; set; }

        /// <summary>
        /// Gets or sets the type of primitive geometry to use for rendering.
        /// </summary>
        public PrimitiveType PrimitiveType { get; set; }

        /// <summary>
        /// Gets a value indicating whether the mesh's material includes transparency.
        /// </summary>
        public override bool HasTransparency => this.Mesh.Material.IsTransparent;

        /// <summary>
        /// Initializes a new instance of the Primitive class.
        /// </summary>
        public Primitive()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the Primitive class with the specified name and mesh.
        /// </summary>
        /// <param name="name">The name to assign to the primitive. Cannot be null or empty.</param>
        /// <param name="mesh">The mesh that defines the geometry of the primitive. Cannot be null.</param>
        public Primitive(String name, Mesh mesh)
        {
            this.Name = name;
            this.Mesh = mesh;
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

            // Get the default shader if none is assigned
            if (this.Shader == null)
            {
                this.Shader = renderer.GetRenderShader("MeshShader");
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
            renderer.DrawMesh(transform, Mesh);
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

            var shader = renderer.GetRenderShader("DepthMeshShader");
            renderer.BindShaderProgram(shader);
            renderer.DrawMesh(this.Transform, Mesh);
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
        }

        /// <summary>
        /// Gets the meshes and materials associated with the primitive.
        /// </summary>
        /// <returns></returns>
        public override Mesh[]? GetMeshes()
        {
            return new Mesh[] { this.Mesh };
        }

        /// <summary>
        /// Computes the axis-aligned bounding box (AABB) for the primitive based on its mesh vertices.
        /// </summary>
        public override void ComputeAABB()
        {
            if (Mesh == null)
            {
                this.AABB = new AABB(Vector3.Zero, Vector3.Zero);
                return;
            }

            this.AABB = this.Mesh.Bounds;
        }

        /// <summary>
        /// Creates a new primitive object with the specified name, material, and type, and registers its mesh with the
        /// asset manager.
        /// </summary>
        /// <remarks>The created mesh is automatically added to the provided asset manager. Supported
        /// primitive types include quad, cube, and sphere. If an unsupported type is specified, a cube is created by
        /// default.</remarks>
        /// <param name="name">The name to assign to the created primitive.</param>
        /// <param name="material">The material to apply to the primitive's mesh. Cannot be null.</param>
        /// <param name="assets">The asset manager used to register the generated mesh. Cannot be null.</param>
        /// <param name="type">The type of primitive to create. Defaults to <see cref="PrimitiveType.Cube"/> if not specified.</param>
        /// <returns>A new <see cref="Primitive"/> instance representing the created primitive with the specified properties.</returns>
        public static Primitive CreatePrimitive(String name, IMaterial material, AssetManager assets, PrimitiveType type = PrimitiveType.Cube)
        {
            var mesh = new Mesh();
            switch (type)
            {
                case PrimitiveType.Quad:
                    mesh = new Quad().GetMesh();
                    break;
                case PrimitiveType.Cube:
                    mesh = new Cube().GetMesh();
                    break;
                case PrimitiveType.Sphere:
                    mesh = new Sphere().GetMesh();
                    break;
                default:
                    mesh = new Cube().GetMesh();
                    break;
            }

            mesh.Name = mesh.ID.ToString();
            mesh.Material = material;
            assets.Add(mesh);
            return new Primitive(name, mesh);
        }

        public override void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            base.Serialize(writer, serializationContext, (w) =>
            {
                w.WritePropertyName("PrimitiveType");
                w.WriteValue(this.PrimitiveType);
                callback?.Invoke(w);
            });
        }

        public override void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            base.Deserialize(obj, serializationContext, (refObj) =>
            {
                this.PrimitiveType = refObj.Value<PrimitiveType>("PrimitiveType");
                if (callback != null)
                {
                    return callback(refObj);
                }
                return true;
            });
        }
    }
}
