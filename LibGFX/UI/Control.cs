using LibGFX.Core;
using LibGFX.Graphics;
using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.UI
{
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

        public abstract void Init(IRenderDevice renderer, Canvas canvas);
        public abstract void Update(Canvas canvas, Window window);
        public abstract void Render(IRenderDevice renderer, Canvas canvas);
        public abstract void Dispose(IRenderDevice renderer, Canvas canvas);


    }
}
