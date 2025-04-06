using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class EnviromentShader : ShaderProgram
    {
        public EnviromentShader()
        {
            this.VertexShader = new Shader(@"
                #version 410 core
                layout (location = 0) in vec3 inPosition;

                out vec3 TexCoords;

                uniform mat4 p_mat;
                uniform mat4 v_mat;
                uniform mat4 m_mat;

                void main()
                {
                    TexCoords = inPosition;
                    gl_Position = p_mat * v_mat * m_mat * vec4(inPosition, 1.0);
                }  
            ");

            this.FragmentShader = new Shader(@"
                #version 410 core
                out vec4 fragColor;

                in vec3 TexCoords;

                uniform samplerCube skybox;

                void main()
                {    
                    // Invertiere die Y-Koordinate
                    vec3 invertedTexCoords = TexCoords;
                    invertedTexCoords.y = -TexCoords.y;
                    fragColor = texture(skybox, invertedTexCoords);
                }
            ");
        }
    }
}
