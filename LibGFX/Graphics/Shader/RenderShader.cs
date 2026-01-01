using LibGFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    /// <summary>
    /// Represents a shader program that consists of a vertex shader and a fragment shader.
    /// </summary>
    public class RenderShader : IShaderProgram, IGraphicsResource
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
        /// Gets a value indicating whether the object has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Default constructor for the ShaderProgram class.
        /// </summary>
        public RenderShader()
        {
            this.VertexShader = new Shader();
            this.FragmentShader = new Shader();
        }

        /// <summary>
        /// Initializes a new instance of the ShaderProgram class with the specified vertex and fragment shaders.
        /// </summary>
        /// <param name="vertexShader"></param>
        /// <param name="fragmentShader"></param>
        public RenderShader(Shader vertexShader, Shader fragmentShader)
        {
            this.VertexShader = vertexShader;
            this.FragmentShader = fragmentShader;
        }

        /// <summary>
        /// Initializes the shader program using the specified render device.
        /// </summary>
        /// <param name="renderer">The render device used to build and initialize the shader program. Cannot be null.</param>
        /// <exception cref="InvalidOperationException">Thrown if the shader program has already been initialized.</exception>
        public void Init(IRenderDevice renderer)
        {
            if(this.IsInitialized)
            {
                throw new InvalidOperationException("ShaderProgram is already initialized.");
            }
            renderer.BuildRenderShader(this);
            IsInitialized = true;
        }

        /// <summary>
        /// Releases the resources associated with this shader program using the specified render device.
        /// </summary>
        /// <param name="renderer">The render device used to dispose of the shader program. Cannot be null.</param>
        /// <exception cref="InvalidOperationException">Thrown if the shader program is not initialized.</exception>
        public void Dispose(IRenderDevice renderer)
        {
            if(!this.IsInitialized)
            {
                throw new InvalidOperationException("ShaderProgram is not initialized.");
            }
            renderer.DisposeRenderShader(this);
            IsInitialized = false;
        }
    }
}
