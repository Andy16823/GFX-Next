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
        /// Initializes a new instance of the StaticModel class.
        /// </summary>
        public StaticModel()
        {
            
        }

        /// <summary>
        /// Creates a new static model game element
        /// Shared models should be used when multiple instances of the same model are needed
        /// </summary>
        /// <param name="name"></param>
        /// <param name="model"></param>
        public StaticModel(String name, Graphics.StaticMeshModel model)
        {
            this.Name = name;
            _model = model;
            this.ComputeAABB();
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
            base.Render(scene, viewport, renderer, camera);
            var transform = this.GetWorldTransform();
            var shader = renderer.GetShaderProgram("MeshShader");

            renderer.BindShaderProgram(shader);
            renderer.PrepareShader("viewPos", camera.Transform.Position);
            if (scene.LightManager != null)
            {
                scene.LightManager.BindLights(viewport, renderer, camera);
            }

            foreach (var mesh in _model.Meshes)
            {
                if(mesh.Material.IsTransparent)
                {
                    renderer.EnableBlend();
                    renderer.SetBlendMode((int) BlendingFactor.SrcAlpha, (int) BlendingFactor.OneMinusSrcAlpha);
                }
                renderer.DrawMesh(transform, mesh);
                if (mesh.Material.IsTransparent)
                {
                    renderer.DisableBlend();
                }
                scene.RenderStats.IncrementDrawCalls();
            }

            renderer.UnbindShaderProgram();
        }

        /// <summary>
        /// Renders the static model for shadow mapping
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public override void RenderShadow(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.RenderShadow(scene, viewport, renderer);
            var transform = this.GetWorldTransform();
            var shader = renderer.GetShaderProgram("DepthMeshShader");
            renderer.BindShaderProgram(shader);
            foreach (var mesh in _model.Meshes)
            {
                renderer.DrawMesh(transform, mesh);
                scene.RenderStats.IncrementDrawCalls();
            }
            renderer.UnbindShaderProgram();
        }

        /// <summary>
        /// Computes the axis-aligned bounding box for the static model
        /// </summary>
        public override void ComputeAABB()
        {
            if (_model.Meshes.Count == 0)
            {
                this.AABB = new AABB(Vector3.Zero, Vector3.Zero);
                return;
            }

            var min = new Vector3(float.MaxValue);
            var max = new Vector3(float.MinValue);

            foreach (var mesh in _model.Meshes)
            {
                foreach (var vertex in mesh.Vertices)
                {
                    min = Vector3.ComponentMin(min, vertex.Position);
                    max = Vector3.ComponentMax(max, vertex.Position);
                }
            }

            this.AABB = new AABB(min, max);
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
        /// Deserializes the StaticModel instance from the specified JSON object using the provided serialization
        /// context.
        /// </summary>
        /// <param name="jObject">The JSON object containing the data to deserialize into this StaticModel instance.</param>
        /// <param name="serializationContext">The context used to resolve references and retrieve objects during deserialization.</param>
        /// <exception cref="Exception">Thrown if the model identifier specified in the JSON object cannot be found in the serialization context.</exception>
        public override void Deserialize(JsonReader reader, SerializationContext serializationContext, Func<JsonReader, string, bool> callback = null)
        {
            // Deserialize base properties
            base.Deserialize(reader, serializationContext, (r, param) =>
            {
                switch (param)
                {
                    case "Model":
                        var modelId = (string)r.Value;
                        var model = serializationContext.GetValue<StaticMeshModel>(modelId);
                        if (model != null)
                        {
                            _model = model;
                        }
                        else
                        {
                            throw new Exception($"StaticModel deserialization failed: Model with ID {modelId} not found in serialization context.");
                        }
                        return true;
                    default:
                        if(callback != null)
                        {
                            return callback(r, param);
                        }
                        break;
                }
                return false;
            });
        }
    }
}
