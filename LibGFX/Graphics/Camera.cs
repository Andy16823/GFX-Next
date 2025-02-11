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
        public float Near { get; set; }
        public float Far { get; set; }

        public abstract Matrix4 GetViewMatrix();
        public abstract Matrix4 GetProjectionMatrix(Viewport viewport);
    }
}
