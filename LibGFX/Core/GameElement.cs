using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
using LibGFX.Math;
using LibGFX.Physics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    /// <summary>
    /// Represents a game element
    /// </summary>
    public abstract class GameElement : IIdentifier, IPropertyTable, ISerialization
    {
        /// <summary>
        /// The name of the game element
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The transform of the game element
        /// </summary>
        public virtual Transform Transform { get; set; } = new Transform();

        /// <summary>
        /// Determines if the game element is visible
        /// </summary>
        public virtual bool Visible { get; set; } = true;

        /// <summary>
        /// Determines if the game element is enabled
        /// </summary>
        public virtual bool Enabled { get; set; } = true;

        /// <summary>
        /// Determines if the game element casts shadows
        /// </summary>
        public virtual bool CastShadows { get; set; } = true;

        /// <summary>
        /// The ID of the game element
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// The parent of the game element
        /// </summary>
        public GameElement Parent { get; set; }

        /// <summary>
        /// The behaviors of the game element
        /// </summary>
        public List<IGameBehavior> Behaviors { get; set; }

        /// <summary>
        /// The axis-aligned bounding box (AABB) of the game element.
        /// </summary>
        public AABB AABB { get; set; }

        /// <summary>
        /// Gets the world axis-aligned bounding box (AABB) of the game element.
        /// </summary>
        public AABB WorldAABB { get => this.GetWorldAABB(); }

        /// <summary>
        /// The children of the game element
        /// </summary>
        public IReadOnlyList<GameElement> Children => _children;

        /// <summary>
        /// Determines if the game element has transparency.
        /// </summary>
        public abstract bool HasTransparency { get; }

        /// <summary>
        /// Gets a collection of custom properties associated with the current instance.
        /// </summary>
        /// <remarks>Use this dictionary to store and retrieve additional metadata or user-defined values
        /// related to the instance. Property names are case-sensitive. Modifying the collection affects only the
        /// current instance.</remarks>
        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        private readonly List<GameElement> _children = new List<GameElement>();

        /// <summary>
        /// Creates a new game element
        /// </summary>
        protected GameElement()
        {
            this.Behaviors = new List<IGameBehavior>();
            this.ID = Guid.NewGuid();
        }

        /// <summary>
        /// Adds a child to the game element
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(GameElement child)
        {
            _children.Add(child);
            child.Parent = this;
        }

        /// <summary>
        /// Initializes the game element
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public virtual void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            this.ComputeAABB();

            this.Behaviors.ForEach(behavior =>
            {
                behavior.OnInit(scene, viewport, renderer);
            });

            _children.ForEach(child =>
            {
                child.Init(scene, viewport, renderer);
            });
        }

        /// <summary>
        /// Renders the game element
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public virtual void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {
            if (this.Visible)
            {
                this.Behaviors.ForEach(b =>
                {
                    b.OnRender(scene, viewport, renderer, camera);
                });

                _children.ForEach(child =>
                {
                    child.Render(scene, viewport, renderer, camera);
                });
            }
        }

        /// <summary>
        /// Renders the shadow of the game element
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public virtual void RenderShadow(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            if (this.Visible && this.CastShadows)
            {
                this.Behaviors.ForEach(b =>
                {
                    b.OnShadowPass(scene, viewport, renderer);
                });

                _children.ForEach(child =>
                {
                    child.RenderShadow(scene, viewport, renderer);
                });
            }
        }

        /// <summary>
        /// Updates the game element
        /// </summary>
        /// <param name="scene"></param>
        public virtual void Update(BaseScene scene, float dt)
        {
            this.Behaviors.ForEach(b =>
            {
                b.OnUpdate(scene, dt);
            });

            _children.ForEach(child =>
            {
                child.Update(scene, dt);
            });
        }

        /// <summary>
        /// Disposes the game element
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="renderer"></param>
        public virtual void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            _children.ForEach(child =>
            {
                child.Dispose(scene, renderer);
            });
            _children.Clear();

            this.Behaviors.ForEach(b =>
            {
                b.OnDispose(scene, renderer);
            });
            this.Behaviors.Clear();
        }

        /// <summary>
        /// Collides the game element
        /// </summary>
        /// <param name="collision"></param>
        public virtual void Collide(Collision collision)
        {
            this.Behaviors.ForEach(b =>
            {
                b.OnCollide(collision);
            });
        }

        /// <summary>
        /// Adds a behavior to the game element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public T AddBehavior<T>(T behavior) where T : IGameBehavior
        {
            this.Behaviors.Add(behavior);
            behavior.SetElement(this);
            return behavior;
        }

        /// <summary>
        /// Gets a behavior from the game element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetBehavior<T>() where T : IGameBehavior
        {
            return this.Behaviors.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Adds a tag to the game element.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddProperty(string key, object value)
        {
            this.Properties.TryAdd(key, value);
        }

        /// <summary>
        /// Removes a tag from the game element.
        /// </summary>
        /// <param name="key"></param>
        public bool RemoveProperty(string key)
        {
            return this.Properties.Remove(key);
        }

        /// <summary>
        /// Checks if the game element has a specific property.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasProperty(string key)
        {
            return this.Properties.ContainsKey(key);
        }

        /// <summary>
        /// Returns the meshes and materials of the game element
        /// </summary>
        /// <returns></returns>
        public virtual Mesh[]? GetMeshes()
        {
            return null;
        }

        /// <summary>
        /// Computes the axis-aligned bounding box (AABB) of the game element.
        /// </summary>
        public abstract void ComputeAABB();

        /// <summary>
        /// Gets the world axis-aligned bounding box (AABB) of the game element by transforming its local AABB using its world transform.
        /// </summary>
        /// <returns></returns>
        public AABB GetWorldAABB()
        {
            var transform = GetWorldTransform();
            return AABB.TransformAABB(this.AABB, transform.Matrix);
        }

        /// <summary>
        /// Gets the world transform of the game element by recursively combining its local transform with its parent's world transform.
        /// </summary>
        /// <returns></returns>
        public Transform GetWorldTransform()
        {
            if (Parent == null)
            {
                return this.Transform;
            }
            else
            {
                return Transform.Attach(Parent.GetWorldTransform(), this.Transform);
            }
        }

        /// <summary>
        /// Searches the immediate child elements and returns the first child of the specified type.
        /// </summary>
        /// <remarks>Only immediate children are considered in the search. If multiple children of type T
        /// exist, only the first is returned.</remarks>
        /// <typeparam name="T">The type of child element to search for. Must derive from GameElement.</typeparam>
        /// <returns>The first child element of type T if found; otherwise, null.</returns>
        public T FindChild<T>() where T : GameElement
        {
            return this.Children.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Returns an array containing all child elements of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of child elements to search for. Must derive from GameElement.</typeparam>
        /// <returns>An array of elements of type T that are children of this instance. The array is empty if no matching
        /// children are found.</returns>
        public T[] FindChildren<T>() where T : GameElement
        {
            return this.Children.OfType<T>().ToArray();
        }


        /// <summary>
        /// Searches for a child element of the specified type with the given name.
        /// </summary>
        /// <remarks>If multiple child elements of type T share the same name, only the first one
        /// encountered is returned. The search is limited to immediate children and does not include nested
        /// descendants.</remarks>
        /// <typeparam name="T">The type of child element to search for. Must derive from GameElement.</typeparam>
        /// <param name="name">The name of the child element to locate. The comparison is case-sensitive.</param>
        /// <returns>The first child element of type T with the specified name, or null if no matching element is found.</returns>
        public T FindChild<T>(string name) where T : GameElement
        {
            return this.Children.OfType<T>().FirstOrDefault(c => c.Name == name);
        }

        /// <summary>
        /// Finds all child elements of the specified type that have the given name.
        /// </summary>
        /// <typeparam name="T">The type of child elements to search for. Must derive from GameElement.</typeparam>
        /// <param name="name">The name of the child elements to find. The comparison is case-sensitive.</param>
        /// <returns>An array containing all child elements of type T whose Name property matches the specified name. The array
        /// will be empty if no matching elements are found.</returns>
        public T[] FindChildren<T>(string name) where T : GameElement
        {
            return this.Children.OfType<T>().Where(c => c.Name == name).ToArray();
        }

        /// <summary>
        /// Serializes the current object and its hierarchy into a JSON representation.
        /// </summary>
        /// <remarks>The returned JSON object includes information about the object's type, identifier,
        /// name, transform, visibility, enabled state, shadow casting, parent, tags, axis-aligned bounding boxes,
        /// children, and custom properties. Behaviors are not currently serialized and are represented as an empty
        /// array.</remarks>
        /// <param name="serializationContext">The context that provides serialization settings and state information used during the serialization
        /// process.</param>
        /// <returns>A <see cref="JObject"/> containing the serialized data of the object, including its properties, transform,
        /// children, and related metadata.</returns>
        public virtual void Serialize(JsonWriter writer, SerializationContext serializationContext, Action<JsonWriter> callback = null)
        {
            // Start writing the JSON object
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(this.GetType().FullName);
            writer.WritePropertyName("ID");
            writer.WriteValue(this.ID.ToString());
            writer.WritePropertyName("Name");
            writer.WriteValue(this.Name);
            writer.WritePropertyName("Transform");
            this.Transform.Serialize(writer, serializationContext);
            writer.WritePropertyName("Visible");
            writer.WriteValue(this.Visible);
            writer.WritePropertyName("Enabled");
            writer.WriteValue(this.Enabled);
            writer.WritePropertyName("CastShadows");
            writer.WriteValue(this.CastShadows);
            writer.WritePropertyName("Parent");
            writer.WriteValue(this.Parent != null ? this.Parent.ID.ToString() : null);
            writer.WritePropertyName("Behaviors");
            writer.WriteStartArray();
            // Empty for now TODO: Serialize behaviors
            writer.WriteEndArray();
            writer.WritePropertyName("AABB");
            Utils.SerializeAABB(this.AABB, writer);
            writer.WritePropertyName("WorldAABB");
            Utils.SerializeAABB(this.WorldAABB, writer);
            
            writer.WritePropertyName("Children");
            writer.WriteStartArray();
            foreach (var child in _children)
            {
                child.Serialize(writer, serializationContext);
            }
            writer.WriteEndArray();

            writer.WritePropertyName("Properties");
            writer.WriteStartObject();
            foreach (var property in this.Properties)
            {
                writer.WritePropertyName(property.Key);
                writer.WriteValue(property.Value);
            }
            writer.WriteEndObject();

            callback?.Invoke(writer);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Deserializes the JSON representation into the current object and its hierarchy.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializationContext"></param>
        /// <param name="callback"></param>
        /// <exception cref="Exception"></exception>
        public virtual void Deserialize(JObject obj, SerializationContext serializationContext, Func<JObject, bool> callback = null)
        {
            // Deserialize basic properties
            this.ID = Guid.Parse(obj.Value<string>("ID")!);
            this.Name = obj.Value<string>("Name")!;
            this.Visible = obj.Value<bool>("Visible");
            this.Enabled = obj.Value<bool>("Enabled");
            this.CastShadows = obj.Value<bool>("CastShadows");
            this.Transform = new Transform();
            this.Transform.Deserialize(obj.Value<JObject>("Transform")!, serializationContext);
            this.AABB = Utils.DeserializeAABB(obj.Value<JObject>("AABB")!);

            // Deserialize Parent if exists
            var parentId = obj.Value<string>("Parent");
            if (parentId != null)
            {
                var parent = serializationContext.GetValue<GameElement>(parentId);
                if (parent == null)
                {
                    throw new Exception("Parent GameElement not found during deserialization.");
                }
            }

            // Deserialize Metadata
            this.Properties.Clear();
            foreach (var property in obj.Value<JObject>("Properties")!)
            {
                var key = property.Key;
                var value = property.Value.ToObject<object>();
                if(!this.Properties.TryAdd(key, value))
                {
                    throw new Exception($"Duplicate property key '{key}' found during deserialization.");
                }
            }

            // Deserialize Children
            foreach (var childToken in obj.Value<JArray>("Children")!)
            {
                var element = Utils.DeserializeGameElement(childToken as JObject, serializationContext);
                if(element == null)
                {
                    throw new Exception("Failed to deserialize child GameElement.");
                }
                this.AddChild(element);
            }

            // Invoke callback if provided
            callback?.Invoke(obj);

            // Register this object in the serialization context
            serializationContext.SetValue<GameElement>(this.ID.ToString(), this);
        }

    }
}
