using LibGFX.Graphics;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    public abstract class GameElement
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public bool Visible { get; set; } = true;
        public bool Enabled { get; set; } = true;
        public List<IGameBehavior> Behaviors { get; set; }

        protected GameElement()
        {
            this.Behaviors = new List<IGameBehavior>();
        }

        public virtual void Init(BaseScene scene, Viewport viewport, IRenderDevice renderer)
        {
            this.Behaviors.ForEach(behavior => {
                behavior.OnInit(scene, viewport, renderer);
            });
        }

        public virtual void Render(BaseScene scene, Viewport viewport, IRenderDevice renderer, Camera camera) 
        {
            if (this.Visible)
            {
                this.Behaviors.ForEach(b => {
                    b.OnRender(scene, viewport, renderer, camera);
                });
            }
        }

        public virtual void Update(BaseScene scene) 
        {
            if (this.Enabled)
            {
                this.Behaviors.ForEach(b =>
                {
                    b.OnUpdate(scene);
                });
            }
        }

        public virtual void Dispose(BaseScene scene, IRenderDevice renderer)
        {
            this.Behaviors.ForEach(b =>
            {
                b.OnDispose(scene, renderer);
            });
        }

        public T AddBehavior<T>(T behavior) where T : IGameBehavior
        {
            this.Behaviors.Add(behavior);
            behavior.SetElement(this);
            return behavior;
        }
    }
}
