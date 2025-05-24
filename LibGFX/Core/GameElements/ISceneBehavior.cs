using LibGFX.Graphics;
using LibGFX.Pyhsics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core.GameElements
{
    /// <summary>
    /// Represents a scene behavior interface
    /// </summary>
    public interface ISceneBehavior
    {
        /// <summary>
        /// Called when the scene is getting initialized
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public void OnInit(BaseScene scene, Viewport viewport, IRenderDevice renderer);

        /// <summary>
        /// Called before the scene is getting updated
        /// </summary>
        /// <param name="scene"></param>
        public void BeforeUpdate(BaseScene scene);

        /// <summary>
        /// Called after the scene is getting updated
        /// </summary>
        /// <param name="scene"></param>
        public void AfterUpdate(BaseScene scene);

        /// <summary>
        /// Called before the physics update
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="physicsHandler"></param>
        public void BeforePhysicsUpdate(BaseScene scene, PhysicsHandler physicsHandler);

        /// <summary>
        /// Called after the physics update
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="physicsHandler"></param>
        public void AfterPhysicsUpdate(BaseScene scene, PhysicsHandler physicsHandler);

        /// <summary>
        /// Called before the scene is getting rendered
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public void BeforeRender(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera);

        /// <summary>
        /// Called after the scene is getting rendered
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        public void AfterRender(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera);

        /// <summary>
        /// Called when the scene is getting disposed
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="renderer"></param>
        public void OnDispose(BaseScene scene, IRenderDevice renderer);
    }
}
