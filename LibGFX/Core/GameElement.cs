using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
using LibGFX.Math;
using LibGFX.Physics;
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
    public abstract class GameElement
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
        /// The children of the game element
        /// </summary>
        public IReadOnlyList<GameElement> Children => _children;


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
        public virtual void Update(BaseScene scene)
        {
            this.Behaviors.ForEach(b =>
            {
                b.OnUpdate(scene);
            });

            _children.ForEach(child =>
            {
                child.Update(scene);
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
        public virtual (Mesh, IMaterial)[]? GetMeshes()
        {
            return null;
        }

        /// <summary>
        /// Computes the axis-aligned bounding box (AABB) of the game element.
        /// </summary>
        public abstract void ComputeAABB();

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
    }
}
