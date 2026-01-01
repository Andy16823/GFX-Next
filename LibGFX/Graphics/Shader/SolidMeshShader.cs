using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    /// <summary>
    /// Solid Mesh Shader Program
    /// </summary>
    public class SolidMeshShader : RenderShader
    {
        public SolidMeshShader()
        {
            this.VertexShader = new Shader(@"
                #version 430 core
                layout(location = 0) in vec3 inPosition;
                uniform mat4 p_mat;
                uniform mat4 v_mat;
                uniform mat4 m_mat;
                void main() {
                    mat4 mvp = m_mat*v_mat*p_mat;
                    gl_Position = vec4(inPosition, 1.0) * mvp;
                }
            ");
            this.FragmentShader = new Shader(@"
                #version 430 core
                out vec4 fragColor;
                uniform vec4 solidColor;
                void main() {
                    fragColor = solidColor;
                }
            ");
        }
    }
}
