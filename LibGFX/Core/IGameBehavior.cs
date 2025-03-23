using LibGFX.Graphics;
using LibGFX.Pyhsics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    public interface IGameBehavior
    {
        void SetElement(GameElement gameElement);
        GameElement GetElement();
        void OnInit(BaseScene scene, Viewport viewport, IRenderDevice renderer);
        void OnRender(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera);
        void OnUpdate(BaseScene scene);
        void OnDispose(BaseScene scene, IRenderDevice renderer);
        void OnCollide(Collision collision);
    }
}
