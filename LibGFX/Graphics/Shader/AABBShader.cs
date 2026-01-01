using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    /// <summary>
    /// AABB Shader Program
    /// Used to render Axis-Aligned Bounding Boxes
    /// </summary>
    public class AABBShader : RenderShader
    {
        public AABBShader()
        {
            this.VertexShader = new Shader(@"
                #version 430 core
                layout(location = 0) in vec3 inPosition;
                uniform mat4 p_mat;
                uniform mat4 v_mat;
                uniform mat4 m_mat;
                uniform vec3 min;
                uniform vec3 max;

                void main() {
                    vec3 box = (inPosition + vec3(1.0)) * 0.5;
                    vec3 scaledPosition = min + box * (max - min);
                    mat4 mvp = m_mat*v_mat*p_mat;
                    gl_Position = vec4(scaledPosition, 1.0) * mvp;
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
