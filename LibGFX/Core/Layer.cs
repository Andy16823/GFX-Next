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
        public GameElement? FindElement(String name)
        {
            return this.Elements.FirstOrDefault(e => e.Name == name);
        }

        /// <summary>
        /// Finds an element by ID using its hash code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GameElement? FindElementByID(int id)
        {
            return this.Elements.FirstOrDefault(e => e.ID.GetHashCode() == id);
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
        /// Finds an element by a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public GameElement? FindElement(Func<GameElement, bool> predicate)
        {
            return this.Elements.FirstOrDefault(predicate);
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
        /// Finds all elements with a specific tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public ICollection<GameElement> FindElementsWithTag(String tag)
        {
            return this.Elements.Where(e => e.Tags.Contains(tag)).ToList();
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
        public ICollection<GameElement> FindElementsWithBehaviors<T>() where T : IGameBehavior
        {
            return this.Elements.Where(e => e.Behaviors.Any(b => b is T)).ToList();
        }

        /// <summary>
        /// Finds all elements of a specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ICollection<GameElement> FindElements<T>() where T : GameElement
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
        /// Populates the object's properties and elements from the specified JSON object using the provided
        /// serialization context.
        /// </summary>
        /// <remarks>This method expects the JSON object to contain all necessary fields, including
        /// 'Name', 'ID', 'Visible', 'Enabled', and an 'Elements' array. Each element in the 'Elements' array must
        /// specify a valid type name that can be resolved at runtime. Existing elements in the collection are not
        /// cleared before new elements are added.</remarks>
        /// <param name="jObject">The JSON object containing the data to deserialize. Must include valid values for all required properties
        /// and elements.</param>
        /// <param name="serializationContext">The context used to assist with deserialization, providing any necessary configuration or state.</param>
        /// <exception cref="Exception">Thrown if an element type specified in the JSON cannot be found during deserialization.</exception>
        public void Deserialize(JsonReader reader, SerializationContext serializationContext, Func<JsonReader, string, bool> callback = null)
        {
            if(reader.TokenType != JsonToken.StartObject)
                throw new Exception("Expected StartObject token");

            while(reader.Read())
            {
                if(reader.TokenType == JsonToken.EndObject)
                    break;

                if(reader.TokenType == JsonToken.PropertyName)
                {
                    string propertyName = reader.Value.ToString();
                    reader.Read(); // Move to the value token

                    switch (propertyName)
                    {
                        case "Name":
                            this.Name = (string)reader.Value;
                            break;
                        case "ID":
                            this.ID = Guid.Parse((string)reader.Value);
                            break;
                        case "Visible":
                            this.Visible = Convert.ToBoolean(reader.Value);
                            break;
                        case "Enabled":
                            this.Enabled = Convert.ToBoolean(reader.Value);
                            break;
                        case "Elements":
                            if (reader.TokenType != JsonToken.StartArray)
                                throw new Exception("Expected StartArray token for Elements");

                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.EndArray)
                                    break;

                                if (reader.TokenType == JsonToken.StartObject)
                                {
                                    var element = Utils.DeserializeGameElement(reader, serializationContext);
                                    if (element == null)
                                    {
                                        throw new Exception("Failed to deserialize child GameElement.");
                                    }
                                    this.Elements.Add(element);
                                }
                            }
                            break;
                        default:
                            if(callback != null && callback(reader, propertyName))
                            {
                                break;
                            }
                            reader.Skip();
                            break;
                    }
                }
            }
        }
    }
}
