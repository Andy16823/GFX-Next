using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGFX.Graphics;
using LibGFX.Math;
using LibGFX.Physics;

namespace LibGFX.Core.Behaviors
{
    public class CullingBehavior : IGameBehavior
    {
        private GameElement _gameElement;


        public void SetElement(GameElement gameElement)
        {
            _gameElement = gameElement;
        }

        public GameElement GetElement()
        {
            return _gameElement;
        }

        public void OnInit(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            
        }

        public void OnShadowPass(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {

        }

        public void OnRender(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera)
        {

        }

        public void OnUpdate(BaseScene scene, float dt)
        {
            var camera = Camera.Current;
            var window = GFX.Instance.GetWindow();
            if(camera != null && window != null)
            {
                var frustum = camera.GetFrustum(window.GetViewport());
                var aabb = _gameElement.WorldAABB;
                if(Frustum.IntersectsAABB(frustum, aabb.Min, aabb.Max))
                {
                    _gameElement.Visible = true;
                }
                else
                {
                    _gameElement.Visible = false;
                }
            }
        }

        public void OnDispose(BaseScene scene, IRenderDevice renderer)
        {

        }

        public void OnCollide(Collision collision)
        {

        }
    }
}
