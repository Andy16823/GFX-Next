using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics.Shader
{
    public class ChromaticAberrationFXShader : ShaderProgram
    {
        public ChromaticAberrationFXShader()
        {
            // Vertex Shader (Standard Fullscreen Quad)
            VertexShader = new Graphics.Shader.Shader(@"
                #version 330 core
                layout (location = 0) in vec3 aPos;
                layout (location = 1) in vec2 aTexCoords;
                
                out vec2 TexCoords;
                
                void main()
                {
                    TexCoords = aTexCoords;
                    gl_Position = vec4(aPos, 1.0);
                }
            ");

            // Fragment Shader (Chromatic Aberration Effect)
            FragmentShader = new Graphics.Shader.Shader(@"
                #version 330 core
                
                in vec2 TexCoords;
                out vec4 FragColor;
                
                uniform sampler2D sourceTexture;
                uniform float intensity;      // Effect strength (0-1)
                uniform float radialAmount;   // Radial distortion amount (0-1)
                uniform vec2 direction;       // Direction of aberration (x, y)
                
                void main()
                {
                    vec2 uv = TexCoords;
                    
                    // Calculate offset
                    vec2 offset = vec2(0.0);
                    
                    if (radialAmount > 0.0)
                    {
                        // Radial chromatic aberration (from center)
                        vec2 centered = uv - 0.5;
                        float dist = length(centered);
                        vec2 dir = normalize(centered);
                        
                        // Stronger at edges
                        offset = dir * dist * intensity * radialAmount;
                    }
                    else
                    {
                        // Directional chromatic aberration
                        offset = direction * intensity;
                    }
                    
                    // Sample each color channel at different positions
                    float r = texture(sourceTexture, uv + offset).r;
                    float g = texture(sourceTexture, uv).g;
                    float b = texture(sourceTexture, uv - offset).b;
                    
                    FragColor = vec4(r, g, b, 1.0);
                }
            ");
        }
    }
}
