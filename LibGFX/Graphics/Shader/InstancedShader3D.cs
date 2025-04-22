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
                in vec4 extras;

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

                void main() {

                    if(extras.x == 0.0) {
                        discard;
                    }

                    // Sample the base color texture
                    vec3 color = texture(material.textureSampler, texCoord).rgb;

                    // Reconstruct the TBN matrix (Tangent, Bitangent, Normal)
                    vec3 T = normalize(tangent.xyz); // Extract Tangent (vec3 part of tangent)
                    vec3 N = normalize(normal); // Use the interpolated normal
                    vec3 B = cross(N, T) * tangent.w; // Compute Bitangent and flip if w < 0
                    mat3 TBN = mat3(T, B, N);

                    // Sample the normal map and transform to world space
                    vec3 normalMap = texture(material.normalSampler, texCoord).rgb;
                    normalMap = normalMap * 2.0 - 1.0; // Transform from [0,1] to [-1,1]
                    vec3 n_normal = normalize(TBN * normalMap);

                    // Calculate ambient lighting
                    vec3 ambient = light.lightIntensity * light.lightColor;

                    // Calculate diffuse lighting
                    vec3 lightDir = normalize(light.lightPos-position);
                    float diff = max(dot(lightDir, n_normal), 0.0);
                    vec3 diffuse = diff * light.lightColor;

                    // Calculate specular lighting
                    vec3 viewDir = normalize(viewPos-position);
                    float spec = 0.0;
                    vec3 halfwayDir = normalize(lightDir+viewDir);
                    spec = pow(max(dot(n_normal, halfwayDir), 0.0), 64.0);
                    vec3 specular = spec * light.lightColor;


                    vec3 result = (ambient + diffuse + specular) * color;
                    fragColor = vec4(result, 1.0) * material.vertexColor;
                }
            ");
        }
    }
}