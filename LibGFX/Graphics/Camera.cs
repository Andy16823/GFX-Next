using LibGFX.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public abstract class Camera : GameElement
    {
        /// <summary>
        /// The current camera
        /// </summary>
        public static Camera Current { get; set; }

        /// <summary>
        /// The near plane of the camera
        /// </summary>
        public float Near { get; set; }

        /// <summary>
        /// The far plane of the camera
        /// </summary>
        public float Far { get; set; }

        /// <summary>
        /// Gets the view matrix of the camera
        /// </summary>
        /// <returns></returns>
        public abstract Matrix4 GetViewMatrix();

        /// <summary>
        /// Gets the projection matrix of the camera
        /// </summary>
        /// <param name="viewport"></param>
        /// <returns></returns>
        public abstract Matrix4 GetProjectionMatrix(Viewport viewport);

        /// <summary>
        /// Gets the aspect ratio of the camera
        /// </summary>
        /// <returns></returns>
        public float GetAspectRatio()
        {
            return this.Transform.Scale.X / this.Transform.Scale.Y;
        }

        /// <summary>
        /// Sets the camera as the current camera
        /// </summary>
        public void SetAsCurrent()
        {
            Current = this;
        }
    }
}
