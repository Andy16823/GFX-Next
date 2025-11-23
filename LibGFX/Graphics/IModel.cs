using LibGFX.Graphics.Animation3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public interface IModel
    {
        public Dictionary<string, Mesh> Meshes { get; set; }
        public AssimpNodeData NodeStructure { get; set; }

        public void Init(IRenderDevice renderer);
        public void Dispose(IRenderDevice renderer);
    }
}
