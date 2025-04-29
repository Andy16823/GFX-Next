using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class LitSpriteShader : ShaderProgram
    {
        public LitSpriteShader()
        {
            this.VertexShader = new Shader(@"
                #version 430 core
                layout(location = 0) in vec3 inPosition;
                layout(location = 1) in vec2 inTexCoord;

                out vec2 texCoord;
                out vec4 vColor;
                out vec3 position;

                uniform mat4 p_mat;
                uniform mat4 v_mat;
                uniform mat4 m_mat;
                uniform vec4 vertexColor;

                void main() {
                    mat4 mvp = m_mat * v_mat * p_mat;
                    texCoord = inTexCoord;
                    vColor = vertexColor;
                    position = vec3(vec4(inPosition, 1.0) * m_mat);
                    gl_Position = vec4(inPosition, 1.0) * mvp;
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 430 core

                in vec2 texCoord;
                in vec4 vColor;
                in vec3 position;

                out vec4 fragColor; 

                uniform sampler2D textureSampler;
                uniform vec4 uvTransform;
                uniform vec2 uvScale;


                uniform vec3 dirLightIntensity;
                uniform vec3 dirLightColor;

                struct PointLight {
                    vec4 position;
                    vec4 color;
                    vec4 radiusIntensity;
                };

                layout(std430, binding = 4) buffer PointLightsBuffer {
                    PointLight pointLights[];
                };

                void main() {
                    vec2 localUV = fract(texCoord * uvScale);
                    vec2 transformedTexCoord = localUV * uvTransform.xy + uvTransform.zw;
                    vec4 texColor = texture(textureSampler, transformedTexCoord);
                    
                    if (texColor.a < 0.1) {
                        discard;
                    }
           
                    vec3 lighting = dirLightColor;

                    for (int i = 0; i < pointLights.length(); i++) {
                        PointLight light = pointLights[i];
                        vec3 lightColor = light.color.xyz;
                        vec3 lightPos = light.position.xyz;
                        float radius = light.radiusIntensity.x;
                        float intensity = light.radiusIntensity.y;

                        vec3 toLight = lightPos - position;
                        float distance = length(toLight);
                        if (distance > radius) continue;
                        
                        vec3 lightDir = normalize(toLight);
                        float attenuation = 1.0 - (distance / radius);
                        lighting += lightColor * attenuation * intensity;
                    }   
                    
                    fragColor = vec4(lighting, 1.0) * texColor * vColor;
                }
            ");
        }
    }
}
