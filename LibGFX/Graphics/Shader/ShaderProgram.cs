using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{

    /// <summary>
    /// Shader flags used to indicate the state of the shader program.
    /// </summary>
    public enum ShaderFlags
    {
        None,
        Loaded,
        Disposed,
        Failed
    }

    /// <summary>
    /// Represents a shader program that consists of a vertex shader and a fragment shader.
    /// </summary>
    public class ShaderProgram
    {
        /// <summary>
        /// Gets or sets the ID of the shader program.
        /// </summary>
        public int ProgramID { get; set; }

        /// <summary>
        /// Gets or sets the vertex shader associated with the program.
        /// </summary>
        public Shader VertexShader { get; set; }

        /// <summary>
        /// Gets or sets the fragment shader associated with the program.
        /// </summary>
        public Shader FragmentShader { get; set; }

        /// <summary>
        /// Gets or sets the shader flags.
        /// </summary>
        public ShaderFlags Flags { get; set; }

        /// <summary>
        /// Default constructor for the ShaderProgram class.
        /// </summary>
        public ShaderProgram()
        {
            this.VertexShader = new Shader();
            this.FragmentShader = new Shader();
        }
    }
}
