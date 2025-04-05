using LibGFX.Core;
using LibGFX.Graphics;
using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.UI
{
    /// <summary>
    /// Represents the different types of control events.
    /// </summary>
    public enum ControlEventType
    {
        MouseEnter,
        MouseLeave,
        MouseDown,
        MouseUp,
        MouseMove,
        MouseWheel,
        KeyDown,
        KeyUp
    }

    /// <summary>
    /// Represents the event arguments for control events.
    /// </summary>
    public struct ControlEventArgs
    {
        public Vector2 Position;
        public ControlEventType Event;
    }

    /// <summary>
    /// Delegate for control events.
    /// </summary>
    /// <param name="control"></param>
    /// <param name="e"></param>
    public delegate void ControlEventHandler(Control control, ControlEventArgs e);

    /// <summary>
    /// Represents a abstract control for rendering UI elements.
    /// </summary>
    public abstract class Control
    {
        /// <summary>
        /// The name of the control
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The transform of the control
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// The render target of the control
        /// </summary>
        public RenderTarget RenderTarget { get; set; }

        /// <summary>
        /// Hovered state of the control
        /// </summary>
        public bool Hovered { get; set; } = false;

        /// <summary>
        /// Event triggered when the mouse enters the control.
        /// </summary>
        public event ControlEventHandler OnMouseEnter;

        /// <summary>
        /// Event triggered when the mouse leaves the control.
        /// </summary>
        public event ControlEventHandler OnMouseLeave;

        /// <summary>
        /// Event triggered when the mouse is pressed on the control.
        /// </summary>
        public event ControlEventHandler OnMouseDown;

        /// <summary>
        /// Checks if the control contains a point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vector2 point)
        {
            var controlPosition = TranslatePosition(this.Transform.Position.Xy);

            return point.X >= controlPosition.X && point.X <= controlPosition.X + Transform.Scale.X &&
                   point.Y >= controlPosition.Y && point.Y <= controlPosition.Y + Transform.Scale.Y;
        }

        /// <summary>
        /// Translates the position of the control to the screen coordinates.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 TranslatePosition(Vector2 point)
        {
            return new Vector2(point.X - (Transform.Scale.X / 2), point.Y - (Transform.Scale.Y / 2));
        }

        /// <summary>
        /// Disposes the events of the control.
        /// </summary>
        protected virtual void DisposeEvents()
        {
            this.OnMouseEnter = null;
            this.OnMouseLeave = null;
            this.OnMouseDown = null;
        }

        /// <summary>
        /// Raises the mouse event for the control.
        /// </summary>
        /// <param name="mousePostion"></param>
        /// <param name="type"></param>
        protected virtual void RaiseMouseEvent(Vector2 mousePostion, ControlEventType type)
        {
            switch (type)
            {
                case ControlEventType.MouseEnter:
                    this.OnMouseEnter?.Invoke(this, new ControlEventArgs()
                    {
                        Position = mousePostion,
                        Event = type
                    });
                    break;
                case ControlEventType.MouseLeave:
                    this.OnMouseLeave?.Invoke(this, new ControlEventArgs()
                    {
                        Position = mousePostion,
                        Event = type
                    });
                    break;
                case ControlEventType.MouseDown:
                    this.OnMouseDown?.Invoke(this, new ControlEventArgs()
                    {
                        Position = mousePostion,
                        Event = type
                    });
                    break;
                case ControlEventType.MouseUp:
                    break;
                case ControlEventType.MouseMove:
                    break;
                case ControlEventType.MouseWheel:
                    break;
                case ControlEventType.KeyDown:
                    break;
                case ControlEventType.KeyUp:
                    break;
                default:
                    break;
            }
        }

        public abstract void Init(IRenderDevice renderer, Canvas canvas);
        public abstract void Update(Canvas canvas, Window window);
        public abstract void Render(IRenderDevice renderer, Canvas canvas);
        public abstract void Dispose(IRenderDevice renderer, Canvas canvas);
    }
}
