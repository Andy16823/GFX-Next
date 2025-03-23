using LibGFX.Core;
using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics
{
    /// <summary>
    /// Represents a physics behavior
    /// </summary>
    public abstract class PhysicsBehavior : IGameBehavior
    {
        /// <summary>
        /// The physics handler
        /// </summary>
        public PhysicsHandler PhysicsHandler { get; set; }

        /// <summary>
        /// Creates a new physics behavior
        /// </summary>
        /// <param name="physicsHandler"></param>
        protected PhysicsBehavior(PhysicsHandler physicsHandler)
        {
            this.PhysicsHandler = physicsHandler;
        }

        /// <summary>
        /// Gets the game element
        /// </summary>
        /// <returns></returns>
        public abstract GameElement GetElement();

        /// <summary>
        /// Disposes the physics behavior
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="renderer"></param>
        public abstract void OnDispose(BaseScene scene, IRenderDevice renderer);

        /// <summary>
        /// Initializes the physics behavior
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public abstract void OnInit(BaseScene scene, Viewport viewport, IRenderDevice renderer);

        /// <summary>
        /// Renders the physics behavior
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public abstract void OnRender(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera);

        /// <summary>
        /// Updates the physics behavior
        /// </summary>
        /// <param name="scene"></param>
        public abstract void OnUpdate(BaseScene scene);

        /// <summary>
        /// Sets the game element
        /// </summary>
        /// <param name="gameElement"></param>
        public abstract void SetElement(GameElement gameElement);

        /// <summary>
        /// Gets the physics object
        /// </summary>
        /// <returns></returns>
        public abstract object GetPhysicsObject();

        /// <summary>
        /// Called when the physics behavior collides with another object
        /// </summary>
        /// <param name="collision"></param>
        public abstract void OnCollide(Collision collision);
    }
}
