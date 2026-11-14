using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class ShaderScope : IDisposable
    {
        private readonly IRenderDevice _renderDevice;
        private int _previousShaderID;
        private bool _disposed;

        public ShaderScope(IShaderProgram shaderProgram, IRenderDevice renderDevice)
        {
            _renderDevice = renderDevice;
            _previousShaderID = _renderDevice.GetCurrentShaderProgramId();
            _renderDevice.BindShaderProgram(shaderProgram.ProgramID);
        }

        public void Dispose()
        {
            if(!_disposed)
            {
                _disposed = true;
                _renderDevice.BindShaderProgram(_previousShaderID);
            }   
        }
    }
}
