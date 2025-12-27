using Assimp;
using LibGFX.Core;
using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
using Newtonsoft.Json.Linq;
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
        public Guid ID { get; private set; } = Guid.NewGuid();
        public Vector4 VertexColor { get; set; }
        public bool Hovered { get; set; } = false;
        public bool IsInitialized { get; private set; } = false;

        public void Deserialize(JObject jObject, SerializationContext serializationContext)
        {
            throw new NotImplementedException();
        }

        public void Dispose(IRenderDevice renderDevice)
        {
            this.IsInitialized = false;
        }

        public void Init(IRenderDevice renderDevice)
        {
            this.IsInitialized = true;
        }

        public JObject Serialize(SerializationContext serializationContext)
        {
            throw new NotImplementedException();
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

        void IMaterial.LoadMaterial(Material asmat, string directory)
        {
            throw new NotImplementedException();
        }
    }
}
