using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class LineShader : ShaderProgram
    {
        public LineShader()
        {
            this.VertexShader = new Shader(@"
                #version 410 core
                layout(location = 0) in vec3 inPosition;

                uniform mat4 p_mat;
                uniform mat4 v_mat;

                void main()
                {
                    gl_Position = p_mat * v_mat * vec4(inPosition, 1.0);
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 410 core

                uniform vec4 vertexColor;
                out vec4 fragColor;

                void main()
                {
                    fragColor = vertexColor; // vec4(0.0, 1.0, 0.0, 1.0)
                }
            ");
        }
    }
}
