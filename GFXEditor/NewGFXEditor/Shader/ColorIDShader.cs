using LibGFX.Graphics.Shader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGFX.Graphics;

namespace NewGFXEditor.Shader
{
    public class ColorIDShader : ShaderProgram
    {
        public ColorIDShader()
        {
            this.VertexShader = new LibGFX.Graphics.Shader.Shader(@"
                #version 330 core

                layout(location = 0) in vec3 inPosition;

                uniform mat4 p_mat;
                uniform mat4 v_mat;
                uniform mat4 m_mat;

                void main() {
                    mat4 mvp = m_mat*v_mat*p_mat;
                    gl_Position = vec4(inPosition, 1.0) * mvp;
                }
            ");

            this.FragmentShader = new LibGFX.Graphics.Shader.Shader(@"
                #version 330 core
                
                out vec4 fragColor;
                in vec3 position;

                uniform vec4 colorId;

                void main() {

                    fragColor = colorId;
                }
            ");
        }
    }
}
