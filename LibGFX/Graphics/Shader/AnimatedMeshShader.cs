using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class AnimatedMeshShader : ShaderProgram
    {
        public AnimatedMeshShader()
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
	
                out vec2 texCoord;
                out vec4 tangent;  
                out vec3 position;
                out vec3 normal;
	
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
                        vec4 localPosition = finalBonesMatrices[boneIds[i]] * vec4(pos,1.0f);
                        totalPosition += localPosition * weights[i];
                        vec3 localNormal = mat3(finalBonesMatrices[boneIds[i]]) * norm;
                    }
		
                    mat4 viewModel = v_mat * m_mat;
                    gl_Position =  p_mat * viewModel * totalPosition;
                    position = vec3(m_mat*vec4(pos, 1.0));
                    normal = transpose(inverse(mat3(m_mat)))*norm;
                    texCoord = tex;
                    tangent = tan;
                }
            ");

            //Creating the fragment shader
            this.FragmentShader = new Shader(@"
                #version 430 core

                in vec3 position;
                in vec3 normal;
                in vec2 texCoord;
                in vec4 tangent;

                out vec4 fragColor;
                uniform sampler2D textureSampler;
                uniform sampler2D normalSampler;
                uniform vec3 lightPos;
                uniform float lightIntensity;
                uniform vec3 lightColor;
                uniform vec3 viewPos;

                void main()
                {
                    // Sample the base color texture
                    vec3 color = texture(textureSampler, texCoord).rgb;

                    // Reconstruct the TBN matrix (Tangent, Bitangent, Normal)
                    vec3 T = normalize(tangent.xyz); // Extract Tangent (vec3 part of tangent)
                    vec3 N = normalize(normal); // Use the interpolated normal
                    vec3 B = cross(N, T) * tangent.w; // Compute Bitangent and flip if w < 0
                    mat3 TBN = mat3(T, B, N);

                    // Sample the normal map and transform to world space
                    vec3 normalMap = texture(normalSampler, texCoord).rgb;
                    normalMap = normalMap * 2.0 - 1.0; // Transform from [0,1] to [-1,1]
                    vec3 n_normal = normalize(TBN * normalMap);

                    // Calculate ambient lighting
                    vec3 ambient = lightIntensity*lightColor;

                    // Calculate diffuse lighting
                    vec3 lightDir = normalize(lightPos-position);
                    float diff = max(dot(lightDir, n_normal), 0.0);
                    vec3 diffuse = diff*lightColor;

                    // Calculate specular lighting
                    vec3 viewDir = normalize(viewPos-position);
                    float spec = 0.0;
                    vec3 halfwayDir = normalize(lightDir+viewDir);
                    spec = pow(max(dot(n_normal, halfwayDir), 0.0), 64.0);
                    vec3 specular = spec*lightColor;


                    vec3 result = (ambient + diffuse + specular) * color;
                    fragColor = vec4(result, 1.0);
                }
            ");
        }
    }
}
