using LibGFX.Graphics.Shader;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Materials
{
    public enum MaterialFlags
    {
        None,
        Loaded,
        Disposed,
        Failed
    }

    public interface IMaterial
    {
        public string Name { get; set; }
        public MaterialFlags Flags { get; set; }
        public void Init(IRenderDevice renderDevice);
        public void Use(IRenderDevice renderDevice);
        public void Dispose(IRenderDevice renderDevice);
    }
}
