using LibGFX.Assets;
using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Primitives;
using LibGFX.Graphics.Renderer.OpenGL;
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
        /// the shader program used for rendering the primitive.
        /// </summary>
        public RenderShader Shader { get; set; }

        /// <summary>
        /// Gets or sets the type of primitive geometry to use for rendering.
        /// </summary>
        public PrimitiveType PrimitiveType { get; set; }

        /// <summary>
        /// Gets or sets the material used to render the object.
        /// </summary>
        public IMaterial Material { get; set; }

        /// <summary>
        /// Gets a value indicating whether the mesh's material includes transparency.
        /// </summary>
        public override bool HasTransparency => Material.IsTransparent;

        // Mesh associated with the primitive
        private Mesh _mesh;

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
        public Primitive(String name, IMaterial material, PrimitiveType type)
        {
            this.Name = name;
            this.Material = material;
            this.PrimitiveType = type;
        }

        /// <summary>
        /// Initializes the primitive with the specified scene, viewport, and renderer.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            // Get the primitive mesh based on the primitive type
            _mesh = renderer.GetPrimitiveMesh(this.PrimitiveType);
            if (_mesh == null)
            {
                throw new Exception($"Failed to get mesh for primitive type {this.PrimitiveType}");
            }

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
            var transform = this.GetWorldTransform();

            renderer.BindShaderProgram(this.Shader);
            renderer.PrepareShader("viewPos", camera.Transform.Position);
            if (scene.LightManager != null)
            {
                scene.LightManager.BindLights(viewport, renderer, camera);
            }
            renderer.DrawMesh(transform, _mesh, Material);
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

            // Use the depth mesh shader for shadow rendering
            var shader = renderer.GetRenderShader("DepthMeshShader");
            renderer.BindShaderProgram(shader);
            renderer.DrawMesh(this.Transform, _mesh, Material);
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
            return new Mesh[] { _mesh };
        }

        /// <summary>
        /// Computes the axis-aligned bounding box (AABB) for the primitive based on its mesh vertices.
        /// </summary>
        public override void ComputeAABB()
        {
            if (_mesh == null)
            {
                this.AABB = new AABB(Vector3.Zero, Vector3.Zero);
                return;
            }

            this.AABB = _mesh.Bounds;
        }

        /// <summary>
        /// Serializes the primitive to JSON format.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="serializationContext"></param>
        /// <param name="callback"></param>
        public override void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            base.Serialize(writer, serializationContext, (w) =>
            {
                w.WritePropertyName("PrimitiveType");
                w.WriteValue((int)this.PrimitiveType);
                w.WritePropertyName("Material");
                w.WriteValue(this.Material.ID.ToString());
                callback?.Invoke(w);
            });
        }

        /// <summary>
        /// Deserializes the primitive from JSON format.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializationContext"></param>
        /// <param name="callback"></param>
        public override void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            base.Deserialize(obj, serializationContext, (refObj) =>
            {
                // Deserialize PrimitiveType
                this.PrimitiveType = (PrimitiveType)refObj.Value<int>("PrimitiveType");
                if (callback != null)
                {
                    return callback(refObj);
                }

                // Deserialize Material
                var materialIdStr = refObj.Value<string>("Material");
                if(materialIdStr != null)
                {
                    var material = serializationContext.GetValue<IMaterial>(materialIdStr);
                    if(material != null)
                    {
                        this.Material = material;
                    }
                }

                return true;
            });
        }
    }
}
