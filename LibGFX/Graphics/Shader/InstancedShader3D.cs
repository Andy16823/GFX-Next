using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class InstancedShader3D : ShaderProgram
    {
        public InstancedShader3D()
        {
            this.VertexShader = new Shader(@"
                #version 430 core

                layout(location = 0) in vec3 inPosition;
                layout(location = 1) in vec2 inTexCoord;
                layout(location = 2) in vec3 inNormal;
                layout(location = 3) in vec4 inTangent;

                layout(binding = 0, std430, row_major) buffer matrixBuffer {
                    mat4 modelMatrices[];
                };

                layout(std430, binding = 1) buffer extrasBuffer {
                    vec4 extraBuffer[];
                };

                out vec3 position;
                out vec3 normal;
                out vec2 texCoord;
                out vec4 tangent; 
                out vec4 extras;

                uniform mat4 p_mat;
                uniform mat4 v_mat;
                uniform mat4 mesh_matrix;

                void main() {
                    mat4 m_mat = mesh_matrix * modelMatrices[gl_InstanceID]; 
                    mat4 mvp = m_mat * v_mat * p_mat;
                    position = vec3(vec4(inPosition, 1.0) * m_mat);
                    normal = inNormal * transpose(inverse(mat3(m_mat)));
                    texCoord = inTexCoord;
                    tangent = inTangent;
                    extras = extraBuffer[gl_InstanceID];
                    gl_Position = vec4(inPosition, 1.0) * mvp;
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 430 core

                in vec3 position;
                in vec3 normal;
                in vec2 texCoord;
                in vec4 tangent;

                out vec4 fragColor;
                uniform vec3 viewPos;

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

                vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
                {
                    vec3 lightDir = normalize(-light.direction);
                    // diffuse shading
                    float diff = max(dot(normal, lightDir), 0.0);
                    // specular shading
                    vec3 reflectDir = reflect(lightDir, normal);
                    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
                    // combine results
                    vec3 ambient  = light.ambient * vec3(texture(material.textureSampler, texCoord));
                    vec3 diffuse  = light.lightColor * diff * vec3(texture(material.textureSampler, texCoord));
                    vec3 specular = light.specular * spec * vec3(texture(material.specularSampler, texCoord));
                    return (ambient + diffuse + specular);
                }  

                vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
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
                    vec3 ambient  = lambient * vec3(texture(material.textureSampler, texCoord));
                    vec3 diffuse  = ldiffuse  * diff * vec3(texture(material.textureSampler, texCoord));
                    vec3 specular = lspecular * spec * vec3(texture(material.specularSampler, texCoord));
                    ambient  *= attenuation;
                    diffuse  *= attenuation;
                    specular *= attenuation;
                    return (ambient + diffuse + specular);
                } 

                void main() {
                    
                    mat3 TBN = getTBN(tangent, normal, material.flipNormal);
                    vec3 normalMap = texture(material.normalSampler, texCoord).rgb;
                    normalMap = normalMap*2.0-1.0;
                    vec3 norm = normalize(TBN*normalMap);
                    vec3 viewDir = normalize(viewPos-position);

                    vec3 result = CalcDirLight(dirLight, norm, viewDir);
                    for (int i = 0; i < pointLights.length(); i++) {
                        result += CalcPointLight(pointLights[i], norm, position, viewDir);
                    } 

                    float alpha = texture(material.textureSampler, texCoord).a;
                    fragColor = vec4(result, alpha);
                }
            ");
        }
    }
}