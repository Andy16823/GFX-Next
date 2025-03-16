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
                    mat4 mvp = p_mat*v_mat*m_mat;
                    position = vec3(m_mat*vec4(inPosition, 1.0));
                    normal = transpose(inverse(mat3(m_mat)))*inNormal;
                    texCoord = inTexCoord;
                    tangent = inTangent;
                    gl_Position = mvp*vec4(inPosition, 1.0);
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 330 core

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

                void main() {

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
