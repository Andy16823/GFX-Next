using LibGFX.Graphics;
using LibGFX.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    /// <summary>
    /// Represents a game behavior interface
    /// </summary>
    public interface IGameBehavior : ICloneable<IGameBehavior>
    {
        /// <summary>
        /// Sets the game element
        /// </summary>
        /// <param name="gameElement"></param>
        void SetElement(GameElement gameElement);

        /// <summary>
        /// Gets the game element
        /// </summary>
        /// <returns></returns>
        GameElement GetElement();

        /// <summary>
        /// Initializes the game behavior
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        void OnInit(BaseScene scene, Viewport viewport, IRenderDevice renderer);

        /// <summary>
        /// Called when the shadow pass is rendered
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        void OnShadowPass(BaseScene scene, Viewport viewport, IRenderDevice renderer);

        /// <summary>
        /// Called when the game behavior is rendered
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        void OnRender(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera);

        /// <summary>
        /// Called when the game behavior is updated
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="dt"></param>
        void OnUpdate(BaseScene scene, float dt);

        /// <summary>
        /// Called when the game behavior is disposed
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="renderer"></param>
        void OnDispose(BaseScene scene, IRenderDevice renderer);

        /// <summary>
        /// Called when the game behavior collides with another object
        /// </summary>
        /// <param name="collision"></param>
        void OnCollide(Collision collision);
    }
}
