using LibGFX.Assets;
using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Primitives;
using LibGFX.Graphics.Shader;
using LibGFX.Math;
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
        /// the shader program used for rendering the primitive.
        /// </summary>
        public ShaderProgram Shader { get; set; }

        /// <summary>
        /// Initializes a new instance of the Primitive class with the specified name, material, and mesh.
        /// </summary>
        /// <remarks>This constructor is obsolete. Use Primitive(String name, Mesh mesh) instead. The
        /// material is assigned directly to the provided mesh.</remarks>
        /// <param name="name">The name to assign to the primitive. Cannot be null.</param>
        /// <param name="material">The material to associate with the mesh. Cannot be null.</param>
        /// <param name="mesh">The mesh to use for the primitive. Cannot be null.</param>
        [Obsolete("Use Primitive(String name, Mesh mesh) instead.")]
        public Primitive(String name, IMaterial material, Mesh mesh)
        {
            this.Name = name;
            this.Mesh = mesh;
            this.Mesh.Material = material;
            this.ComputeAABB();
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
            this.ComputeAABB();
        }

        /// <summary>
        /// Initializes a new instance of the Primitive class using the specified name, material, and primitive source.
        /// </summary>
        /// <remarks>This constructor is obsolete. Use Primitive(String name, Mesh mesh) instead. 
        /// The reason for deprecation is that an GameElement should never own IGraphicsRessource. This ressources should
        /// be managed from the AssetManager or similar systems to have them collected together and avoid duplicates.
        /// </remarks>
        /// <param name="name">The name to assign to the new primitive. Cannot be null.</param>
        /// <param name="material">The material to apply to the mesh of the new primitive. Cannot be null.</param>
        /// <param name="primitive">The source primitive from which to obtain the mesh. Cannot be null.</param>
        [Obsolete("Use Primitive(String name, Mesh mesh) instead.")]
        public Primitive(String name, IMaterial material, IPrimitive primitive)
        {
            this.Name = name;
            this.Mesh = primitive.GetMesh();
            this.Mesh.Material = material;
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
            this.Mesh.Material = material;  
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

            var shader = renderer.GetShaderProgram("DepthMeshShader");
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

        /// <summary>
        /// Serializes the current object to a JSON representation, including type and mesh information.
        /// </summary>
        /// <remarks>The returned JSON object includes additional fields specific to the primitive, such
        /// as the fully qualified type name and the mesh ID if available. This method extends the base serialization
        /// with primitive-specific data.</remarks>
        /// <param name="serializationContext">The context that provides information and services for the serialization process.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized data for the object, including its type and associated
        /// mesh identifier.</returns>
        public override JObject Serialize(SerializationContext serializationContext)
        {
            var baseInfo = base.Serialize(serializationContext);
            // Add primitive-specific data to the JSON object
            baseInfo["Type"] = this.GetType().FullName;
            baseInfo["Mesh"] = Mesh?.ID.ToString();
            return baseInfo;
        }

        /// <summary>
        /// Deserializes the primitive object from the specified JSON object using the provided serialization context.
        /// </summary>
        /// <param name="jObject">The JSON object containing the serialized data for the primitive.</param>
        /// <param name="serializationContext">The context used to resolve references and retrieve objects during deserialization.</param>
        /// <exception cref="Exception">Thrown if the mesh referenced in the JSON object cannot be found in the serialization context.</exception>
        public override void Deserialize(JObject jObject, SerializationContext serializationContext)
        {
            base.Deserialize(jObject, serializationContext);

            // Deserialize primitive-specific data if needed
            var meshId = jObject["Mesh"].ToString();
            var mesh = serializationContext.GetValue<Mesh>(meshId);
            if(serializationContext.GetValue<Mesh>(meshId) != null)
            {
                this.Mesh = mesh;
            }
            else
            {
                throw new Exception("Failed to deserialize Primitive: Mesh with ID " + meshId + " not found in serialization context.");
            }
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
    }
}
