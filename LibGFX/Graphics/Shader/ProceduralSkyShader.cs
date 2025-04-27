using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class ProceduralSkyShader : ShaderProgram
    {
        public ProceduralSkyShader()
        {
            this.VertexShader = new Shader(@"
                #version 410 core
                layout (location = 0) in vec3 inPosition;

                out vec3 position;

                uniform mat4 p_mat;
                uniform mat4 v_mat;
                uniform mat4 m_mat;

                void main()
                {
                    position = inPosition;
                    gl_Position = vec4(inPosition, 1.0) * m_mat * v_mat * p_mat;
                }  
            ");

            this.FragmentShader = new Shader(@"
                #version 410 core
                out vec4 fragColor;

                in vec3 position;

                uniform vec3 skyTopColor;
                uniform vec3 skyBottomColor;
                uniform vec3 sunDirection;
                uniform vec3 sunColor;
                uniform float sunSize;
                uniform float sunIntensity;
                uniform float skylineOffset;
                uniform float skylineScale;

                // Optional coverage
                uniform bool coverage;
                uniform sampler2D coverageTexture;
                uniform float coverageFactor;
                uniform vec3 cloudColor;

                void main() {
                    vec3 dir = normalize(position);

                    // Sky-Background: Interpoloate between top and bottom color based on the y-coordinate
                    float t = dir.y * 0.5 + 0.5;

                    // Interpolation between skyline offset and skyline scale
                    t = smoothstep(skylineOffset - skylineScale, skylineOffset + skylineScale, dir.y);

                    // Create the skycolor based on the interpolation
                    vec3 skyColor = mix(skyBottomColor, skyTopColor, t);

                    // Optional add cloud coverage
                    if(coverage) {
                        // Interpolation between the bottom and top to fade in the clouds
                        float cloudFactor = smoothstep(0.0, 1.0, dir.y);

                        // Get the noise value from the texture
                        vec2 noiseCords = vec2(dir.x, dir.z);
                        float cloudNoise = texture(coverageTexture, noiseCords).r;
                        float clouds = smoothstep(0.3, 0.7, cloudNoise); // Soft cloud noise

                        // Calculate the cloud coverage intensity
                        clouds *= coverageFactor;

                        // Use the cloudfactor to fade in the clouds
                        clouds *= cloudFactor;

                        // Apply the skyline offset and scale to the clouds
                        clouds *= smoothstep(skylineOffset - skylineScale, skylineOffset + skylineScale, dir.y);

                        // Generate the cloud color and mix it with the sky color
                        vec3 cloudColor = mix(skyColor, cloudColor, clouds);

                        // Set the final sky color with the cloud coverage
                        skyColor = cloudColor;
                    }

                    // Add the sun
                    float sunFactor = max(dot(dir, normalize(sunDirection)), 0.0);
                    sunFactor = pow(sunFactor, sunSize);

                    // Create the final color for the enviroment
                    vec3 finalColor = skyColor+sunColor*sunFactor*sunIntensity;
                    fragColor = vec4(finalColor, 1.0);
                }
            ");
        }
    }
}
