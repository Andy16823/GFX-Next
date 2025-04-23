using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class MeshShader : ShaderProgram
    {
        public MeshShader()
        {
            this.VertexShader = new Shader(@"
                #version 330 core

                layout(location = 0) in vec3 inPosition;
                layout(location = 1) in vec2 inTexCoord;
                layout(location = 2) in vec3 inNormal;
                layout(location = 3) in vec4 inTangent;

                out vec3 position;
                out vec3 normal;
                out vec2 texCoord;
                out vec4 tangent;  

                uniform mat4 p_mat;
                uniform mat4 v_mat;
                uniform mat4 m_mat;

                void main() {
                    mat4 mvp = m_mat*v_mat*p_mat;
                    position = vec3(vec4(inPosition, 1.0) * m_mat);
                    normal = inNormal * transpose(inverse(mat3(m_mat)));
                    texCoord = inTexCoord;
                    tangent = inTangent;
                    gl_Position = vec4(inPosition, 1.0) * mvp;
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 330 core

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

                struct Material {
                    sampler2D textureSampler;
                    sampler2D normalSampler;
                    sampler2D specularSampler;
                    vec4 vertexColor;
                    float shininess;
                };
                uniform Material material;

                void main() {
                    vec3 norm = normalize(-normal);
                    vec3 lightDir = normalize(-dirLight.direction);
                    float diff = max(dot(norm, lightDir), 0.0);

                    vec3 viewDir = normalize(viewPos - position);
                    vec3 reflectDir = reflect(-lightDir, norm);
                    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

                    // Calculate ambient lighting
                    vec3 ambient = dirLight.ambient * vec3(texture(material.textureSampler, texCoord));
                    vec3 diffuse = dirLight.lightColor  * diff * vec3(texture(material.textureSampler, texCoord)); 
                    vec3 specular = dirLight.specular * spec * vec3(texture(material.specularSampler, texCoord));

                    fragColor = vec4(ambient + diffuse + specular, 1.0); 
                }
            ");
        }
    }
}