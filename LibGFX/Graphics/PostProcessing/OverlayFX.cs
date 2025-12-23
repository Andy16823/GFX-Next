using LibGFX.Graphics.Shader;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.PostProcessing
{
    internal class OverlayFX : IPostProcessFilter
    {
        public Vector4 Color { get; set; }
        private ShaderProgram _shader;

        public OverlayFX()
        {
            _shader = new OverlayFXShader();
        }

        public void Apply(PostProcessStack stack, IRenderDevice renderer, int sourceTexture)
        {
            renderer.BindShaderProgram(_shader);
            renderer.PrepareShader("overlayColor", Color);
            renderer.PrepareShader("sourceTexture", 0, sourceTexture);
            renderer.DrawFullScreenQuad();
        }

        public void Init(PostProcessStack stack, Viewport viewport, IRenderDevice renderer)
        {
            renderer.BuildShaderProgram(_shader);
        }

        public void Dispose(PostProcessStack stack, IRenderDevice renderer)
        {
            renderer.DisposeShaderProgram(_shader);
        }
    }
}
