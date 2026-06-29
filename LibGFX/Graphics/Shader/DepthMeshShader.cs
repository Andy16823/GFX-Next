using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class DepthMeshShader : RenderShader
    {
        public DepthMeshShader()
        {
            this.VertexShader = new Shader(@"
                #version 460 core
                layout (location = 0) in vec3 inPosition;

                uniform mat4 p_mat;
                uniform mat4 v_mat;
                uniform mat4 m_mat;

                void main()
                {
                    gl_Position = vec4(inPosition, 1.0) * m_mat;
                }
            ");

            this.GeometryShader = new Shader(@"
                #version 460 core
                layout (triangles, invocations = 4) in;
                layout (triangle_strip, max_vertices = 3) out;

                layout (std140, binding = 3, row_major) uniform LightSpaceMatrices
                {
                    mat4 lightSpaceMatrices[16];
                };

                void main()
                {
                    for(int i = 0; i < 3; ++i)
                    {
                        gl_Position = gl_in[i].gl_Position * lightSpaceMatrices[gl_InvocationID]; // ✅
                        gl_Layer = gl_InvocationID; // Set the layer for the current invocation
                        EmitVertex();
                    }
                    EndPrimitive();
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 460 core

                out vec4 fragColor;

                void main()
                { 

                }
            ");
        }
    }
}
