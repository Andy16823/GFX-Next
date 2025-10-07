using LibGFX.Core;
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
    /// <summary>
    /// Represents a abstract canvas for rendering UI elements.
    /// </summary>
    public abstract class Canvas
    {
        /// <summary>
        /// The name of the canvas
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The camera of the canvas
        /// </summary>
        public Camera Camera { get; set; }

        /// <summary>
        /// The transform of the canvas
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// The controls of the canvas
        /// </summary>
        public Dictionary<String, Control> Controls { get; set; }

        /// <summary>
        /// The render target of the canvas
        /// </summary>
        public RenderTarget2D RenderTarget { get; set; }

        /// <summary>
        /// The list of render actions to be performed during the render phase
        /// </summary>
        private readonly List<Action<Viewport, IRenderDevice>> _renderActions = new();

        /// <summary>
        /// Adds a render action to be performed during the render phase
        /// This action will be discarded after being executed once
        /// </summary>
        /// <param name="action"></param>
        public void AddRenderAction(Action<Viewport, IRenderDevice> action)
        {
            _renderActions.Add(action);
        }

        /// <summary>
        /// Processes and executes all render actions added to the canvas and then clears the list
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public void ProcessRenderActions(Viewport viewport, IRenderDevice renderer)
        {
            foreach (var action in _renderActions)
            {
                action(viewport, renderer);
            }
            _renderActions.Clear();
        }

        /// <summary>
        /// Adds a control to the canvas
        /// </summary>
        /// <param name="control"></param>
        public void AddControl(Control control)
        {
            this.Controls.Add(control.Name, control);
        }

        /// <summary>
        /// Removes a control from the canvas
        /// </summary>
        /// <param name="name"></param>
        public void RemoveControl(String name)
        {
            this.Controls.Remove(name);
        }

        /// <summary>
        /// Gets a control from the canvas
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Control GetControl(String name)
        {
            return this.Controls[name];
        }

        /// <summary>
        /// Gets a control from the canvas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetControl<T>(String name) where T : Control
        {
            return (T)this.Controls[name];
        }

        /// <summary>
        /// Gets a control from the canvas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetControl<T>() where T : Control
        {
            return (T)this.Controls.Values.FirstOrDefault(c => c is T);
        }

        /// <summary>
        /// Initializes the canvas
        /// </summary>
        /// <param name="renderer"></param>
        public abstract void Init(IRenderDevice renderer);

        /// <summary>
        /// Renders the canvas
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="renderer"></param>
        public abstract void Render(Viewport viewport, IRenderDevice renderer);

        /// <summary>
        /// Updates the canvas
        /// </summary>
        public abstract void Update(Window window);

        /// <summary>
        /// Disposes the canvas
        /// </summary>
        /// <param name="renderer"></param>
        public abstract void Dispose(IRenderDevice renderer);

        /// <summary>
        /// Gets the mouse position in the canvas
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public abstract Vector2 GetMousePosition(Window window);
    }
}
