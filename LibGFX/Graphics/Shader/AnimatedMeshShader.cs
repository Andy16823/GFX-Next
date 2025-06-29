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
                uniform mat4 lightSpaceMatrix;
	
                const int MAX_BONES = 100;
                const int MAX_BONE_INFLUENCE = 4;
                uniform mat4 finalBonesMatrices[MAX_BONES];
	
                out vec2 texCoord;
                out vec4 tangent;  
                out vec3 position;
                out vec3 normal;
                out vec4 fragPosLightSpace;
	
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
                    position = vec3(totalPosition * m_mat);
                    normal = norm * transpose(inverse(mat3(m_mat)));
                    texCoord = tex;
                    tangent = tan;
                    fragPosLightSpace = vec4(position, 1.0) * lightSpaceMatrix;
                }
            ");

            //Creating the fragment shader
            this.FragmentShader = new Shader(@"
                #version 430 core

                in vec3 position;
                in vec3 normal;
                in vec2 texCoord;
                in vec4 tangent;
                in vec4 fragPosLightSpace;

                out vec4 fragColor;
                uniform vec3 viewPos;
                uniform sampler2D shadowMap;

                struct DirLight {
                    vec3 direction;
                    vec3 lightColor;
                    float lightIntensity;
                    vec3 ambient;
                    vec3 specular;
                };
                uniform DirLight dirLight;

                struct PointLight {
                    vec4 position;
                    vec4 constantLinearQuadratic;
                    vec4 ambient;
                    vec4 diffuse;
                    vec4 specular;
                };
                layout(std430, binding = 4) buffer PointLightsBuffer {
                    PointLight pointLights[];
                };

                struct Material {
                    sampler2D textureSampler;
                    sampler2D normalSampler;
                    sampler2D specularSampler;
                    vec4 vertexColor;
                    float shininess;
                    bool flipNormal;
                    vec2 uvScale;
                };
                uniform Material material;

                mat3 getTBN(vec4 tangent, vec3 normal, bool flipnormal) {
                   if (flipnormal == false) {
                        normal = -normal;
                    }
                    vec3 T = normalize(tangent.xyz);
                    vec3 N = normalize(normal);
                    vec3 B = cross(N, T)*tangent.w;
                    mat3 TBN = mat3(T, B, N);
                    return TBN;
                }

                vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir, float shadow, vec2 texCoords)
                {
                    vec3 lightDir = normalize(-light.direction);
                    // diffuse shading
                    float diff = max(dot(normal, lightDir), 0.0);
                    // specular shading
                    vec3 reflectDir = reflect(lightDir, normal);
                    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
                    // combine results
                    vec3 ambient  = light.ambient * vec3(texture(material.textureSampler, texCoords));
                    vec3 diffuse  = light.lightColor * diff * vec3(texture(material.textureSampler, texCoords));
                    vec3 specular = light.specular * spec * vec3(texture(material.specularSampler, texCoords));

                    vec3 lightning = (ambient + (1.0 -shadow) * (diffuse + specular));
                    return lightning;
                }  

                vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir, vec2 texCoords)
                {
                    vec3 lightPos = light.position.xyz;
                    float constant = light.constantLinearQuadratic.x;
                    float linear = light.constantLinearQuadratic.y;
                    float quadratic = light.constantLinearQuadratic.z;
                    vec3 lambient = light.ambient.xyz;
                    vec3 ldiffuse = light.diffuse.xyz;
                    vec3 lspecular = light.specular.xyz;

                    vec3 lightDir = normalize(-(lightPos - fragPos));
                    // diffuse shading
                    float diff = max(dot(normal, lightDir), 0.0);
                    // specular shading
                    vec3 reflectDir = reflect(lightDir, normal);
                    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
                    // attenuation
                    float distance = length(lightPos - fragPos);
                    float attenuation = 1.0 / (constant + linear * distance + quadratic * (distance * distance));    
                    // combine results
                    vec3 ambient  = lambient * vec3(texture(material.textureSampler, texCoords));
                    vec3 diffuse  = ldiffuse  * diff * vec3(texture(material.textureSampler, texCoords));
                    vec3 specular = lspecular * spec * vec3(texture(material.specularSampler, texCoords));
                    ambient  *= attenuation;
                    diffuse  *= attenuation;
                    specular *= attenuation;
                    return (ambient + diffuse + specular);
                } 

                float ShadowCalculation(vec4 fragPosLightSpace, vec3 normal, DirLight light) {
                    vec3 lightDir = normalize(-light.direction);
                    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
                    projCoords = projCoords * 0.5 + 0.5;
                    float closestDepth = texture(shadowMap, projCoords.xy).r; 
                    float currentDepth = projCoords.z;
                    float bias = max(0.05 * (1.0 - dot(normal, lightDir)), 0.005);  
                    float shadow = 0.0;
                    vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
                    for(int x = -1; x <= 1; ++x)
                    {
                        for(int y = -1; y <= 1; ++y)
                        {
                            float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r; 
                            shadow += currentDepth - bias > pcfDepth ? 1.0 : 0.0;        
                        }    
                    }
                    shadow /= 9.0;
                    
                    if(projCoords.z > 1.0)
                            shadow = 0.0;

                    return shadow;
                }

                void main() {
                    vec2 localUV = fract(texCoord * material.uvScale);

                    mat3 TBN = getTBN(tangent, normal, material.flipNormal);
                    vec3 normalMap = texture(material.normalSampler, localUV).rgb;
                    normalMap = normalMap*2.0-1.0;
                    vec3 norm = normalize(TBN*normalMap);
                    vec3 viewDir = normalize(viewPos-position);

                    float shadow = ShadowCalculation(fragPosLightSpace, normal, dirLight);
                    vec3 result = CalcDirLight(dirLight, norm, viewDir, shadow, localUV);
                    for (int i = 0; i < pointLights.length(); i++) {
                        result += CalcPointLight(pointLights[i], norm, position, viewDir, localUV);
                    } 

                    float alpha = texture(material.textureSampler, localUV).a;
                    result *= material.vertexColor.rgb;
                    alpha *= material.vertexColor.a;

                    fragColor = vec4(result, alpha);
                }
            ");
        }
    }
}
