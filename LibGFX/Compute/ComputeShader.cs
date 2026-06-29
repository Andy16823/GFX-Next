using LibGFX.Assets;
using LibGFX.Core;
using LibGFX.Graphics;
using LibGFX.Graphics.Shader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Compute
{
    /// <summary>
    /// Represents a compute shader program, including its source code and execution parameters.
    /// </summary>
    /// <remarks>Use this class to manage the source and configuration of a compute shader for GPU-based
    /// parallel processing tasks. The properties provide access to the shader's source code, the number of invocations,
    /// and the program identifier required for execution within a graphics or compute API.</remarks>
    public class ComputeShader : IShaderProgram, IIdentifier, IAsset
    {
        /// <summary>
        /// Name of the compute shader.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Unique identifier for the compute shader.
        /// </summary>
        public Guid ID { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the source code of the shader program.
        /// </summary>
        public String ShaderSource { get; set; }

        /// <summary>
        /// Gets or sets the number of times the associated operation has been invoked.
        /// </summary>
        public int Invocations { get; set; }

        /// <summary>
        /// Gets a value indicating whether the object has been successfully initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Program ID of the compute shader.
        /// </summary>
        public int ProgramID { get; set; }

        /// <summary>
        /// Disposes of the resources used by the compute shader.
        /// </summary>
        /// <param name="renderer"></param>
        public void Dispose(IRenderDevice renderer)
        {
            renderer.DisposeComputeShader(this);    
            this.IsInitialized = false;
        }

        /// <summary>
        /// Initializes the component using the specified render device.
        /// </summary>
        /// <param name="renderer">The render device to be used for initialization. Cannot be null.</param>
        public void Init(IRenderDevice renderer)
        {
            renderer.BuildComputeShader(this);
            this.IsInitialized = true;
        }

        public void FreeCPUResources()
        {
            // Nothing to free for now.
        }
    }
}
