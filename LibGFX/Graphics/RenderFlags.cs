using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public class RenderFlags
    {
        [Flags] 
        public enum ClearFlags
        {
            None = 0,
            Color = 1,
            Depth = 2,
            Stencil = 4,
        }

        public enum TextureFilterMode
        {
            Nearest,
            Linear,
            MipmapNearest,
            MipmapLinear
        }

        public enum TextureWrapMode
        {
            ClampToEdge,
            Repeat,
            MirroredRepeat
        }
    }
}
