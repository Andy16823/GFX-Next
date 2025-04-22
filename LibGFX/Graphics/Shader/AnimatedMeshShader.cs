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
		
                    mat4 viewModel = m_mat * v_mat;
                    gl_Position = totalPosition * viewModel * p_mat;
                    position = vec3(vec4(pos, 1.0) * m_mat);
                    normal = norm * transpose(inverse(mat3(m_mat)));
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
                uniform vec3 viewPos;

                struct Light {
                    vec3 lightPos;
                    vec3 lightColor;
                    float lightIntensity;
                    vec3 ambient;
                    vec3 specular;
                };
                uniform Light light;


                struct Material {
                    sampler2D textureSampler;
                    sampler2D normalSampler;
                    vec4 vertexColor;
                };
                uniform Material material;

                void main()
                {
                    vec3 norm = normalize(normal);
                    vec3 lightDir = normalize(light.lightPos - position);
                    float diff = max(dot(norm, lightDir), 0.0);

                    vec3 viewDir = normalize(viewPos - position);
                    vec3 reflectDir = reflect(-lightDir, norm);
                    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0);

                    // Calculate ambient lighting
                    vec3 ambient = light.ambient * vec3(texture(material.textureSampler, texCoord));
                    vec3 diffuse = light.lightColor  * diff * vec3(texture(material.textureSampler, texCoord)); 
                    vec3 specular = light.specular * spec;

                    fragColor = vec4(ambient + diffuse + specular, 1.0); 
                    //fragColor = vec4(ambient + diffuse + specular, 1.0) * material.vertexColor;
                }
            ");
        }
    }
}
