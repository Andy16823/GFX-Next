using LibGFX.Graphics;
using LibGFX.Math;
using LibGFX.Pyhsics;
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
        public Transform Transform { get; set; } = new Transform();

        /// <summary>
        /// Determines if the game element is visible
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Determines if the game element is enabled
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The behaviors of the game element
        /// </summary>
        public List<IGameBehavior> Behaviors { get; set; }

        /// <summary>
        /// Creates a new game element
        /// </summary>
        protected GameElement()
        {
            this.Behaviors = new List<IGameBehavior>();
        }

        /// <summary>
        /// Initializes the game element
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public virtual void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            this.Behaviors.ForEach(behavior => {
                behavior.OnInit(scene, viewport, renderer);
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
                this.Behaviors.ForEach(b => {
                    b.OnRender(scene, viewport, renderer, camera);
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
        }

        /// <summary>
        /// Disposes the game element
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="renderer"></param>
        public virtual void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            this.Behaviors.ForEach(b =>
            {
                b.OnDispose(scene, renderer);
            });
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
    }
}
