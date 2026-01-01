using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class DepthInstancedShader3D : RenderShader
    {
        public DepthInstancedShader3D()
        {
            this.VertexShader = new Shader(@"
                #version 430 core
                layout (location = 0) in vec3 inPosition;

                layout(binding = 0, std430, row_major) buffer matrixBuffer {
                    mat4 modelMatrices[];
                };

                layout(std430, binding = 1) buffer extrasBuffer {
                    vec4 extraBuffer[];
                };

                uniform mat4 p_mat;
                uniform mat4 v_mat;
                uniform mat4 mesh_matrix;

                void main()
                {
                    mat4 m_mat = mesh_matrix * modelMatrices[gl_InstanceID]; 
                    mat4 mvp = m_mat * v_mat * p_mat;

                    gl_Position = vec4(inPosition, 1.0) * mvp;
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 430 core

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
