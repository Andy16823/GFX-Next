using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class AnimatedDepthMeshShader : RenderShader
    {
        public AnimatedDepthMeshShader()
        {
            this.VertexShader = new Shader(@"
                #version 430 core

                layout(location = 0) in vec3 pos;
                layout(location = 1) in vec2 tex;
                layout(location = 2) in vec3 norm;
                layout(location = 3) in vec4 tan;
                layout(location = 4) in ivec4 boneIds; 
                layout(location = 5) in vec4 weights;
	
                uniform mat4 p_mat;
                uniform mat4 v_mat;
                uniform mat4 m_mat;
	
                const int MAX_BONES = 100;
                const int MAX_BONE_INFLUENCE = 4;
                uniform mat4 finalBonesMatrices[MAX_BONES];
	
	
                void main()
                {
                    vec4 totalPosition = vec4(0.0f);
                    for(int i = 0 ; i < MAX_BONE_INFLUENCE ; i++)
                    {
                        if(boneIds[i] == -1) 
                            continue;
                        if(boneIds[i] >=MAX_BONES) 
                        {
                            totalPosition = vec4(pos,1.0f);
                            break;
                        }
                        vec4 localPosition =vec4(pos,1.0f) * finalBonesMatrices[boneIds[i]];
                        totalPosition +=  weights[i] * localPosition;
                        vec3 localNormal = norm * mat3(finalBonesMatrices[boneIds[i]]);
                    }
		
                    mat4 viewModel = m_mat * v_mat;
                    gl_Position = totalPosition * viewModel * p_mat;
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 330 core

                out vec4 fragColor;

                void main()
                { 
                    // Debug
                    // fragColor = vec4(1.0, 1.0, 1.0, 1.0); // Uncomment for debugging
                    // gl_FragDepth = gl_FragCoord.z;
                }
            ");
        }
    }
}
