using LibGFX.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    /// <summary>
    /// Represents a layer in the scene
    /// </summary>
    public class Layer : IIdentifier, ISerialization
    {
        /// <summary>
        /// The name of the layer
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets the unique identifier for this instance.
        /// </summary>
        public Guid ID { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Determines if the layer is visible
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Determines if the layer is enabled
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The elements of the layer
        /// </summary>
        public List<GameElement> Elements { get; set; }

        /// <summary>
        /// Initializes a new instance of the Layer class.
        /// Used for deserialization purposes.
        /// </summary>
        public Layer()
        {
            this.Elements = new List<GameElement>();
        }

        /// <summary>
        /// Creates a new layer
        /// </summary>
        /// <param name="name"></param>
        public Layer(String name)
        {
            this.Name = name;
            this.Elements = new List<GameElement>();
        }

        /// <summary>
        /// Initializes the layer
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            this.Elements.ForEach(e =>
            {
                e.Init(scene, viewport, renderer);
            });
        }

        /// <summary>
        /// Renders the layer
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public void RenderLayer(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            if (this.Visible)
            {
                this.Elements.ForEach(e => {
                    if (e.Visible)
                    {
                        e.Render(scene, viewport, renderer, camera);
                    }
                });
            }
        }

        /// <summary>
        /// Renders the shadows of the layer
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public void RenderShadows(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            if (this.Visible)
            {
                this.Elements.ForEach(e =>
                {
                    if (e.CastShadows && e.Visible)
                    {
                        e.RenderShadow(scene, viewport, renderer);
                    }
                });
            }
        }

        /// <summary>
        /// Updates the layer
        /// </summary>
        /// <param name="scene"></param>
        public void Update(BaseScene scene, float dt)
        {
            if (this.Enabled)
            {
                this.Elements.ForEach(e =>
                {
                    e.Update(scene, dt);
                });
            }
        }

        /// <summary>
        /// Disposes the layer
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="renderer"></param>
        public void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            this.Elements.ForEach(e =>
            {
                e.Dispose(scene, renderer);
            });
        }

        /// <summary>
        /// Finds an element by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public T? FindElement<T>(String name) where T : GameElement
        {
            return this.Elements.OfType<T>().FirstOrDefault(e => e.Name == name);
        }

        /// <summary>
        /// Finds an element by ID as a string representation of the GUID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GameElement? FindElementByID(String id)
        {
            return this.Elements.FirstOrDefault(e => e.ID.ToString() == id);
        }

        /// <summary>
        /// Finds an element by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GameElement? FindElement(Guid id)
        {
            return this.Elements.FirstOrDefault(e => e.ID == id);
        }

        /// <summary>
        /// Finds all elements with a specific property key
        /// </summary>
        /// <param name="key">The key of the property to search for.</param>
        /// <returns>A collection of game elements that have the specified property key.</returns>
        public ICollection<GameElement> GetElementsWithProperty(String key)
        {
            return this.Elements.Where(e => e.HasProperty(key)).ToList();
        }

        /// <summary>
        /// Finds all elements that match a specific predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public ICollection<GameElement> FindElements(Func<GameElement, bool> predicate)
        {
            return this.Elements.Where(predicate).ToList();
        }

        /// <summary>
        /// Finds all elements with a specific behavior type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ICollection<GameElement> GetElementsWithBehavior<T>() where T : IGameBehavior
        {
            return this.Elements.Where(e => e.Behaviors.Any(b => b is T)).ToList();
        }

        /// <summary>
        /// Finds all elements of a specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ICollection<GameElement> GetElements<T>() where T : GameElement
        {
            return this.Elements.Where(e => e is T).ToList();
        }

        /// <summary>
        /// Tries to add an element to the layer
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool TryRemoveElement(GameElement element)
        {
            if (this.Elements.Contains(element))
            {
                this.Elements.Remove(element);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Serializes the current object and its child elements into a JSON representation using the specified
        /// serialization context.
        /// </summary>
        /// <param name="serializationContext">The context that provides configuration and state information for the serialization process.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized data of the object, including its properties and child
        /// elements.</returns>
        public void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Name");
            writer.WriteValue(this.Name);
            writer.WritePropertyName("ID");
            writer.WriteValue(this.ID.ToString());
            writer.WritePropertyName("Visible");
            writer.WriteValue(this.Visible);
            writer.WritePropertyName("Enabled");
            writer.WriteValue(this.Enabled);
            writer.WritePropertyName("Elements");
            writer.WriteStartArray();
            foreach (var element in this.Elements)
            {
                element.Serialize(writer, serializationContext);
            }
            writer.WriteEndArray();
            callback?.Invoke(writer);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Deserializes the current object and its child elements from a JSON representation using the specified
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializationContext"></param>
        /// <param name="callback"></param>
        /// <exception cref="Exception"></exception>
        public void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            this.Name = obj.Value<string>("Name");
            this.ID = Guid.Parse(obj.Value<string>("ID"));
            this.Visible = obj.Value<bool>("Visible");
            this.Enabled = obj.Value<bool>("Enabled");
            var elementsArray = obj.Value<JArray>("Elements");
            foreach (var elementToken in elementsArray)
            {
                var elementObj = (JObject)elementToken;
                var element = Utils.DeserializeGameElement(elementObj, serializationContext);
                if (element == null)
                {
                    throw new Exception("Failed to deserialize child GameElement.");
                }
                this.Elements.Add(element);
            }
            callback?.Invoke(obj);
        }
    }
}
