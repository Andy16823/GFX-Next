using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using LibGFX.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core.GameElements
{
    /// <summary>
    /// Represents a handle for a 3D point light within a game scene, providing management and synchronization of the
    /// light's position with its associated transform.
    /// </summary>
    /// <remarks>Use this class to control a point light's placement and lifecycle in a 3D environment. The
    /// light's position is automatically updated when the transform changes. Dispose of the handle when the light is no
    /// longer needed to release resources and detach event handlers.</remarks>
    public class PointLight3DHandle : GameElement
    {
        /// <summary>
        /// Gets or sets the point light source used for 3D scene illumination.
        /// </summary>
        /// <remarks>Changing this property affects how objects in the scene are lit and how shadows are
        /// rendered. The light source position and intensity determine the appearance of lighting effects.</remarks>
        public PointLight3D LightSource { get; set; }

        /// <summary>
        /// Gets a value indicating whether the image contains any transparent pixels.
        /// </summary>
        public override bool HasTransparency => false;

        /// <summary>
        /// Initializes a new instance of the PointLight3DHandle class.
        /// </summary>
        public PointLight3DHandle()
        {

        }

        /// <summary>
        /// Initializes a new instance of the PointLight3DHandle class with the specified name, position, color, range,
        /// and intensity.
        /// </summary>
        /// <remarks>If no transform is assigned, a new Transform is created and initialized with the
        /// specified position. The transform's Changed event is automatically subscribed to for internal
        /// updates.
        /// This class adds the point light to the scene and manages its lifecycle, including disposal of resources
        /// </remarks>
        /// <param name="name">The unique name used to identify the point light instance.</param>
        /// <param name="position">The position of the point light in 3D space.</param>
        /// <param name="color">The color of the point light, including its alpha component.</param>
        /// <param name="range">The effective range of the point light, in world units. Must be greater than zero.</param>
        /// <param name="intesity">The intensity of the point light. Must be a non-negative value.</param>
        public PointLight3DHandle(String name, Vector3 position, Vector4 color, float range = 10f, float intesity = 1.0f)
        {
            this.Name = name;
            LightSource = new PointLight3D(position, color, range, intesity);

            if(this.Transform == null) 
            {
                this.Transform = new Transform();
            }

            this.Transform.Position = position;
            this.Transform.Changed += Transform_Changed;
        }

        /// <summary>
        /// Updates the light source position to match the position of the specified transform.
        /// </summary>
        /// <param name="obj">The transform whose position is used to update the light source. Cannot be null.</param>
        private void Transform_Changed(Transform obj)
        {
            LightSource.Position = this.Transform.Position;
            Debug.WriteLine($"PointLight3DHandle: Updated LightSource position to {LightSource.Position}");
        }

        public override void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            base.Init(scene, viewport, renderer);
            if(!scene.LightManager.ContainsLight(LightSource))
            {
                scene.AddLight<PointLight3D>(LightSource);
            }
            else
            {
                Debug.WriteLine($"LightSource with ID {LightSource.ID} is already present in the scene.");
            }
        }

        /// <summary>
        /// Releases resources used by the object and detaches event handlers associated with the specified scene and
        /// render device.
        /// </summary>
        /// <param name="scene">The scene context for which resources should be released.</param>
        /// <param name="renderer">The render device associated with the scene that is used for resource management.</param>
        public override void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            base.Dispose(scene, renderer);
            if(this.Transform != null)
            {
                this.Transform.Changed -= Transform_Changed;
            }
            // TODO: Add light removal from scene
            //scene.RemoveLight<PointLight3D>(LightSource);
        }

        /// <summary>
        /// Calculates and updates the axis-aligned bounding box (AABB) for the current light source based on its
        /// position and range.
        /// </summary>
        /// <remarks>This method sets the AABB property to encompass the area affected by the light
        /// source. Call this method after modifying the light source's position or range to ensure the bounding box
        /// remains accurate.</remarks>
        public override void ComputeAABB()
        {
            AABB = new AABB(
                LightSource.Position - new Vector3(LightSource.Range),
                LightSource.Position + new Vector3(LightSource.Range)
            );
        }

        /// <summary>
        /// Serializes the current object to a JSON representation, including type and light source information.
        /// </summary>
        /// <param name="serializationContext">The context that provides settings and state for the serialization process.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized representation of the object, including its type and
        /// serialized light source.</returns>
        public override void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            base.Serialize(writer, serializationContext, (w) =>
            {
                w.WritePropertyName("LightSource");
                w.WriteValue(this.LightSource.ID.ToString());
                callback?.Invoke(w);
            });
        }

        /// <summary>
        /// Deserializes the object from a JSON representation, restoring its state including the light source
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="serializationContext"></param>
        /// <param name="callback"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public override void Deserialize(JsonReader reader, SerializationContext serializationContext, Func<JsonReader, string, bool> callback = null)
        {
            base.Deserialize(reader, serializationContext, (r, param) =>
            {
                switch (param)
                {
                    case "LightSource":
                        var lightSourceId = (string) r.Value;
                        var lightSource = serializationContext.GetValue<Light>(lightSourceId);
                        if (lightSource != null && lightSource is PointLight3D pointLight)
                        {
                            LightSource = pointLight;
                            this.Transform.Position = LightSource.Position;
                        }
                        else if (lightSourceId != null)
                        {
                            throw new InvalidOperationException($"Light with ID {lightSourceId} is not a PointLight3D or could not be found.");
                        }
                        else
                        {
                            throw new InvalidOperationException("LightSource ID is missing in the JSON data.");
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
            this.Transform.Position = LightSource.Position;
            this.Transform.Changed += Transform_Changed;
        }
    }
}
