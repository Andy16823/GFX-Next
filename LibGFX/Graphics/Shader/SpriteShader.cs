using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class SpriteShader : ShaderProgram
    {
        public SpriteShader()
        {
            this.VertexShader = new Shader(@"
                #version 410 core
                layout(location = 0) in vec3 inPosition;
                layout(location = 1) in vec2 inTexCoord;

                out vec2 texCoord;
                out vec4 vColor;

                uniform mat4 p_mat;
                uniform mat4 v_mat;
                uniform mat4 m_mat;
                uniform vec4 vertexColor;

                void main() {
                    mat4 mvp = m_mat * v_mat * p_mat;
                    gl_Position = vec4(inPosition, 1.0) * mvp;
                    texCoord = inTexCoord;
                    vColor = vertexColor;
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 330 core

                in vec2 texCoord;
                in vec4 vColor;

                out vec4 fragColor; 

                uniform sampler2D textureSampler;
                uniform vec4 uvTransform;
                uniform vec2 uvScale;

                void main() {
                    vec2 localUV = fract(texCoord * uvScale);
                    vec2 transformedTexCoord = localUV * uvTransform.xy + uvTransform.zw;

                    fragColor = texture(textureSampler, transformedTexCoord) * vColor;
                }
            ");
        }
    }
}
