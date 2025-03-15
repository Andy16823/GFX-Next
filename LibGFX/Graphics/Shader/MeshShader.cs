using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class MeshShader : ShaderProgram
    {
        public MeshShader()
        {
            this.VertexShader = new Shader(@"
                #version 410 core
                layout(location = 0) in vec3 inPosition;
                layout(location = 1) in vec2 inTexCoord;
                layout(location = 2) in vec3 inNormal;

                out vec3 position;
                out vec3 normal;
                out vec2 texCoord;

                uniform mat4 p_mat;
                uniform mat4 v_mat;
                uniform mat4 m_mat;
                
                void main() {
                    mat4 mvp = p_mat * v_mat * m_mat;
                    gl_Position = mvp * vec4(inPosition, 1.0);
                    position = inPosition;
                    normal = inNormal;
                    texCoord = inTexCoord;
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 410 core

                out vec4 FragColor; 
                
                in vec3 position;
                in vec3 normal;
                in vec2 texCoord;

                uniform vec4 vertexColor;
                uniform sampler2D textureSampler;

                void main()
                { 
                    vec4 texColor = texture(textureSampler, texCoord);
                    //texColor.rgb = pow(texColor.rgb, vec3(1.0 / gamma));
                    FragColor = texColor;
                }
            ");
        }
    }
}
