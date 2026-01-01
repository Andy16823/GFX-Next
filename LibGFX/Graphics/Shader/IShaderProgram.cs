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
    }
}
