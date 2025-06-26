using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class DepthMeshShader : ShaderProgram
    {
        public DepthMeshShader()
        {
            this.VertexShader = new Shader(@"
                #version 330 core
                layout (location = 0) in vec3 inPosition;

                uniform mat4 p_mat;
                uniform mat4 v_mat;
                uniform mat4 m_mat;

                void main()
                {
                    mat4 mvp = m_mat*v_mat*p_mat;
                    gl_Position = vec4(inPosition, 1.0) * mvp;
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 330 core

                out vec4 fragColor;

                void main()
                { 
                    // Debug
                    // fragColor = vec4(1.0, 1.0, 1.0, 1.0); // Uncomment for debugging
                    // gl_FragDepth = gl_FragCoord.z;
                }
            ");
        }
    }
}
