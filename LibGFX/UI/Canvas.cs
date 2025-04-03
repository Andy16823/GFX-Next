using LibGFX.Graphics;
using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.UI
{
    public abstract class Canvas
    {
        public String Name { get; set; }
        public Camera Camera { get; set; }
        public Transform Transform { get; set; }
        public Dictionary<String, Control> Controls { get; set; }
        public RenderTarget RenderTarget { get; set; }

        public void AddControl(Control control)
        {
            this.Controls.Add(control.Name, control);
        }

        public void RemoveControl(String name)
        {
            this.Controls.Remove(name);
        }

        public Control GetControl(String name)
        {
            return this.Controls[name];
        }

        public T GetControl<T>(String name) where T : Control
        {
            return (T)this.Controls[name];
        }

        public T GetControl<T>() where T : Control
        {
            return (T)this.Controls.Values.FirstOrDefault(c => c is T);
        }

        public abstract void Init(IRenderDevice renderer);
        public abstract void Render(Viewport viewport, IRenderDevice renderer);
        public abstract void Update();
        public abstract void Dispose(IRenderDevice renderer);
    }
}
