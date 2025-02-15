using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class FontShader : ShaderProgram
    {
        public FontShader()
        {
            this.VertexShader = new Shader(@"
                #version 330 core
                layout (location = 0) in vec4 vertex;
                out vec2 TexCoords;

                uniform mat4 p_mat;

                void main()
                {
                    gl_Position = p_mat * vec4(vertex.xy, 0.0, 1.0);
                    TexCoords = vertex.zw;
                }  
            ");

            this.FragmentShader = new Shader(@"
                #version 330 core
                in vec2 TexCoords;
                out vec4 color;

                uniform sampler2D textureSampler;
                uniform vec4 vertexColor;

                void main()
                {    
                    vec4 sampled = vec4(1.0, 1.0, 1.0, texture(textureSampler, TexCoords).r);
                    color = vertexColor * sampled;
                }
            ");
        }
    }
}
