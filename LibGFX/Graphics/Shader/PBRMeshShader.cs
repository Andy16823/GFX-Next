using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class PBRMeshShader : ShaderProgram
    {
        public PBRMeshShader()
        {
            this.VertexShader = new Shader(@"
                #version 430 core
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
                    mat4 mvp = m_mat * v_mat * p_mat;
                    position = vec3(vec4(inPosition, 1.0) * m_mat);
                    normal = inNormal * transpose(inverse(mat3(m_mat)));
                    texCoord = inTexCoord;
                    tangent = inTangent;
                    gl_Position = vec4(inPosition, 1.0) * mvp;
                }
            ");

            this.FragmentShader = new Shader(@"
                #version 430 core

                out vec4 fragColor; 

                in vec3 position;
                in vec3 normal;
                in vec2 texCoord;
                in vec4 tangent;  

                uniform vec3 albedo;
                uniform float metallic;
                uniform float roughness;
                uniform float ao;
                uniform vec3 camPos;
                uniform int numLights;
                const float PI = 3.14159265359;

                struct PointLightData
                {
                    vec3 position;
                    float intensity;
                    vec4 color;
                };

                layout(std430, binding = 0) buffer PointLights
                {
                    PointLightData lights[];
                };

                float DistributionGGX(vec3 N, vec3 H, float roughness)
                {
                    float a      = roughness*roughness;
                    float a2     = a*a;
                    float NdotH  = max(dot(N, H), 0.0);
                    float NdotH2 = NdotH*NdotH;
	
                    float num   = a2;
                    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
                    denom = PI * denom * denom;
	
                    return num / denom;
                };

                float GeometrySchlickGGX(float NdotV, float roughness)
                {
                    float r = (roughness + 1.0);
                    float k = (r*r) / 8.0;

                    float num   = NdotV;
                    float denom = NdotV * (1.0 - k) + k;
	
                    return num / denom;
                };

                float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
                {
                    float NdotV = max(dot(N, V), 0.0);
                    float NdotL = max(dot(N, L), 0.0);
                    float ggx2  = GeometrySchlickGGX(NdotV, roughness);
                    float ggx1  = GeometrySchlickGGX(NdotL, roughness);
	
                    return ggx1 * ggx2;
                };

                vec3 fresnelSchlick(float cosTheta, vec3 F0)
                {
                    return F0 + (1.0 - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
                }; 

                void main() {
                    vec3 N = normalize(normal);
                    vec3 V = normalize(camPos - position);

                    vec3 F0 = vec3(0.04); 
                    F0 = mix(F0, albedo, metallic);
            
                    // reflectance equation
                    vec3 Lo = vec3(0.0);
                    for(int i = 0; i < numLights; ++i) 
                    {
                        vec3 lightPosition = lights[i].position;
                        vec3 lightColor = lights[i].color.rgb;

                        // light direction
                        // calculate per-light radiance
                        vec3 L = normalize(lightPosition - position);
                        vec3 H = normalize(V + L);
                        float distance    = length(lightPosition - position);
                        float attenuation = 1.0 / (distance * distance);
                        vec3 radiance     = lightColor * attenuation;        
    
                        // cook-torrance brdf
                        float NDF = DistributionGGX(N, H, roughness);        
                        float G   = GeometrySmith(N, V, L, roughness);      
                        vec3 F    = fresnelSchlick(max(dot(H, V), 0.0), F0);       
    
                        vec3 kS = F;
                        vec3 kD = vec3(1.0) - kS;
                        kD *= 1.0 - metallic;	  
    
                        vec3 numerator    = NDF * G * F;
                        float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.0001;
                        vec3 specular     = numerator / denominator;  
        
                        // add to outgoing radiance Lo
                        float NdotL = max(dot(N, L), 0.0);                
                        Lo += (kD * albedo / PI + specular) * radiance * NdotL; 
                    }   

                    vec3 ambient = vec3(0.03) * albedo * ao;
                    vec3 color = ambient + Lo;

                    color = color / (color + vec3(1.0));
                    color = pow(color, vec3(1.0/2.2));  

                    fragColor = vec4(color, 1.0);
                }
            ");
        }
    }
}
