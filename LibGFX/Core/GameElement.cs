using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
using LibGFX.Math;
using LibGFX.Physics;
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
        /// A set of tags associated with the game element, used for categorization or filtering.
        /// </summary>
        public HashSet<string> Tags { get; private set; }

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
            this.Tags = new HashSet<string>();
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
        /// <param name="tag"></param>
        public void AddTag(string tag)
        {
            this.Tags.Add(tag);
        }

        /// <summary>
        /// Removes a tag from the game element.
        /// </summary>
        /// <param name="tag"></param>
        public void RemoveTag(string tag)
        {
            this.Tags.Remove(tag);
        }

        /// <summary>
        /// Checks if the game element has a specific tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool HasTag(string tag)
        {
            return this.Tags.Contains(tag);
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
        public virtual JObject Serialize(SerializationContext serializationContext)
        {
            var childrenArray = new JArray();
            foreach (var child in _children)
            {
                childrenArray.Add(child.Serialize(serializationContext));
            }

            return new JObject()
            {
                ["Type"] = this.GetType().FullName,
                ["ID"] = this.ID.ToString(),
                ["Name"] = this.Name,
                ["Transform"] = this.Transform.Serialize(serializationContext),
                ["Visible"] = this.Visible,
                ["Enabled"] = this.Enabled,
                ["CastShadows"] = this.CastShadows,
                ["Parent"] = this.Parent != null ? this.Parent.ID.ToString() : null,
                ["Behaviors"] = new JArray(), // Empty for now TODO: Serialize behaviors
                ["Tags"] = new JArray(this.Tags.ToArray()),
                ["AABB"] = Utils.SerializeAABB(this.AABB),
                ["WorldAABB"] = Utils.SerializeAABB(this.WorldAABB),
                ["Children"] = childrenArray,
                ["Properties"] = JObject.FromObject(this.Properties)
            };
        }

        /// <summary>
        /// Populates the properties of the current object by deserializing data from the specified JSON object using
        /// the provided serialization context.
        /// </summary>
        /// <remarks>This method updates the state of the current object based on the contents of
        /// <paramref name="jObject"/>. It also registers the object in the serialization context to support reference
        /// resolution for child and parent relationships. All child elements are deserialized after the parent is
        /// registered to ensure correct hierarchy reconstruction.</remarks>
        /// <param name="jObject">A <see cref="JObject"/> containing the serialized data to deserialize into this object. Must not be null and
        /// must contain all required fields.</param>
        /// <param name="serializationContext">A <see cref="SerializationContext"/> used to resolve references and manage object identity during
        /// deserialization. Must not be null.</param>
        /// <exception cref="Exception">Thrown if a referenced parent object or a required type cannot be found during deserialization.</exception>
        public virtual void Deserialize(JObject jObject, SerializationContext serializationContext)
        {
            // Get Identifier properties
            this.ID = Guid.Parse(jObject["ID"].Value<string>());
            this.Name = jObject["Name"].Value<string>();

            // Get Transform
            this.Transform = new Transform();
            this.Transform.Deserialize((JObject)jObject["Transform"], serializationContext);

            // Get Renderable properties
            this.Visible = jObject["Visible"].Value<bool>();
            this.Enabled = jObject["Enabled"].Value<bool>();
            this.CastShadows = jObject["CastShadows"].Value<bool>();

            // Get Parent if parent exists
            if (jObject["Parent"].Value<string>() != null)
            {
                var parentId = jObject["Parent"].Value<string>();
                var parent = serializationContext.GetValue<GameElement>(parentId);
                if (parent == null)
                {
                    throw new Exception("Parent GameElement not found during deserialization.");
                }
            }

            // Get Behaviors TODO: Deserialize behaviors
            this.Behaviors = new List<IGameBehavior>();

            // Get Tags
            this.Tags = new HashSet<string>();
            foreach (var tagToken in (JArray)jObject["Tags"])
            {
                this.Tags.Add(tagToken.Value<string>());
            }

            // Get AABB
            this.AABB = Utils.DeserializeAABB((JObject)jObject["AABB"]);
            // WorldAABB is computed, no need to deserialize

            // Get Properties
            this.Properties.Clear();
            var propertiesObject = (JObject)jObject["Properties"];
            foreach (var property in propertiesObject)
            {
                this.Properties[property.Key] = property.Value.ToObject<object>();
            }

            // Register this object in the serialization context so that children can reference it as parent
            serializationContext.SetValue(this.ID.ToString(), this);

            // Deserialize Children after registering parent
            this._children.Clear();
            foreach (var childToken in (JArray)jObject["Children"])
            {
                var childObject = (JObject)childToken;
                var typeName = childObject["Type"].Value<string>();
                var type = Type.GetType(typeName);
                if (type == null)
                {
                    throw new Exception($"Type '{typeName}' not found during deserialization.");
                }
                var child = (GameElement)Activator.CreateInstance(type);
                child.Deserialize(childObject, serializationContext);
                this.AddChild(child);
            }
        }

    }
}
