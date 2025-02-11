using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class RectShader : ShaderProgram
    {
        public RectShader()
        {
            this.VertexShader = new Shader(@"
                #version 410 core
                layout(location = 0) in vec3 inPosition;

                out vec3 position;

                uniform mat4 p_mat;
                uniform mat4 v_mat;
                uniform mat4 m_mat;
                
                void main() {
                    mat4 mvp = p_mat * v_mat * m_mat;
                    gl_Position = mvp * vec4(inPosition, 1.0);
                    position = inPosition;
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 410 core

                out vec4 fragColor; 
                in vec3 position;

                uniform vec4 vertexColor;
                uniform float aspect;
                uniform float borderWidth;
                uniform bool wireframe;

                void main() {
                    if(wireframe == false) {
                        fragColor = vertexColor;
                    }
                    else 
                    {
                        float bw = (borderWidth / 100) * aspect;
                        float maxX = 0.5 - bw / aspect;
                        float minX = -0.5 + bw / aspect;
                        float maxY = 0.5 - bw;
                        float minY = -0.5 + bw;

                        if (position.x < maxX && position.x > minX && position.y < maxY && position.y > minY) {
                            discard;
                        } else {
                            fragColor = vertexColor;
                        }  
                    }
                }
            ");
        }
    }
}
