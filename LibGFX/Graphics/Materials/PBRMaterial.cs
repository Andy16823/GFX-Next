using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Materials
{
    public class PBRMaterial : IMaterial
    {
        public string Name { get; set; }
        public MaterialFlags Flags { get; set; }
        public Vector3 Albedo { get; set; } = new Vector3(1, 1, 0);
        public float Metallic { get; set; } = 1.0f;
        public float Roughness { get; set; } = 0.25f;
        public float Occlusion { get; set; } = 1.0f;

        public void Dispose(IRenderDevice renderDevice)
        {
            this.Flags = MaterialFlags.Disposed;
        }

        public void Init(IRenderDevice renderDevice)
        {
            if (this.Flags == MaterialFlags.Loaded)
            {
                return;
            }
            this.Flags = MaterialFlags.Loaded;
        }

        public void Use(IRenderDevice renderDevice)
        {
            renderDevice.PrepareShader("albedo", Albedo);
            renderDevice.PrepareShader("metallic", Metallic);
            renderDevice.PrepareShader("roughness", Roughness);
            renderDevice.PrepareShader("ao", Occlusion);
        }
    }
}
