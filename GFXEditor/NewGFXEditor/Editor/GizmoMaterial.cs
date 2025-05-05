using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewGFXEditor.Editor
{
    public class GizmoMaterial : IMaterial
    {
        public string Name { get; set; }
        public MaterialFlags Flags { get; set; }
        public Vector4 VertexColor { get; set; }
        public bool Hovered { get; set; } = false;

        public void Dispose(IRenderDevice renderDevice)
        {
            this.Flags = MaterialFlags.Disposed;
        }

        public void Init(IRenderDevice renderDevice)
        {
            this.Flags = MaterialFlags.Loaded;
        }

        public void Use(IRenderDevice renderDevice)
        {
            if(this.Hovered)
            {
                renderDevice.PrepareShader("vertexColor", new Vector4(1, 1, 0, 1));
            }
            else
            {
                renderDevice.PrepareShader("vertexColor", this.VertexColor);
            }
        }
    }
}
