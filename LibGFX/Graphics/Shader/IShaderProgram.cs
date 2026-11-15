using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    /// <summary>
    /// Basic interface for shader programs.
    /// </summary>
    public interface IShaderProgram
    {
        /// <summary>
        /// The ID of the shader program.
        /// </summary>
        int ProgramID { get; set; }

        /// <summary>
        /// Starts a shader scope for rendering operations, returning a ShaderScope instance that manages the lifecycle of the shader usage.
        /// </summary>
        /// <param name="renderDevice">The render device to be used. Cannot be null.</param>
        /// <returns>A ShaderScope instance that manages the lifecycle of the shader usage.</returns>
        public ShaderScope Begin(IRenderDevice renderDevice);
    }
}
