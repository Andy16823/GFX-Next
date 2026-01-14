using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGFX.Graphics;
using LibGFX.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Graphics.OpenGL4;
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
        /// Gets a value indicating whether the image contains any transparent pixels.
        /// </summary>
        public override bool HasTransparency => _model.HasTransparency;

        /// <summary>
        /// Initializes a new instance of the StaticModel class.
        /// </summary>
        public StaticModel()
        {
            
        }

        /// <summary>
        /// Creates a new static model game element
        /// </summary>
        /// <param name="name"></param>
        /// <param name="model"></param>
        public StaticModel(String name, Graphics.StaticMeshModel model)
        {
            this.Name = name;
            _model = model;
        }

        /// <summary>
        /// Creates a new static model game element
        /// Shared models should be used when multiple instances of the same model are needed
        /// </summary>
        /// <param name="name"></param>
        /// <param name="model"></param>
        public StaticModel(String name, Vector3 position, Graphics.StaticMeshModel model)
        {
            this.Name = name;
            _model = model;
            this.Transform.Position = position; 
        }

        /// <summary>
        /// Creates a new static model game element
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        /// <param name="model"></param>
        public StaticModel(String name, Vector3 position, Vector3 scale, Graphics.StaticMeshModel model)
        {
            this.Name = name;
            this.Transform.Position = position;
            this.Transform.Scale = scale;
            _model = model;
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
            if (this.Visible)
            {
                // Call base render method
                base.Render(scene, viewport, renderer, camera);

                // Get world transform
                var transform = this.GetWorldTransform();

                // Render each mesh
                foreach (var mesh in _model.Meshes)
                {
                    // Bind the material, which sets up the shader and textures
                    mesh.Material.Use(renderer);

                    // Prepare shader uniforms
                    renderer.PrepareShader("viewPos", camera.Transform.Position);
                    scene.LightManager?.BindLights(viewport, renderer, camera);

                    // Draw the mesh and increment draw call count
                    renderer.DrawMesh(transform, mesh);
                    scene.RenderStats.IncrementDrawCalls();

                    // Unbind the material after rendering
                    mesh.Material.Disable(renderer);
                }
            }
        }

        /// <summary>
        /// Renders the static model for shadow mapping
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void RenderShadow(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            if(this.Visible) {
                // Call base render shadow method
                base.RenderShadow(scene, viewport, renderer);

                // Get world transform
                var transform = this.GetWorldTransform();

                // Use the depth mesh shader for shadow rendering no materials needed
                var shader = renderer.GetRenderShader("DepthMeshShader");
                renderer.BindShaderProgram(shader);

                // Render each mesh
                foreach (var mesh in _model.Meshes)
                {
                    renderer.DrawMesh(transform, mesh);
                    scene.RenderStats.IncrementDrawCalls();
                }

                // Unbind the shader program after rendering
                renderer.UnbindShaderProgram();
            }
        }

        /// <summary>
        /// Computes the axis-aligned bounding box for the static model
        /// </summary>
        public override void ComputeAABB()
        {
            if (_model.Meshes.Count == 0)
            {
                this.AABB = new AABB(Vector3.Zero, Vector3.Zero);
                this.AABB = new AABB(Vector3.Zero, Vector3.Zero);
                return;
            }

            AABB aabb = _model.Meshes[0].Bounds;
            for (int i = 1; i < _model.Meshes.Count; i++)
            {
                aabb = AABB.Combine(aabb, _model.Meshes[i].Bounds);
            }
            this.AABB = aabb;
        }

        /// <summary>
        /// Retrieves all meshes contained in the model.
        /// </summary>
        /// <returns>An array of <see cref="Mesh"/> objects representing the meshes in the model, or <see langword="null"/> if no
        /// meshes are available. The array will be empty if the model contains no meshes.</returns>
        public override Mesh[]? GetMeshes()
        {
            return _model.Meshes.ToArray();
        }

        /// <summary>
        /// Retrieves the static mesh model associated with this instance.
        /// </summary>
        /// <returns>The <see cref="Graphics.StaticMeshModel"/> representing the current static mesh. Returns null if no model is
        /// set.</returns>
        public Graphics.StaticMeshModel GetModel()
        {
            return _model;
        }

        /// <summary>
        /// Serializes the current object to a JSON representation using the specified serialization context.
        /// </summary>
        /// <param name="serializationContext">The context that provides information and settings for the serialization process.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized representation of the object, including type and model
        /// information.</returns>
        public override void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            // Serialize base properties
            base.Serialize(writer, serializationContext, (w) =>
            {
                w.WritePropertyName("Model");
                w.WriteValue(_model.ID.ToString());
                callback?.Invoke(w);
            });
        }

        /// <summary>
        /// Deserializes the object from a JSON representation using the specified serialization context.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializationContext"></param>
        /// <param name="callback"></param>
        /// <exception cref="Exception"></exception>
        public override void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            // Deserialize base properties
            base.Deserialize(obj, serializationContext, (refObj) =>
            {
                var modelId = refObj.Value<string>("Model");
                var model = serializationContext.GetValue<StaticMeshModel>(modelId!);
                if (model != null)
                {
                    _model = model;
                }
                else
                {
                    throw new Exception($"StaticModel deserialization failed: Model with ID {modelId} not found in serialization context.");
                }

                if(callback != null)
                {
                    return callback(refObj);
                }

                return true;
            });
        }
    }
}
